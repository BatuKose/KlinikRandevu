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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
