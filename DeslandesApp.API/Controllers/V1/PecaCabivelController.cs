using DeslandesApp.Domain.Interfaces.Services;
using DeslandesApp.Domain.Models.Dtos.Requests.PecaCabivel;
using DeslandesApp.Domain.Models.Dtos.Responses.PecaCabivel;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeslandesApp.API.Controllers.V1
{
    [Route("api/v1/pecacabivel")]
    [ApiController]
    public class PecaCabivelController(IPecaCabivelService pecaCabivelService) : ControllerBase
    {
        [HttpPost("cadastrar")]
        public async Task<IActionResult> PostAsync([FromBody] PecaCabivelRequest request)
        {
            var response = await pecaCabivelService.AdicionarAsync(request);

            return StatusCode(201, new
            {
                success = true,
                message = $"Peça {response.NomePeca} cadastrada com sucesso.",
                data = response
            });
        }

        [HttpGet("consultar-peca-paginacao")]
        public async Task<IActionResult> GetPaginacao(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, 100);

            var result = await pecaCabivelService
                .ConsultarPecaCabivelPaginacaoAsync(pageNumber, pageSize, searchTerm);

            return Ok(result);
        }

        [HttpGet("obter-por-id/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await pecaCabivelService.ObterPorIdAsync(id);

            if (response == null)
                return NotFound(new { message = "Peça não encontrada." });

            return Ok(response);
        }

        [HttpPut("atualizar-peca/{id}")]
        public async Task<IActionResult> PutAsync(Guid id, [FromBody] PecaCabivelUpdateRequest request)
        {
            var response = await pecaCabivelService.ModificarAsync(id, request);

            return Ok(new
            {
                success = true,
                message = $"Peça {response.NomePeca} atualizada com sucesso.",
                data = response
            });
        }

        [HttpDelete("deletar-peca/{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await pecaCabivelService.ExcluirAsync(id);

            return Ok(new
            {
                success = true,
                message = "Peça removida com sucesso."
            });
        }
        [HttpGet("ativos")]
        public async Task<IActionResult> GetAtivos()
        {
            var result = await pecaCabivelService.ListarAtivosAsync();
            return Ok(result);
        }
        [HttpGet("buscar-auto-complete")]
        public async Task<IActionResult> Buscar([FromQuery] string term)
        {
            var result = await pecaCabivelService.BuscarAsync(term);
            return Ok(result);
        }
    }
}

