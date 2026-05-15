using DeslandesApp.Domain.Models.Dtos.Requests.Lembrete;
using DeslandesApp.Domain.Models.Dtos.Requests.Vara;
using DeslandesApp.Domain.Models.Dtos.Responses.lembretes;
using DeslandesApp.Domain.Models.Dtos.Responses.Vara;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Interfaces.Services
{
    public interface ILembreteService : IBaseService<LembreteRequest, LembreteUpdateRequest, LembreteResponse, Guid>
    {
        Task<List<LembreteResponse>> ObterLembretesAsync();
    }
}
