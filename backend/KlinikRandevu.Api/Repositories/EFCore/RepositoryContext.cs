using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    public class RepositoryContext:DbContext
    {
        public RepositoryContext(DbContextOptions options) : base(options){ }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Poliklinik>Polikliniks { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoktorCalismaPlani>DoktorCalismaPlanis { get; set; }
        public DbSet<MuayeneKaydi> MuayeneKaydis {  get; set; }
        public DbSet<Randevu>Randevus { get; set; } 
        public DbSet<SistemParametresi>parametreler {  get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserLog> userLogs { get; set; }
        public DbSet<Yetki> Yetkiler { get; set; }
        public DbSet<UserYetki> UserYetkiler { get; set; }
        public DbSet<Tatil> Tatil { get; set; }
        public DbSet<IcdApiEntegrasyon> IcdApiEntegrasyon { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetProperties())
            .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?)))
                {
                    property.SetColumnType("datetime");
                }
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
