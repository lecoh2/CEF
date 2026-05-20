using AutoMapper;
using DeslandesApp.Domain.Interfaces.Repositories;
using DeslandesApp.Domain.Interfaces.Services;
using DeslandesApp.Domain.Models.Dtos.Requests.Intimacao;
using DeslandesApp.Domain.Models.Dtos.Responses.Intimacao;
using DeslandesApp.Domain.Models.Dtos.Responses.LoteTrabalho;
using DeslandesApp.Domain.Models.Dtos.Responses.PecaCabivel;
using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Domain.Models.Enum;
using DeslandesApp.Domain.Models.Enum.DeslandesApp.Domain.Models.Enum;
using DeslandesApp.Domain.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Services
{
    public class IntimacaoService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor,
    IHistoricoGeralService historicoGeralService
) : BaseService(httpContextAccessor),
    IIntimacaoService
    {
        public async Task<IntimacaoResponse> AdicionarAsync(IntimacaoRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                // =========================
                // VALIDA PROCESSO
                // =========================
                var processo = await unitOfWork.ProcessoRepository
                    .GetByIdAsync(request.ProcessoId)
                    ?? throw new InvalidOperationException("Processo não encontrado.");

                // =========================
                // VERIFICA DUPLICIDADE
                // (Processo + DataIntimacao)
                // =========================
                var existente = await unitOfWork.IntimacaoRepository.GetByAsync(x =>
                    x.ProcessoId == request.ProcessoId &&
                    x.DataIntimacao.Date == request.DataIntimacao.Date
                );

                if (existente != null)
                    throw new InvalidOperationException("Intimação já importada para este processo nessa data.");

                // =========================
                // MAPEIA ENTIDADE
                // =========================
                var intimacao = mapper.Map<Intimacao>(request);

                // =========================
                // REGRAS PADRÃO DO SISTEMA
                // =========================
                intimacao.DataImportacao = DateTime.Now;
                intimacao.StatusTriagem = StatusTriagem.PendenteDeLeitura;
                intimacao.StatusCumprimento = StatusCumprimento.Pendente;

                // garante consistência
                intimacao.TextoIntimacao = request.TextoIntimacao?.Trim();

                // =========================
                // VALIDAÇÃO SIMPLES (opcional mas recomendado)
                // =========================
                if (intimacao.ProcessoId == Guid.Empty)
                    throw new InvalidOperationException("Processo é obrigatório.");

                if (intimacao.DataIntimacao == default)
                    throw new InvalidOperationException("Data da intimação é obrigatória.");

                // =========================
                // SALVA
                // =========================
                await unitOfWork.IntimacaoRepository.AddAsync(intimacao);

                // =========================
                // COMMIT
                // =========================
                await unitOfWork.CommitAsync();

                // =========================
                // RETORNO
                // =========================
                var response = mapper.Map<IntimacaoResponse>(intimacao);

                // garante número do processo no retorno (caso mapping não traga)
                response = response with
                {
                    NumeroProcesso = processo.NumeroProcesso
                };

                return response;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public Task<PageResult<IntimacaoResponse>> ConsultarAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public async Task<PageResult<IntimacaoPaginacaoResponse>> ConsultarIntimacaoPaginacaoAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            var paged = await unitOfWork.IntimacaoRepository
                   .GetIntimacaoPaginacaoAsync(pageNumber, pageSize, searchTerm);

            if (paged == null || !paged.Items.Any())
            {
                return new PageResult<IntimacaoPaginacaoResponse>
                {
                    Items = new List<IntimacaoPaginacaoResponse>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }

            return paged;
        }

        public async Task<PageResult<IntimacaoPaginacaoResponse>> ConsultarParaTriagemAsync(
     DateTime? data,
     StatusTriagem? status,
     int pageNumber,
     int pageSize)
        {
            return await unitOfWork.IntimacaoRepository
                .ConsultarParaTriagemAsync(data, status, pageNumber, pageSize);
        }

        public void Dispose()
        {
           unitOfWork.Dispose();
        }

        public async Task DistribuirParaAdvogadoAsync(Guid intimacaoId, Guid advogadoId)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                // =========================
                // BUSCA INTIMAÇÃO
                // =========================
                var intimacao = await unitOfWork.IntimacaoRepository
                    .GetByIdAsync(intimacaoId)
                    ?? throw new InvalidOperationException("Intimação não encontrada.");

                // =========================
                // VALIDA ADVOGADO
                // =========================
                var advogado = await unitOfWork.UsuarioRepository
                    .GetByIdAsync(advogadoId)
                    ?? throw new InvalidOperationException("Usuário (advogado) não encontrado.");

                // =========================
                // REGRA: não pode estar em lote
                // =========================
                if (intimacao.LoteId.HasValue)
                    throw new InvalidOperationException("Intimação já está vinculada a um lote.");

                // =========================
                // REGRA: distribuição individual
                // =========================
                intimacao.AdvogadoId = advogadoId;

                // opcional: limpar lote se existir
                intimacao.LoteId = null;

                // =========================
                // STATUS (opcional mas recomendado)
                // =========================
                // você pode manter Triada ou criar outro status futuro tipo "Distribuída"
                intimacao.StatusTriagem = StatusTriagem.Triada;

                // =========================
                // ATUALIZA
                // =========================
                await unitOfWork.IntimacaoRepository.UpdateAsync(intimacao);

                // =========================
                // COMMIT
                // =========================
                await unitOfWork.CommitAsync();
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
        public async Task DistribuirParaLoteAsync(Guid intimacaoId, Guid loteId)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                // =========================
                // BUSCA INTIMAÇÃO
                // =========================
                var intimacao = await unitOfWork.IntimacaoRepository
                    .GetByIdAsync(intimacaoId)
                    ?? throw new InvalidOperationException("Intimação não encontrada.");

                // =========================
                // BUSCA LOTE
                // =========================
                var lote = await unitOfWork.LoteTrabalhoRepository
                    .GetByIdAsync(loteId)
                    ?? throw new InvalidOperationException("Lote não encontrado.");

                // =========================
                // REGRA: não pode já estar em lote
                // =========================
                if (intimacao.LoteId.HasValue)
                    throw new InvalidOperationException("Intimação já está vinculada a um lote.");

                // =========================
                // REGRA: não pode estar em distribuição individual
                // =========================
                if (intimacao.AdvogadoId.HasValue)
                    throw new InvalidOperationException("Intimação já foi distribuída para um advogado.");

                // =========================
                // VINCULA LOTE
                // =========================
                intimacao.LoteId = loteId;

                // =========================
                // STATUS (importante para fluxo)
                // =========================
                intimacao.StatusTriagem = StatusTriagem.Triada;

                // =========================
                // ATUALIZA
                // =========================
                await unitOfWork.IntimacaoRepository.UpdateAsync(intimacao);

                // =========================
                // COMMIT
                // =========================
                await unitOfWork.CommitAsync();
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
        public async Task<IntimacaoResponse> ExcluirAsync(Guid id)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                var usuarioId = ObterUsuarioId();

                // =========================
                // BUSCA INTIMAÇÃO
                // =========================
                var intimacao = await unitOfWork.IntimacaoRepository
                    .GetByIdAsync(id)
                    ?? throw new InvalidOperationException("Intimação não encontrada.");

                // =========================
                // SNAPSHOT ANTES
                // =========================
                var antes = await unitOfWork.IntimacaoRepository
                    .ConsultarIntimacaoCompletaPorIdAsync(id);

                var dadosAntes = new
                {
                    antes.ProcessoId,
                    NumeroProcesso = antes.Processo?.NumeroProcesso,

                    antes.DataIntimacao,
                    antes.TextoIntimacao,

                    antes.StatusTriagem,
                    antes.PecaCabivelId,
                    NomePeca = antes.PecaCabivel?.NomePeca,

                    antes.AdvogadoId,
                    antes.LoteId,

                    antes.StatusCumprimento
                };

                // =========================
                // EXCLUSÃO LÓGICA
                // =========================
                intimacao.Excluido = true;
                intimacao.DataExclusao = DateTime.Now;
                intimacao.UsuarioExclusaoId = usuarioId;

                await unitOfWork.IntimacaoRepository.UpdateAsync(intimacao);

                // =========================
                // SNAPSHOT DEPOIS
                // =========================
                var dadosDepois = new
                {
                    Excluido = true,
                    DataExclusao = intimacao.DataExclusao
                };

                // =========================
                // HISTÓRICO
                // =========================
                await historicoGeralService.RegistrarAsync(
                    TipoEntidade.Intimacao,
                    id,
                    usuarioId,
                    dadosAntes,
                    dadosDepois,
                    "Exclusão lógica da intimação"
                );

                // =========================
                // COMMIT
                // =========================
                await unitOfWork.CommitAsync();

                // =========================
                // RETORNO
                // =========================
                return mapper.Map<IntimacaoResponse>(intimacao);
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<IntimacaoResponse> ModificarAsync(Guid id, IntimacaoUpdateRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                var usuarioId = ObterUsuarioId();

                // =========================
                // BUSCA INTIMAÇÃO
                // =========================
                var intimacao = await unitOfWork.IntimacaoRepository
                    .GetByIdAsync(id)
                    ?? throw new InvalidOperationException("Intimação não encontrada.");

                // =========================
                // SNAPSHOT ANTES
                // =========================
                var intimacaoAntes = await unitOfWork.IntimacaoRepository
                    .ConsultarIntimacaoCompletaPorIdAsync(id);

                var dadosAntes = new
                {
                    intimacaoAntes.ProcessoId,
                    NumeroProcesso = intimacaoAntes.Processo?.NumeroProcesso,

                    intimacaoAntes.DataIntimacao,
                    intimacaoAntes.TextoIntimacao,
                    intimacaoAntes.StatusTriagem,
                    intimacaoAntes.PrazoIndividualCpc,
                    intimacaoAntes.ComplexidadeReal,
                    intimacaoAntes.ObservacaoTriagem,

                    NomePecaCabivel = intimacaoAntes.PecaCabivel?.NomePeca,
                    Advogado = intimacaoAntes.Advogado?.NomeUsuario,
                    Lote = intimacaoAntes.Lote?.NumeroLote
                };

                // =========================
                // CAMPOS BÁSICOS
                // =========================
                intimacao.TextoIntimacao = request.TextoIntimacao?.Trim();
                intimacao.DataIntimacao = request.DataIntimacao;

                // =========================
                // TRIAGEM
                // =========================
                intimacao.PecaCabivelId = request.PecaCabivelId;
                intimacao.ComplexidadeReal = request.ComplexidadeReal;
                intimacao.ObservacaoTriagem = request.ObservacaoTriagem;

                // =========================
                // CÁLCULO DE PRAZO (se peça informada)
                // =========================
                if (request.PecaCabivelId.HasValue)
                {
                    var peca = await unitOfWork.PecaCabivelRepository
                        .GetByIdAsync(request.PecaCabivelId.Value);

                    if (peca == null)
                        throw new InvalidOperationException("Peça cabível não encontrada.");

                    intimacao.PrazoIndividualCpc = DateTime.Now.AddDays(peca.PrazoDias);
                }

                // =========================
                // STATUS TRIAGEM
                // =========================
                intimacao.StatusTriagem = StatusTriagem.Triada;

                intimacao.DataImportacao = intimacao.DataImportacao; // mantém original

                // =========================
                // UPDATE
                // =========================
                await unitOfWork.IntimacaoRepository.UpdateAsync(intimacao);

                // =========================
                // SNAPSHOT DEPOIS
                // =========================
                var intimacaoDepois = await unitOfWork.IntimacaoRepository
                    .ConsultarIntimacaoCompletaPorIdAsync(id);

                var dadosDepois = new
                {
                    intimacaoDepois.ProcessoId,
                    NumeroProcesso = intimacaoDepois.Processo?.NumeroProcesso,

                    intimacaoDepois.DataIntimacao,
                    intimacaoDepois.TextoIntimacao,
                    intimacaoDepois.StatusTriagem,
                    intimacaoDepois.PrazoIndividualCpc,
                    intimacaoDepois.ComplexidadeReal,
                    intimacaoDepois.ObservacaoTriagem,

                    NomePecaCabivel = intimacaoDepois.PecaCabivel?.NomePeca,
                    Advogado = intimacaoDepois.Advogado?.NomeUsuario,
                    Lote = intimacaoDepois.Lote?.NumeroLote
                };

                // =========================
                // HISTÓRICO
                // =========================
                await historicoGeralService.RegistrarAsync(
                    TipoEntidade.Intimacao,
                    id,
                    usuarioId,
                    dadosAntes,
                    dadosDepois,
                    request.ObservacaoTriagem
                );

                // =========================
                // COMMIT
                // =========================
                await unitOfWork.CommitAsync();

                // =========================
                // RETORNO
                // =========================
                var response = mapper.Map<IntimacaoResponse>(intimacao);

                response = response with
                {
                    NumeroProcesso = intimacaoDepois.Processo?.NumeroProcesso ?? ""
                };

                return response;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<IntimacaoResponse> ModificarTriagemAsync(Guid id, IntimacaoTriagemRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                var usuarioId = ObterUsuarioId();

                // =========================
                // BUSCA INTIMAÇÃO
                // =========================
                var intimacao = await unitOfWork.IntimacaoRepository
                    .GetByIdAsync(id)
                    ?? throw new InvalidOperationException("Intimação não encontrada.");

                // =========================
                // SNAPSHOT ANTES
                // =========================
                var antes = await unitOfWork.IntimacaoRepository
                    .ConsultarIntimacaoCompletaPorIdAsync(id);

                var dadosAntes = new
                {
                    antes.PecaCabivelId,
                    NomePeca = antes.PecaCabivel?.NomePeca,

                    antes.PrazoIndividualCpc,
                    antes.ComplexidadeReal,
                    antes.ObservacaoTriagem,
                    antes.StatusTriagem
                };

                // =========================
                // PEÇA CABÍVEL
                // =========================
                if (request.PecaCabivelId.HasValue)
                {
                    var peca = await unitOfWork.PecaCabivelRepository
                        .GetByIdAsync(request.PecaCabivelId.Value)
                        ?? throw new InvalidOperationException("Peça cabível não encontrada.");

                    intimacao.PecaCabivelId = peca.Id;

                    // =========================
                    // PRAZO AUTOMÁTICO
                    // =========================
                    intimacao.PrazoIndividualCpc =
                        DateTime.Now.AddDays(peca.PrazoDias);
                }
                else
                {
                    intimacao.PecaCabivelId = null;
                    intimacao.PrazoIndividualCpc = null;
                }

                // =========================
                // COMPLEXIDADE
                // =========================
                intimacao.ComplexidadeReal = request.ComplexidadeReal;

                // =========================
                // OBSERVAÇÃO
                // =========================
                intimacao.ObservacaoTriagem = request.ObservacaoTriagem?.Trim();

                // =========================
                // STATUS
                // =========================
                intimacao.StatusTriagem = StatusTriagem.Triada;

                // =========================
                // UPDATE
                // =========================
                await unitOfWork.IntimacaoRepository.UpdateAsync(intimacao);

                // =========================
                // SNAPSHOT DEPOIS
                // =========================
                var depois = await unitOfWork.IntimacaoRepository
                    .ConsultarIntimacaoCompletaPorIdAsync(id);

                var dadosDepois = new
                {
                    depois.PecaCabivelId,
                    NomePeca = depois.PecaCabivel?.NomePeca,

                    depois.PrazoIndividualCpc,
                    depois.ComplexidadeReal,
                    depois.ObservacaoTriagem,
                    depois.StatusTriagem
                };

                // =========================
                // HISTÓRICO
                // =========================
                await historicoGeralService.RegistrarAsync(
                    TipoEntidade.Intimacao,
                    id,
                    usuarioId,
                    dadosAntes,
                    dadosDepois,
                    request.ObservacaoTriagem
                );

                // =========================
                // COMMIT
                // =========================
                await unitOfWork.CommitAsync();

                // =========================
                // RETORNO
                // =========================
                var response = mapper.Map<IntimacaoResponse>(intimacao);

                response = response with
                {
                    NumeroProcesso = depois.Processo?.NumeroProcesso ?? ""
                };

                return response;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<ObterIntimacaoRespnse?> ObterPorIdAsync(Guid id)
        {
            var intimacao = await unitOfWork.IntimacaoRepository.ObterCompletoPorIdAsync(id);

            if (intimacao == null)
                return null;

            return mapper.Map<ObterIntimacaoRespnse>(intimacao);
        }
        public async Task<IntimacaoDashboardResponse> GetDashboardAsync()
        {
            return await unitOfWork.IntimacaoRepository
                .GetDashboardAsync();
        }

    }
}

