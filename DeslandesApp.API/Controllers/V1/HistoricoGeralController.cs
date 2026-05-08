using DeslandesApp.Domain.Interfaces.Services;
using DeslandesApp.Domain.Models.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeslandesApp.API.Controllers.V1
{
    [Route("api/v1/historicogral")]
    [ApiController]
    public class HistoricoGeralController(IHistoricoGeralService historicoGeralService) : ControllerBase
    {

        [HttpGet("historico/{entidade}/{entidadeId}")]
        public async Task<IActionResult> ObterHistorico(
    TipoEntidade entidade,
    Guid entidadeId)
        {
            var historico = await historicoGeralService.ObterPorEntidadeAsync(entidade, entidadeId);
            return Ok(historico);
        }
    }
}
