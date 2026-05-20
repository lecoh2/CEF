using DeslandesApp.Domain.Models.Enum.DeslandesApp.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Dtos.Responses.PecaCabivel
{
    public record PecaCabivelPaginacaoResponse
    (
        Guid Id,
        string NomePeca,
         int PrazoDias,
         ComplexidadeEnum SugestaoComplexidadePadrao
    );
}
