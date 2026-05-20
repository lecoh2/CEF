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
    public class IntimacaoMap : IEntityTypeConfiguration<Intimacao>
    {
        public void Configure(EntityTypeBuilder<Intimacao> builder)
        {
            builder.ToTable("INTIMACAO");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TextoIntimacao)
                .HasColumnType("VARCHAR(MAX)");

            builder.Property(x => x.ObservacaoTriagem)
                .HasColumnType("VARCHAR(MAX)");

            builder.Property(x => x.StatusTriagem)
                .IsRequired();

            builder.Property(x => x.StatusCumprimento)
                .IsRequired();

            // PROCESSO
            builder.HasOne(x => x.Processo)
                .WithMany()
                .HasForeignKey(x => x.ProcessoId);

            // PEÇA
            builder.HasOne(x => x.PecaCabivel)
                .WithMany()
                .HasForeignKey(x => x.PecaCabivelId);

            // ADVOGADO
            builder.HasOne(x => x.Advogado)
                .WithMany()
                .HasForeignKey(x => x.AdvogadoId);

            // LOTE
            builder.HasOne(x => x.Lote)
                .WithMany()
                .HasForeignKey(x => x.LoteId);

            // ÍNDICES
            builder.HasIndex(x => x.DataIntimacao);

            builder.HasIndex(x => x.StatusTriagem);

            builder.HasIndex(x => x.AdvogadoId);
        }
    }
}
