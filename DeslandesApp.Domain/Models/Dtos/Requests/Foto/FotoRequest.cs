using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Dtos.Requests.Foto
{
    public class FotoRequest
    {
        public Guid Id { get; set; }
        public string Foto { get; set; } = string.Empty;
    }
}
