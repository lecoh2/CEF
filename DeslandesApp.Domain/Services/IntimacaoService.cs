using AutoMapper;
using ClosedXML.Excel;
using DeslandesApp.Domain.Interfaces.Repositories;
using DeslandesApp.Domain.Interfaces.Services;
using DeslandesApp.Domain.Models.Dtos.Requests.Intimacao;
using DeslandesApp.Domain.Models.Dtos.Responses.Intimacao;
using DeslandesApp.Domain.Models.Dtos.Responses.LoteTrabalho;
using DeslandesApp.Domain.Models.Dtos.Responses.PecaCabivel;
using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Domain.Models.Enum;
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
    IHistoricoGeralService historicoGeralService, IProcessoService processoService 
) : BaseService(httpContextAccessor),
    IIntimacaoService
    {
        public async Task<IntimacaoResponse> AdicionarAsync(IntimacaoRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                if (request.ProcessoId == Guid.Empty)
                    throw new InvalidOperationException("Processo obrigatório.");

                if (request.DataIntimacao == default)
                    throw new InvalidOperationException("Data obrigatória.");

                var processo = await unitOfWork.ProcessoRepository.GetByIdAsync(request.ProcessoId)
                    ?? throw new InvalidOperationException("Processo não encontrado.");

                var duplicado = await unitOfWork.IntimacaoRepository.GetByAsync(x =>
                    x.ProcessoId == request.ProcessoId &&
                    x.DataIntimacao.Date == request.DataIntimacao.Date);

                if (duplicado != null)
                    throw new InvalidOperationException("Intimação já cadastrada para esta data.");

                var entidade = mapper.Map<Intimacao>(request);

                entidade.TextoIntimacao = request.TextoIntimacao?.Trim();
                entidade.DataImportacao = DateTime.Now;
                entidade.StatusTriagem = StatusTriagem.PendenteDeLeitura;
                entidade.StatusCumprimento = StatusCumprimento.Pendente;

                await unitOfWork.IntimacaoRepository.AddAsync(entidade);
                await unitOfWork.CommitAsync();

                var response = mapper.Map<IntimacaoResponse>(entidade);

                return response with
                {
                    NumeroProcesso = processo.NumeroProcesso
                };
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<PageResult<IntimacaoResponse>> ConsultarAsync(
         int pageNumber,
         int pageSize)
        {
            return new PageResult<IntimacaoResponse>
            {
                Items = [],
                TotalCount = 0,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
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

                var intimacao = await unitOfWork.IntimacaoRepository.GetByIdAsync(id)
                    ?? throw new InvalidOperationException("Não encontrada.");

                var antes = await unitOfWork.IntimacaoRepository
                    .ConsultarIntimacaoCompletaPorIdAsync(id);

                var snapshotAntes = new
                {
                    antes.TextoIntimacao,
                    antes.DataIntimacao,
                    antes.StatusTriagem,
                    antes.PecaCabivelId
                };

                intimacao.TextoIntimacao = request.TextoIntimacao?.Trim();
                intimacao.DataIntimacao = request.DataIntimacao;
                intimacao.PecaCabivelId = request.PecaCabivelId;
                intimacao.ObservacaoTriagem = request.ObservacaoTriagem?.Trim();

                intimacao.StatusTriagem = StatusTriagem.Triada;

                if (request.PecaCabivelId.HasValue)
                {
                    var peca = await unitOfWork.PecaCabivelRepository.GetByIdAsync(request.PecaCabivelId.Value)
                        ?? throw new InvalidOperationException("Peça não encontrada.");

                    intimacao.PrazoIndividualCpc = DateTime.Now.AddDays(peca.PrazoDias);
                }

                await unitOfWork.IntimacaoRepository.UpdateAsync(intimacao);

                var depois = await unitOfWork.IntimacaoRepository
                    .ConsultarIntimacaoCompletaPorIdAsync(id);

                await historicoGeralService.RegistrarAsync(
                    TipoEntidade.Intimacao,
                    id,
                    usuarioId,
                    snapshotAntes,
                    depois,
                    "Atualização de intimação"
                );

                await unitOfWork.CommitAsync();

                var response = mapper.Map<IntimacaoResponse>(intimacao);

                return response with
                {
                    NumeroProcesso = depois.Processo?.NumeroProcesso ?? ""
                };
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
        public async Task<ResultadoImportacaoIntimacaoResponse> ImportarAsync(IFormFile arquivo)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                var response = new ResultadoImportacaoIntimacaoResponse();

                if (arquivo == null || arquivo.Length == 0)
                    throw new Exception("Arquivo inválido.");

                using var stream = new MemoryStream();
                await arquivo.CopyToAsync(stream);
                stream.Position = 0;

                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RowsUsed().Skip(1);

                response = response with
                {
                    TotalLinhas = rows.Count()
                };

                foreach (var row in rows)
                {
                    var numeroProcesso = row.Cell(1).GetString().Trim();
                    var dataIntimacao = row.Cell(2).GetDateTime();
                    var texto = row.Cell(3).GetString().Trim();

                    if (string.IsNullOrWhiteSpace(numeroProcesso) || string.IsNullOrWhiteSpace(texto))
                        continue;

                    var processo = await unitOfWork.ProcessoRepository
                        .ObterPorNumeroAsync(numeroProcesso);

                    if (processo == null)
                        continue;

                    var existe = await unitOfWork.IntimacaoRepository
                        .ExisteDuplicidadeAsync(processo.Id, dataIntimacao, texto);

                    if (existe)
                        continue;

                    var intimacao = new Intimacao
                    {
                        ProcessoId = processo.Id,
                        DataImportacao = DateTime.Now,
                        DataIntimacao = dataIntimacao,
                        TextoIntimacao = texto,
                        StatusTriagem = StatusTriagem.PendenteDeLeitura,
                        StatusCumprimento = StatusCumprimento.Pendente
                    };

                    await unitOfWork.IntimacaoRepository.AddAsync(intimacao);

                    response.Importadas.Add(new IntimacaoImportadaResponse
                    {
                        IntimacaoId = intimacao.Id,
                        NumeroProcesso = processo.NumeroProcesso,
                        DataIntimacao = dataIntimacao,
                        TextoIntimacao = texto
                    });
                }

                await unitOfWork.CommitAsync();
                return response;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
        public async Task DistribuirParaAdvogadoAsync(Guid intimacaoId, Guid advogadoId)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                var usuarioId = ObterUsuarioId()
                    ?? throw new InvalidOperationException("Usuário não autenticado.");

                var intimacao = await unitOfWork.IntimacaoRepository.GetByIdAsync(intimacaoId)
                    ?? throw new InvalidOperationException("Intimação não encontrada.");

                var advogado = await unitOfWork.UsuarioRepository.GetByIdAsync(advogadoId)
                    ?? throw new InvalidOperationException("Advogado não encontrado.");

                if (intimacao.LoteId.HasValue)
                    throw new InvalidOperationException("Já pertence a lote.");

                var antes = new { intimacao.AdvogadoId, intimacao.LoteId };

                intimacao.AdvogadoId = advogadoId;
                intimacao.LoteId = null;
                intimacao.StatusTriagem = StatusTriagem.Triada;

                await unitOfWork.IntimacaoRepository.UpdateAsync(intimacao);

                await historicoGeralService.RegistrarAsync(
                    TipoEntidade.Intimacao,
                    intimacao.Id,
                    usuarioId,
                    antes,
                    new { intimacao.AdvogadoId, intimacao.LoteId },
                    "Distribuição para advogado"
                );

                await unitOfWork.CommitAsync();
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
        public async Task<List<Intimacao>> GetNaoTriadasAsync()
        {
            return await unitOfWork.IntimacaoRepository.GetNaoTriadasAsync();
        }

        public async Task<List<Intimacao>> BuscarPorProcessoAsync(string numeroProcesso)
        {
            if (string.IsNullOrWhiteSpace(numeroProcesso))
                throw new InvalidOperationException("Número do processo inválido.");

            return await unitOfWork.IntimacaoRepository.BuscarPorProcessoAsync(numeroProcesso);
        }

        public async Task<int> CountPorStatusAsync(StatusTriagem status)
        {
            return await unitOfWork.IntimacaoRepository.CountPorStatusTriagemAsync(status);
        }

        public async Task<int> CountPorLoteAsync(Guid loteId)
        {
            return await unitOfWork.IntimacaoRepository.CountPorLoteAsync(loteId);
        }

        public async Task<int> CountPorAdvogadoAsync(Guid advogadoId)
        {
            return await unitOfWork.IntimacaoRepository.CountPorAdvogadoAsync(advogadoId);
        }


    }
}

