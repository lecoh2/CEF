using DeslandesApp.Domain.Interfaces.Services;
using DeslandesApp.Domain.Models.Dtos.Requests.LoteTrabalho;
using DeslandesApp.Domain.Models.Dtos.Responses.LoteTrabalho;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeslandesApp.API.Controllers.V1
{
    [Route("api/v1/lotetrabalho")]
    [ApiController]
    public class LoteTrabalhoController(ILoteTrabalhoService loteTrabalholService) : ControllerBase
    {
        [HttpPost("cadastrar-lote-trabalho")]
        [ProducesResponseType(typeof(LoteTrabalhoResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> PostAsync([FromBody] LoteTrabalhoRequest request)
        {
            var response = await loteTrabalholService.AdicionarAsync(request);

            return StatusCode(StatusCodes.Status201Created, new
            {
                success = true,
                message = $"Lote de Trabalho {response.NumeroLote} cadastrado com sucesso.",
                data = response
            });
        }
        [HttpGet("consultar-lote-trabalho-paginacao")]
        public async Task<IActionResult> ConsultarLotePaginacao(
       [FromQuery] int pageNumber = 1,
       [FromQuery] int pageSize = 10,
       [FromQuery] string? searchTerm = null)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, 100);

            var loteTrabalhoPaged = await loteTrabalholService
                .ConsultarLoteTrabalhoPaginacaoAsync(pageNumber, pageSize, searchTerm);

            return Ok(loteTrabalhoPaged);
        }
        [HttpPut("atualizar-lote-trabalho/{id}")]
        [ProducesResponseType(typeof(LoteTrabalhoResponse), 200)]
        public async Task<IActionResult> PutAsync(Guid id, [FromBody] LoteTrabalhoUpdateRequest request)
        {
            var response = await loteTrabalholService.ModificarAsync(id, request);
            return Ok(new
            {
                success = true,
                message = $"Lote de Trabalho {response.NumeroLote} atualizado com sucesso.",
                data = response
            });
        }
        [HttpDelete("exluir-lote-de-trabalho/{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await loteTrabalholService.ExcluirAsync(id);

            return Ok(new
            {
                success = true,
                message = "Lote removida com sucesso."
            });
        }
        [HttpGet("consular-lote-de-trabalho-por-id/{id}")]
        [ProducesResponseType(typeof(LoteTrabalhoResponse), 200)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var response = await loteTrabalholService.ObterPorIdAsync(id);

            return Ok(response);
        }
    }
}
