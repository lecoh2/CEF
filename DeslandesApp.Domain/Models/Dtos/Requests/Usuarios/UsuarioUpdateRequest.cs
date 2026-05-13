using DeslandesApp.Domain.Models.Dtos.Requests.GrupoNiveis;
using DeslandesApp.Domain.Models.Dtos.Requests.GrupoSetores;
using DeslandesApp.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Dtos.Requests.Usuarios
{
    public record UsuarioUpdateRequest
  (
      string NomeUsuario,
      string Login,
      string Senha,
      DateTime DataAtualizacao,
      string Email,
      string? Observacao,

      List<GrupoSetorRequest>? GrupoSetores,
      List<GrupoNivelRequest>? GrupoNiveis
  );
}
