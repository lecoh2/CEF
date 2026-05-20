using DeslandesApp.Domain.Interfaces.Services;
using DeslandesApp.Domain.Models.Dtos.Requests.Intimacao;
using DeslandesApp.Domain.Models.Dtos.Responses.Intimacao;
using DeslandesApp.Domain.Models.Enum.DeslandesApp.Domain.Models.Enum;
using DeslandesApp.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeslandesApp.API.Controllers.V1
{
    [Route("api/v1/intimacao")]
    [ApiController]
    public class IntimacaoController(IIntimacaoService intimacaoService) : ControllerBase
    {
        [HttpPost("cadastrar-intimacao")]
        [ProducesResponseType(typeof(IntimacaoResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> PostAsync([FromBody] IntimacaoRequest request)
        {
            var response = await intimacaoService.AdicionarAsync(request);

            return StatusCode(StatusCodes.Status201Created, new
            {
                success = true,
                message = $"Intimacao cadastrada com sucesso.",
                data = response
            });
        }
        [HttpGet("consultar-intimacao-paginacao")]
        public async Task<IActionResult> ConsultarIntimacaoPaginacao(
       [FromQuery] int pageNumber = 1,
       [FromQuery] int pageSize = 10,
       [FromQuery] string? searchTerm = null)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, 100);

            var intimacaoPaged = await intimacaoService
                .ConsultarIntimacaoPaginacaoAsync(pageNumber, pageSize, searchTerm);

            return Ok(intimacaoPaged);
        }
        [HttpPut("atualizar-intimacao/{id}")]
        [ProducesResponseType(typeof(IntimacaoResponse), 200)]
        public async Task<IActionResult> PutAsync(Guid id, [FromBody] IntimacaoUpdateRequest request)
        {
            var response = await intimacaoService.ModificarAsync(id, request);
            return Ok(new
            {
                success = true,
                message = $"Peça {response.NomeAdvogado} atualizado com sucesso.",
                data = response
            });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await intimacaoService.ExcluirAsync(id);

            return Ok(new
            {
                success = true,
                message = "Peça removida com sucesso."
            });
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IntimacaoResponse), 200)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var response = await intimacaoService.ObterPorIdAsync(id);

            return Ok(response);
        }
        [HttpPut("triagem/{id}")]
        [ProducesResponseType(typeof(IntimacaoResponse), 200)]
        public async Task<IActionResult> TriarAsync(Guid id, [FromBody] IntimacaoTriagemRequest request)
        {
            var response = await intimacaoService.ModificarTriagemAsync(id, request);

            return Ok(new
            {
                success = true,
                message = "Intimação triada com sucesso.",
                data = response
            });
        }
        [HttpPut("distribuir-advogado/{id}")]
        public async Task<IActionResult> DistribuirParaAdvogadoAsync(Guid id, Guid advogadoId)
        {
            await intimacaoService.DistribuirParaAdvogadoAsync(id, advogadoId);

            return Ok(new
            {
                success = true,
                message = "Intimação distribuída para advogado com sucesso."
            });
        }
        [HttpPut("distribuir-lote/{id}")]
        public async Task<IActionResult> DistribuirParaLoteAsync(Guid id, Guid loteId)
        {
            await intimacaoService.DistribuirParaLoteAsync(id, loteId);

            return Ok(new
            {
                success = true,
                message = "Intimação distribuída para lote com sucesso."
            });
        }
        [HttpGet("triagem")]
        [HttpGet("triagem")]
        public async Task<IActionResult> GetTriagemAsync(
    [FromQuery] DateTime? data,
    [FromQuery] StatusTriagem? status,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
        {
            var result = await intimacaoService.ConsultarParaTriagemAsync(
                data,
                status,
                pageNumber,
                pageSize);

            return Ok(result);
        }
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardAsync()
        {
            var result = await intimacaoService.GetDashboardAsync();

            return Ok(result);
        }
    }
}
