using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    public class MuayeneRepository:IMuayeneRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public MuayeneRepository(RepositoryContext repositoryContext)
        {
            _repositoryContext=repositoryContext;
        }

        public void CalismaPlaniOlustur(DoktorCalismaPlani plan)
        {
           _repositoryContext.DoktorCalismaPlanis.Add(plan);
        }

        public async Task<bool> doktorVarMI(int number)
        {
            var doktor = await _repositoryContext.Doctors.AnyAsync(d=>d.doktorNo==number);
            return doktor;
        }

        public async Task<int?> PolMaxRanevu(int number)
        {
           var süre=await _repositoryContext.Polikliniks.Where(p=>p.PolNo==number).Select(p=>p.GunlukMaksRandevuSayisi)
                .FirstOrDefaultAsync();
            return süre;
        }

        public async Task<int?> PolMaxSüre(int number)
        {
            var süre = await _repositoryContext.Polikliniks
                .Where(p => p.PolNo == number)
                .Select(p => p.MaxRandevuSuresi)
                .FirstOrDefaultAsync();
            return süre;
        }

        public async Task<bool> PolRandevuMüsaitMi(int number)
        {
            var müsaitlik=await _repositoryContext.Polikliniks.AnyAsync(p=>p.PolNo==number && p.OnlineRandevuAktif==true);
            return müsaitlik;
        }

        public async Task<bool> polVarMI(int number)
        {
            var pol= await _repositoryContext.Polikliniks.AnyAsync(p=>p.PolNo==number &&p.isActive==true);
            return pol;
        }
    }
}
