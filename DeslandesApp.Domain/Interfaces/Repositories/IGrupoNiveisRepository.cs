using DeslandesApp.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Interfaces.Repositories
{
    public interface IGrupoNiveisRepository : IBaseRepository<GrupoNiveis, Guid>
    {
        Task<GrupoNiveis> GetByIdUsuarioIdNivel(Guid idUsuario, Guid idNivel);
        Task<GrupoNiveis> ExistUsuarioNivelAsync(Guid idUsuario, Guid idNivel);
        Task RemoverPorUsuarioId(Guid usurioId);
    }
}

