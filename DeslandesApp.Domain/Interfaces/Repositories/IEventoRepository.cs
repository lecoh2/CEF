using DeslandesApp.Domain.Models.Dtos.Responses.Caso;
using DeslandesApp.Domain.Models.Dtos.Responses.Evento;
using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Interfaces.Repositories
{
    public interface IEventoRepository : IBaseRepository<Evento, Guid>
    {
        Task<PageResult<EventoPaginacaoResponse>> GetEventoPaginacaoAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<Evento> ConsultarEventoComRelacionamentosAsync(Guid idEvento);
        Task<List<Evento>> GetKanbanAsync();
        Task<Evento?> ObterCompletoPorIdAsync(Guid id);
        Task<List<Evento>> ConsultarUltimosAsync(int quantidade);
        Task<List<Evento>> ObterEventosLembreteAsync();
    }
}

