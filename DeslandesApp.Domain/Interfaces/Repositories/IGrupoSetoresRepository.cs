using DeslandesApp.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Interfaces.Repositories
{
    public interface IGrupoSetoresRepository : IBaseRepository<GrupoSetores, Guid>
    {
        Task<GrupoSetores> GetByIdUSuarioIdSetor(Guid idUsuario, Guid idSetor);
        Task<GrupoSetores> ExistUsuarioSetorAsync(Guid idUsuario, Guid idSetor);
        Task RemoverPorUsuarioId(Guid usurioId);
    }
}
