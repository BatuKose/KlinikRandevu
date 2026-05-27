using Entities.Constants;
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
    internal class YetkiConfig : IEntityTypeConfiguration<Yetki>
    {
        public void Configure(EntityTypeBuilder<Yetki> builder)
        {
            builder.ToTable("yetkiler");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();
            builder.Property(x => x.Ad)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(x => x.Kod)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(x => x.Kod)
                .IsUnique();
            builder.HasData(
                new Yetki { Id = 1, Ad = "Randevu Açma", Kod = YetkiKodlari.RandevuAcma },
                new Yetki { Id = 2, Ad = "Muayene Açma", Kod = YetkiKodlari.MuayeneAcma }
            );
        }
    }
}
