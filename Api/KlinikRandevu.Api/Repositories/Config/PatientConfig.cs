using Entities.Enums;
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
    internal class PatientConfig : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Surname).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Protocol).IsRequired();
            builder.Property(x => x.Address).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Phone).IsRequired().HasMaxLength(20);
            builder.Property(x => x.BloodType).IsRequired(); 
            builder.Property(x => x.Gender).IsRequired();     

            builder.HasData(
                new Patient
                {
                    Id = 1,
                    Name = "BATUHAN",
                    Surname = "KÖSE",
                    Protocol = 20261,
                    Address = "BURSA, Türkiye",
                    Phone = "5378102935",
                    BirthDate = new DateTime(2001, 2, 21),
                    BloodType = BloodTypeEnum.OPositive,   
                    Gender = GenderEnum.male,         
                    IsActive = true,
                    TcKimlik=11111111111
                });
        }
    }
}

