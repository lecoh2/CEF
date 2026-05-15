using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Dtos.Responses.Foto
{
    public class FotoResponse
    {
        public Guid IdFoto { get; set; }
        public Guid IdUsuario { get; set; }
        public string FotoNome { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty; // URL para acesso via Angular
    }
}
