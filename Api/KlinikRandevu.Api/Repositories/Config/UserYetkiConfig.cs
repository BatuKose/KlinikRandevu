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
    internal class UserYetkiConfig : IEntityTypeConfiguration<UserYetki>
    {
        public void Configure(EntityTypeBuilder<UserYetki> builder)
        {
            builder.ToTable("user_yetkiler");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.HasOne(x => x.User)
                .WithMany()   
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Yetki)
                .WithMany(y => y.UserYetkiler)
                .HasForeignKey(x => x.YetkiId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.UserId, x.YetkiId })
                .IsUnique();
        }
    }
}
