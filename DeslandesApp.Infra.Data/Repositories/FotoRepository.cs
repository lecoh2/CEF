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
    public class FotoRepository(DataContext dataContext) : BaseRepository<Fotos, Guid>(dataContext), IFotoRepository
    {
  
        public async Task<Fotos> GetById(Guid idFoto)
        {
            return await dataContext.Set<Fotos>()
               .FirstOrDefaultAsync(f => f.Id == idFoto);
        }
        public async Task<Fotos?> GetFotoPorUsuarioAsync(Guid idUsuario)
        {
            return await dataContext.Set<Fotos>()
               .FirstOrDefaultAsync(f => f.IdUsuario == idUsuario);
        }
    }
}
