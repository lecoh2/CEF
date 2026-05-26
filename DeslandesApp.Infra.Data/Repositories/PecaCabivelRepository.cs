using DeslandesApp.Domain.Interfaces.Repositories;
using DeslandesApp.Domain.Models.Dtos.Responses.PecaCabivel;
using DeslandesApp.Domain.Models.Dtos.Responses.Processo;
using DeslandesApp.Domain.Models.Entities;
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
    public class PecaCabivelRepository(DataContext dataContext)
        : BaseRepository<PecaCabivel, Guid>(dataContext), IPecaCabivelRepository
    {
        public async Task<PageResult<PecaCabivelPaginacaoResponse>> GetPecaCabivelPaginacaoAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {

            var query = dataContext.PecaCabivel
       .AsNoTracking()

       .AsQueryable();

            // --- filtro ---
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();

                query = query.Where(p =>
                    p.NomePeca.ToLower().Contains(term)


                );
            }

            // --- total ---
            var totalCount = await query.CountAsync();

            var items = await query
       .OrderBy(u => u.NomePeca)
       .Skip((pageNumber - 1) * pageSize)
       .Take(pageSize)
       .Select(u => new PecaCabivelPaginacaoResponse(
           u.Id,
           u.NomePeca,
           u.PrazoDias,
           u.SugestaoComplexidadePadrao
       ))
       .ToListAsync();


            return new PageResult<PecaCabivelPaginacaoResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        public async Task<PecaCabivel?> ObterCompletoPorIdAsync(Guid id)
        {
            return await dataContext.PecaCabivel
                .AsNoTracking()
                .Where(x => x.Id == id)
                // futuro: .Include(x => x.Intimacoes)
                .FirstOrDefaultAsync();
        }
        public async Task<bool> ExistePorNomeAsync(string nome, Guid? idIgnorar = null)
        {
            return await dataContext.PecaCabivel
                .AsNoTracking()
                .AnyAsync(x =>
                    x.NomePeca == nome &&
                    (!idIgnorar.HasValue || x.Id != idIgnorar.Value)
                );
        }
        public async Task<List<PecaCabivel>> GetAllAtivosAsync()
        {
            return await dataContext.PecaCabivel
                .AsNoTracking()
                .Where(x => !x.Excluido)
                .OrderBy(x => x.NomePeca)
                .ToListAsync();
        }
        public async Task<List<PecaCabivel>> BuscarPorTermoAsync(string term)
        {
            return await dataContext.PecaCabivel
                .AsNoTracking()
                .Where(x => x.NomePeca.Contains(term))
                .OrderBy(x => x.NomePeca)
                .Take(10)
                .ToListAsync();
        }

    }
}

