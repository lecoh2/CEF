using DeslandesApp.Domain.Models.Dtos.Requests.LoteTrabalho;
using DeslandesApp.Domain.Models.Dtos.Responses.LoteTrabalho;
using DeslandesApp.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Interfaces.Services
{
    public interface ILoteTrabalhoService : IBaseService<LoteTrabalhoRequest, LoteTrabalhoUpdateRequest, LoteTrabalhoResponse, Guid>
    {
        Task<PageResult<LoteTrabalhoPaginacaoResponse>> ConsultarLoteTrabalhoPaginacaoAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<ObterLoteTrabalhoResponse?> ObterPorIdAsync(Guid id);
        Task<List<LoteTrabalhoResponse>> ListarAtivosAsync();
        Task<List<LoteTrabalhoResponse>> BuscarPorResponsavelAsync(Guid responsavelId)
    }
}
