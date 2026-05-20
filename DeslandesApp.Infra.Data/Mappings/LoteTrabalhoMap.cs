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
    public class LoteTrabalhoMap : IEntityTypeConfiguration<LoteTrabalho>
    {
        public void Configure(EntityTypeBuilder<LoteTrabalho> builder)
        {
            builder.ToTable("LOTE_TRABALHO");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NumeroLote)
                .HasMaxLength(50);

            builder.Property(x => x.Status)
                .IsRequired();

            // =========================
            // RESPONSÁVEL
            // =========================
            builder.HasOne(x => x.Responsavel)
                .WithMany()
                .HasForeignKey(x => x.ResponsavelId)
                .OnDelete(DeleteBehavior.NoAction);

            // =========================
            // COORDENADOR
            // =========================
            builder.HasOne(x => x.Coordenador)
                .WithMany()
                .HasForeignKey(x => x.CoordenadorId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
