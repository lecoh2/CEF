using DeslandesApp.Domain.Interfaces.Services;
using DeslandesApp.Domain.Models.Dtos.Requests.Foto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeslandesApp.API.Controllers.V1
{
    [Route("api/v1/foto")]
    [ApiController]
  
    public class FotoController(IFotoServices fotoService) : ControllerBase
    {
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFile([FromForm] UploadFotoRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var fileUrl = await fotoService.SaveImageAsync(request.File);

            return Ok(new { FileUrl = fileUrl });
        }
        /// <summary>
        /// Cadastra foto no banco
        /// </summary>
        [HttpPost("cadastrar-ou-atualizar")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CadastrarOuAtualizarFoto(
            [FromForm] FotoRequest request,
            IFormFile file)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await fotoService.CadastrarOuAtualizarFotoAsync(request, file);

            return Ok(result);
        }
    }
}
