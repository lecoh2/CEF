using DeslandesApp.Domain.Models.Dtos.Responses.GrupoNiveis;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoSetores;
using DeslandesApp.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Dtos.Responses.Usuarios
{
    public record class UsuarioPaginacaoResponse
     (
         Guid Id,
             string NomeUsuario,
             string Login,
             Status? Status,
             DateTime DataCadastro,
               List<GrupoSetorPaginacaoResponse> GrupoSetores,
        List<GrupoNivelPaginacaoResponse> GrupoNiveis
     );
}
