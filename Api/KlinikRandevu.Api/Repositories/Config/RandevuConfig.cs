using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Config
{
    internal class RandevuConfig : IEntityTypeConfiguration<Randevu>
    {
        public void Configure(EntityTypeBuilder<Randevu> builder)
        {
            builder.ToTable("Randevular");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.DoktorNo).IsRequired();
            builder.Property(x => x.PolNo).IsRequired();
            builder.Property(x => x.HastaTc).IsRequired();
            builder.Property(x => x.ProtocolNo).IsRequired();
            builder.Property(x => x.RandevuTarihi).IsRequired();
            builder.Property(x => x.SureDakika).IsRequired();
            builder.Property(x => x.Notlar).HasMaxLength(500).IsRequired(false);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.HasIndex(x => new { x.DoktorNo, x.RandevuTarihi }).IsUnique();
        }
    }
}
