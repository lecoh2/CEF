using DeslandesApp.Domain.Models.Enum.DeslandesApp.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Dtos.Responses.LoteTrabalho
{
    public record LoteTrabalhoResponse
    {
        public Guid Id { get; init; }

        public string NumeroLote { get; init; } = string.Empty;

        public Guid ResponsavelId { get; init; }

        public string ResponsavelNome { get; init; } = string.Empty;

        public Guid CoordenadorId { get; init; }

        public string CoordenadorNome { get; init; } = string.Empty;

        public DateTime DataCriacao { get; init; }

        public DateTime DataPrazoLote { get; init; }

        public StatusLote Status { get; init; }

        public int QuantidadeIntimacoes { get; init; }
    }
}
