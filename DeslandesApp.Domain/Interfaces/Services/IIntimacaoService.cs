using DeslandesApp.Domain.Models.Dtos.Requests.Intimacao;
using DeslandesApp.Domain.Models.Dtos.Responses.Intimacao;
using DeslandesApp.Domain.Models.Dtos.Responses.LoteTrabalho;
using DeslandesApp.Domain.Models.Dtos.Responses.PecaCabivel;
using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Domain.Models.Enum;
using DeslandesApp.Domain.Models.Enum.DeslandesApp.Domain.Models.Enum;
using DeslandesApp.Domain.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Interfaces.Services
{
    public interface IIntimacaoService :
     IBaseService<IntimacaoRequest, IntimacaoUpdateRequest, IntimacaoResponse, Guid>
    {
        Task<PageResult<IntimacaoPaginacaoResponse>> ConsultarIntimacaoPaginacaoAsync(int pageNumber, int pageSize, string? searchTerm = null);


        Task DistribuirParaAdvogadoAsync(Guid intimacaoId, Guid advogadoId);

        Task DistribuirParaLoteAsync(Guid intimacaoId, Guid loteId);

        Task<IntimacaoResponse> ModificarTriagemAsync(
            Guid id,
            IntimacaoTriagemRequest request
        );
        Task<ObterIntimacaoRespnse?> ObterPorIdAsync(Guid id);

        Task<PageResult<IntimacaoPaginacaoResponse>> ConsultarParaTriagemAsync(
     DateTime? data,
     StatusTriagem? status,
     int pageNumber,
     int pageSize);
        Task<IntimacaoDashboardResponse> GetDashboardAsync();
        Task<ResultadoImportacaoIntimacaoResponse>ImportarAsync(IFormFile arquivo);
        Task<List<Intimacao>> GetNaoTriadasAsync();
        Task<List<Intimacao>> BuscarPorProcessoAsync(string numeroProcesso);

        Task<int> CountPorStatusAsync(StatusTriagem status);
        Task<int> CountPorLoteAsync(Guid loteId);
        Task<int> CountPorAdvogadoAsync(Guid advogadoId);

    }
}
