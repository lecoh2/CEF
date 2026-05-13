using DeslandesApp.Domain.Models.Dtos.Responses.GrupoNiveis;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoSetores;
using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Domain.Models.Enum;
using DeslandesApp.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Dtos.Responses.Usuarios
{
    public record UsuariosResponse(
        Guid Id,
        string NomeUsuario,
        string Login,
        DateTime DataCadastro,
        Status? Status,
        string Email,

        List<GrupoSetorResponse> GrupoSetores,
        List<GrupoNivelResponse> GrupoNiveis
    );


}
