using DeslandesApp.Domain.Interfaces.Services;
using DeslandesApp.Domain.Models.Dtos.Requests.PecaCabivel;
using DeslandesApp.Domain.Models.Dtos.Requests.Processo;
using DeslandesApp.Domain.Models.Dtos.Responses.PecaCabivel;
using DeslandesApp.Domain.Models.Dtos.Responses.Processo;
using DeslandesApp.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeslandesApp.API.Controllers.V1
{
    [Route("api/v1/pecacabivel")]
    [ApiController]
    public class PecaCabivelController(IPecaCabivelService pecaCabivelService) : ControllerBase
    {
        [HttpPost("cadastrar-peca-cabivel")]
        [ProducesResponseType(typeof(PecaCabivelResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> PostAsync([FromBody] PecaCabivelRequest request)
        {
            var response = await pecaCabivelService.AdicionarAsync(request);

            return StatusCode(StatusCodes.Status201Created, new
            {
                success = true,
                message = $"Peça {response.NomePeca} cadastrada com sucesso.",
                data = response
            });
        }
        [HttpGet("consultar-peca-cabivel-paginacao")]
        public async Task<IActionResult> ConsultarPecaCabivelPaginacao(
       [FromQuery] int pageNumber = 1,
       [FromQuery] int pageSize = 10,
       [FromQuery] string? searchTerm = null)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, 100);

            var pecaCabivelPaged = await pecaCabivelService
                .ConsultarPecaCabivelPaginacaoAsync(pageNumber, pageSize, searchTerm);

            return Ok(pecaCabivelPaged);
        }
        [HttpPut("atualizar-peca-cabivel/{id}")]
        [ProducesResponseType(typeof(PecaCabivelResponse), 200)]
        public async Task<IActionResult> PutAsync(Guid id, [FromBody] PecaCabivelUpdateRequest request)
        {
            var response = await pecaCabivelService.ModificarAsync(id, request);
            return Ok(new
            {
                success = true,
                message = $"Peça {response.NomePeca} atualizado com sucesso.",
                data = response
            });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await pecaCabivelService.ExcluirAsync(id);

            return Ok(new
            {
                success = true,
                message = "Peça removida com sucesso."
            });
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PecaCabivelResponse), 200)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var response = await pecaCabivelService.ObterPorIdAsync(id);

            return Ok(response);
        }
    }
}
