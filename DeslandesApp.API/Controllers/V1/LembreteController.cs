using DeslandesApp.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeslandesApp.API.Controllers.V1
{
    [Route("api/v1/lembrete")]
    [ApiController]
    public class LembreteController(ILembreteService lembreteService) : ControllerBase 
    {
        [HttpGet("lembretes")]
        public async Task<IActionResult> ObterLembretes()
        {
            var result = await lembreteService.ObterLembretesAsync();
            return Ok(result);
        }
    }
}
