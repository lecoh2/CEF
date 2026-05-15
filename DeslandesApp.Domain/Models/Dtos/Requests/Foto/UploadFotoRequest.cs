using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Dtos.Requests.Foto
{
    public record UploadFotoRequest
    {
        [Required(ErrorMessage = "O arquivo é obrigatório.")]
        [Display(Name = "Arquivo")]
        public IFormFile File { get; init; } = default!;
    }
}
