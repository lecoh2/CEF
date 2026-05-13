using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Dtos.Responses.GrupoNiveis
{
    public record GrupoNivelResponse(
        Guid IdNivel,
        string NomeNivel
    );
}
