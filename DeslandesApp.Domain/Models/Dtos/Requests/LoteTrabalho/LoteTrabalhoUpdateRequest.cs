using DeslandesApp.Domain.Models.Enum.DeslandesApp.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Dtos.Requests.LoteTrabalho
{
    public record LoteTrabalhoUpdateRequest
    {
        public Guid ResponsavelId { get; set; }

        public DateTime DataPrazoLote { get; set; }

        public StatusLote Status { get; set; }
    }
}
