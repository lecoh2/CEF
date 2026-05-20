using DeslandesApp.Domain.Models.Enum.DeslandesApp.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Dtos.Responses.Intimacao
{
    public record IntimacaoTriagemRequest
    {
        public Guid? PecaCabivelId { get; set; }

        public ComplexidadeEnum? ComplexidadeReal { get; set; }

        public string? ObservacaoTriagem { get; set; }
    }
}
