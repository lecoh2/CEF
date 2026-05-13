using DeslandesApp.Domain.Interfaces.Services;
using DeslandesApp.Domain.Models.Dtos.Requests.Usuarios;
using DeslandesApp.Domain.Models.Dtos.Responses.Usuarios;
using DeslandesApp.Domain.Services;
using DeslandesApp.Domain.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeslandesApp.API.Controllers.V1
{
    [Route("api/v1/usuarios")]
    [ApiController]
    public class UsuariosController(IUsuarioService usuarioService) : ControllerBase
    {
        [HttpPost("cadastrar-usuario")]
        [ProducesResponseType(typeof(UsuariosResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> PostAsync([FromBody] UsuariosRequest request)
        {
            var response = await usuarioService.AdicionarAsync(request);

            return StatusCode(StatusCodes.Status201Created, new
            {
                success = true,
                message = $"Usuário {response.NomeUsuario} cadastrado com sucesso.",
                data = response
            });
        }

        [HttpPut("atualizar-usuario/{id}")]
        [ProducesResponseType(typeof(UsuariosResponse), 200)]
        public async Task<IActionResult> PutAsync(Guid id, [FromBody] UsuarioUpdateRequest request)
        {
            var response = await usuarioService.ModificarAsync(id, request);
            return StatusCode(StatusCodes.Status201Created, new
            {
                success = true,
                message = $"Usuário {response.NomeUsuario} atualizado com sucesso.",
                data = response
            });
        }
        [HttpGet("consultar-usuario-responsavel")]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await usuarioService.ConsultarAsync();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(UsuariosResponse), 200)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var response = await usuarioService.ExcluirAsync(id);
            return StatusCode(200, response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PageResult<UsuariosResponse>), 201)]
        public async Task<IActionResult> GetAllAsync
            ([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var response = await usuarioService.ConsultarAsync(pageNumber, pageSize);
            return StatusCode(200, response);
        }
        [HttpGet("consultar-usuarios-por-id-perfil/{id}")]
        [ProducesResponseType(typeof(PageResult<UsuariosResponse>), 201)]
        public async Task<IActionResult> ConsultarUsuariosPerfil(Guid id)
        {
            try
            {
                var usuario = await usuarioService.ConsultarUsuariosPerfil(id);
                //HTTP 200 (OK)
                return StatusCode(200, usuario);
            }
            catch (ApplicationException e)
            {
                //HTTP 400 (BAD REQUEST)
                return StatusCode(400, new { e.Message });
            }
            catch (Exception e)
            {
                //HTTP 500 (INTERNAL SERVER ERROR)
                return StatusCode(500, new { e.Message });
            }
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UsuariosResponse), 200)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var response = await usuarioService.ObterPorIdAsync(id);
            return StatusCode(200, response);
        }
        [AllowAnonymous]
        [HttpPost("autenticar-usuario")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AutenticarUsuario([FromBody] AutenticarUsuarioRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    sucesso = false,
                    mensagem = "Requisição inválida. Verifique os campos enviados.",
                    erros = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }

            // Captura IP
            var clientIp = Request.Headers.ContainsKey("X-Forwarded-For")
                ? Request.Headers["X-Forwarded-For"].FirstOrDefault()
                : HttpContext.Connection.RemoteIpAddress?.ToString();

            clientIp ??= "IP não identificado";

            // Captura UserAgent
            var userAgent = Request.Headers["User-Agent"].FirstOrDefault()
                            ?? "UserAgent não informado";

            var response = await usuarioService.AutenticarUsuarioAsync(dto, clientIp, userAgent);

            return StatusCode(StatusCodes.Status201Created, new
            {
                success = true,
                message = $"Usuário {response.Login} autenticado com sucesso.",
                data = response
            });
        }

        [HttpGet("consultar-usuarios-paginacao")]
        public async Task<IActionResult> ConsultarUsuarioPaginacao(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? searchTerm = null)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, 100);

            var usuarioPaged = await usuarioService
                .ConsultarUsuariosComPaginacaoAsync(pageNumber, pageSize, searchTerm);

            return Ok(usuarioPaged);
        }

        [HttpPut("desbloquear-usuario/{id}")]
        public async Task<IActionResult> DesbloquearUsuario(Guid id)
        {
           
                await usuarioService.DesbloquearUsuario(id);

                return StatusCode(200, new
                {
                    mensagem = "Usuário desbloqueado com sucesso."

                });
        
        }
        [HttpGet("consultar-usuarios-por-id/{id}")]
        public async Task<IActionResult> ConsultarUsuarios(Guid id)
        {
            var usuario = await usuarioService.ObterPorIdAsync(id);

            return Ok(usuario);
        }

    }
}

