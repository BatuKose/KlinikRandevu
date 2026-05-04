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
    internal class MuayeneKaydiConfig : IEntityTypeConfiguration<MuayeneKaydi>
    {
        public void Configure(EntityTypeBuilder<MuayeneKaydi> builder)
        {
            builder.ToTable("MuayeneKayitlari");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ProtocolNo).IsRequired();
            builder.Property(x => x.DoktorNo).IsRequired();
            builder.Property(x => x.PolNo).IsRequired();
            builder.Property(x => x.HastaTc).IsRequired();
            builder.Property(x => x.MuayeneTarihi).IsRequired();
            builder.Property(x => x.BaslangicSaati).IsRequired();
            builder.Property(x => x.BitisSaati).IsRequired();
            builder.Property(x => x.RandevuId).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
        }
    }
}
