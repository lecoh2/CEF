using DeslandesApp.Domain.Models.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Validators
{
    public class PecaCabivelValidator : AbstractValidator<PecaCabivel>
    {
        public PecaCabivelValidator()
        {
            RuleFor(x => x.NomePeca)
                .NotEmpty()
                .WithMessage("O nome da peça é obrigatório.")
                .MaximumLength(150)
                .WithMessage("O nome da peça deve ter no máximo 150 caracteres.");

            RuleFor(x => x.PrazoDias)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O prazo deve ser maior ou igual a zero.");

            RuleFor(x => x.SugestaoComplexidadePadrao)
                .IsInEnum()
                .WithMessage("Complexidade inválida.");
        }
    }
}
