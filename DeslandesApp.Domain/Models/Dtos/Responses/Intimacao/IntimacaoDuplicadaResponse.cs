using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Dtos.Responses.Intimacao
{
    public record IntimacaoDuplicadaResponse
    {
        public string NumeroProcesso { get; init; } = string.Empty;

        public DateTime DataIntimacao { get; init; }

        public string Motivo { get; init; } = string.Empty;
    }
}
