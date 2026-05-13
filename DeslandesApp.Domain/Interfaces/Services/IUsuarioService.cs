using DeslandesApp.Domain.Models.Dtos.Requests.Usuarios;
using DeslandesApp.Domain.Models.Dtos.Responses.Usuarios;
using DeslandesApp.Domain.Utils;

namespace DeslandesApp.Domain.Interfaces.Services
{
    public interface IUsuarioService :
        IBaseService<UsuariosRequest, UsuarioUpdateRequest, UsuariosResponse, Guid>,
        IQueryService<UsuariosResponse, Guid>
    {
        Task<AutenticarUsuarioResponse> AutenticarUsuarioAsync(
            AutenticarUsuarioRequest request,
            string ip,
            string userAgent);

        Task<PageResult<UsuarioPaginacaoResponse>> ConsultarUsuariosComPaginacaoAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null);

        Task<UsuariosResponse> ConsultarUsuariosPerfil(Guid id);

        Task<List<UsuariosResponse>> ConsultarAsync();
        Task DesbloquearUsuario(Guid idUsuario);
        Task<UsuariosResponse?> ObterPorIdAsync(Guid id);
    }
}