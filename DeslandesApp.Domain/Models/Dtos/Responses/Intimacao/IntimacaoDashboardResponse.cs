using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Dtos.Responses.Intimacao
{
    public record IntimacaoDashboardResponse
    {
        public int Total { get; init; }

        public int Pendentes { get; init; }

        public int Triadas { get; init; }

        public int DistribuidasAdvogado { get; init; }

        public int DistribuidasLote { get; init; }

        public int Concluidas { get; init; }
    }
}
