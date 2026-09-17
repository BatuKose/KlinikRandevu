using Entities.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Config
{
    internal class PoliklinikConfig : IEntityTypeConfiguration<Poliklinik>
    {
        public void Configure(EntityTypeBuilder<Poliklinik> builder)
        {
            builder.ToTable("Poliklinikler");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.PolNo).IsRequired();
            builder.HasIndex(x => x.PolNo).IsUnique();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Aciklama).HasMaxLength(500);
            builder.Property(x => x.PolUzKod).IsRequired().HasConversion<int>();
            builder.Property(x => x.DoktorNo).IsRequired();
            builder.Property(x => x.KatNo).IsRequired(false);
            builder.Property(x => x.OdaNo).HasMaxLength(20).IsRequired(false);
            builder.Property(x => x.MaxRandevuSuresi).IsRequired(false);
            builder.Property(x => x.GunlukMaksRandevuSayisi).IsRequired(false);
            builder.Property(x => x.Telefon).HasMaxLength(20).IsRequired(false);
            builder.Property(x => x.OnlineRandevuAktif).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.isActive).IsRequired().HasDefaultValue(true);

            builder.HasData(
                new Poliklinik
                {
                    Id=1,
                    PolNo=1,
                    Name="İÇ HASTALIKLARI HÜSEYİN AĞAC",
                    Aciklama="İÇ Hastalıkları pol öğleden önce",
                    PolUzKod=Entities.Enums.PoliklinikEnum.UzmanlikBransi.IcHastaliklari,
                    DoktorNo=1,
                    KatNo=1,
                    OdaNo="Birinci kat",
                    MaxRandevuSuresi=10,
                    GunlukMaksRandevuSayisi=30,
                    Telefon="1650",
                    OnlineRandevuAktif=true,
                    isActive=true
                }
                );
        }
    }
}
