
using DeslandesApp.Domain.Models.Dtos.Requests.PecaCabivel;
using DeslandesApp.Domain.Models.Dtos.Responses.PecaCabivel;
using DeslandesApp.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Interfaces.Services
{
    public interface IPecaCabivelService : IBaseService<PecaCabivelRequest, PecaCabivelUpdateRequest, PecaCabivelResponse, Guid>
    {
        Task<PageResult<PecaCabivelPaginacaoResponse>> ConsultarPecaCabivelPaginacaoAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<ObterPecacabivelResponse?> ObterPorIdAsync(Guid id);

    }
}
