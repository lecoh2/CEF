using AutoMapper;
using DeslandesApp.Domain.Helpers;
using DeslandesApp.Domain.Interfaces.Repositories;
using DeslandesApp.Domain.Interfaces.Services;
using DeslandesApp.Domain.Models.Dtos.Requests.Evento;
using DeslandesApp.Domain.Models.Dtos.Requests.Kaban;
using DeslandesApp.Domain.Models.Dtos.Responses.Evento;
using DeslandesApp.Domain.Models.Dtos.Responses.Processo;
using DeslandesApp.Domain.Models.Dtos.Responses.Tarefa;
using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Domain.Models.Enum;
using DeslandesApp.Domain.Utils;
using DeslandesApp.Domain.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Services
{
    public class EventoService(IUnitOfWork unitOfWork,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor,
    IHistoricoGeralService historicoGeralService, INotificacaoService notificacaoService
) : BaseService(httpContextAccessor), IEventoService
    {
        public async Task<CriarEventoResponse> AdicionarAsync(CriarEventoRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                // =========================
                // DTO -> ENTIDADE
                // =========================
                var evento = mapper.Map<Evento>(request);

                var agora = DateTime.Now;
                var hoje = DateOnly.FromDateTime(agora);
                var horaAtual = TimeOnly.FromDateTime(agora);

                evento.UsuarioCriacaoId = ObterUsuarioId();

                // =========================
                // 🧹 NORMALIZAÇÃO
                // =========================
                evento.Titulo = evento.Titulo?.Trim();
                evento.Endereco = evento.Endereco?.Trim();
                evento.Observacao = evento.Observacao?.Trim();

                evento.DataCadastro = DateTime.Now;
                evento.DataAtualizacao = DateTime.Now;

                // =========================
                // 🧠 STATUS KANBAN
                // =========================
                if (request.statusGeralKanban.HasValue)
                    evento.StatusGeralKanban = request.statusGeralKanban.Value;

                if (evento.DataFinal.HasValue && evento.DataFinal.Value < hoje)
                {
                    evento.StatusGeralKanban = StatusGeralKanban.Concluido;
                }
                else if (
                    evento.DataInicial == hoje &&
                    evento.HoraInicial <= horaAtual &&
                    (evento.HoraFinal == null || evento.HoraFinal >= horaAtual)
                )
                {
                    evento.StatusGeralKanban = StatusGeralKanban.Em_Andamento;
                }

                // =========================
                // 🕒 DIA INTEIRO
                // =========================
                if (evento.DiaInteiro)
                {
                    evento.HoraInicial = TimeOnly.MinValue;
                    evento.HoraFinal = TimeOnly.MaxValue;
                }

                // =========================
                // 🔁 RECORRÊNCIA
                // =========================
                if (evento.IntervaloRecorrencia < 1)
                    throw new InvalidOperationException("Intervalo da recorrência deve ser maior ou igual a 1.");

                if (evento.TipoRecorrencia != TipoRecorrencia.Nenhuma)
                {
                    if (evento.DataFimRecorrencia.HasValue && evento.QuantidadeOcorrencias.HasValue)
                        throw new InvalidOperationException("Informe apenas DataFimRecorrencia ou QuantidadeOcorrencias.");

                    if (!evento.DataFimRecorrencia.HasValue && !evento.QuantidadeOcorrencias.HasValue)
                        throw new InvalidOperationException("Recorrência precisa de um critério de término.");

                    if (evento.TipoRecorrencia == TipoRecorrencia.Semanal &&
                        (evento.DiasSemana == null || !evento.DiasSemana.Any()))
                    {
                        throw new InvalidOperationException("Informe ao menos um dia da semana para recorrência semanal.");
                    }
                }
                else
                {
                    evento.IntervaloRecorrencia = 1;
                    evento.DiasSemana = new List<DayOfWeek>();
                    evento.DataFimRecorrencia = null;
                    evento.QuantidadeOcorrencias = null;
                }

                // =========================
                // 🔗 VÍNCULO
                // =========================
                evento.DefinirVinculo(
                    request.ProcessoId,
                    request.CasoId,
                    request.AtendimentoId
                );

                evento.ValidarVinculo();

                // =========================
                // 🔍 VALIDA EXISTÊNCIA
                // =========================
                if (request.ProcessoId.HasValue)
                {
                    var processo = await unitOfWork.ProcessoRepository.GetByIdAsync(request.ProcessoId.Value);
                    if (processo == null)
                        throw new InvalidOperationException("Processo não encontrado.");
                }

                if (request.CasoId.HasValue)
                {
                    var caso = await unitOfWork.CasoRepository.GetByIdAsync(request.CasoId.Value);
                    if (caso == null)
                        throw new InvalidOperationException("Caso não encontrado.");
                }

                if (request.AtendimentoId.HasValue)
                {
                    var atendimento = await unitOfWork.AtendimentoRepository.GetByIdAsync(request.AtendimentoId.Value);
                    if (atendimento == null)
                        throw new InvalidOperationException("Atendimento não encontrado.");
                }

                // =========================
                // VALIDAÇÃO FLUENT
                // =========================
                var validator = new EventoValidator();
                var result = validator.Validate(evento);

                if (!result.IsValid)
                    throw new ValidationException(result.Errors);

                // =========================
                // 💾 SALVAR EVENTO
                // =========================
                await unitOfWork.EventoRepository.AddAsync(evento);

                // =========================
                // 👥 RESPONSÁVEIS
                // =========================
                if (request.GrupoEventoResponsaveis?.Any() == true)
                {
                    foreach (var item in request.GrupoEventoResponsaveis)
                    {
                        var usuario = await unitOfWork.UsuarioRepository.GetByIdAsync(item.UsuarioId);

                        if (usuario == null)
                            throw new InvalidOperationException($"Usuário {item.UsuarioId} não encontrado.");

                        await unitOfWork.GrupoEventoResponsavelRepository.AddAsync(
                            new GrupoEventoResponsavel
                            {
                                EventoId = evento.Id,
                                UsuarioId = item.UsuarioId
                            }
                        );
                    }
                }

                // =========================
                // 🏷️ ETIQUETAS
                // =========================
                if (request.GrupoEventoEtiquetas?.Any() == true)
                {
                    foreach (var grupoEtiqueta in request.GrupoEventoEtiquetas)
                    {
                        var etiqueta = await unitOfWork.EtiquetaRepository
                            .GetByIdAsync(grupoEtiqueta.EtiquetaId);

                        if (etiqueta == null)
                            throw new InvalidOperationException("Etiqueta não encontrada.");

                        await unitOfWork.GrupoEventoEtiquetasRepository.AddAsync(
                            new GrupoEventoEtiquetas
                            {
                                EventoId = evento.Id,
                                EtiquetaId = grupoEtiqueta.EtiquetaId
                            }
                        );
                    }
                }

                // =========================
                // COMMIT
                // =========================
                await unitOfWork.CommitAsync();

                // =========================
                // 🔔 NOTIFICAÇÃO SEGURA
                // =========================
                var responsaveis = request.GrupoEventoResponsaveis?
                    .Select(x => x.UsuarioId)
                    .Distinct()
                    .ToList() ?? new List<Guid>();

                try
                {
                    foreach (var usuarioId in responsaveis)
                    {
                        await notificacaoService.CriarNotificacaoAsync(
                            usuarioId,
                            "Novo evento criado",
                            evento.Titulo,
                            TipoEntidade.Evento,
                            evento.Id
                        );
                    }
                }
                catch
                {
                    // não interrompe fluxo caso notificação falhe
                }

                return mapper.Map<CriarEventoResponse>(evento);
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public Task<PageResult<CriarEventoResponse>> ConsultarAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }
        public async Task<PageResult<EventoPaginacaoResponse>> ConsultarEventoPaginacaoAsync(
     int pageNumber,
     int pageSize,
     string? searchTerm = null)
        {
            var paged = await unitOfWork.EventoRepository
                .GetEventoPaginacaoAsync(pageNumber, pageSize, searchTerm);

            if (paged == null || !paged.Items.Any())
            {
                return new PageResult<EventoPaginacaoResponse>
                {
                    Items = new List<EventoPaginacaoResponse>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }

            return paged;
        }
        public void Dispose()
        {
            unitOfWork.Dispose();
        }

        public Task<CriarEventoResponse> ExcluirAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        //public async Task<CriarEventoResponse> ModificarAsync(Guid id, UpdateEventoRequest request)
        //{
        //    await unitOfWork.BeginTransactionAsync();

        //    try
        //    {
        //        var evento = await unitOfWork.EventoRepository.GetByIdAsync(id);

        //        if (evento == null)
        //            throw new ApplicationException("Evento não encontrado.");

        //        var usuarioId = ObterUsuarioId();

        //        // =========================
        //        // ANTES (COM RELACIONAMENTOS)
        //        // =========================
        //        var eventoAntes = await unitOfWork.EventoRepository
        //            .ConsultarEventoComRelacionamentosAsync(id);

        //        if (eventoAntes == null)
        //            throw new ApplicationException("Evento para histórico não encontrado.");

        //        var dadosAntes = new
        //        {
        //            eventoAntes.Titulo,
        //            eventoAntes.DataInicial,
        //            eventoAntes.HoraInicial,
        //            eventoAntes.DataFinal,
        //            eventoAntes.HoraFinal,
        //            eventoAntes.DiaInteiro,
        //            eventoAntes.Endereco,
        //            eventoAntes.Observacao,
        //            eventoAntes.Modalidade,
        //            eventoAntes.StatusGeralKanban,

        //            Responsaveis = eventoAntes.GrupoEventoResponsaveis?
        //                .Select(r => r.Usuario?.NomeUsuario)
        //                .Where(n => n != null)
        //                .ToList()
        //        };

        //        //  =========================
        //        // ATUALIZAÇÃO
        //        // =========================
        //        mapper.Map(request, evento);

        //        var agora = DateTime.Now;
        //        var hoje = DateOnly.FromDateTime(agora);
        //        var horaAtual = TimeOnly.FromDateTime(agora);

        //        //  STATUS
        //        evento.StatusGeralKanban = request.StatusKaban ?? evento.StatusGeralKanban;

        //        if (evento.DataFinal.HasValue && evento.DataFinal.Value < hoje)
        //        {
        //            evento.StatusGeralKanban = StatusGeralKanban.Concluido;
        //        }
        //        else if (evento.DataInicial == hoje &&
        //                 evento.HoraInicial <= horaAtual &&
        //                 (evento.HoraFinal == null || evento.HoraFinal >= horaAtual))
        //        {
        //            evento.StatusGeralKanban = StatusGeralKanban.Em_Andamento;
        //        }

        //        //  NORMALIZAÇÃO
        //        evento.Titulo = evento.Titulo.Trim();
        //        evento.Endereco = evento.Endereco?.Trim();
        //        evento.Observacao = evento.Observacao?.Trim();

        //        //  DIA INTEIRO
        //        if (evento.DiaInteiro)
        //        {
        //            evento.HoraInicial = TimeOnly.MinValue;
        //            evento.HoraFinal = TimeOnly.MaxValue;
        //        }

        //        //  RECORRÊNCIA
        //        if (evento.IntervaloRecorrencia < 1)
        //            throw new InvalidOperationException("Intervalo da recorrência deve ser maior ou igual a 1.");

        //        if (evento.TipoRecorrencia != TipoRecorrencia.Nenhuma)
        //        {
        //            if (evento.DataFimRecorrencia.HasValue && evento.QuantidadeOcorrencias.HasValue)
        //                throw new InvalidOperationException("Informe apenas DataFimRecorrencia ou QuantidadeOcorrencias.");

        //            if (!evento.DataFimRecorrencia.HasValue && !evento.QuantidadeOcorrencias.HasValue)
        //                throw new InvalidOperationException("Recorrência precisa de um critério de término.");

        //            if (evento.TipoRecorrencia == TipoRecorrencia.Semanal &&
        //                (evento.DiasSemana == null || !evento.DiasSemana.Any()))
        //                throw new InvalidOperationException("Informe ao menos um dia da semana.");
        //        }
        //        else
        //        {
        //            evento.IntervaloRecorrencia = 1;
        //            evento.DiasSemana = new List<DayOfWeek>();
        //            evento.DataFimRecorrencia = null;
        //            evento.QuantidadeOcorrencias = null;
        //        }

        //        //  VALIDAÇÃO
        //        var validator = new EventoValidator();
        //        var result = validator.Validate(evento);

        //        if (!result.IsValid)
        //            throw new ValidationException(result.Errors);

        //        await unitOfWork.EventoRepository.UpdateAsync(evento);

        //        //  =========================
        //        // RESPONSÁVEIS (SE FOR ATUALIZAR)
        //        // =========================
        //        /*
        //        await unitOfWork.GrupoEventoResponsavelRepository.DeleteByEventoIdAsync(id);

        //        if (request.GrupoEventoResponsavel != null && request.GrupoEventoResponsavel.Any())
        //        {
        //            foreach (var item in request.GrupoEventoResponsavel)
        //            {
        //                var usuario = await unitOfWork.UsuarioRepository.GetByIdAsync(item.UsuarioId);

        //                if (usuario == null)
        //                    throw new InvalidOperationException($"Usuário {item.UsuarioId} não encontrado.");

        //                await unitOfWork.GrupoEventoResponsavelRepository.AddAsync(new GrupoEventoResponsavel
        //                {
        //                    EventoId = evento.Id,
        //                    UsuarioId = item.UsuarioId
        //                });
        //            }
        //        }
        //        */

        //        //  =========================
        //        // DEPOIS
        //        // =========================
        //        var eventoDepois = await unitOfWork.EventoRepository
        //            .ConsultarEventoComRelacionamentosAsync(id);

        //        if (eventoDepois == null)
        //            throw new ApplicationException("Evento atualizado não encontrado.");

        //        var dadosDepois = new
        //        {
        //            eventoDepois.Titulo,
        //            eventoDepois.DataInicial,
        //            eventoDepois.HoraInicial,
        //            eventoDepois.DataFinal,
        //            eventoDepois.HoraFinal,
        //            eventoDepois.DiaInteiro,
        //            eventoDepois.Endereco,
        //            eventoDepois.Observacao,
        //            eventoDepois.Modalidade,
        //            eventoDepois.StatusGeralKanban,

        //            Responsaveis = eventoDepois.GrupoEventoResponsaveis?
        //                .Select(r => r.Usuario?.NomeUsuario)
        //                .Where(n => n != null)
        //                .ToList()
        //        };

        //        // 🧾 =========================
        //        // HISTÓRICO
        //        // =========================
        //        var historico = new EventoHistorico
        //        {
        //            EventoId = evento.Id,
        //            UsuarioId = usuarioId,
        //            DataAlteracao = DateTime.Now,
        //            Observacao = request.Observacao ?? "",
        //            DadosAntes = JsonConvert.SerializeObject(dadosAntes),
        //            DadosDepois = JsonConvert.SerializeObject(dadosDepois)
        //        };

        //        await unitOfWork.EventoHistoricoRepository.AddAsync(historico);

        //        await unitOfWork.CommitAsync();

        //        return mapper.Map<CriarEventoResponse>(eventoDepois);
        //    }
        //    catch
        //    {
        //        await unitOfWork.RollbackAsync();
        //        throw;
        //    }
        //}
        public async Task<CriarEventoResponse> ModificarAsync(
      Guid id,
      UpdateEventoRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                // =========================
                // 🔍 BUSCA
                // =========================
                var evento = await unitOfWork.EventoRepository
                    .GetByIdAsync(id);

                if (evento == null)
                    throw new ApplicationException("Evento não encontrado.");

                var usuarioId = ObterUsuarioId();

                // =========================
                // 📸 SNAPSHOT ANTES
                // =========================
                var eventoAntes = await unitOfWork.EventoRepository
                    .ConsultarEventoComRelacionamentosAsync(id);

                var dadosAntes = new
                {
                    eventoAntes.Titulo,
                    eventoAntes.DataInicial,
                    eventoAntes.HoraInicial,
                    eventoAntes.DataFinal,
                    eventoAntes.HoraFinal,
                    eventoAntes.DiaInteiro,
                    eventoAntes.Endereco,
                    eventoAntes.Observacao,
                    eventoAntes.Modalidade,
                    eventoAntes.StatusGeralKanban,

                    Processo = eventoAntes.Processo != null
                        ? eventoAntes.Processo.Pasta
                        : null,

                    Caso = eventoAntes.Caso != null
                        ? eventoAntes.Caso.Pasta
                        : null,

                    Atendimento = eventoAntes.Atendimento != null
                        ? eventoAntes.Atendimento.Assunto
                        : null,

                    TipoVinculo = eventoAntes.TipoVinculoId.ToString(),

                    Responsaveis = eventoAntes
                        .GrupoEventoResponsaveis?
                        .Select(r => r.Usuario?.NomeUsuario)
                        .Where(n => !string.IsNullOrWhiteSpace(n))
                        .ToList(),

                    Etiquetas = eventoAntes
                        .GrupoEventoEtiquetas?
                        .Select(e => e.Etiqueta?.Nome)
                        .Where(n => !string.IsNullOrWhiteSpace(n))
                        .ToList()
                };

                // =========================
                // 👥 RESPONSÁVEIS ANTES
                // =========================
                var responsaveisAntes = eventoAntes
                    .GrupoEventoResponsaveis?
                    .Select(x => x.UsuarioId)
                    .Distinct()
                    .ToList() ?? new List<Guid>();

                // =========================
                // ✏️ CAMPOS BÁSICOS
                // =========================
                mapper.Map(request, evento);

                evento.DataAtualizacao = DateTime.Now;

                // =========================
                // 🔗 DEFINE VÍNCULO
                // =========================
                evento.DefinirVinculo(
                    request.ProcessoId,
                    request.CasoId,
                    request.AtendimentoId
                );

                // =========================
                // ✅ VALIDA REGRA DOMÍNIO
                // =========================
                evento.ValidarVinculo();

                // =========================
                // 👥 RESPONSÁVEIS
                // =========================
                evento.GrupoEventoResponsaveis.Clear();

                if (request.GrupoEventoResponsavel != null)
                {
                    evento.GrupoEventoResponsaveis =
                        request.GrupoEventoResponsavel
                        .Select(x => new GrupoEventoResponsavel
                        {
                            UsuarioId = x.UsuarioId,
                            EventoId = evento.Id
                        })
                        .ToList();
                }

                // =========================
                // 👥 RESPONSÁVEIS DEPOIS
                // =========================
                var responsaveisDepois = request
                    .GrupoEventoResponsavel?
                    .Select(x => x.UsuarioId)
                    .Distinct()
                    .ToList() ?? new List<Guid>();

                // =========================
                // 🏷️ ETIQUETAS
                // =========================
                evento.GrupoEventoEtiquetas.Clear();

                if (request.GrupoEventoEtiquetas != null)
                {
                    evento.GrupoEventoEtiquetas =
                        request.GrupoEventoEtiquetas
                        .Select(x => new GrupoEventoEtiquetas
                        {
                            EtiquetaId = x.EtiquetaId,
                            EventoId = evento.Id
                        })
                        .ToList();
                }

                // =========================
                // 🧠 STATUS
                // =========================
                var agora = DateTime.Now;
                var hoje = DateOnly.FromDateTime(agora);
                var horaAtual = TimeOnly.FromDateTime(agora);

                if (
                    evento.DataFinal.HasValue &&
                    evento.DataFinal.Value < hoje
                )
                {
                    evento.StatusGeralKanban =
                        StatusGeralKanban.Concluido;
                }
                else if (evento.DataInicial > hoje)
                {
                    evento.StatusGeralKanban =
                        StatusGeralKanban.A_Fazer;
                }
                else if (
                    evento.DataInicial <= hoje &&
                    (
                        !evento.DataFinal.HasValue ||
                        evento.DataFinal >= hoje
                    ) &&
                    evento.HoraInicial <= horaAtual &&
                    (
                        evento.HoraFinal == null ||
                        evento.HoraFinal >= horaAtual
                    )
                )
                {
                    evento.StatusGeralKanban =
                        StatusGeralKanban.Em_Andamento;
                }

                // =========================
                // 🧹 NORMALIZAÇÃO
                // =========================
                evento.Titulo = evento.Titulo.Trim();
                evento.Endereco = evento.Endereco?.Trim();
                evento.Observacao = evento.Observacao?.Trim();

                // =========================
                // 🕒 DIA INTEIRO
                // =========================
                if (evento.DiaInteiro)
                {
                    evento.HoraInicial = TimeOnly.MinValue;
                    evento.HoraFinal = TimeOnly.MaxValue;
                }

                // =========================
                // 🔁 RECORRÊNCIA
                // =========================
                if (evento.IntervaloRecorrencia < 1)
                {
                    throw new InvalidOperationException(
                        "Intervalo da recorrência deve ser maior ou igual a 1."
                    );
                }

                if (evento.TipoRecorrencia != TipoRecorrencia.Nenhuma)
                {
                    if (
                        evento.DataFimRecorrencia.HasValue &&
                        evento.QuantidadeOcorrencias.HasValue
                    )
                    {
                        throw new InvalidOperationException(
                            "Informe apenas DataFimRecorrencia ou QuantidadeOcorrencias."
                        );
                    }

                    if (
                        !evento.DataFimRecorrencia.HasValue &&
                        !evento.QuantidadeOcorrencias.HasValue
                    )
                    {
                        throw new InvalidOperationException(
                            "Recorrência precisa de um critério de término."
                        );
                    }

                    if (
                        evento.TipoRecorrencia ==
                        TipoRecorrencia.Semanal &&
                        (
                            evento.DiasSemana == null ||
                            !evento.DiasSemana.Any()
                        )
                    )
                    {
                        throw new InvalidOperationException(
                            "Informe ao menos um dia da semana."
                        );
                    }
                }
                else
                {
                    evento.IntervaloRecorrencia = 1;

                    evento.DiasSemana =
                        new List<DayOfWeek>();

                    evento.DataFimRecorrencia = null;

                    evento.QuantidadeOcorrencias = null;
                }

                // =========================
                // ✅ VALIDAÇÃO
                // =========================
                var validator = new EventoValidator();

                var result = validator.Validate(evento);

                if (!result.IsValid)
                    throw new ValidationException(result.Errors);

                // =========================
                // 💾 UPDATE
                // =========================
                await unitOfWork.EventoRepository
                    .UpdateAsync(evento);

                // =========================
                // 📸 SNAPSHOT DEPOIS
                // =========================
                var eventoDepois = await unitOfWork
                    .EventoRepository
                    .ConsultarEventoComRelacionamentosAsync(id);

                var dadosDepois = new
                {
                    eventoDepois.Titulo,
                    eventoDepois.DataInicial,
                    eventoDepois.HoraInicial,
                    eventoDepois.DataFinal,
                    eventoDepois.HoraFinal,
                    eventoDepois.DiaInteiro,
                    eventoDepois.Endereco,
                    eventoDepois.Observacao,
                    eventoDepois.Modalidade,
                    eventoDepois.StatusGeralKanban,

                    Processo = eventoDepois.Processo != null
                        ? eventoDepois.Processo.Pasta
                        : null,

                    Caso = eventoDepois.Caso != null
                        ? eventoDepois.Caso.Pasta
                        : null,

                    Atendimento = eventoDepois.Atendimento != null
                        ? eventoDepois.Atendimento.Assunto
                        : null,

                    TipoVinculo =
                        eventoDepois.TipoVinculoId.ToString(),

                    Responsaveis = eventoDepois
                        .GrupoEventoResponsaveis?
                        .Select(r => r.Usuario?.NomeUsuario)
                        .Where(n => !string.IsNullOrWhiteSpace(n))
                        .ToList(),

                    Etiquetas = eventoDepois
                        .GrupoEventoEtiquetas?
                        .Select(e => e.Etiqueta?.Nome)
                        .Where(n => !string.IsNullOrWhiteSpace(n))
                        .ToList()
                };

                // =========================
                // 🧾 HISTÓRICO
                // =========================
                await historicoGeralService.RegistrarAsync(
                    TipoEntidade.Evento,
                    evento.Id,
                    usuarioId,
                    dadosAntes,
                    dadosDepois,
                    "Evento atualizado"
                );

                // =========================
                // ✅ COMMIT
                // =========================
                await unitOfWork.CommitAsync();

                // =========================
                // 🔔 NOTIFICAÇÕES
                // =========================
                try
                {
                    // NOVOS RESPONSÁVEIS
                    var novosResponsaveis = responsaveisDepois
                        .Except(responsaveisAntes)
                        .ToList();

                    foreach (var responsavelId in novosResponsaveis)
                    {
                        await notificacaoService.CriarNotificacaoAsync(
                            responsavelId,
                            "Você foi adicionado em um evento",
                            evento.Titulo,
                            TipoEntidade.Evento,
                            evento.Id
                        );
                    }

                    // STATUS ALTERADO
                    if (
                        eventoAntes.StatusGeralKanban !=
                        eventoDepois.StatusGeralKanban
                    )
                    {
                        foreach (var responsavelId in responsaveisDepois)
                        {
                            await notificacaoService.CriarNotificacaoAsync(
                                responsavelId,
                                "Status do evento atualizado",
                                $"Novo status: {eventoDepois.StatusGeralKanban}",
                                TipoEntidade.Evento,
                                evento.Id
                            );
                        }
                    }

                    // DATA ALTERADA
                    if (
                        eventoAntes.DataInicial != eventoDepois.DataInicial ||
                        eventoAntes.HoraInicial != eventoDepois.HoraInicial ||
                        eventoAntes.DataFinal != eventoDepois.DataFinal ||
                        eventoAntes.HoraFinal != eventoDepois.HoraFinal
                    )
                    {
                        foreach (var responsavelId in responsaveisDepois)
                        {
                            await notificacaoService.CriarNotificacaoAsync(
                                responsavelId,
                                "Data ou horário do evento alterado",
                                evento.Titulo,
                                TipoEntidade.Evento,
                                evento.Id
                            );
                        }
                    }
                }
                catch
                {
                    // não interrompe fluxo caso notificação falhe
                }

                // =========================
                // 📤 RETORNO
                // =========================
                return mapper.Map<CriarEventoResponse>(eventoDepois);
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<ObterEventoResponse?> ObterPorIdAsync(Guid id)
        {
            var evento = await unitOfWork.EventoRepository.ObterCompletoPorIdAsync(id);

            if (evento == null)
                return null;

            return mapper.Map<ObterEventoResponse>(evento);
        }
        public async Task MoverCardAsync(MoverKanbanCardRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                if (request.Tipo == "Tarefa")
                {
                    var evento = await unitOfWork.EventoRepository.GetByIdAsync(request.Id);

                    if (evento == null)
                        throw new Exception("Tarefa não encontrada");

                    evento.StatusGeralKanban = request.NovoStatus;
                    evento.DataAtualizacao = DateTime.Now;
                }
                else if (request.Tipo == "Evento")
                {
                    var evento = await unitOfWork.EventoRepository.GetByIdAsync(request.Id);

                    if (evento == null)
                        throw new Exception("Evento não encontrado");

                    evento.StatusGeralKanban = request.NovoStatus;
                }
                else
                {
                    throw new Exception("Tipo inválido");
                }

                await unitOfWork.CommitAsync();
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
        public async Task AtualizarStatusAutomatico()
        {
            var hoje = DateOnly.FromDateTime(DateTime.Now);

            // Busca todos os eventos
            var eventos = await unitOfWork.EventoRepository.GetAllAsync();

            // Filtra apenas os que não estão concluídos e já passaram da data final
            var eventosParaAtualizar = eventos
                .Where(e => e.StatusGeralKanban != StatusGeralKanban.Concluido &&
                            e.DataFinal.HasValue && e.DataFinal < hoje)
                .ToList();

            foreach (var evento in eventosParaAtualizar)
            {
                evento.StatusGeralKanban = StatusGeralKanban.Concluido;
            }

            if (eventosParaAtualizar.Any())
                await unitOfWork.CommitAsync();
        }

        public async Task<List<EventoService>> ConsultarUltimosAsync(int quantidade)
        {
            var dados = await unitOfWork.ProcessoRepository
                .ConsultarUltimosAsync(quantidade);

            return mapper.Map<List<EventoService>>(dados);
        }

    }
}
