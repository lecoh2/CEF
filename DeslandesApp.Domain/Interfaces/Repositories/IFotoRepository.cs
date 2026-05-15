using DeslandesApp.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Interfaces.Repositories
{
    public interface IFotoRepository : IBaseRepository<Fotos, Guid>
    {
        Task<Fotos> GetById(Guid idFoto);
        Task<Fotos?> GetFotoPorUsuarioAsync(Guid idUsuario);
    }
}
