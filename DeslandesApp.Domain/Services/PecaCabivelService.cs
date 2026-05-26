using AutoMapper;
using DeslandesApp.Domain.Interfaces.Repositories;
using DeslandesApp.Domain.Interfaces.Services;
using DeslandesApp.Domain.Models.Dtos.Requests.PecaCabivel;
using DeslandesApp.Domain.Models.Dtos.Responses.PecaCabivel;
using DeslandesApp.Domain.Models.Dtos.Responses.Processo;
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
    public class PecaCabivelService(
 IUnitOfWork unitOfWork,
 IMapper mapper,
 IHttpContextAccessor httpContextAccessor,
 IHistoricoGeralService historicoGeralService, INotificacaoService notificacaoService
) : BaseService(httpContextAccessor), IPecaCabivelService
    {
        public async Task<PecaCabivelResponse> AdicionarAsync(PecaCabivelRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                var usuarioId = ObterUsuarioId()
                    ?? throw new InvalidOperationException("Usuário não autenticado.");

                var peca = mapper.Map<PecaCabivel>(request);

                peca.NomePeca = peca.NomePeca?.Trim().ToUpper();

                var validator = new PecaCabivelValidator();
                var result = validator.Validate(peca);

                if (!result.IsValid)
                    throw new ValidationException(result.Errors);

                // ✔ usa método correto do repository
                var existe = await unitOfWork.PecaCabivelRepository
                    .ExistePorNomeAsync(peca.NomePeca);

                if (existe)
                    throw new InvalidOperationException("Já existe uma peça com esse nome.");

                await unitOfWork.PecaCabivelRepository.AddAsync(peca);

                await unitOfWork.CommitAsync();

                return mapper.Map<PecaCabivelResponse>(peca);
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public Task<PageResult<PecaCabivelResponse>> ConsultarAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public async Task<PageResult<PecaCabivelPaginacaoResponse>> ConsultarPecaCabivelPaginacaoAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {

            var paged = await unitOfWork.PecaCabivelRepository
                .GetPecaCabivelPaginacaoAsync(pageNumber, pageSize, searchTerm);

            if (paged == null || !paged.Items.Any())
            {
                return new PageResult<PecaCabivelPaginacaoResponse>
                {
                    Items = new List<PecaCabivelPaginacaoResponse>(),
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

        public async Task<PecaCabivelResponse> ExcluirAsync(Guid id)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                var usuarioId = ObterUsuarioId()
                    ?? throw new InvalidOperationException("Usuário não autenticado.");

                var peca = await unitOfWork.PecaCabivelRepository
                    .GetByIdAsync(id)
                    ?? throw new InvalidOperationException("Peça cabível não encontrada.");

                if (peca.Excluido)
                    throw new InvalidOperationException("Peça já excluída.");

                var dadosAntes = new
                {
                    peca.NomePeca,
                    peca.Excluido
                };

                peca.Excluido = true;
                peca.DataExclusao = DateTime.Now;
                peca.UsuarioExclusaoId = usuarioId;

                await unitOfWork.PecaCabivelRepository.UpdateAsync(peca);

                await historicoGeralService.RegistrarAsync(
                    TipoEntidade.PecaCabivel,
                    peca.Id,
                    usuarioId,
                    dadosAntes,
                    new { peca.Excluido, peca.DataExclusao },
                    "Exclusão da peça cabível"
                );

                await unitOfWork.CommitAsync();

                return mapper.Map<PecaCabivelResponse>(peca);
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
                var usuarioId = ObterUsuarioId()
                    ?? throw new InvalidOperationException("Usuário não autenticado.");

                var peca = await unitOfWork.PecaCabivelRepository
                    .GetByIdAsync(id)
                    ?? throw new InvalidOperationException("Peça cabível não encontrada.");

                var dadosAntes = new
                {
                    peca.NomePeca,
                    peca.PrazoDias,
                    peca.SugestaoComplexidadePadrao
                };

                peca.NomePeca = request.NomePeca?.Trim().ToUpper();
                peca.PrazoDias = request.PrazoDias;
                peca.SugestaoComplexidadePadrao = request.SugestaoComplexidadePadrao;

                var validator = new PecaCabivelValidator();
                var result = validator.Validate(peca);

                if (!result.IsValid)
                    throw new ValidationException(result.Errors);

                // ✔ usa método correto do repository
                var existe = await unitOfWork.PecaCabivelRepository
                    .ExistePorNomeAsync(peca.NomePeca, id);

                if (existe)
                    throw new InvalidOperationException("Já existe uma peça com esse nome.");

                await unitOfWork.PecaCabivelRepository.UpdateAsync(peca);

                var dadosDepois = new
                {
                    peca.NomePeca,
                    peca.PrazoDias,
                    peca.SugestaoComplexidadePadrao
                };

                await historicoGeralService.RegistrarAsync(
                    TipoEntidade.PecaCabivel,
                    peca.Id,
                    usuarioId,
                    dadosAntes,
                    dadosDepois,
                    "Atualização de peça cabível"
                );

                await unitOfWork.CommitAsync();

                return mapper.Map<PecaCabivelResponse>(peca);
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
        public async Task<ObterPecacabivelResponse?> ObterPorIdAsync(Guid id)
        {
            var entity = await unitOfWork.PecaCabivelRepository.ObterCompletoPorIdAsync(id);

            return entity == null
                ? null
                : mapper.Map<ObterPecacabivelResponse>(entity);
        }
        public async Task<List<PecaCabivelResponse>> ListarAtivosAsync()
        {
            var lista = await unitOfWork.PecaCabivelRepository.GetAllAtivosAsync();

            return mapper.Map<List<PecaCabivelResponse>>(lista);
        }
        public async Task<List<PecaCabivelResponse>> BuscarAsync(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return new List<PecaCabivelResponse>();

            var result = await unitOfWork.PecaCabivelRepository.BuscarPorTermoAsync(term.Trim());

            return mapper.Map<List<PecaCabivelResponse>>(result);
        }
    }
}
