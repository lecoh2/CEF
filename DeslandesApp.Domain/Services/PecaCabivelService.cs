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
        public async Task<PecaCabivelResponse> AdicionarAsync(
       PecaCabivelRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                // =========================
                // DTO -> ENTIDADE
                // =========================
                var pecaCabivel =
                    mapper.Map<PecaCabivel>(request);

                // =========================
                // NORMALIZAÇÃO
                // =========================
                pecaCabivel.NomePeca =
                    pecaCabivel.NomePeca
                        ?.Trim()
                        .ToUpper();

                // =========================
                // VALIDAÇÃO
                // =========================
                var validator =
                    new PecaCabivelValidator();

                var result =
                    validator.Validate(pecaCabivel);

                if (!result.IsValid)
                {
                    throw new ValidationException(
                        result.Errors
                    );
                }

                // =========================
                // DUPLICIDADE
                // =========================
                var existente =
                    await unitOfWork
                        .PecaCabivelRepository
                        .GetByAsync(x =>
                            x.NomePeca ==
                            pecaCabivel.NomePeca
                        );

                if (existente != null)
                {
                    throw new InvalidOperationException(
                        "Já existe uma peça cadastrada com este nome."
                    );
                }

                // =========================
                // SALVAR
                // =========================
                await unitOfWork
                    .PecaCabivelRepository
                    .AddAsync(pecaCabivel);

                // =========================
                // COMMIT
                // =========================
                await unitOfWork.CommitAsync();

                // =========================
                // RETORNO
                // =========================
                return mapper.Map<PecaCabivelResponse>(
                    pecaCabivel
                );
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
                var peca = await unitOfWork
                    .PecaCabivelRepository
                    .GetByIdAsync(id);

                if (peca == null)
                {
                    throw new InvalidOperationException(
                        "Peça cabível não encontrada."
                    );
                }

                if (peca.Excluido)
                {
                    throw new InvalidOperationException(
                        "Peça já excluída."
                    );
                }

                peca.Excluido = true;

                peca.DataExclusao = DateTime.Now;

                peca.UsuarioExclusaoId =
                    ObterUsuarioId();

                await unitOfWork
                    .PecaCabivelRepository
                    .UpdateAsync(peca);

                // =========================
                // HISTÓRICO
                // =========================
                await historicoGeralService
                    .RegistrarAsync(
                        TipoEntidade.PecaCabivel,
                        peca.Id,
                        ObterUsuarioId(),
                        new
                        {
                            Status = "ATIVO"
                        },
                        new
                        {
                            Status = "EXCLUÍDO"
                        },
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
        public async Task<ObterPecacabivelResponse?> ObterPorIdAsync(Guid id)
        {
            var pecaoCabivel = await unitOfWork.PecaCabivelRepository.ObterCompletoPorIdAsync(id);

            if (pecaoCabivel == null)
                return null;

            return mapper.Map<ObterPecacabivelResponse>(pecaoCabivel);        }

     
    }
}
