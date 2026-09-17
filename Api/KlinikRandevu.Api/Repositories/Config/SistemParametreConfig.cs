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
    public class SistemParametresiConfiguration : IEntityTypeConfiguration<SistemParametresi>
    {
        public void Configure(EntityTypeBuilder<SistemParametresi> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.ParametreAdi).IsRequired().HasMaxLength(100);
            builder.HasIndex(p => p.ParametreAdi).IsUnique();
            builder.Property(p => p.Deger1).HasMaxLength(500);
            builder.Property(p => p.Deger2).HasMaxLength(500);
            builder.Property(p => p.Deger3).HasMaxLength(500);
            builder.Property(p => p.Deger4).HasMaxLength(500);
            builder.Property(p => p.Deger5).HasMaxLength(500);
            builder.Property(p => p.Aciklama).HasMaxLength(500);
            builder.HasData(
                new SistemParametresi
                {
                    Id=1,
                    ParametreAdi="KADIN_DOGUM_ERKEK_YASAKLA",
                    Deger1="EVET",
                    Aciklama="D1: Aktif mi (EVET/HAYIR), D2: Hata mesajı"
                },
                new SistemParametresi
                {
                    Id=2,
                    ParametreAdi="PEDIATRI_YAS_LIMITI",
                    Deger1="EVET",
                    Deger2="0",
                    Deger3="16",
                    Aciklama="D1: Aktif mi, D2: Min yaş, D3: Max yaş"
                }
                );

        }
    }
}
