using Entities.Data_Transfer_Objects.Muayene;
using Entities.Enums;
using Entities.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
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
        public async Task<bool> hastaVarmiProtokol(int number)
        {
            var hasta = await _repositoryContext.Patients.AnyAsync(h => h.Protocol==number);
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
        public async Task<Randevu?> HastanınRanevusunuGetir(long hastaTc, int doktorNo, DateTime tarih)
        {
            return await _repositoryContext.Randevus
                .FirstOrDefaultAsync(r =>
                    r.HastaTc == hastaTc &&
                    r.DoktorNo == doktorNo &&
                    r.RandevuTarihi == tarih);
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

        public  void MuayeneKaydiOlustur(MuayeneKaydi muayene)
        {
             _repositoryContext.Add(muayene);
        }

        public async Task<bool> AyniGünMuayenesiVarmi(int pol, int protokol, DateTime muayenetarihi)
        {
            
            var result = await _repositoryContext.MuayeneKaydis.AnyAsync(
                 m => m.IsActive== true && m.PolNo==pol && m.ProtocolNo==protokol
                 && m.MuayeneTarihi==muayenetarihi);
            return result;
        }
        public async Task<List<HastaRandevulariniGetirDTO>> HastaRandevulariniGetir( DateTime baslangic, DateTime bitis)
        {
            var sqlParams = new[]
            {
                new SqlParameter("@baslangic", SqlDbType.DateTime) { Value = baslangic },
                new SqlParameter("@bitis", SqlDbType.DateTime) { Value = bitis },
            };

            return await _repositoryContext.Database
                .SqlQueryRaw<HastaRandevulariniGetirDTO>(@"
            SELECT 
                r.Id AS DosyaId,
                p.Protocol AS Protokol,
                p.Name AS Ad,
                p.Surname AS Soyad,
                p.TcKimlik AS Tc,
                pol.Name AS Poliklinik,
                d.DoktorAd AS Doktor,
                uz.Ad AS UzmanlikDali,
                r.RandevuTarihi AS RandevuTarihi
            FROM Randevular AS r
            INNER JOIN Patients AS p ON p.Protocol = r.ProtocolNo
            INNER JOIN Poliklinikler AS pol ON pol.PolNo = r.PolNo
            INNER JOIN Doktorlar AS d ON d.doktorNo = r.DoktorNo
            LEFT JOIN UzmanlikDallari AS uz ON uz.Kod = d.doktorUzKod
            WHERE r.RandevuTarihi IS NOT NULL
              AND r.RandevuTarihi > '1900-01-01'
              AND p.IsActive = 1
              AND pol.IsActive = 1
              AND r.RandevuTarihi BETWEEN @baslangic AND @bitis
            ORDER BY r.RandevuTarihi", sqlParams).ToListAsync();
        }
        public async Task<List<HastaRandevulariniGetirDTO>> HastanınRandevulariniGetir(int protokol)
        {
            var sqlParams = new[]
            {
                new SqlParameter("@protkol", SqlDbType.Int) { Value = protokol }           
            };

            return await _repositoryContext.Database
                .SqlQueryRaw<HastaRandevulariniGetirDTO>(@"
            SELECT 
                r.Id AS DosyaId,
                p.Protocol AS Protokol,
                p.Name AS Ad,
                p.Surname AS Soyad,
                p.TcKimlik AS Tc,
                pol.Name AS Poliklinik,
                d.DoktorAd AS Doktor,
                uz.Ad AS UzmanlikDali,
                r.RandevuTarihi AS RandevuTarihi
            FROM Randevular AS r
            INNER JOIN Patients AS p ON p.Protocol = r.ProtocolNo
            INNER JOIN Poliklinikler AS pol ON pol.PolNo = r.PolNo
            INNER JOIN Doktorlar AS d ON d.doktorNo = r.DoktorNo
            LEFT JOIN UzmanlikDallari AS uz ON uz.Kod = d.doktorUzKod
            WHERE r.RandevuTarihi IS NOT NULL
              AND r.RandevuTarihi > '1900-01-01'
              AND p.IsActive = 1
              AND pol.IsActive = 1
               AND p.Protocol=@protkol
            ORDER BY r.RandevuTarihi", sqlParams).ToListAsync();
        }
        public async Task<List<DoktorRandevuHatirlatmaEmailDTO>>DoktorRandevuHatirlatma(int doktorno)
        {
            var sqlParams = new[]
            {
                new SqlParameter("@doktorno",SqlDbType.Int){Value = doktorno}
            };
            return await _repositoryContext.Database.SqlQueryRaw<DoktorRandevuHatirlatmaEmailDTO>("" +
                @"SELECT 
                    d.DoktorAd AS doktorad,
                    p.Name AS polad,
                    pa.Name AS hastaad,
                    pa.Surname AS hastsoyad,
                    r.RandevuTarihi AS randevutarihi,
	                d.Email as doktormail
                FROM Randevular r
                INNER JOIN Doktorlar d ON r.DoktorNo = d.DoktorNo
                INNER JOIN Poliklinikler p ON p.PolNo = r.PolNo
                INNER JOIN Patients pa ON pa.Protocol = r.ProtocolNo
                WHERE CAST(r.RandevuTarihi AS DATE) = CAST(GETDATE() AS DATE)
                  AND d.isActive = 1
                  AND d.Email IS NOT NULL
                  and r.DoktorNo=@doktorno
                ORDER BY d.DoktorNo, r.RandevuTarihi;", sqlParams).ToListAsync();
        }
        public async Task<Doctor>DoktoruGetir(int number)
        {
            var doktor = await _repositoryContext.Doctors.SingleOrDefaultAsync(d=>d.doktorNo==number);
            return doktor;
        }
        public async Task<Poliklinik> PolGetir(int number)
        {
            var pol = await _repositoryContext.Polikliniks.SingleOrDefaultAsync(d => d.PolNo==number);
            return pol;
        }
        public async Task<int> DoktorIleriRandevuSorgula(int number)
        {
            var sqlParams = new[]
            {
                new SqlParameter("@doktorno", SqlDbType.Int) { Value = number }
            };

            return await _repositoryContext.Database
                .SqlQueryRaw<int>(@"
            SELECT 
                COUNT(*) AS Value
            FROM Randevular AS r
            INNER JOIN Patients AS p ON p.Protocol = r.ProtocolNo
            INNER JOIN Poliklinikler AS pol ON pol.PolNo = r.PolNo
            INNER JOIN Doktorlar AS d ON d.doktorNo = r.DoktorNo
            WHERE r.DoktorNo = @doktorno
              AND r.RandevuTarihi >= CAST(GETDATE() AS DATE)", sqlParams).FirstAsync();
        }
        public async Task<int> PolIleriRandevuSorgula(int number)
        {
            var sqlParams = new[]
            {
                new SqlParameter("@polno", SqlDbType.Int) { Value = number }
            };

            return await _repositoryContext.Database
                .SqlQueryRaw<int>(@"
            SELECT 
                COUNT(*) AS Value
            FROM Randevular AS r
            INNER JOIN Patients AS p ON p.Protocol = r.ProtocolNo
            INNER JOIN Poliklinikler AS pol ON pol.PolNo = r.PolNo
            INNER JOIN Doktorlar AS d ON d.doktorNo = r.DoktorNo
            WHERE  pol.PolNo = @polno
              AND r.RandevuTarihi >= CAST(GETDATE() AS DATE)", sqlParams).FirstAsync();
        }
        public async Task<PoliklinikEnum.UzmanlikBransi> PolUzmanlikKoduAsync(int polNo)
        {
            var uzmanlik = await _repositoryContext.Polikliniks
                .Where(p => p.PolNo == polNo)
                .Select(p => p.PolUzKod)
                .SingleOrDefaultAsync();
            return uzmanlik;
        }
        
    }
}
