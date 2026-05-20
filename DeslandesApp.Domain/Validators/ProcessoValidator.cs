using DeslandesApp.Domain.Models.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Validators
{
    public class ProcessoValidator : AbstractValidator<Processo>
    {
        public ProcessoValidator()
        {
           
     //       RuleFor(x => x.NumeroProcesso)
     //.Matches(@"^\d{7}-\d{2}\.\d{4}\.\d{1}\.\d{2}\.\d{4}$")
     //.WithMessage("Número de processo inválido. Use o padrão CNJ: 0000000-00.0000.0.00.0000");
            RuleFor(x => x.Titulo)
         .NotEmpty()
         .Matches(@"^[\p{L}\p{N}\s\.\-\,\/ºª:]+$")
         .WithMessage("O campo deve conter apenas letras, números e caracteres válidos.");
            RuleFor(x => x.LinkTribunal)
        .Cascade(CascadeMode.Stop)
        .Must(link =>
            string.IsNullOrWhiteSpace(link) ||
            Uri.TryCreate(
                link.StartsWith("http")
                    ? link
                    : $"https://{link}",
                UriKind.Absolute,
                out _
            )
        )
        .WithMessage("Informe um link válido.");

        }
    }
}
