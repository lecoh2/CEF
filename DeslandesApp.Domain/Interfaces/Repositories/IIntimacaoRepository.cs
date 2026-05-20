using DeslandesApp.Domain.Models.Dtos.Responses.Intimacao;
using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Domain.Models.Enum.DeslandesApp.Domain.Models.Enum;
using DeslandesApp.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Interfaces.Repositories
{
    public interface
        IIntimacaoRepository : IBaseRepository<Intimacao, Guid>
    {
        Task<PageResult<IntimacaoPaginacaoResponse>> GetIntimacaoPaginacaoAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<Intimacao?> ConsultarIntimacaoCompletaPorIdAsync(Guid id);
        Task<Intimacao?> ObterCompletoPorIdAsync(Guid id);
        Task<PageResult<IntimacaoPaginacaoResponse>> ConsultarParaTriagemAsync(
    DateTime? data,
    StatusTriagem? status,
    int pageNumber,
    int pageSize);

        Task<IntimacaoDashboardResponse> GetDashboardAsync();
    }
}

