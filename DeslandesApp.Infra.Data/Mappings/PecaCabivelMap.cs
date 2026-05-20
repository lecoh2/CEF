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
    public class PecaCabivelMap : IEntityTypeConfiguration<PecaCabivel>
    {
        public void Configure(EntityTypeBuilder<PecaCabivel> builder)
        {
            builder.ToTable("PECA_CABIVEL");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NomePeca)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
