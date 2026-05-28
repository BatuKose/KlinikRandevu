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
    internal class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");
            builder.HasKey(x => x.UserID);

            builder.Property(x => x.UserID)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.UserName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Password)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.Email)
                .HasMaxLength(100);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Surname)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.RefreshToken)
                .HasMaxLength(256);

            builder.Property(x => x.RefreshTokenExpiry);

            builder.HasData(
                new User
                {
                    UserID=1,
                    UserName="ADMIN",
                    Email="xxxx@gmail.com",
                    Name="BATUHAN",
                    Surname="KÖSE",
                    Password="12345"
                });
        }
    }
}
