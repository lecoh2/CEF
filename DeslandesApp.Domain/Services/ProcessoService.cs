using AutoMapper;
using DeslandesApp.Domain.Helpers;
using DeslandesApp.Domain.Interfaces.Repositories;
using DeslandesApp.Domain.Interfaces.Services;
using DeslandesApp.Domain.Models.Dtos.Requests.Processo;
using DeslandesApp.Domain.Models.Dtos.Responses.Atendimento;
using DeslandesApp.Domain.Models.Dtos.Responses.Caso;
using DeslandesApp.Domain.Models.Dtos.Responses.Pessoas;
using DeslandesApp.Domain.Models.Dtos.Responses.Processo;
using DeslandesApp.Domain.Models.Dtos.Responses.Usuarios;
using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Domain.Models.Enum;
using DeslandesApp.Domain.Utils;
using DeslandesApp.Domain.Validators;
using DeslandesApp.Domain.ValueObjects;
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
    public class ProcessoService(IUnitOfWork unitOfWork, IMapper mapper,
        IHttpContextAccessor httpContextAccessor, IHistoricoGeralService historicoGeralService,
         FunctionsHelper functionsHelper) : IProcessoService
    {
        public async Task<ProcessoResponse> AdicionarAsync(ProcessoRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            //  DTO -> Entidade
            var processo = mapper.Map<Processo>(request);

            // 🧹 Normalização segura
            processo.Pasta = processo.Pasta?.Trim().ToUpper();
            processo.Titulo = processo.Titulo?.Trim().ToUpper();
            processo.NumeroProcesso = processo.NumeroProcesso?.Trim();
            processo.LinkTribunal = processo.LinkTribunal?.Trim();
            processo.Instancia = request.Instancia.HasValue
     ? (Instancia?)request.Instancia.Value
     : null;
            processo.Acesso = request.Acesso.HasValue
     ? (Acesso?)request.Acesso.Value : null;
            processo.DataCadastro = DateTime.Now;

            //  Responsável
            processo.UsuarioResponsavelId = request.UsuarioResponsavelId;

            //  Validação Fluent
            var validator = new ProcessoValidator();
            var result = validator.Validate(processo);

            if (!result.IsValid)
                throw new ValidationException(result.Errors);

            //  VALIDA VARA
            if (processo.VaraId == Guid.Empty)
                throw new InvalidOperationException("Vara é obrigatória.");

            var vara = await unitOfWork.VaraRepository.GetByIdAsync(processo.VaraId);

            if (vara == null)
                throw new InvalidOperationException("Vara não encontrada.");

            //  VALIDA USUÁRIO RESPONSÁVEL (opcional se for obrigatório)
            if (processo.UsuarioResponsavelId.HasValue)
            {
                var usuario = await unitOfWork.UsuarioRepository
                    .GetByIdAsync(processo.UsuarioResponsavelId.Value);

                if (usuario == null)
                    throw new InvalidOperationException("Usuário responsável não encontrado.");
            }

            //  Verifica duplicidade
            var existente = await unitOfWork.ProcessoRepository.GetByAsync(p =>
                p.Pasta == processo.Pasta ||
                p.NumeroProcesso == processo.NumeroProcesso);

            if (existente != null)
            {
                if (existente.Pasta == processo.Pasta)
                    throw new InvalidOperationException("Nome de pasta já utilizado por outro processo.");

                if (existente.NumeroProcesso == processo.NumeroProcesso)
                    throw new InvalidOperationException("Nº do processo já cadastrado no sistema.");
            }

            //  Salva processo
            await unitOfWork.ProcessoRepository.AddAsync(processo);

            //  N:N - Clientes
            if (request.GrupoClienteProcesso != null && request.GrupoClienteProcesso.Any())
            {
                foreach (var grupos in request.GrupoClienteProcesso)
                {
                    var grupoClienteProcesso = new GrupoClienteProcesso
                    {
                        ProcessoId = processo.Id,
                        QualificacaoId = grupos.IdQualificacao,
                        PessoaId = grupos.IdPessoa.Value
                    };

                    await unitOfWork.GrupoClientesProcessosRepository.AddAsync(grupoClienteProcesso);
                }
            }

            //  N:N - Envolvidos
            if (request.GrupoEnvolvidosProcesso != null && request.GrupoEnvolvidosProcesso.Any())
            {
                foreach (var grupos in request.GrupoEnvolvidosProcesso)
                {
                    var grupoEnvolvidos = new GrupoEnvolvidosProcesso
                    {
                        ProcessoId = processo.Id,
                        QualificacaoId = grupos.IdQualificacao,
                        PessoaId = grupos.IdPessoa
                    };

                    await unitOfWork.GrupoEnvolvidosProcessosRepository.AddAsync(grupoEnvolvidos);
                }
            }
            if (request.GrupoEtiquetasProcesso != null && request.GrupoEtiquetasProcesso.Any())
            {
                foreach (var grupoEtiqueta in request.GrupoEtiquetasProcesso)
                {
                    var etiqueta = await unitOfWork.EtiquetaRepository.GetByIdAsync(grupoEtiqueta.EtiquetaId);
                    if (etiqueta == null) throw new InvalidOperationException("Etiqueta não encontrada.");
                    var processoEtiqueta = new GrupoEtiquetasProcessos
                    {
                        ProcessoId = processo.Id,
                        EtiquetaId = grupoEtiqueta.EtiquetaId
                    };
                    await unitOfWork.GrupoEtiquetasProcessosRepository.AddAsync(processoEtiqueta);
                }
            }
            //  Commit único
            await unitOfWork.CommitAsync();

            //  Retorno
            return mapper.Map<ProcessoResponse>(processo);
        }
        public Task<PageResult<ProcessoResponse>> ConsultarAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }
        public async Task<PageResult<ProcessoPaginacaoResponse>> ConsultarProcessoPaginacaoAsync(
      int pageNumber,
      int pageSize,
      string? searchTerm = null)
        {
            var paged = await unitOfWork.ProcessoRepository
                .GetProcessoPaginacaoAsync(pageNumber, pageSize, searchTerm);

            if (paged == null || !paged.Items.Any())
            {
                return new PageResult<ProcessoPaginacaoResponse>
                {
                    Items = new List<ProcessoPaginacaoResponse>(),
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

        public Task<ProcessoResponse> ExcluirAsync(Guid id)
        {
            throw new NotImplementedException();
        }
        public async Task<ProcessoResponse> ModificarAsync(Guid id, ProcessoUpdateRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                var processo = await unitOfWork.ProcessoRepository.GetByIdAsync(id)
                    ?? throw new ApplicationException("Processo não encontrado.");

                var usuarioId = functionsHelper.ObterUsuarioId();

                // =========================
                // SNAPSHOT ANTES
                // =========================
                var processoAntes = await unitOfWork.ProcessoRepository
                    .ConsultarProcessoComRelacionamentosAsync(id);

                var dadosAntes = new
                {
                    processoAntes.Titulo,
                    processoAntes.Pasta,
                    processoAntes.NumeroProcesso,
                    processoAntes.LinkTribunal,
                    processoAntes.Objeto,
                    processoAntes.ValorCausa,
                    processoAntes.ValorCondenacao,
                    processoAntes.Distribuido,
                    processoAntes.Observacao,

                    Instancia = processoAntes.Instancia?.ToString(),
                    Acesso = processoAntes.Acesso?.ToString(),

                    NomeVara = processoAntes.Vara?.NomeVara,
                    NomeForo = processoAntes.Vara?.Foro?.NomeForo,

                    Clientes = processoAntes.GrupoClienteProcesso?
          .Select(x => x.Pessoa?.Nome)
          .ToList(),

                    Envolvidos = processoAntes.GrupoEnvolvidosProcesso?
          .Select(x => x.Pessoa?.Nome)
          .ToList(),

                    Etiquetas = processoAntes.GrupoEtiquetasProcessos?
          .Select(x => x.Etiqueta?.Nome)
          .ToList()
                };

                // =========================
                // CAMPOS BÁSICOS (MANUAL)
                // =========================
                processo.Titulo = request.Titulo;
                processo.Pasta = request.Pasta;
                processo.NumeroProcesso = request.NumeroProcesso;
                processo.LinkTribunal = request.LinkTribunal;
                processo.Objeto = request.Objeto;
                processo.ValorCausa = request.ValorCausa;
                processo.ValorCondenacao = request.ValorCondenacao;
                processo.Distribuido = request.Distribuido;
                processo.Observacao = request.Observacao;

                processo.Instancia = request.Instancia.HasValue
     ? (Instancia?)request.Instancia.Value
     : null;
                processo.Acesso = request.Acesso.HasValue
                    ? (Acesso?)request.Acesso.Value
                    : null;

                processo.VaraId = request.VaraId;
                processo.AcaoId = request.AcaoId;
                processo.UsuarioResponsavelId = request.UsuarioResponsavelId;

                // =========================
                // 👥 CLIENTES (RESET)
                // =========================
                await unitOfWork.GrupoClientesProcessosRepository.RemoverClienteProcessoPorId(id);

                if (request.GrupoClienteProcesso?.Any() == true)
                {
                    foreach (var item in request.GrupoClienteProcesso)
                    {
                        await unitOfWork.GrupoClientesProcessosRepository.AddAsync(new GrupoClienteProcesso
                        {
                            ProcessoId = id,
                            PessoaId = item.IdPessoa.Value,
                            QualificacaoId = item.IdQualificacao
                        });
                    }
                }

                // =========================
                // 👥 ENVOLVIDOS (RESET)
                // =========================
                await unitOfWork.GrupoEnvolvidosProcessosRepository.RemoverEnvolvidosProcessoPorId(id);

                if (request.GrupoEnvolvidosProcesso?.Any() == true)
                {
                    foreach (var item in request.GrupoEnvolvidosProcesso)
                    {
                        await unitOfWork.GrupoEnvolvidosProcessosRepository.AddAsync(new GrupoEnvolvidosProcesso
                        {
                            ProcessoId = id,
                            PessoaId = item.IdPessoa,
                            QualificacaoId = item.IdQualificacao
                        });
                    }
                }

                // =========================
                // 🏷️ ETIQUETAS (RESET)
                // =========================
                await unitOfWork.GrupoEtiquetasProcessosRepository.RemoverEtiquetaProcessoPorId(id);

                if (request.GrupoEtiquetasProcesso?.Any() == true)
                {
                    foreach (var item in request.GrupoEtiquetasProcesso)
                    {
                        await unitOfWork.GrupoEtiquetasProcessosRepository.AddAsync(new GrupoEtiquetasProcessos
                        {
                            ProcessoId = id,
                            EtiquetaId = item.EtiquetaId
                        });
                    }
                }

                // =========================
                // UPDATE
                // =========================
                await unitOfWork.ProcessoRepository.UpdateAsync(processo);

                // =========================
                // SNAPSHOT DEPOIS
                // =========================
                var processoDepois = await unitOfWork.ProcessoRepository
                    .ConsultarProcessoComRelacionamentosAsync(id);
                var dadosDepois = new
                {
                    processoDepois.Titulo,
                    processoDepois.Pasta,
                    processoDepois.NumeroProcesso,
                    processoDepois.LinkTribunal,
                    processoDepois.Objeto,
                    processoDepois.ValorCausa,
                    processoDepois.ValorCondenacao,
                    processoDepois.Distribuido,
                    processoDepois.Observacao,

                    Instancia = processoDepois.Instancia?.ToString(),
                    Acesso = processoDepois.Acesso?.ToString(),

                    NomeVara = processoDepois.Vara?.NomeVara,
                    NomeForo = processoDepois.Vara?.Foro?.NomeForo,

                    Clientes = processoDepois.GrupoClienteProcesso?
                        .Select(x => x.Pessoa?.Nome)
                        .ToList(),

                    Envolvidos = processoDepois.GrupoEnvolvidosProcesso?
                        .Select(x => x.Pessoa?.Nome)
                        .ToList(),

                    Etiquetas = processoDepois.GrupoEtiquetasProcessos?
                        .Select(x => x.Etiqueta?.Nome)
                        .ToList()
                };

                // =========================
                // HISTÓRICO 🔥
                // =========================
                await historicoGeralService.RegistrarAsync(
                    TipoEntidade.Processo,
                    id,
                    usuarioId,
                    dadosAntes,
                    dadosDepois,
                    request.Observacao
                );

                await unitOfWork.CommitAsync();

                return mapper.Map<ProcessoResponse>(processoDepois);
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
        public async Task<ObterProcessoResponse?> ObterPorIdAsync(Guid id)
        {
            var processo = await unitOfWork.ProcessoRepository.ObterCompletoPorIdAsync(id);

            if (processo == null)
                return null;

            return mapper.Map<ObterProcessoResponse>(processo);
        }
        public async Task AdicionarSetorAsync(Guid idUsuario, Guid idSetor)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                var usuario = await unitOfWork.UsuarioRepository.GetByIdAsync(idUsuario);
                if (usuario is null)
                    throw new ApplicationException("Usuário não encontrado.");

                var setor = await unitOfWork.SetorRepository.GetByIdAsync(idSetor);
                if (setor is null)
                    throw new ApplicationException("Setor não encontrado.");

                var existeVinculo = await unitOfWork.GrupoSetoresRepository
                    .ExistUsuarioSetorAsync(idUsuario, idSetor);

                if (existeVinculo != null)
                    throw new ApplicationException("Este usuário já está vinculado a esse setor.");

                var grupoSetor = new GrupoSetores
                {
                    IdUsuario = idUsuario,
                    IdSetor = idSetor
                };

                await unitOfWork.GrupoSetoresRepository.AddAsync(grupoSetor);

                await unitOfWork.CommitAsync();
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
        public async Task RemoverSetorAsync(Guid idUsuario, Guid idSetor)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                var entidade = await unitOfWork.GrupoSetoresRepository
                    .GetByIdUSuarioIdSetor(idUsuario, idSetor);

                if (entidade is null)
                    throw new ApplicationException("Vínculo entre usuário e setor não encontrado.");

                await unitOfWork.GrupoSetoresRepository.DeleteAsync(entidade);

                await unitOfWork.CommitAsync();
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
        public async Task<List<ProcessoAutoComplete>> ConsultarProcessoAutoCompleteAsync(string? termo = null)
        {
            return await unitOfWork.ProcessoRepository.ConsultarProcessoAutoCompleteAsync(termo);
        }

        public async Task<List<ProcessoResumoResponse>> ConsultarUltimosAsync(int quantidade)
        {
            return await unitOfWork.ProcessoRepository
                .ConsultarUltimosAsync(quantidade);
        }
        public async Task<List<GraficoProcessoResponse>> ConsultarGraficoProcesso()
        {
            var dados = await unitOfWork.ProcessoRepository.GetGraficoProcessoAsync();

            var meses = Enumerable.Range(1, 12);

            var resultado = new List<GraficoProcessoResponse>();

            foreach (var mes in meses)
            {
                var item = dados.FirstOrDefault(d => d.Mes == mes);

                resultado.Add(new GraficoProcessoResponse
                {
                    Mes = mes,
                    Quantidade = item?.Quantidade ?? 0
                });
            }

            return resultado;
        }
        //public async Task<List<AtendimentoResponseDto>> Consultar5UltimosAtendimento()
        //{
        //    try
        //    {
        //        var atendimentos = await _unitOfWork.AtendimentoRepository.GetUltimas5AtendimentosAsync();

        //        if (atendimentos == null || !atendimentos.Any())
        //            throw new ApplicationException("Nenhuma Atendimento encontrado.");

        //        return atendimentos;
        //    }
        //    catch (Exception ex)
        //    {
        //        await _unitOfWork.RollbackAsync();
        //        throw new ApplicationException("Erro ao consultar atendiemntos.", ex);
        //    }
        //}
        public async Task<int> ContarProcessoAnoAtual()
        {
            return await unitOfWork.ProcessoRepository.ContarProcessoAnoAtual();
        }
        public async Task<int> ContarTotal()
        {
            return await unitOfWork.ProcessoRepository.ContarTotal();
        }
    }
}
