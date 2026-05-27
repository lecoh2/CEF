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
                var responsavel = await unitOfWork.UsuarioRepository
                    .GetByIdAsync(request.ResponsavelId)
                    ?? throw new InvalidOperationException("Responsável não encontrado.");

                var coordenadorId = ObterUsuarioId()
                    ?? throw new InvalidOperationException("Usuário não autenticado.");

                var coordenador = await unitOfWork.UsuarioRepository
                    .GetByIdAsync(coordenadorId)
                    ?? throw new InvalidOperationException("Coordenador não encontrado.");

                // 🔥 AGORA CORRETO (sem GetAll)
                var ultimoLote = await unitOfWork.LoteTrabalhoRepository
                    .ObterUltimoLoteAsync();

                var sequencial = 1;

                if (!string.IsNullOrWhiteSpace(ultimoLote?.NumeroLote))
                {
                    var partes = ultimoLote.NumeroLote.Split('-');

                    if (int.TryParse(partes.Last(), out var num))
                        sequencial = num + 1;
                }

                var numeroLote = $"LOT-{DateTime.Now.Year}-{sequencial:D4}";

                var lote = new LoteTrabalho
                {
                    Id = Guid.NewGuid(),
                    NumeroLote = numeroLote,
                    ResponsavelId = request.ResponsavelId,
                    CoordenadorId = coordenadorId,
                    DataCriacao = DateTime.Now,
                    DataPrazoLote = request.DataPrazoLote,
                    Status = StatusLote.Aberto
                };

                if (lote.DataPrazoLote.Date < DateTime.Today)
                    throw new InvalidOperationException("Prazo inválido.");

                await unitOfWork.LoteTrabalhoRepository.AddAsync(lote);
                await unitOfWork.CommitAsync();

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
                var usuarioId = ObterUsuarioId()
                    ?? throw new InvalidOperationException("Usuário não autenticado.");

                var lote = await unitOfWork.LoteTrabalhoRepository
                    .GetByIdAsync(id)
                    ?? throw new InvalidOperationException("Lote não encontrado.");

                var antes = new
                {
                    lote.NumeroLote,
                    lote.ResponsavelId,
                    lote.CoordenadorId,
                    lote.DataPrazoLote,
                    lote.Status
                };

                lote.Excluido = true;
                lote.DataExclusao = DateTime.Now;
                lote.Status = StatusLote.Cancelado;

                await unitOfWork.LoteTrabalhoRepository.UpdateAsync(lote);

                await unitOfWork.CommitAsync();

                await historicoGeralService.RegistrarAsync(
                    TipoEntidade.LoteTrabalho,
                    lote.Id,
                    usuarioId,
                    antes,
                    new { lote.Excluido, lote.DataExclusao, lote.Status },
                    "Exclusão lógica de lote"
                );

                return new LoteTrabalhoResponse
                {
                    Id = lote.Id,
                    NumeroLote = lote.NumeroLote,
                    ResponsavelId = lote.ResponsavelId,
                    CoordenadorId = lote.CoordenadorId,
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

        public async Task<LoteTrabalhoResponse> ModificarAsync(Guid id, LoteTrabalhoUpdateRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                var usuarioId = ObterUsuarioId()
                    ?? throw new InvalidOperationException("Usuário não autenticado.");

                var lote = await unitOfWork.LoteTrabalhoRepository
                    .GetByIdAsync(id)
                    ?? throw new InvalidOperationException("Lote não encontrado.");

                var responsavel = await unitOfWork.UsuarioRepository
                    .GetByIdAsync(request.ResponsavelId)
                    ?? throw new InvalidOperationException("Responsável não encontrado.");

                var coordenador = await unitOfWork.UsuarioRepository
                    .GetByIdAsync(usuarioId)
                    ?? throw new InvalidOperationException("Coordenador não encontrado.");

                var antes = new
                {
                    lote.NumeroLote,
                    lote.ResponsavelId,
                    lote.CoordenadorId,
                    lote.DataPrazoLote,
                    lote.Status
                };

                if (request.DataPrazoLote.Date < DateTime.Today)
                    throw new InvalidOperationException("Prazo inválido.");

                lote.ResponsavelId = request.ResponsavelId;
                lote.CoordenadorId = usuarioId;
                lote.DataPrazoLote = request.DataPrazoLote;
                lote.Status = request.Status;

                await unitOfWork.LoteTrabalhoRepository.UpdateAsync(lote);
                await unitOfWork.CommitAsync();

                await historicoGeralService.RegistrarAsync(
                    TipoEntidade.LoteTrabalho,
                    lote.Id,
                    usuarioId,
                    antes,
                    new
                    {
                        lote.NumeroLote,
                        lote.ResponsavelId,
                        lote.CoordenadorId,
                        lote.DataPrazoLote,
                        lote.Status
                    },
                    "Atualização de lote"
                );

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

        public async Task<ObterLoteTrabalhoResponse?> ObterPorIdAsync(Guid id)
        {
            var lote = await unitOfWork.LoteTrabalhoRepository
                .ObterCompletoPorIdAsync(id);

            return lote == null
                ? null
                : mapper.Map<ObterLoteTrabalhoResponse>(lote);
        }

        public async Task<List<LoteTrabalhoResponse>> ListarAtivosAsync()
        {
            var lista = await unitOfWork.LoteTrabalhoRepository.GetAllAtivosAsync();

            return mapper.Map<List<LoteTrabalhoResponse>>(lista);
        }
        public async Task<List<LoteTrabalhoResponse>> BuscarPorResponsavelAsync(Guid responsavelId)
        {
            var lista = await unitOfWork.LoteTrabalhoRepository
                .ObterPorResponsavelAsync(responsavelId);

            return mapper.Map<List<LoteTrabalhoResponse>>(lista);
        }
    }
}

