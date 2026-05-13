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
    public class GrupoSetorRepository(DataContext dataContext)
        : BaseRepository<GrupoSetores, Guid>(dataContext), IGrupoSetoresRepository
    {
        public async Task<GrupoSetores> ExistUsuarioSetorAsync(Guid idUsuario, Guid idSetor)
        {
            return await dataContext.GrupoSetores
                .FirstOrDefaultAsync(gr => gr.IdUsuario == idUsuario && gr.IdSetor == idSetor);
        }

        public async Task<GrupoSetores> GetByIdUSuarioIdSetor(Guid idUsuario, Guid idSetor)
        {
            return await dataContext.GrupoSetores

        .Where(gr => gr.IdUsuario == idUsuario && gr.IdSetor == idSetor)
        .FirstOrDefaultAsync();
        }
        public async Task RemoverPorUsuarioId(Guid usuarioId)
        {
            var registros = await dataContext.GrupoSetores
                .Where(x => x.IdUsuario == usuarioId)
                .ToListAsync();

            dataContext.GrupoSetores.RemoveRange(registros);
        }

    }
}