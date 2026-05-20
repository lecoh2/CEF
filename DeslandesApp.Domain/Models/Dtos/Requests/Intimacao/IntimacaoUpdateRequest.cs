using DeslandesApp.Domain.Models.Enum.DeslandesApp.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Dtos.Requests.Intimacao
{
    public record IntimacaoUpdateRequest
    {
        public Guid ProcessoId { get; init; }

        public DateTime DataIntimacao { get; init; }

        public string TextoIntimacao { get; init; } = string.Empty;

        public Guid? PecaCabivelId { get; init; }

        public ComplexidadeEnum? ComplexidadeReal { get; init; }

        public string? ObservacaoTriagem { get; init; }
    }
}
