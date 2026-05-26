using DeslandesApp.Domain.Models.Enum;
using DeslandesApp.Domain.Models.Enum.DeslandesApp.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Dtos.Responses.Intimacao
{
    public record ObterIntimacaoRespnse
    {
        public Guid Id { get; init; }

        // PROCESSO
        public Guid ProcessoId { get; init; }
        public string NumeroProcesso { get; init; } = string.Empty;

        // INTIMAÇÃO
        public DateTime DataIntimacao { get; init; }
        public DateTime DataImportacao { get; init; }

        // TRIAGEM
        public StatusTriagem StatusTriagem { get; init; }

        public Guid? PecaCabivelId { get; init; }
        public string? NomePecaCabivel { get; init; }

        public ComplexidadeEnum? ComplexidadeReal { get; init; }

        public string? ObservacaoTriagem { get; init; }

        // PRAZO
        public DateTime? PrazoIndividualCpc { get; init; }

        // DISTRIBUIÇÃO
        public Guid? AdvogadoId { get; init; }
        public string? NomeAdvogado { get; init; }

        public Guid? LoteId { get; init; }
        public string? NumeroLote { get; init; }

        // STATUS FINAL
        public StatusCumprimento StatusCumprimento { get; init; }
    }
}
