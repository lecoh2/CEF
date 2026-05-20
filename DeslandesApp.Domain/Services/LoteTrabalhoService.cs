using AutoMapper;
using DeslandesApp.Domain.Interfaces.Repositories;
using DeslandesApp.Domain.Interfaces.Services;
using DeslandesApp.Domain.Models.Dtos.Requests.LoteTrabalho;
using DeslandesApp.Domain.Models.Dtos.Requests.PecaCabivel;
using DeslandesApp.Domain.Models.Dtos.Responses.LoteTrabalho;
using DeslandesApp.Domain.Models.Dtos.Responses.PecaCabivel;
using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Domain.Models.Enum;
using DeslandesApp.Domain.Models.Enum.DeslandesApp.Domain.Models.Enum;
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
    public class LoteTrabalhoService(
      IUnitOfWork unitOfWork,
      IMapper mapper,
      IHttpContextAccessor httpContextAccessor,
      IHistoricoGeralService historicoGeralService
  ) : BaseService(httpContextAccessor),
      ILoteTrabalhoService
    {
        public async Task<LoteTrabalhoResponse> AdicionarAsync(LoteTrabalhoRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                // =========================
                // VALIDA USUÁRIO RESPONSÁVEL
                // =========================
                var responsavel = await unitOfWork
                    .UsuarioRepository
                    .GetByIdAsync(request.ResponsavelId);

                if (responsavel == null)
                    throw new InvalidOperationException("Responsável não encontrado.");

                // =========================
                // COORDENADOR (LOGADO)
                // =========================
                var coordenadorId = ObterUsuarioId();

                if (!coordenadorId.HasValue)
                    throw new InvalidOperationException("Usuário não autenticado.");

                var coordenador = await unitOfWork
                    .UsuarioRepository
                    .GetByIdAsync(coordenadorId.Value);

                if (coordenador == null)
                    throw new InvalidOperationException("Coordenador não encontrado.");

                // =========================
                // GERAR NÚMERO DO LOTE
                // =========================
                var ultimoLote = (await unitOfWork
                    .LoteTrabalhoRepository
                    .GetAllAsync())
                    .OrderByDescending(x => x.DataCriacao)
                    .FirstOrDefault();

                int sequencial = 1;

                if (ultimoLote != null &&
                    ultimoLote.NumeroLote.Contains("-"))
                {
                    var partes = ultimoLote.NumeroLote.Split('-');
                    if (int.TryParse(partes.Last(), out var num))
                        sequencial = num + 1;
                }

                var numeroLote = $"LOT-{DateTime.Now.Year}-{sequencial:D4}";

                // =========================
                // CRIA ENTIDADE
                // =========================
                var lote = new LoteTrabalho
                {
                    Id = Guid.NewGuid(),
                    NumeroLote = numeroLote,
                    ResponsavelId = request.ResponsavelId,
                    CoordenadorId = coordenadorId.Value,
                    DataCriacao = DateTime.Now,
                    DataPrazoLote = request.DataPrazoLote,
                    Status = StatusLote.Aberto
                };

                // =========================
                // VALIDAÇÃO (se quiser Fluent depois pode plugar aqui)
                // =========================
                if (lote.DataPrazoLote < DateTime.Today)
                    throw new InvalidOperationException("Prazo do lote não pode ser menor que hoje.");

                // =========================
                // SALVA
                // =========================
                await unitOfWork.LoteTrabalhoRepository.AddAsync(lote);

                await unitOfWork.CommitAsync();

                // =========================
                // RETORNO (manual ou AutoMapper)
                // =========================
                return new LoteTrabalhoResponse
                {
                    Id = lote.Id,
                    NumeroLote = lote.NumeroLote,
                    ResponsavelId = lote.ResponsavelId,
                    ResponsavelNome = responsavel.NomeUsuario,
                    CoordenadorId = lote.CoordenadorId,
                    CoordenadorNome = coordenador.NomeUsuario,
                    DataCriacao = lote.DataCriacao,
                    DataPrazoLote = lote.DataPrazoLote,
                    Status = lote.Status,
                    QuantidadeIntimacoes = 0
                };
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public Task<PageResult<LoteTrabalhoResponse>> ConsultarAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public async Task<PageResult<LoteTrabalhoPaginacaoResponse>> ConsultarLoteTrabalhoPaginacaoAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            var paged = await unitOfWork.LoteTrabalhoRepository
                   .GetLoteTrabalhoPaginacaoAsync(pageNumber, pageSize, searchTerm);

            if (paged == null || !paged.Items.Any())
            {
                return new PageResult<LoteTrabalhoPaginacaoResponse>
                {
                    Items = new List<LoteTrabalhoPaginacaoResponse>(),
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

        public async Task<LoteTrabalhoResponse> ExcluirAsync(Guid id)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                var lote = await unitOfWork
                    .LoteTrabalhoRepository
                    .GetByIdAsync(id)
                    ?? throw new InvalidOperationException("Lote não encontrado.");

                var usuarioId = ObterUsuarioId();

                if (!usuarioId.HasValue)
                    throw new InvalidOperationException("Usuário não autenticado.");

                // =========================
                // SNAPSHOT
                // =========================
                var loteAntes = new
                {
                    lote.NumeroLote,
                    lote.ResponsavelId,
                    lote.CoordenadorId,
                    lote.DataPrazoLote,
                    lote.Status
                };

                // =========================
                // SOFT DELETE
                // =========================
                lote.Excluido = true;
                lote.DataExclusao = DateTime.Now;

                lote.Status = StatusLote.Cancelado; // se tiver esse status

                await unitOfWork.LoteTrabalhoRepository.UpdateAsync(lote);

                await unitOfWork.CommitAsync();

                // =========================
                // HISTÓRICO
                // =========================
                await historicoGeralService.RegistrarAsync(
                    TipoEntidade.LoteTrabalho,
                    lote.Id,
                    usuarioId.Value,
                    loteAntes,
                    new
                    {
                        lote.Excluido,
                        lote.DataExclusao,
                        lote.Status
                    },
                    "Exclusão lógica de lote de trabalho"
                );

                return new LoteTrabalhoResponse
                {
                    Id = lote.Id,
                    NumeroLote = lote.NumeroLote,
                    ResponsavelId = lote.ResponsavelId,
                    ResponsavelNome = "",
                    CoordenadorId = lote.CoordenadorId,
                    CoordenadorNome = "",
                    DataCriacao = lote.DataCriacao,
                    DataPrazoLote = lote.DataPrazoLote,
                    Status = lote.Status,
                    QuantidadeIntimacoes = 0
                };
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<LoteTrabalhoResponse> ModificarAsync(
      Guid id,
      LoteTrabalhoUpdateRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                // =========================
                // BUSCA LOTE
                // =========================
                var lote = await unitOfWork
                    .LoteTrabalhoRepository
                    .GetByIdAsync(id)
                    ?? throw new InvalidOperationException("Lote não encontrado.");

                var usuarioId = ObterUsuarioId();

                if (!usuarioId.HasValue)
                    throw new InvalidOperationException("Usuário não autenticado.");

                // =========================
                // SNAPSHOT ANTES
                // =========================
                var loteAntes = new
                {
                    lote.NumeroLote,
                    lote.ResponsavelId,
                    lote.CoordenadorId,
                    lote.DataPrazoLote,
                    lote.Status
                };

                // =========================
                // VALIDA RESPONSÁVEL
                // =========================
                var responsavel = await unitOfWork
                    .UsuarioRepository
                    .GetByIdAsync(request.ResponsavelId);

                if (responsavel == null)
                    throw new InvalidOperationException("Responsável não encontrado.");

                // =========================
                // COORDENADOR (LOGADO)
                // =========================
                var coordenador = await unitOfWork
                    .UsuarioRepository
                    .GetByIdAsync(usuarioId.Value);

                if (coordenador == null)
                    throw new InvalidOperationException("Coordenador não encontrado.");

                // =========================
                // ATUALIZAÇÃO
                // =========================
                lote.ResponsavelId = request.ResponsavelId;
                lote.CoordenadorId = usuarioId.Value;
                lote.DataPrazoLote = request.DataPrazoLote;
                lote.Status = request.Status;

                if (lote.DataPrazoLote < DateTime.Today)
                    throw new InvalidOperationException("Prazo do lote não pode ser menor que hoje.");

                await unitOfWork.LoteTrabalhoRepository.UpdateAsync(lote);

                // =========================
                // COMMIT
                // =========================
                await unitOfWork.CommitAsync();

                // =========================
                // HISTÓRICO
                // =========================
                await historicoGeralService.RegistrarAsync(
                    TipoEntidade.LoteTrabalho,
                    lote.Id,
                    usuarioId.Value,
                    loteAntes,
                    new
                    {
                        lote.NumeroLote,
                        lote.ResponsavelId,
                        lote.CoordenadorId,
                        lote.DataPrazoLote,
                        lote.Status
                    },
                    "Alteração de lote de trabalho"
                );

                // =========================
                // RETORNO
                // =========================
                return new LoteTrabalhoResponse
                {
                    Id = lote.Id,
                    NumeroLote = lote.NumeroLote,
                    ResponsavelId = lote.ResponsavelId,
                    ResponsavelNome = responsavel.NomeUsuario,
                    CoordenadorId = lote.CoordenadorId,
                    CoordenadorNome = coordenador.NomeUsuario,
                    DataCriacao = lote.DataCriacao,
                    DataPrazoLote = lote.DataPrazoLote,
                    Status = lote.Status,
                    QuantidadeIntimacoes = 0
                };
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<PecaCabivelResponse> ModificarAsync(
          Guid id,
          PecaCabivelUpdateRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                // =========================
                // BUSCA PEÇA
                // =========================
                var peca = await unitOfWork
                    .PecaCabivelRepository
                    .GetByIdAsync(id);

                if (peca == null)
                {
                    throw new InvalidOperationException(
                        "Peça cabível não encontrada."
                    );
                }

                // =========================
                // SNAPSHOT ANTES
                // =========================
                var dadosAntes = new
                {
                    peca.NomePeca,
                    peca.PrazoDias,
                    Complexidade =
                        peca.SugestaoComplexidadePadrao.ToString()
                };

                // =========================
                // NORMALIZAÇÃO
                // =========================
                peca.NomePeca = request.NomePeca
                    ?.Trim()
                    ?.ToUpper();

                peca.PrazoDias = request.PrazoDias;

                peca.SugestaoComplexidadePadrao =
                    request.SugestaoComplexidadePadrao;

                // =========================
                // VALIDAÇÃO
                // =========================
                var validator = new PecaCabivelValidator();

                var result = validator.Validate(peca);

                if (!result.IsValid)
                {
                    throw new ValidationException(
                        result.Errors
                    );
                }

                // =========================
                // DUPLICIDADE
                // =========================
                var existente = await unitOfWork
                    .PecaCabivelRepository
                    .GetByAsync(x =>
                        x.Id != id
                        &&
                        x.NomePeca == peca.NomePeca
                    );

                if (existente != null)
                {
                    throw new InvalidOperationException(
                        "Já existe uma peça cadastrada com esse nome."
                    );
                }

                // =========================
                // UPDATE
                // =========================
                await unitOfWork
                    .PecaCabivelRepository
                    .UpdateAsync(peca);

                // =========================
                // SNAPSHOT DEPOIS
                // =========================
                var dadosDepois = new
                {
                    peca.NomePeca,
                    peca.PrazoDias,
                    Complexidade =
                        peca.SugestaoComplexidadePadrao.ToString()
                };

                // =========================
                // HISTÓRICO
                // =========================
                await historicoGeralService
                    .RegistrarAsync(
                        TipoEntidade.PecaCabivel,
                        peca.Id,
                        ObterUsuarioId(),
                        dadosAntes,
                        dadosDepois,
                        "Atualização de peça cabível"
                    );

                // =========================
                // COMMIT
                // =========================
                await unitOfWork.CommitAsync();

                // =========================
                // RETORNO
                // =========================
                return mapper.Map<PecaCabivelResponse>(peca);
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
        public async Task<ObterLoteTrabalhoResponse?> ObterPorIdAsync(Guid id)
        {
            var processo = await unitOfWork.LoteTrabalhoRepository.ObterCompletoPorIdAsync(id);

            if (processo == null)
                return null;

            return mapper.Map<ObterLoteTrabalhoResponse>(processo);
        }


    }
}

