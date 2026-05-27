using DeslandesApp.Domain.Interfaces.Repositories;
using DeslandesApp.Domain.Models.Dtos.Responses.LoteTrabalho;
using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Domain.Models.Enum;
using DeslandesApp.Domain.Models.Enum.DeslandesApp.Domain.Models.Enum;
using DeslandesApp.Domain.Utils;
using DeslandesApp.Infra.Data.Contexts;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Infra.Data.Repositories
{
    public class LoteTrabalhoRepository(DataContext dataContext) : BaseRepository<LoteTrabalho, Guid>(dataContext), ILoteTrabalhoRepository
    {
        public async Task<PageResult<LoteTrabalhoPaginacaoResponse>> GetLoteTrabalhoPaginacaoAsync(
         int pageNumber,
         int pageSize,
         string? searchTerm = null)
        {
            var query = dataContext.LoteTrabalho
                .AsNoTracking()
                .Where(x => !x.Excluido); // 🔥 IMPORTANTE

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();

                query = query.Where(p =>
                    p.NumeroLote.ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(u => u.DataCriacao)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new LoteTrabalhoPaginacaoResponse(
                    u.Id,
                    u.NumeroLote,
                    u.ResponsavelId,
                    u.Responsavel.NomeUsuario,
                    u.CoordenadorId,
                    u.Coordenador.NomeUsuario,
                    u.DataCriacao,
                    u.DataPrazoLote,
                    u.Status,
                    u.Intimacoes.Count
                ))
                .ToListAsync();

            return new PageResult<LoteTrabalhoPaginacaoResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        public async Task<LoteTrabalho?> ObterCompletoPorIdAsync(Guid id)
        {
            return await dataContext.LoteTrabalho
                .AsNoTracking()
                .Include(x => x.Responsavel)
                .Include(x => x.Coordenador)
                .FirstOrDefaultAsync(x => x.Id == id && !x.Excluido);
        }
        // =========================
        // 🔥 NOVOS MÉTODOS IMPORTANTES
        // =========================

        public async Task<LoteTrabalho?> ObterUltimoLoteAsync()
        {
            return await dataContext.LoteTrabalho
                .AsNoTracking()
                .Where(x => !x.Excluido)
                .OrderByDescending(x => x.DataCriacao)
                .FirstOrDefaultAsync();
        }
        public async Task<bool> ExisteNumeroLoteAsync(string numero)
        {
            return await dataContext.LoteTrabalho
                .AsNoTracking()
                .AnyAsync(x => x.NumeroLote == numero && !x.Excluido);
        }

        public async Task<List<LoteTrabalho>> GetAllAtivosAsync()
        {
            return await dataContext.LoteTrabalho
                .AsNoTracking()
                .Where(x => !x.Excluido)
                .OrderByDescending(x => x.DataCriacao)
                .ToListAsync();
        }

        public async Task<List<LoteTrabalho>> ObterPorResponsavelAsync(Guid responsavelId)
        {
            return await dataContext.LoteTrabalho
                .AsNoTracking()
                .Where(x => x.ResponsavelId == responsavelId && !x.Excluido)
                .ToListAsync();
        }
    }

}

