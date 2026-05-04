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
    internal class DoktorCalismaPlaniConfig : IEntityTypeConfiguration<DoktorCalismaPlani>
    {
        public void Configure(EntityTypeBuilder<DoktorCalismaPlani> builder)
        {
            builder.ToTable("CalismaPlanlari");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.DoktorNo).IsRequired();
            builder.Property(x => x.PolNo).IsRequired();
            builder.Property(x => x.GunAdi).IsRequired().HasConversion<int>();
            builder.Property(x => x.BaslangicSaati).IsRequired();
            builder.Property(x => x.BitisSaati).IsRequired();
            builder.Property(x => x.RandevuSuresiDk).IsRequired();
            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            builder.HasData(
                // Hüseyin Ağac - Pzt/Çarş/Cuma
                new DoktorCalismaPlani { Id=1, DoktorNo=1, PolNo=1, GunAdi=DayOfWeek.Monday, BaslangicSaati=new TimeSpan(8, 0, 0), BitisSaati=new TimeSpan(12, 0, 0), RandevuSuresiDk=10, IsActive=true },
                new DoktorCalismaPlani { Id=2, DoktorNo=1, PolNo=1, GunAdi=DayOfWeek.Wednesday, BaslangicSaati=new TimeSpan(8, 0, 0), BitisSaati=new TimeSpan(12, 0, 0), RandevuSuresiDk=10, IsActive=true },
                new DoktorCalismaPlani { Id=3, DoktorNo=1, PolNo=1, GunAdi=DayOfWeek.Friday, BaslangicSaati=new TimeSpan(8, 0, 0), BitisSaati=new TimeSpan(12, 0, 0), RandevuSuresiDk=10, IsActive=true },
                // Ayşe Demir - Salı/Perşembe
                new DoktorCalismaPlani { Id=4, DoktorNo=2, PolNo=2, GunAdi=DayOfWeek.Tuesday, BaslangicSaati=new TimeSpan(9, 0, 0), BitisSaati=new TimeSpan(17, 0, 0), RandevuSuresiDk=15, IsActive=true },
                new DoktorCalismaPlani { Id=5, DoktorNo=2, PolNo=2, GunAdi=DayOfWeek.Thursday, BaslangicSaati=new TimeSpan(9, 0, 0), BitisSaati=new TimeSpan(17, 0, 0), RandevuSuresiDk=15, IsActive=true },
                // Mehmet Yılmaz - Pzt'den Cuma'ya
                new DoktorCalismaPlani { Id=6, DoktorNo=3, PolNo=3, GunAdi=DayOfWeek.Monday, BaslangicSaati=new TimeSpan(10, 0, 0), BitisSaati=new TimeSpan(16, 0, 0), RandevuSuresiDk=20, IsActive=true },
                new DoktorCalismaPlani { Id=7, DoktorNo=3, PolNo=3, GunAdi=DayOfWeek.Tuesday, BaslangicSaati=new TimeSpan(10, 0, 0), BitisSaati=new TimeSpan(16, 0, 0), RandevuSuresiDk=20, IsActive=true },
                new DoktorCalismaPlani { Id=8, DoktorNo=3, PolNo=3, GunAdi=DayOfWeek.Wednesday, BaslangicSaati=new TimeSpan(10, 0, 0), BitisSaati=new TimeSpan(16, 0, 0), RandevuSuresiDk=20, IsActive=true },
                new DoktorCalismaPlani { Id=9, DoktorNo=3, PolNo=3, GunAdi=DayOfWeek.Thursday, BaslangicSaati=new TimeSpan(10, 0, 0), BitisSaati=new TimeSpan(16, 0, 0), RandevuSuresiDk=20, IsActive=true },
                new DoktorCalismaPlani { Id=10, DoktorNo=3, PolNo=3, GunAdi=DayOfWeek.Friday, BaslangicSaati=new TimeSpan(10, 0, 0), BitisSaati=new TimeSpan(16, 0, 0), RandevuSuresiDk=20, IsActive=true }
            );
        }
    }
}
