using DeslandesApp.Domain.Models.Dtos.Responses.PecaCabivel;
using DeslandesApp.Domain.Models.Dtos.Responses.Processo;
using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Interfaces.Repositories
{
    public interface IPecaCabivelRepository : IBaseRepository<PecaCabivel, Guid>
    {
        Task<PageResult<PecaCabivelPaginacaoResponse>> GetPecaCabivelPaginacaoAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<PecaCabivel?> ObterCompletoPorIdAsync(Guid id);
        Task<List<PecaCabivel>> BuscarPorTermoAsync(string term);
        Task<bool> ExistePorNomeAsync(string nome, Guid? idIgnorar = null);
        Task<List<PecaCabivel>> GetAllAtivosAsync();

    }
}
