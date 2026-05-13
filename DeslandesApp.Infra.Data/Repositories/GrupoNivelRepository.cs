using DeslandesApp.Domain.Interfaces.Repositories;
using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Infra.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Infra.Data.Repositories
{
    public class GrupoNivelRepository(DataContext dataContext)
        : BaseRepository<GrupoNiveis, Guid>(dataContext), IGrupoNiveisRepository
    {
        public async Task<GrupoNiveis> ExistUsuarioNivelAsync(Guid idUsuario, Guid idNivel)
        {
            return await dataContext.GrupoNiveis
               .FirstOrDefaultAsync(gr => gr.IdUsuario == idUsuario && gr.IdNivel == idNivel);
        }

        public async Task<GrupoNiveis> GetByIdUsuarioIdNivel(Guid idUsuario, Guid idNivel)
        {
            return await dataContext.GrupoNiveis

        .Where(gr => gr.IdUsuario == idUsuario && gr.IdNivel == idNivel)
        .FirstOrDefaultAsync();
        }

        public async Task RemoverPorUsuarioId(Guid usuarioId)
        {
            var registros = await dataContext.GrupoNiveis
                .Where(x => x.IdUsuario == usuarioId)
                .ToListAsync();

            dataContext.GrupoNiveis.RemoveRange(registros);
        }
    }
}
