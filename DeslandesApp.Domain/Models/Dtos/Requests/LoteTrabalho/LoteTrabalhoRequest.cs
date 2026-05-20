using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Dtos.Requests.LoteTrabalho
{
    public record LoteTrabalhoRequest
    {
        public Guid ResponsavelId { get; init; }

        public Guid CoordenadorId { get; init; }

        public DateTime DataPrazoLote { get; init; }

        public List<Guid> IntimacoesIds { get; init; } = [];
    }
}
