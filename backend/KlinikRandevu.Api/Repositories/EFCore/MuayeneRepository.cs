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

        public async Task<bool> CakisanRandevuVarMi(int doktorNo,int polNo, DateTime yeniBaslangic, DateTime yeniBitis)
        {
           var gunBaslingc = yeniBaslangic.Date;
           var günBitis=yeniBitis.AddDays(1);
           var ayniGunRandevular = await _repositoryContext.Randevus
              .Where(r =>
                  r.DoktorNo == doktorNo &&
                  r.HastaTc != null &&
                  r.RandevuTarihi >= gunBaslingc &&
                  r.RandevuTarihi < günBitis)
              .Select(r => new { r.RandevuTarihi, r.SureDakika })
              .ToListAsync();

            return ayniGunRandevular.Any(
                r=>r.RandevuTarihi<yeniBaslangic && r.RandevuTarihi.AddMinutes(r.SureDakika)>yeniBaslangic
                );
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

        public async Task<bool> hastaVarmi(long number)
        {
            var hasta = await _repositoryContext.Patients.AnyAsync(h => h.TcKimlik==number);
            return hasta;
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

        public void RandevuOlustur(Randevu randevu)
        {
            _repositoryContext.Randevus.Add(randevu);
        }

        public async Task<bool> randevuSaatiVarmı(int dnumber, int pnumber, TimeSpan baslangic, TimeSpan randevuBitis)
        {
            
            var saat = await _repositoryContext.DoktorCalismaPlanis.AnyAsync(r => r.DoktorNo==dnumber&&
                r.PolNo==pnumber && r.IsActive==true
                && r.BaslangicSaati<=baslangic&& r.BitisSaati>=r.BitisSaati
                );
            return saat;
        }

        public async Task<bool> randevuVarmı(int dnumber, int pnumber, DayOfWeek day)
        {

            var randevu = await _repositoryContext.DoktorCalismaPlanis.AnyAsync(r => r.DoktorNo==dnumber&&
                r.PolNo==pnumber&& r.GunAdi==day && r.IsActive==true
                );
            return randevu;
        }
        public async Task<bool> HastaAyniGunRandevusuVarMi(long hastaTc, int doktorNo, DateTime tarih)
        {
            var gunBaslangic = tarih.Date;
            var gunBitis = gunBaslangic.AddDays(1);

            return await _repositoryContext.Randevus.AnyAsync(r =>
                r.HastaTc == hastaTc &&
                r.DoktorNo == doktorNo &&
                r.RandevuTarihi >= gunBaslangic &&
                r.RandevuTarihi < gunBitis);
        }
        public async Task<DoktorCalismaPlani?> CalismaPlaniGetirAsync( int doktorNo, int polNo, DayOfWeek gun,
        TimeSpan baslangic, TimeSpan bitis)
        {
            return await _repositoryContext.DoktorCalismaPlanis
                .FirstOrDefaultAsync(r =>
                    r.DoktorNo == doktorNo &&
                    r.PolNo == polNo &&
                    r.GunAdi == gun &&
                    r.IsActive == true &&
                    r.BaslangicSaati <= baslangic &&
                    r.BitisSaati >= bitis
                );
        }
    }
}
