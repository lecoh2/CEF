using DeslandesApp.Domain.Models.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Validators
{
    public class PessoaFisicaValidator : AbstractValidator<PessoaFisica>
    {
        public PessoaFisicaValidator()
        {
            RuleFor(p => p.Nome)
                .NotEmpty().WithMessage("O nome é obrigatório")
                .Length(6, 100).WithMessage("O nome deve conter entre 6 e 100 caracteres");
            RuleFor(p => p.CPF).NotEmpty().WithMessage("O CPF é obrigatório");
     //       RuleFor(p => p.Telefone)
     //.NotEmpty().WithMessage("Informe pelo menos um número de telefone")
     //.Must(t => !string.IsNullOrWhiteSpace(t) &&
     //           System.Text.RegularExpressions.Regex.IsMatch(
     //               t.Trim(),
     //               @"^\(?\d{2}\)?\s?9?\d{4}-?\d{4}$"))
     //.WithMessage("Telefone inválido");

            //RuleFor(p => p.IdSexo)
            //    .NotEmpty()
            //    .WithMessage("Escolha um genero");

         ////   RuleFor(p => p.ValorEmail.EnderecoEmail)
         //       .NotEmpty().WithMessage("Informe pelo menos um e-mail para contato")
         //       .EmailAddress().WithMessage("Informe um e-mail válido");
        }
    }
}
