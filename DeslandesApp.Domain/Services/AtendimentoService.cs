using AutoMapper;
using DeslandesApp.Domain.Helpers;
using DeslandesApp.Domain.Interfaces.Repositories;
using DeslandesApp.Domain.Interfaces.Services;
using DeslandesApp.Domain.Models.Dtos.Requests.Atendimento;
using DeslandesApp.Domain.Models.Dtos.Requests.Caso;
using DeslandesApp.Domain.Models.Dtos.Responses.Atendimento;
using DeslandesApp.Domain.Models.Dtos.Responses.Caso;
using DeslandesApp.Domain.Models.Dtos.Responses.Processo;
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
    public class AtendiemntoService(IUnitOfWork unitOfWork, IMapper mapper,
        IHttpContextAccessor httpContextAccessor, IHistoricoGeralService historicoGeralService,
          FunctionsHelper functionsHelper) : IAtendimentoService
    {
        //public async Task<CriarAtendimentoClienteResponse> AdicionarAsync(CriarAtendimentoClienteRequest request)
        //{
        //    await unitOfWork.BeginTransactionAsync();

        //    //  DTO -> Entidade
        //    var atendimento = mapper.Map<Atendimento>(request);

        //    //  Normalização
        //    atendimento.Assunto = atendimento.Assunto?.Trim().ToUpper();
        //    atendimento.Registro = atendimento.Registro?.Trim();
        //    atendimento.DataCadastro = DateTime.Now;
        //    atendimento.DataAtualizacao = DateTime.Now;

        //    //  Responsável
        //    atendimento.ResponsavelId = request.ResponsavelId;

        //    //  VALIDAÇÃO DE VÍNCULO (AGORA OPCIONAL)
        //    int count = 0;

        //    if (request.ProcessoId.HasValue) count++;
        //    if (request.CasoId.HasValue) count++;
        //    if (request.AtendimentoPaiId.HasValue) count++;

        //    if (count > 1)
        //        throw new InvalidOperationException("O atendimento não pode ter mais de um vínculo.");

        //    //  RESOLVE VÍNCULO AUTOMATICAMENTE (SE EXISTIR)
        //    if (request.ProcessoId.HasValue)
        //    {
        //        var processo = await unitOfWork.ProcessoRepository.GetByIdAsync(request.ProcessoId.Value);

        //        if (processo == null)
        //            throw new InvalidOperationException("Processo não encontrado.");

        //        atendimento.ProcessoId = processo.Id;
        //        atendimento.TipoVinculoId = TipoVinculo.Processo;
        //    }
        //    else if (request.CasoId.HasValue)
        //    {
        //        var caso = await unitOfWork.CasoRepository.GetByIdAsync(request.CasoId.Value);

        //        if (caso == null)
        //            throw new InvalidOperationException("Caso não encontrado.");

        //        atendimento.CasoId = caso.Id;
        //        atendimento.TipoVinculoId = TipoVinculo.Caso;
        //    }
        //    else if (request.AtendimentoPaiId.HasValue)
        //    {
        //        var atendimentoPai = await unitOfWork.AtendimentoRepository.GetByIdAsync(request.AtendimentoPaiId.Value);

        //        if (atendimentoPai == null)
        //            throw new InvalidOperationException("Atendimento pai não encontrado.");

        //        atendimento.AtendimentoPaiId = atendimentoPai.Id;
        //        atendimento.TipoVinculoId = TipoVinculo.Atendimento;
        //    }
        //    else
        //    {
        //        //  Sem vínculo
        //        atendimento.TipoVinculoId = null; // IMPORTANTE: TipoVinculo deve ser nullable
        //    }

        //    //  VALIDA REGRA DE DOMÍNIO
        //    atendimento.ValidarVinculo();

        //    //  Validação Fluent
        //    var validator = new AtendimentoValidator();
        //    var result = validator.Validate(atendimento);

        //    if (!result.IsValid)
        //        throw new ValidationException(result.Errors);

        //    //  VALIDA RESPONSÁVEL
        //    if (atendimento.ResponsavelId.HasValue)
        //    {
        //        var usuario = await unitOfWork.UsuarioRepository
        //            .GetByIdAsync(atendimento.ResponsavelId.Value);

        //        if (usuario == null)
        //            throw new InvalidOperationException("Responsável não encontrado.");
        //    }

        //    //  Salva Atendimento
        //    await unitOfWork.AtendimentoRepository.AddAsync(atendimento);

        //    //  N:N - CLIENTES
        //    if (request.GrupoAtendimentoCliente != null && request.GrupoAtendimentoCliente.Any())
        //    {
        //        foreach (var item in request.GrupoAtendimentoCliente)
        //        {
        //            var pessoa = await unitOfWork.PessoaRepository.GetByIdAsync(item.PessoaId.Value);

        //            if (pessoa == null)
        //                throw new InvalidOperationException("Pessoa não encontrada.");

        //            var grupoCliente = new GrupoAtendimentoCliente
        //            {
        //                AtendimentoId = atendimento.Id,
        //                PessoaId = item.PessoaId.Value
        //            };

        //            await unitOfWork.GrupoAtendimentoClienteRepository.AddAsync(grupoCliente);
        //        }
        //    }

        //    //  N:N - ETIQUETAS (OPCIONAL)
        //    if (request.GrupoAtendimentoEtiquetas != null && request.GrupoAtendimentoEtiquetas.Any())
        //    {
        //        foreach (var item in request.GrupoAtendimentoEtiquetas)
        //        {
        //            var etiqueta = await unitOfWork.EtiquetaRepository.GetByIdAsync(item.EtiquetaId);

        //            if (etiqueta == null)
        //                throw new InvalidOperationException("Etiqueta não encontrada.");

        //            var grupoEtiqueta = new GrupoEtiquetasAtendimentos
        //            {
        //                AtendimentoId = atendimento.Id,
        //                EtiquetaId = item.EtiquetaId
        //            };

        //            await unitOfWork.GrupoEtiquetasAtendimentoRepository.AddAsync(grupoEtiqueta);
        //        }
        //    }

        //    //  Commit
        //    await unitOfWork.CommitAsync();

        //    return mapper.Map<CriarAtendimentoClienteResponse>(atendimento);
        //}
        public async Task<CriarAtendimentoClienteResponse> AdicionarAsync(CriarAtendimentoClienteRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                // =========================
                // DTO -> ENTIDADE
                // =========================
                var atendimento = mapper.Map<Atendimento>(request);

                // =========================
                // NORMALIZAÇÃO
                // =========================
                atendimento.Assunto = atendimento.Assunto?.Trim().ToUpper();
                atendimento.Registro = atendimento.Registro?.Trim();

                atendimento.DataCadastro = DateTime.Now;
                atendimento.DataAtualizacao = DateTime.Now;

                atendimento.ResponsavelId = request.ResponsavelId;

                // =========================
                // 🔗 DEFINE VÍNCULO
                // =========================
                atendimento.DefinirVinculo(
                    request.ProcessoId,
                    request.CasoId,
                    request.AtendimentoPaiId
                );

                // =========================
                // ✅ VALIDA DOMÍNIO
                // =========================
                atendimento.ValidarVinculo();

                // =========================
                // ✅ VALIDA EXISTÊNCIA DO VÍNCULO
                // =========================
                if (request.ProcessoId.HasValue)
                {
                    var processo = await unitOfWork.ProcessoRepository
                        .GetByIdAsync(request.ProcessoId.Value);

                    if (processo == null)
                        throw new InvalidOperationException("Processo não encontrado.");
                }

                if (request.CasoId.HasValue)
                {
                    var caso = await unitOfWork.CasoRepository
                        .GetByIdAsync(request.CasoId.Value);

                    if (caso == null)
                        throw new InvalidOperationException("Caso não encontrado.");
                }

                if (request.AtendimentoPaiId.HasValue)
                {
                    var atendimentoPai = await unitOfWork.AtendimentoRepository
                        .GetByIdAsync(request.AtendimentoPaiId.Value);

                    if (atendimentoPai == null)
                        throw new InvalidOperationException("Atendimento pai não encontrado.");
                }

                // =========================
                // ✅ VALIDAÇÃO FLUENT
                // =========================
                var validator = new AtendimentoValidator();
                var result = validator.Validate(atendimento);

                if (!result.IsValid)
                    throw new ValidationException(result.Errors);

                // =========================
                // ✅ VALIDA RESPONSÁVEL
                // =========================
                if (atendimento.ResponsavelId.HasValue)
                {
                    var usuario = await unitOfWork.UsuarioRepository
                        .GetByIdAsync(atendimento.ResponsavelId.Value);

                    if (usuario == null)
                        throw new InvalidOperationException("Responsável não encontrado.");
                }

                // =========================
                // 💾 SALVA ATENDIMENTO
                // =========================
                await unitOfWork.AtendimentoRepository.AddAsync(atendimento);

                // =========================
                // 👥 CLIENTES (N:N)
                // =========================
                if (request.GrupoAtendimentoCliente?.Any() == true)
                {
                    foreach (var item in request.GrupoAtendimentoCliente)
                    {
                        var pessoa = await unitOfWork.PessoaRepository
                            .GetByIdAsync(item.PessoaId.Value);

                        if (pessoa == null)
                            throw new InvalidOperationException("Pessoa não encontrada.");

                        var grupoCliente = new GrupoAtendimentoCliente
                        {
                            AtendimentoId = atendimento.Id,
                            PessoaId = item.PessoaId.Value
                        };

                        await unitOfWork.GrupoAtendimentoClienteRepository
                            .AddAsync(grupoCliente);
                    }
                }

                // =========================
                // 🏷️ ETIQUETAS (N:N)
                // =========================
                if (request.GrupoAtendimentoEtiquetas?.Any() == true)
                {
                    foreach (var item in request.GrupoAtendimentoEtiquetas)
                    {
                        var etiqueta = await unitOfWork.EtiquetaRepository
                            .GetByIdAsync(item.EtiquetaId);

                        if (etiqueta == null)
                            throw new InvalidOperationException("Etiqueta não encontrada.");

                        var grupoEtiqueta = new GrupoEtiquetasAtendimentos
                        {
                            AtendimentoId = atendimento.Id,
                            EtiquetaId = item.EtiquetaId
                        };

                        await unitOfWork.GrupoEtiquetasAtendimentoRepository
                            .AddAsync(grupoEtiqueta);
                    }
                }

                // =========================
                // ✅ COMMIT
                // =========================
                await unitOfWork.CommitAsync();

                return mapper.Map<CriarAtendimentoClienteResponse>(atendimento);
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
        public Task<PageResult<CriarAtendimentoClienteResponse>> ConsultarAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }
        public async Task<PageResult<AtendimentoPaginacaoResponse>> ConsultarAtendimentoPaginacaoAsync(
     int pageNumber,
     int pageSize,
     string? searchTerm = null)
        {
            var paged = await unitOfWork.AtendimentoRepository
                .GetAtendimentoPaginacaoAsync(pageNumber, pageSize, searchTerm);

            if (paged == null || !paged.Items.Any())
            {
                return new PageResult<AtendimentoPaginacaoResponse>
                {
                    Items = new List<AtendimentoPaginacaoResponse>(),
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

        public Task<CriarAtendimentoClienteResponse> ExcluirAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<CriarAtendimentoClienteResponse> ModificarAsync(Guid id, AtendimentoClienteUpdateRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                var atendimento = await unitOfWork.AtendimentoRepository.GetByIdAsync(id)
                    ?? throw new ApplicationException("Atendimento não encontrado.");

                var usuarioId = functionsHelper.ObterUsuarioId();

                // =========================
                // SNAPSHOT ANTES
                // =========================
                var atendimentoAntes = await unitOfWork.AtendimentoRepository
                    .ConsultarAtendimentoComRelacionamentosAsync(id);

                var dadosAntes = new
                {
                    atendimentoAntes.Assunto,
                    atendimentoAntes.Registro,
                    atendimentoAntes.ProcessoId,
                    atendimentoAntes.CasoId,
                    atendimentoAntes.AtendimentoPaiId,
                    Clientes = atendimentoAntes.GrupoClientes?.Select(x => x.PessoaId).ToList(),
                    Etiquetas = atendimentoAntes.GrupoEtiquetasAtendimentos?.Select(x => x.EtiquetaId).ToList()
                };

                // =========================
                // CAMPOS BÁSICOS (SEM AUTO MAPPER)
                // =========================
                atendimento.Assunto = request.Assunto;
                atendimento.Registro = request.Registro;

                atendimento.ResponsavelId = request.ResponsavelId;
                atendimento.DataAtualizacao = DateTime.Now;

                // =========================
                // 🔗 VÍNCULO (SEMPRE MANUAL)
                // =========================
                atendimento.DefinirVinculo(
                    request.ProcessoId,
                    request.CasoId,
                    request.AtendimentoPaiId
                );
                atendimento.ValidarVinculo();
                // =========================
                // 👥 CLIENTES (RESET)
                // =========================
                await unitOfWork.GrupoAtendimentoClienteRepository.RemoverPorAtendimentoId(id);

                if (request.GrupoAtendimentoCliente?.Any() == true)
                {
                    foreach (var item in request.GrupoAtendimentoCliente)
                    {
                        await unitOfWork.GrupoAtendimentoClienteRepository.AddAsync(new GrupoAtendimentoCliente
                        {
                            AtendimentoId = id,
                            PessoaId = item.PessoaId.Value
                        });
                    }
                }

                // =========================
                // 🏷️ ETIQUETAS (RESET)
                // =========================
                await unitOfWork.GrupoEtiquetasAtendimentoRepository.RemoverPorAtendimentoId(id);

                if (request.GrupoAtendimentoEtiqueta?.Any() == true)
                {
                    foreach (var item in request.GrupoAtendimentoEtiqueta)
                    {
                        await unitOfWork.GrupoEtiquetasAtendimentoRepository.AddAsync(new GrupoEtiquetasAtendimentos
                        {
                            AtendimentoId = id,
                            EtiquetaId = item.EtiquetaId
                        });
                    }
                }

                // =========================
                // UPDATE ENTIDADE
                // =========================
                await unitOfWork.AtendimentoRepository.UpdateAsync(atendimento);

                // =========================
                // SNAPSHOT DEPOIS
                // =========================
                var atendimentoDepois = await unitOfWork.AtendimentoRepository
                    .ConsultarAtendimentoComRelacionamentosAsync(id);

                var dadosDepois = new
                {
                    atendimentoDepois.Assunto,
                    atendimentoDepois.Registro,
                    atendimentoDepois.ProcessoId,
                    atendimentoDepois.CasoId,
                    atendimentoDepois.AtendimentoPaiId,
                    Clientes = atendimentoDepois.GrupoClientes?.Select(x => x.PessoaId).ToList(),
                    Etiquetas = atendimentoDepois.GrupoEtiquetasAtendimentos?.Select(x => x.EtiquetaId).ToList()
                };

                await historicoGeralService.RegistrarAsync(
                    TipoEntidade.Atendimento,
                    id,
                    usuarioId,
                    dadosAntes,
                    dadosDepois,
                    request.Observacao
                );

                await unitOfWork.CommitAsync();

                return mapper.Map<CriarAtendimentoClienteResponse>(atendimentoDepois);
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
        public async Task<List<AtendimentoAutoComplete>> ConsultarAtendimentoAutoCompleteAsync(string? termo = null)
        {
            return await unitOfWork.AtendimentoRepository.ConsultarAtendimentoAutoCompleteAsync(termo);
        }

        public async Task<ObterAtendimentoResponse?> ObterPorIdAsync(Guid id)
        {
            var atendimento = await unitOfWork.AtendimentoRepository.ObterCompletoPorIdAsync(id);

            if (atendimento == null)
                return null;

            return mapper.Map<ObterAtendimentoResponse>(atendimento);
        }
        public async Task<List<ObterAtendimentoResponse>> ConsultarUltimosAsync(int quantidade)
        {
            var dados = await unitOfWork.CasoRepository
                .ConsultarUltimosAsync(quantidade);

            return mapper.Map<List<ObterAtendimentoResponse>>(dados);
        }
        public async Task<List<GraficoAtendimentoResponse>> ConsultarGraficAtendimento()
        {
            var dados = await unitOfWork.AtendimentoRepository.GetGraficoAtendimentoAsync();

            var meses = Enumerable.Range(1, 12);

            var tipos = dados.Select(d => d.TipoVinculo).Distinct();

            var resultado = new List<GraficoAtendimentoResponse>();

            foreach (var tipo in tipos)
            {
                foreach (var mes in meses)
                {
                    var item = dados.FirstOrDefault(d => d.Mes == mes && d.TipoVinculo == tipo);

                    resultado.Add(new GraficoAtendimentoResponse
                    {
                        Mes = mes,
                        TipoVinculo = tipo,
                        Tipo = tipo switch
                        {
                            TipoVinculo.Processo => "Processo",
                            TipoVinculo.Caso => "Caso",
                            TipoVinculo.Atendimento => "Atendimento",
                            _ => "Sem vínculo"
                        },
                        Quantidade = item?.Quantidade ?? 0
                    });
                }
            }

            return resultado;
        }
    }
}
