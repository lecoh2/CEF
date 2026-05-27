using DeslandesApp.Domain.Interfaces.Services;
using DeslandesApp.Domain.Models.Dtos.Requests.Intimacao;
using DeslandesApp.Domain.Models.Dtos.Responses.Intimacao;
using DeslandesApp.Domain.Models.Enum;
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
                message = "Intimação atualizada com sucesso.",
                data = response
            });
        }
        [HttpDelete("excluir-intimacao/{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await intimacaoService.ExcluirAsync(id);

            return Ok(new
            {
                success = true,
                message = "Intimação removida com sucesso."
            });
        }
        [HttpGet("consultar-intimacao-por-id/{id}")]
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
        public async Task<IActionResult> DistribuirParaAdvogadoAsync(
    Guid id,
    [FromQuery] Guid advogadoId)
        {
            await intimacaoService.DistribuirParaAdvogadoAsync(id, advogadoId);

            return Ok(new
            {
                success = true,
                message = "Intimação distribuída para advogado com sucesso."
            });
        }
        [HttpPut("distribuir-lote/{id}")]
        public async Task<IActionResult> DistribuirParaLoteAsync(
    Guid id,
    [FromQuery] Guid loteId)
        {
            await intimacaoService.DistribuirParaLoteAsync(id, loteId);

            return Ok(new
            {
                success = true,
                message = "Intimação distribuída para lote com sucesso."
            });
        }
        [HttpGet("triagem-paginacao")]
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
        [HttpPost("importar")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportarAsync([FromForm] IFormFile arquivo)
        {
            var result =
                await intimacaoService.ImportarAsync(arquivo);

            return Ok(new
            {
                success = true,
                message = "Importação realizada com sucesso.",
                data = result
            });
        }
        [HttpGet("pendentes")]
        public async Task<IActionResult> GetPendentes()
        {
            var result = await intimacaoService.GetPendentesAsync();
            return Ok(result);
        }
        // =========================
        // PENDENTES DE TRIAGEM
        // =========================
        [HttpGet("pendentes-triagem")]
        public async Task<IActionResult> GetPendentesTriagem()
        {
            var result = await intimacaoService.GetNaoTriadasAsync();

            return Ok(new
            {
                success = true,
                data = result
            });
        }
        // =========================
        // BUSCAR POR PROCESSO
        // =========================
        [HttpGet("buscar-por-processo")]
        public async Task<IActionResult> BuscarPorProcesso([FromQuery] string numeroProcesso)
        {
            var result = await intimacaoService.BuscarPorProcessoAsync(numeroProcesso);

            return Ok(new
            {
                success = true,
                data = result
            });
        }

        // =========================
        // CONTAGEM POR STATUS
        // =========================
        [HttpGet("count-status")]
        public async Task<IActionResult> CountPorStatus([FromQuery] StatusTriagem status)
        {
            var result = await intimacaoService.CountPorStatusAsync(status);

            return Ok(new
            {
                success = true,
                data = result
            });
        }

        // =========================
        // CONTAGEM POR LOTE
        // =========================
        [HttpGet("count-lote/{loteId}")]
        public async Task<IActionResult> CountPorLote(Guid loteId)
        {
            var result = await intimacaoService.CountPorLoteAsync(loteId);

            return Ok(new
            {
                success = true,
                data = result
            });
        }

        // =========================
        // CONTAGEM POR ADVOGADO
        // =========================
        [HttpGet("count-advogado/{advogadoId}")]
        public async Task<IActionResult> CountPorAdvogado(Guid advogadoId)
        {
            var result = await intimacaoService.CountPorAdvogadoAsync(advogadoId);

            return Ok(new
            {
                success = true,
                data = result
            });
        }
    }
}
