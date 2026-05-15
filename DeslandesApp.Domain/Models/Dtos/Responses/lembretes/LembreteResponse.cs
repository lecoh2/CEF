using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Dtos.Responses.lembretes
{
    public class LembreteResponse
    {
        public Guid Id { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public DateTime Data { get; set; }

        public string Tipo { get; set; } = string.Empty;
        // "Tarefa" ou "Evento"

        public string Categoria { get; set; } = string.Empty;
        public bool Recorrente { get; set; }
        public bool DiaInteiro { get; set; }
        // Hoje / Amanhã / Próximos
    }
}
