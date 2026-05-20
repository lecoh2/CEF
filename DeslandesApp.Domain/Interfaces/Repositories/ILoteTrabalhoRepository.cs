using DeslandesApp.Domain.Models.Dtos.Responses.LoteTrabalho;

using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Interfaces.Repositories
{
    public interface ILoteTrabalhoRepository : IBaseRepository<LoteTrabalho, Guid>
    {
        Task<PageResult<LoteTrabalhoPaginacaoResponse>> GetLoteTrabalhoPaginacaoAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<LoteTrabalho?> ObterCompletoPorIdAsync(Guid id);
    }
}
