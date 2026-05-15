using AutoMapper;
using DeslandesApp.Domain.Contracts.Security;
using DeslandesApp.Domain.Interfaces.Repositories;
using DeslandesApp.Domain.Interfaces.Services;
using DeslandesApp.Domain.Models.Dtos.Requests.Foto;
using DeslandesApp.Domain.Models.Dtos.Responses.Foto;
using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Domain.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Services
{
    public class FotoService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor,
        IJwtTokenService _jwtTokenService, IHistoricoGeralService historicoGeralService)
        : BaseService(httpContextAccessor), IFotoServices
    {
        private readonly string _basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        public Task<FotoResponse> AdicionarAsync(FotoRequest request)
        {
            throw new NotImplementedException();
        }

   
        public Task<PageResult<FotoResponse>> ConsultarAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            unitOfWork.Dispose();
        }

        public Task<FotoResponse> ExcluirAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<FotoResponse> ModificarAsync(Guid id, FotoUpdateRequest request)
        {
            throw new NotImplementedException();
        }

      
        public async Task<(string fileName, string relativeUrl)> SaveImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ApplicationException("Nenhum arquivo enviado.");

            if (!Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);

            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(_basePath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativeUrl = $"/uploads/{fileName}";
            return (fileName, relativeUrl);
        }
        public async Task<FotoResponse> CadastrarOuAtualizarFotoAsync(
    FotoRequest request,
    IFormFile file)
        {
            if (request == null || request.Id == Guid.Empty)
                throw new ApplicationException("ID do usuário é obrigatório.");

            if (file == null || file.Length == 0)
                throw new ApplicationException("O arquivo da foto é obrigatório.");

            await unitOfWork.BeginTransactionAsync();

            string? caminhoAntigo = null;

            try
            {
                var fotoAtual = await unitOfWork
                    .FotoRepository
                    .GetFotoPorUsuarioAsync(request.Id);

                // guarda caminho antigo
                if (fotoAtual != null && !string.IsNullOrEmpty(fotoAtual.FotoNome))
                {
                    caminhoAntigo = Path.Combine(_basePath, fotoAtual.FotoNome);
                }

                // salva nova imagem
                var (fileName, relativeUrl) = await SaveImageAsync(file);

                if (fotoAtual == null)
                {
                    fotoAtual = new Fotos
                    {
                        Id = Guid.NewGuid(),
                        IdUsuario = request.Id,
                        FotoNome = fileName,
                        FileUrl = relativeUrl,
                        DataCadastro = DateTime.Now
                    };

                    await unitOfWork.FotoRepository.AddAsync(fotoAtual);
                }
                else
                {
                    fotoAtual.FotoNome = fileName;
                    fotoAtual.FileUrl = relativeUrl;
                    fotoAtual.DataAtualizacao = DateTime.Now;

                    await unitOfWork.FotoRepository.UpdateAsync(fotoAtual);
                }

                await unitOfWork.CommitAsync();

                // remove antiga só depois do commit
                if (!string.IsNullOrEmpty(caminhoAntigo) && File.Exists(caminhoAntigo))
                {
                    File.Delete(caminhoAntigo);
                }

                var response = mapper.Map<FotoResponse>(fotoAtual);

                response.FileUrl = relativeUrl;

                return response;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
