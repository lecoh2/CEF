using DeslandesApp.Domain.Models.Dtos.Requests.Tarefa;
using DeslandesApp.Domain.Models.Dtos.Responses.Processo;
using DeslandesApp.Domain.Models.Dtos.Responses.Tarefa;
using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Interfaces.Repositories
{
    public interface ITarefaRepository : IBaseRepository<Tarefa, Guid>
    {
        Task<PageResult<TarefaPaginacaoResponse>> GetTarefaPaginacaoAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<Tarefa?> ConsultarComRelacionamentosAsync(Guid id);
        Task<List<Tarefa>> GetKanbanAsync();
        Task<Tarefa?> ObterCompletoPorIdAsync(Guid id);
        Task<List<Tarefa>> ConsultarUltimosAsync(int quantidade);
        Task<List<Tarefa>> ObterTarefasLembreteAsync( Guid? usuarioId,bool ehAdministrador );
    }
}
