using DeslandesApp.Domain.Models.Enum.DeslandesApp.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Dtos.Requests.PecaCabivel
{
    public record PecaCabivelRequest
    {
        public string NomePeca { get; init; } = string.Empty;

        public int PrazoDias { get; init; }

        public ComplexidadeEnum SugestaoComplexidadePadrao { get; init; }
    }
}
