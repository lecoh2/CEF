using AutoMapper;
using DeslandesApp.Domain.Helpers;
using DeslandesApp.Domain.Interfaces.Repositories;
using DeslandesApp.Domain.Interfaces.Services;
using DeslandesApp.Domain.Models.Dtos.Requests.Kaban;
using DeslandesApp.Domain.Models.Dtos.Requests.ListaTarefas;
using DeslandesApp.Domain.Models.Dtos.Requests.Processo;
using DeslandesApp.Domain.Models.Dtos.Requests.Tarefa;
using DeslandesApp.Domain.Models.Dtos.Responses.ListaTarefas;
using DeslandesApp.Domain.Models.Dtos.Responses.Processo;
using DeslandesApp.Domain.Models.Dtos.Responses.Tarefa;
using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Domain.Models.Enum;
using DeslandesApp.Domain.Utils;
using DeslandesApp.Domain.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Services
{
    public class TarefaService(IUnitOfWork unitOfWork,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor,
    IHistoricoGeralService historicoGeralService, INotificacaoService notificacaoService
) : BaseService(httpContextAccessor) ,ITarefaService
    {
        public async Task<CriarTarefaResponse> AdicionarAsync(CriarTarefaRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                // =========================
                // DTO -> ENTIDADE
                // =========================
                var tarefa = mapper.Map<Tarefa>(request);

                // =========================
                // NORMALIZAÇÃO
                // =========================
                tarefa.Descricao = tarefa.Descricao?.Trim();

                tarefa.DataCadastro = DateTime.Now;
                tarefa.DataAtualizacao = DateTime.Now;

                tarefa.StatusGeralKanban = request.StatusGeralKanban;

                tarefa.UsuarioCriacaoId = ObterUsuarioId();

                // =========================
                // 🔗 VALIDAÇÃO DE VÍNCULO
                // =========================
                int count = 0;

                if (request.ProcessoId.HasValue) count++;
                if (request.CasoId.HasValue) count++;
                if (request.AtendimentoId.HasValue) count++;

                if (count > 1)
                {
                    throw new InvalidOperationException(
                        "A tarefa não pode ter mais de um vínculo."
                    );
                }

                // =========================
                // 🔗 DEFINE VÍNCULO
                // =========================
                tarefa.DefinirVinculo(
                    request.ProcessoId,
                    request.CasoId,
                    request.AtendimentoId
                );

                // =========================
                // 🔍 VALIDA EXISTÊNCIA
                // =========================
                if (tarefa.ProcessoId.HasValue)
                {
                    var processo = await unitOfWork.ProcessoRepository
                        .GetByIdAsync(tarefa.ProcessoId.Value);

                    if (processo == null)
                    {
                        throw new InvalidOperationException(
                            "Processo não encontrado."
                        );
                    }
                }

                if (tarefa.CasoId.HasValue)
                {
                    var caso = await unitOfWork.CasoRepository
                        .GetByIdAsync(tarefa.CasoId.Value);

                    if (caso == null)
                    {
                        throw new InvalidOperationException(
                            "Caso não encontrado."
                        );
                    }
                }

                if (tarefa.AtendimentoId.HasValue)
                {
                    var atendimento = await unitOfWork.AtendimentoRepository
                        .GetByIdAsync(tarefa.AtendimentoId.Value);

                    if (atendimento == null)
                    {
                        throw new InvalidOperationException(
                            "Atendimento não encontrado."
                        );
                    }
                }

                // =========================
                // ✅ REGRA DOMÍNIO
                // =========================
                tarefa.ValidarVinculo();

                // =========================
                // ✅ VALIDAÇÃO FLUENT
                // =========================
                var validator = new TarefaValidator();

                var result = validator.Validate(tarefa);

                if (!result.IsValid)
                {
                    throw new ValidationException(result.Errors);
                }

                // =========================
                // 👤 RESPONSÁVEL
                // =========================
                if (tarefa.UsuarioCriacaoId.HasValue)
                {
                    var usuario = await unitOfWork.UsuarioRepository
                        .GetByIdAsync(tarefa.UsuarioCriacaoId.Value);

                    if (usuario == null)
                    {
                        throw new InvalidOperationException(
                            "Responsável não encontrado."
                        );
                    }
                }

                // =========================
                // 💾 SALVA TAREFA
                // =========================
                await unitOfWork.TarefaRepository.AddAsync(tarefa);

                // =========================
                // ✅ CHECKLIST
                // =========================
                if (
                    request.ListasTarefa != null &&
                    request.ListasTarefa.Any()
                )
                {
                    var ultimaOrdem =
                        await unitOfWork.ListaTarefaRepository
                            .ObterMaiorOrdemPorTarefaId(tarefa.Id) ?? 0;

                    int incremento = 0;

                    foreach (var item in request.ListasTarefa)
                    {
                        var descricao = item.Descricao?.Trim();

                        if (string.IsNullOrWhiteSpace(descricao))
                            continue;

                        incremento += 10;

                        var lista = new ListaTarefa
                        {
                            TarefaId = tarefa.Id,
                            Descricao = descricao,
                            Ordem = ultimaOrdem + incremento,
                            Concluida = false,
                            DataConclusao = null
                        };

                        await unitOfWork.ListaTarefaRepository
                            .AddAsync(lista);
                    }
                }

                // =========================
                // 🏷️ ETIQUETAS
                // =========================
                if (
                    request.GrupoTarefasEtiquetas != null &&
                    request.GrupoTarefasEtiquetas.Any()
                )
                {
                    foreach (var grupoEtiqueta in request.GrupoTarefasEtiquetas)
                    {
                        var etiqueta = await unitOfWork.EtiquetaRepository
                            .GetByIdAsync(grupoEtiqueta.EtiquetaId);

                        if (etiqueta == null)
                        {
                            throw new InvalidOperationException(
                                "Etiqueta não encontrada."
                            );
                        }

                        await unitOfWork.GrupoTarefasEtiquetasRepository
                            .AddAsync(new GrupoTarefasEtiquetas
                            {
                                TarefaId = tarefa.Id,
                                EtiquetaId = grupoEtiqueta.EtiquetaId
                            });
                    }
                }

                // =========================
                // 👥 RESPONSÁVEIS
                // =========================
                if (
                    request.GrupoTarefaResponsaveis != null &&
                    request.GrupoTarefaResponsaveis.Any()
                )
                {
                    foreach (var envolvido in request.GrupoTarefaResponsaveis)
                    {
                        var usuario = await unitOfWork.UsuarioRepository
                            .GetByIdAsync(envolvido.UsuarioId);

                        if (usuario == null)
                        {
                            throw new InvalidOperationException(
                                "Usuário não encontrado."
                            );
                        }

                        await unitOfWork.GrupoTarefaResponsaveisRepository
                            .AddAsync(new GrupoTarefaResponsaveis
                            {
                                TarefaId = tarefa.Id,
                                UsuarioId = envolvido.UsuarioId
                            });
                    }
                }
                var responsaveis = request.GrupoTarefaResponsaveis
    .Select(x => x.UsuarioId)
    .ToList();

                // =========================
                // COMMIT
                // =========================
                await unitOfWork.CommitAsync();
                // 🔔 disparar notificação
                foreach (var usuarioId in responsaveis)
                {
                    await notificacaoService.CriarNotificacaoAsync(
                        usuarioId,
                        "Nova tarefa criada",
                        tarefa.Descricao,
                        TipoEntidade.Tarefa,
                        tarefa.Id
                    );
                }
                return mapper.Map<CriarTarefaResponse>(tarefa);
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
        public async Task ReordenarListaAsync(List<ReordenarListaTarefaRequest> request)
        {
            if (request == null || !request.Any())
                return;

            await unitOfWork.BeginTransactionAsync();

            var ids = request.Select(x => x.Id).ToList();

            var listas = await unitOfWork.ListaTarefaRepository.ObterPorIdsAsync(ids);

            if (listas.Count != request.Count)
                throw new InvalidOperationException("Um ou mais itens não foram encontrados.");

            var requestDict = request.ToDictionary(x => x.Id, x => x.Ordem);

            foreach (var lista in listas)
            {
                lista.Ordem = requestDict[lista.Id];
            }

            await unitOfWork.CommitAsync();
        }

        public Task<PageResult<CriarTarefaResponse>> ConsultarAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }
        public async Task<PageResult<TarefaPaginacaoResponse>> ConsultarTarefaPaginacaoAsync(
     int pageNumber,
     int pageSize,
     string? searchTerm = null)
        {
            var paged = await unitOfWork.TarefaRepository
                .GetTarefaPaginacaoAsync(pageNumber, pageSize, searchTerm);

            if (paged == null || !paged.Items.Any())
            {
                return new PageResult<TarefaPaginacaoResponse>
                {
                    Items = new List<TarefaPaginacaoResponse>(),
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

        public Task<CriarTarefaResponse> ExcluirAsync(Guid id)
        {
            throw new NotImplementedException();
        }
        public async Task<CriarTarefaResponse> ModificarAsync(
     Guid id,
     TarefaUpdateRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                // =========================
                // 🔍 BUSCA
                // =========================
                var tarefa = await unitOfWork.TarefaRepository
                    .GetByIdAsync(id);

                if (tarefa == null)
                {
                    throw new InvalidOperationException(
                        "Tarefa não encontrada."
                    );
                }

                var usuarioId = ObterUsuarioId();

                // =========================
                // 📸 SNAPSHOT ANTES
                // =========================
                var tarefaAntes = await unitOfWork.TarefaRepository
                    .ConsultarComRelacionamentosAsync(id);

                var dadosAntes = new
                {
                    tarefaAntes.Descricao,
                    tarefaAntes.DataTarefa,
                    tarefaAntes.StatusGeralKanban,
                    tarefaAntes.Prioridade,

                    Processo = tarefaAntes.Processo != null
                        ? tarefaAntes.Processo.Pasta
                        : null,

                    Caso = tarefaAntes.Caso != null
                        ? tarefaAntes.Caso.Pasta
                        : null,

                    Atendimento = tarefaAntes.Atendimento != null
                        ? tarefaAntes.Atendimento.Assunto
                        : null,

                    Responsaveis = tarefaAntes.GrupoTarefaResponsaveis?
                        .Select(x => x.Usuario?.NomeUsuario)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList(),

                    Etiquetas = tarefaAntes.GrupoTarefasEtiquetas?
                        .Select(x => x.Etiqueta?.Nome)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList()
                };

                // =========================
                // 👥 RESPONSÁVEIS ANTES
                // =========================
                var responsaveisAntes = tarefaAntes
                    .GrupoTarefaResponsaveis?
                    .Select(x => x.UsuarioId)
                    .Distinct()
                    .ToList() ?? new List<Guid>();

                // =========================
                // ✏️ CAMPOS BÁSICOS
                // =========================
                if (!string.IsNullOrWhiteSpace(request.Descricao))
                {
                    tarefa.Descricao = request.Descricao.Trim();
                }

                if (request.DataTarefa.HasValue)
                {
                    tarefa.DataTarefa = request.DataTarefa;
                }

                if (request.Prioridade.HasValue)
                {
                    tarefa.Prioridade = request.Prioridade.Value;
                }

                if (request.StatusGeralKanban.HasValue)
                {
                    tarefa.StatusGeralKanban =
                        request.StatusGeralKanban.Value;
                }

                tarefa.DataAtualizacao = DateTime.Now;

                // =========================
                // 🔗 VALIDAÇÃO VÍNCULO
                // =========================
                int count = 0;

                if (request.ProcessoId.HasValue) count++;
                if (request.CasoId.HasValue) count++;
                if (request.AtendimentoId.HasValue) count++;

                if (count > 1)
                {
                    throw new InvalidOperationException(
                        "A tarefa não pode ter mais de um vínculo."
                    );
                }

                // =========================
                // 🔗 DEFINE VÍNCULO
                // =========================
                tarefa.DefinirVinculo(
                    request.ProcessoId,
                    request.CasoId,
                    request.AtendimentoId
                );

                // =========================
                // 🔍 VALIDA EXISTÊNCIA
                // =========================
                if (tarefa.ProcessoId.HasValue)
                {
                    var processo = await unitOfWork.ProcessoRepository
                        .GetByIdAsync(tarefa.ProcessoId.Value);

                    if (processo == null)
                    {
                        throw new InvalidOperationException(
                            "Processo não encontrado."
                        );
                    }
                }

                if (tarefa.CasoId.HasValue)
                {
                    var caso = await unitOfWork.CasoRepository
                        .GetByIdAsync(tarefa.CasoId.Value);

                    if (caso == null)
                    {
                        throw new InvalidOperationException(
                            "Caso não encontrado."
                        );
                    }
                }

                if (tarefa.AtendimentoId.HasValue)
                {
                    var atendimento = await unitOfWork
                        .AtendimentoRepository
                        .GetByIdAsync(tarefa.AtendimentoId.Value);

                    if (atendimento == null)
                    {
                        throw new InvalidOperationException(
                            "Atendimento não encontrado."
                        );
                    }
                }

                // =========================
                // ✅ REGRA DOMÍNIO
                // =========================
                tarefa.ValidarVinculo();

                // =========================
                // ✅ CHECKLIST (RESET)
                // =========================
                await unitOfWork.ListaTarefaRepository
                    .RemoverPorTarefaId(id);

                if (request.ListasTarefa?.Any() == true)
                {
                    int ordem = 0;

                    foreach (var item in request.ListasTarefa)
                    {
                        if (string.IsNullOrWhiteSpace(item.Descricao))
                            continue;

                        ordem += 10;

                        await unitOfWork.ListaTarefaRepository
                            .AddAsync(new ListaTarefa
                            {
                                TarefaId = id,
                                Descricao = item.Descricao.Trim(),
                                Ordem = ordem,
                                Concluida = item.Concluida,
                                DataConclusao = item.Concluida
                                    ? DateTime.Now
                                    : null
                            });
                    }
                }

                // =========================
                // 🏷️ ETIQUETAS (RESET)
                // =========================
                await unitOfWork
                    .GrupoTarefasEtiquetasRepository
                    .RemoverPorTarefaId(id);

                if (request.GrupoTarefasEtiquetas?.Any() == true)
                {
                    foreach (var item in request.GrupoTarefasEtiquetas)
                    {
                        var etiqueta = await unitOfWork
                            .EtiquetaRepository
                            .GetByIdAsync(item.EtiquetaId);

                        if (etiqueta == null)
                        {
                            throw new InvalidOperationException(
                                "Etiqueta não encontrada."
                            );
                        }

                        await unitOfWork
                            .GrupoTarefasEtiquetasRepository
                            .AddAsync(new GrupoTarefasEtiquetas
                            {
                                TarefaId = id,
                                EtiquetaId = item.EtiquetaId
                            });
                    }
                }

                // =========================
                // 👥 RESPONSÁVEIS (RESET)
                // =========================
                await unitOfWork
                    .GrupoTarefaResponsaveisRepository
                    .RemoverPorTarefaId(id);

                if (request.GrupoTarefaResponsaveis?.Any() == true)
                {
                    foreach (var item in request.GrupoTarefaResponsaveis)
                    {
                        var usuario = await unitOfWork
                            .UsuarioRepository
                            .GetByIdAsync(item.UsuarioId);

                        if (usuario == null)
                        {
                            throw new InvalidOperationException(
                                "Usuário não encontrado."
                            );
                        }

                        await unitOfWork
                            .GrupoTarefaResponsaveisRepository
                            .AddAsync(new GrupoTarefaResponsaveis
                            {
                                TarefaId = id,
                                UsuarioId = item.UsuarioId
                            });
                    }
                }

                // =========================
                // 👥 RESPONSÁVEIS DEPOIS
                // =========================
                var responsaveisDepois = request
                    .GrupoTarefaResponsaveis?
                    .Select(x => x.UsuarioId)
                    .Distinct()
                    .ToList() ?? new List<Guid>();

                // =========================
                // 💾 UPDATE
                // =========================
                await unitOfWork.TarefaRepository
                    .UpdateAsync(tarefa);

                // =========================
                // 📸 SNAPSHOT DEPOIS
                // =========================
                var tarefaDepois = await unitOfWork.TarefaRepository
                    .ConsultarComRelacionamentosAsync(id);

                var dadosDepois = new
                {
                    tarefaDepois.Descricao,
                    tarefaDepois.DataTarefa,
                    tarefaDepois.StatusGeralKanban,
                    tarefaDepois.Prioridade,

                    Processo = tarefaDepois.Processo != null
                        ? tarefaDepois.Processo.Pasta
                        : null,

                    Caso = tarefaDepois.Caso != null
                        ? tarefaDepois.Caso.Pasta
                        : null,

                    Atendimento = tarefaDepois.Atendimento != null
                        ? tarefaDepois.Atendimento.Assunto
                        : null,

                    Responsaveis = tarefaDepois.GrupoTarefaResponsaveis?
                        .Select(x => x.Usuario?.NomeUsuario)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList(),

                    Etiquetas = tarefaDepois.GrupoTarefasEtiquetas?
                        .Select(x => x.Etiqueta?.Nome)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList()
                };

                // =========================
                // 🧾 HISTÓRICO
                // =========================
                await historicoGeralService.RegistrarAsync(
                    TipoEntidade.Tarefa,
                    tarefa.Id,
                    usuarioId,
                    dadosAntes,
                    dadosDepois,
                    "Tarefa atualizada"
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
                            "Você foi adicionado em uma tarefa",
                            tarefa.Descricao,
                            TipoEntidade.Tarefa,
                            tarefa.Id
                        );
                    }

                    // STATUS ALTERADO
                    if (
                        tarefaAntes.StatusGeralKanban !=
                        tarefaDepois.StatusGeralKanban
                    )
                    {
                        foreach (var responsavelId in responsaveisDepois)
                        {
                            await notificacaoService.CriarNotificacaoAsync(
                                responsavelId,
                                "Status da tarefa atualizado",
                                $"Novo status: {tarefaDepois.StatusGeralKanban}",
                                TipoEntidade.Tarefa,
                                tarefa.Id
                            );
                        }
                    }

                    // PRIORIDADE ALTERADA
                    if (
                        tarefaAntes.Prioridade !=
                        tarefaDepois.Prioridade
                    )
                    {
                        foreach (var responsavelId in responsaveisDepois)
                        {
                            await notificacaoService.CriarNotificacaoAsync(
                                responsavelId,
                                "Prioridade da tarefa alterada",
                                tarefa.Descricao,
                                TipoEntidade.Tarefa,
                                tarefa.Id
                            );
                        }
                    }
                }
                catch
                {
                    // não interrompe fluxo
                }

                // =========================
                // 📤 RETORNO
                // =========================
                return mapper.Map<CriarTarefaResponse>(tarefa);
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
        public async Task<ObterTarefaResponse?> ObterPorIdAsync(Guid id)
        {
            var tarefa = await unitOfWork.TarefaRepository.ObterCompletoPorIdAsync(id);

            if (tarefa == null)
                return null;

            return mapper.Map<ObterTarefaResponse>(tarefa);
        }
        public async Task MoverCardAsync(MoverKanbanCardRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                if (request.Tipo == "Tarefa")
                {
                    var tarefa = await unitOfWork.TarefaRepository.GetByIdAsync(request.Id);

                    if (tarefa == null)
                        throw new Exception("Tarefa não encontrada");

                    tarefa.StatusGeralKanban = request.NovoStatus;
                    tarefa.DataAtualizacao = DateTime.Now;
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
        public async Task AtualizarStatusTarefasAutomatico()
        {
            var eventos = await unitOfWork.EventoRepository.GetAllAsync();

            var hoje = DateOnly.FromDateTime(DateTime.Now);

            foreach (var evento in eventos)
            {
                if (evento.DataFinal.HasValue && evento.DataFinal < hoje)
                {
                    evento.StatusGeralKanban = StatusGeralKanban.Concluido;
                }
            }

            await unitOfWork.CommitAsync();
        }       
        public async Task<List<ListaTarefasResponse>> ConsultarListaTarefaAutoCompleteAsync(string? termo = null)
        {
            return await unitOfWork.ListaTarefaRepository.ConsultarListaTarefaAutoCompleteAsync(termo);
        }

        public async Task<List<ObterTarefaResponse>> ConsultarUltimosAsync(int quantidade)
        {
            var dados = await unitOfWork.TarefaRepository
                .ConsultarUltimosAsync(quantidade);

            return mapper.Map<List<ObterTarefaResponse>>(dados);
        }
    }
}