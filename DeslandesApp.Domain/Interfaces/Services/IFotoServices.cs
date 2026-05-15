using DeslandesApp.Domain.Models.Dtos.Requests.Foro;
using DeslandesApp.Domain.Models.Dtos.Requests.Foto;
using DeslandesApp.Domain.Models.Dtos.Requests.Vara;
using DeslandesApp.Domain.Models.Dtos.Responses.Foro;
using DeslandesApp.Domain.Models.Dtos.Responses.Foto;
using DeslandesApp.Domain.Models.Dtos.Responses.Vara;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Interfaces.Services
{
    public interface IFotoServices : IBaseService<FotoRequest, FotoUpdateRequest, FotoResponse, Guid>
    {
        Task<(string fileName, string relativeUrl)> SaveImageAsync(IFormFile file);
        //Task<FotosResponseDto> CadastrarFotoAsync(FotosRequestDto request);
        // Task<FotosResponseDto> CadastrarFotoAsync(FotosRequestDto request, IFormFile file);
        Task<FotoResponse> CadastrarOuAtualizarFotoAsync(FotoRequest request, IFormFile file);
    }
}
