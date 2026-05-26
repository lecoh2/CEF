using DeslandesApp.Domain.Models.Enum;
using DeslandesApp.Domain.Models.Enum.DeslandesApp.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Dtos.Responses.Intimacao
{
    public record IntimacaoPaginacaoResponse
    {
        public Guid Id { get; init; }

        public string NumeroProcesso { get; init; } = string.Empty;

        public DateTime DataIntimacao { get; init; }

        public StatusTriagem StatusTriagem { get; init; }

        public string? NomePecaCabivel { get; init; }
    }
}
