using DeslandesApp.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Infra.Data.Mappings
{
    public class EnderecoMap : IEntityTypeConfiguration<Endereco>
    {
        public void Configure(EntityTypeBuilder<Endereco> builder)
        {

            builder.ToTable("ENDERECO");

            builder.HasKey(e => e.Id);
          
            builder.Property(e => e.Logradouro).HasColumnName("LOGRADOURO").IsRequired(false); 
            builder.Property(e => e.Numero).HasColumnName("NUMERO").IsRequired(false);
            builder.Property(e => e.Complemento)
    .HasColumnName("COMPLEMENTO")
    .HasMaxLength(250)
    .IsRequired(false); // <- ESSENCIAL
            builder.Property(e => e.Bairro).HasColumnName("BAIRRO");
            builder.Property(e => e.Cep).HasColumnName("CEP");
            builder.Property(e => e.Localidade).HasColumnName("LOCALIDADE").HasMaxLength(100).IsRequired(false);
            builder.Property(e => e.Uf).HasColumnName("UF").HasMaxLength(2).IsRequired(false);
            builder.Property(e => e.IdPessoa).HasColumnName("PESSOA_ID").IsRequired(false);

            #region Relacionamentos
            builder.HasOne(e => e.Pessoa)
                .WithOne(p => p.Endereco)
                .HasForeignKey<Endereco>(e => e.IdPessoa)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion
        }

    }
}
