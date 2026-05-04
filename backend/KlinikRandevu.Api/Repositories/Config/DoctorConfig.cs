using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Enums.PoliklinikEnum;

namespace Repositories.Config
{
    internal class DoctorConfig : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.ToTable("Doktorlar");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.doktorNo).IsRequired();
            builder.HasIndex(x => x.doktorNo).IsUnique();
            builder.Property(x => x.DoktorAd).IsRequired().HasMaxLength(100);
            builder.Property(x => x.doktorUzKod).IsRequired().HasConversion<int>();
            builder.Property(x => x.doktorTc).IsRequired();
            builder.HasIndex(x => x.doktorTc).IsUnique();
            builder.Property(x => x.ServisNo).IsRequired();
            builder.Property(x => x.tescilNO).IsRequired();

            builder.HasData(
                new Doctor
                {
                    Id = 1,
                    doktorNo = 1,
                    DoktorAd = "HÜSEYİN AĞAC",
                    doktorUzKod = UzmanlikBransi.IcHastaliklari,
                    doktorTc = 11111111111,
                    ServisNo = 1,
                    tescilNO = 10001
                },
                new Doctor
                {
                    Id = 2,
                    doktorNo = 2,
                    DoktorAd = "AYŞE DEMİR",
                    doktorUzKod = UzmanlikBransi.Kardiyoloji,
                    doktorTc = 22222222222,
                    ServisNo = 2,
                    tescilNO = 10002
                },
                new Doctor
                {
                    Id = 3,
                    doktorNo = 3,
                    DoktorAd = "MEHMET YILMAZ",
                    doktorUzKod = UzmanlikBransi.Noroloji,
                    doktorTc = 33333333333,
                    ServisNo = 3,
                    tescilNO = 10003
                }
            );
        }
    }
}
