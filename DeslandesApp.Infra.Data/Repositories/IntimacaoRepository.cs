using DeslandesApp.Domain.Interfaces.Repositories;
using DeslandesApp.Domain.Models.Dtos.Responses.Intimacao;

using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Domain.Models.Enum;
using DeslandesApp.Domain.Models.Enum.DeslandesApp.Domain.Models.Enum;
using DeslandesApp.Domain.Utils;
using DeslandesApp.Infra.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Infra.Data.Repositories
{
    public class IntimacaoRepository(DataContext dataContext) : BaseRepository<Intimacao, Guid>(dataContext), IIntimacaoRepository
    {
        public async Task<Intimacao?> ConsultarIntimacaoCompletaPorIdAsync(Guid id)
        {
            return await dataContext.Intimacao
                .AsNoTracking()
                .Where(x => x.Id == id)

                .Include(x => x.Processo)

                .Include(x => x.PecaCabivel)

                .Include(x => x.Advogado)

                .Include(x => x.Lote)

                .FirstOrDefaultAsync();
        }

        public async Task<PageResult<IntimacaoPaginacaoResponse>> GetIntimacaoPaginacaoAsync(
     int pageNumber,
     int pageSize,
     string? searchTerm = null)
        {
            var query = dataContext.Intimacao
                .AsNoTracking()
                .AsQueryable();

            // =========================
            // FILTRO
            // =========================
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();

                query = query.Where(x =>
                    x.Processo.NumeroProcesso.Contains(term) ||
                    x.Advogado.NomeUsuario.Contains(term) ||
                    (x.PecaCabivel != null && x.PecaCabivel.NomePeca.Contains(term))
                );
            }

            // =========================
            // TOTAL
            // =========================
            var totalCount = await query.CountAsync();

            // =========================
            // PAGINAÇÃO
            // =========================
            var items = await query
                .OrderByDescending(x => x.DataIntimacao)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new IntimacaoPaginacaoResponse
                {
                    Id = x.Id,
                    NumeroProcesso = x.Processo.NumeroProcesso,
                    DataIntimacao = x.DataIntimacao,
                    StatusTriagem = x.StatusTriagem,
                    NomePecaCabivel = x.PecaCabivel != null
                        ? x.PecaCabivel.NomePeca
                        : null
                })
                .ToListAsync();

            return new PageResult<IntimacaoPaginacaoResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        public async Task<Intimacao?> ObterCompletoPorIdAsync(Guid id)
        {
            return await dataContext.Intimacao
                .AsNoTracking()
                .Where(x => x.Id == id)

                // PROCESSO
                .Include(x => x.Processo)
                    .ThenInclude(p => p.Vara)

                // PEÇA CABÍVEL
                .Include(x => x.PecaCabivel)

                // ADVOGADO
                .Include(x => x.Advogado)

                // LOTE
                .Include(x => x.Lote)

                .FirstOrDefaultAsync();
        }
        public async Task<PageResult<IntimacaoPaginacaoResponse>> ConsultarParaTriagemAsync(
    DateTime? data,
    StatusTriagem? status,
    int pageNumber,
    int pageSize)
        {
            var query = dataContext.Intimacao
                .AsNoTracking()
                .AsQueryable();

            if (data.HasValue)
                query = query.Where(x => x.DataIntimacao.Date == data.Value.Date);

            if (status.HasValue)
                query = query.Where(x => x.StatusTriagem == status);

            var total = await query.CountAsync();

            var items = await query
                .Include(x => x.Processo)
                .Include(x => x.PecaCabivel)
                .OrderByDescending(x => x.DataIntimacao)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new IntimacaoPaginacaoResponse
                {
                    Id = x.Id,
                    NumeroProcesso = x.Processo.NumeroProcesso,
                    DataIntimacao = x.DataIntimacao,
                    StatusTriagem = x.StatusTriagem,
                    NomePecaCabivel = x.PecaCabivel != null ? x.PecaCabivel.NomePeca : null
                })
                .ToListAsync();

            return new PageResult<IntimacaoPaginacaoResponse>
            {
                Items = items,
                TotalCount = total,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<IntimacaoDashboardResponse> GetDashboardAsync()
        {
            var query = dataContext.Intimacao
                .AsNoTracking()
                .AsQueryable();

            var total = await query.CountAsync();

            var pendentes = await query.CountAsync(x =>
                x.StatusTriagem == StatusTriagem.PendenteDeLeitura);

            var triadas = await query.CountAsync(x =>
                x.StatusTriagem == StatusTriagem.Triada);

            var distribuidasAdvogado = await query.CountAsync(x =>
                x.AdvogadoId != null);

            var distribuidasLote = await query.CountAsync(x =>
                x.LoteId != null);

            var concluidas = await query.CountAsync(x =>
                x.StatusCumprimento == StatusCumprimento.Cumprido);

            return new IntimacaoDashboardResponse
            {
                Total = total,
                Pendentes = pendentes,
                Triadas = triadas,
                DistribuidasAdvogado = distribuidasAdvogado,
                DistribuidasLote = distribuidasLote,
                Concluidas = concluidas
            };
        }
        public async Task<bool> ExisteDuplicidadeAsync(
    Guid processoId,
    DateTime dataIntimacao,
    string texto)
        {
            return await dataContext.Intimacao
                .AnyAsync(x =>

                    x.ProcessoId == processoId &&

                    x.DataIntimacao.Date ==
                        dataIntimacao.Date &&

                    x.TextoIntimacao == texto
                );
        }
    }
}
