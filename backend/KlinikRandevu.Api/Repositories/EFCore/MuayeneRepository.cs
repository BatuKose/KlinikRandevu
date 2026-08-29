using Entities.Data_Transfer_Objects.Muayene;
using Entities.Enums;
using Entities.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Repositories.EFCore
{
    public class MuayeneRepository : IMuayeneRepository
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
                  r.HastaTc > 0 &&
                  r.RandevuTarihi >= gunBaslingc &&
                  r.RandevuTarihi < günBitis
                  && r.iptal==false
                  )
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
        public async Task<bool> hastaTelNoVarmi(int number)
        {
            var hasta = await _repositoryContext.Patients.Where(h=> h.Protocol==number && h.Phone !=null).AnyAsync();
            return hasta;
        }
        public async Task<string?> HastaCepTelefonGetir(int protokol)
        {
            var cepTel = await _repositoryContext.Patients
                .Where(p => p.Protocol == protokol)  
                .Select(p => p.Phone)                    
                .FirstOrDefaultAsync();                  
            return cepTel;
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
                r.RandevuTarihi < gunBitis
                && r.iptal==false
                );
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
        public async Task<int>ileriTarihliRandevuVarmi(int protokol)
        {

            try
            {
                return await _repositoryContext.Database
                    .SqlQueryRaw<int>(@"
            SELECT COUNT(*) AS Value
            FROM Randevular r
            WHERE r.ProtocolNo = @protokol
              AND r.iptal = 0
              AND r.RandevuTarihi >= CAST(GETDATE() AS DATE)
        ", new SqlParameter("@protokol", protokol))
                    .FirstAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
                throw;
            }
            ;
        }
        public async Task<int>HastaninHicAktifMuayenesiOlduMU(int protokol)
        {
            var sqlParams = new[]
            {
                new SqlParameter("@protokol",SqlDbType.BigInt) { Value = protokol }

            };
            return await _repositoryContext.Database.SqlQueryRaw<int>(@"
                SELECT COUNT(*) AS Value
                FROM MuayeneKayitlari m
                WHERE m.ProtocolNo = @protokol AND m.IsActive = 1
                ", sqlParams).FirstAsync();
        }
        public async Task<List<RandevuluHastalarinBilgilerDTO>>RandevuluHastaBilgileri( DateTime baslangic, DateTime bitis,bool muayeneOlduMu)
        {
            var sqlParams = new[]
            {
                new SqlParameter("@baslangic",SqlDbType.DateTime) {Value = baslangic},
                new SqlParameter("@bitis",SqlDbType.DateTime) {Value=bitis}
            };
            var muayneSarti = muayeneOlduMu ? " " : "and m.Id is  not null";
            var sql = $@"
            select r.RandevuTarihi as randevutarihi, d.DoktorAd,
                   pol.Name as poladi, u.Ad as uzmanlik,
                   CONCAT(p.Name, ' ', p.Surname) as hasta,
                   CASE p.Gender
                        WHEN 1 THEN 'Kadın'
                        WHEN 2 THEN 'Erkek'
                        ELSE 'Belirtilmedi'
                   END AS cinsiyet,
                   p.Address as adres
            from Randevular r
            left join MuayeneKayitlari m on r.Id = m.RandevuId
            inner join Doktorlar d on d.doktorNo = r.DoktorNo
            inner join Patients p on p.Protocol = r.ProtocolNo
            inner join Poliklinikler pol on pol.PolNo = r.PolNo
            inner join UzmanlikDallari u on u.Kod = pol.PolUzKod
            where r.iptal = 0 and d.isActive = 1 and p.IsActive = 1
              and r.RandevuTarihi between @baslangic and @bitis
              {muayneSarti}
            ORDER BY r.RandevuTarihi";
            try
            {
                return await _repositoryContext.Database
                    .SqlQueryRaw<RandevuluHastalarinBilgilerDTO>(sql, sqlParams)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
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
        public async Task<Doctor?>DoktoruGetir(int number)
        {
            var doktor = await _repositoryContext.Doctors.SingleOrDefaultAsync(d=>d.doktorNo==number);
            return doktor;
        }
        public async Task<Poliklinik?> PolGetir(int number)
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
        public async Task<Randevu?>GetRandevuById(int id)
        {
            var randevu = await _repositoryContext.Randevus.SingleOrDefaultAsync(x=>x.Id== id && x.iptal==false);
            return randevu;

        }
        public void teshisEkle(teshisler teshisler)
        {
            _repositoryContext.Add(teshisler);
        }
        public async Task<MuayeneKaydi?>GetMuayeneById(int id)
        {
            try
            {
                var muayene = await _repositoryContext.MuayeneKaydis.SingleOrDefaultAsync(i => i.Id== id && i.IsActive==true);
                return muayene;

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
        public async Task<bool>MuayenedeAyniTeshisdenVarmi(int sira,string tcode)
        {
            bool result = await _repositoryContext.Teshisler.AllAsync(m => m.Id==sira && m.teshisKod==tcode);
            return result;
        }
        public async Task<bool>MuayenedeTeshisVarMı(int muayeneid)
        {
            bool result = await _repositoryContext.Teshisler.AnyAsync(t => t.muayeneId==muayeneid);
            return result;
        }
        public async Task<List<JobHatirlatmaSorguDTO>> JobYarininRandevuluHastalari(DateTime basla, DateTime bitis)
        {
            var sqlParams = new[]
            {
        new SqlParameter("@baslangic", basla),  
        new SqlParameter("@bitis", bitis)
    };

            return await _repositoryContext.Database
                .SqlQueryRaw<JobHatirlatmaSorguDTO>(@"
            select 
                r.RandevuTarihi as RandevuTarihi,
                pol.Name        as Poliklinik,
                p.Email         as Email,
                p.Phone as numara,
                d.DoktorAd      as DoktorAd,
                r.hatirlatmaMailiGonderildi as randevuMailGonderildimi,
                r.Id as randevuId
            from Randevular r
            INNER JOIN Patients p    on p.Protocol = r.ProtocolNo
            INNER JOIN Poliklinikler pol on pol.PolNo = r.PolNo
            INNER JOIN Doktorlar d    on d.doktorNo = r.DoktorNo
            where r.RandevuTarihi between @baslangic and @bitis
              and r.hatirlatmaMailiGonderildi = 0
        ", sqlParams).ToListAsync();
        }
        public async Task HatirlatmaMilUpte(IEnumerable<int> randevuIdler)
        {
            if(!randevuIdler.Any())
            {
                return;
            }
            var idListesi = string.Join(",", randevuIdler);
            await _repositoryContext.Database.ExecuteSqlRawAsync(
           $"UPDATE Randevular SET hatirlatmaMailiGonderildi = 1 WHERE Id IN ({idListesi})");
        }
        public async Task HatirlatmaTaahütnameUpdateSMS(IEnumerable<int> taahütnameIdler)
        {
            if (taahütnameIdler == null || !taahütnameIdler.Any())
                return;

            var idListesi = string.Join(",", taahütnameIdler);

            await _repositoryContext.Database.ExecuteSqlRawAsync(
                $"update taahütname set BilgilendirmeSms=1 where Id IN ({idListesi})");
        }
        public async Task HatirlatmaBekleyenRandevuUpdate(IEnumerable<int> bekleyenId)
        {
            if (bekleyenId == null || !bekleyenId.Any())
                return;

            var idListesi = string.Join(",", bekleyenId);

            await _repositoryContext.Database.ExecuteSqlRawAsync(
                $"update RandevuBekleyenHastalar set Bilgilendirme=1 where Id IN ({idListesi})");
        }

        public async Task HatirlatmaTaahütnameUpdateMAIL(IEnumerable<int> taahütnameIdler)
        {
            if (taahütnameIdler == null || !taahütnameIdler.Any())
                return;

            var idListesi = string.Join(",", taahütnameIdler);

            await _repositoryContext.Database.ExecuteSqlRawAsync(
                $"update taahütname set BilgilendirmeMail=1 where Id IN ({idListesi})");
        }

        public async Task HatirlatmaTaahütnameUpdateALL(IEnumerable<int> taahütnameIdler)
        {
            if (taahütnameIdler == null || !taahütnameIdler.Any())
                return;

            var idListesi = string.Join(",", taahütnameIdler);

            await _repositoryContext.Database.ExecuteSqlRawAsync(
                $"update taahütname set BilgilendirmeMail=1, BilgilendirmeSms=1 where Id IN ({idListesi})");
        }

        public async Task<List<int>> DoktorIdleriniGetir()
        {
            return await _repositoryContext.Doctors
                .Where(d => d.isActive==true)
                .Select(d => d.Id)
                .ToListAsync();
        }

        public async Task<List<(int id,TimeSpan baslangicSaati)>> MuayenesiKapanmamisMuayeneIdleriniGetir()
        {
            return await _repositoryContext.MuayeneKaydis
             .Where(m => m.MuayeneTarihi == DateTime.Today && m.BitisSaati == null)
             .Select(m => new ValueTuple<int, TimeSpan>(m.Id, m.BaslangicSaati))
             .ToListAsync();
        }
        public async Task<int> MuayeneKapatmaUpdate(TimeSpan sure)
        {
            return await _repositoryContext.Database.ExecuteSqlRawAsync(
                "UPDATE MuayeneKayitlari " +
                "SET BitisSaati = DATEADD(MINUTE, @dk, BaslangicSaati) " +
                "WHERE MuayeneTarihi = CAST(GETDATE() AS date) AND BitisSaati IS NULL",
                new SqlParameter("@dk", (int)sure.TotalMinutes));
        }
        public async Task<List<YariniHastalariniGetirDTO>> YariniHastalariniGetir()
        {
            return await _repositoryContext.Database.SqlQueryRaw<YariniHastalariniGetirDTO>(@"
                SELECT 
                    r.DoktorNo                    AS doktor,
                    r.PolNo                       AS polno,
                    r.ProtocolNo                  AS protokol,
                    r.RandevuTarihi               AS tarih,
                    CAST(r.RandevuTarihi AS TIME) AS muayenesaati,
                    r.HastaTc                     AS tc,
	                r.Id                          AS randevuid
                FROM Randevular r
                LEFT JOIN MuayeneKayitlari m ON m.RandevuId = r.Id
                WHERE r.RandevuTarihi >= DATEADD(DAY, 1, CAST(GETUTCDATE() AS DATE))
                AND r.RandevuTarihi <  DATEADD(DAY, 2, CAST(GETUTCDATE() AS DATE))
                AND r.iptal = 0 and m.Id is null
                ORDER BY r.RandevuTarihi;
             ").ToListAsync();
        }
        public async Task<List<TaahütBilgilendirme>>YaklasanTahütBilgilendirme(int  sms)
        {
            string sql = "";
            if (sms == 1)
                sql = "and t.BilgilendirmeSms=0";
            else if (sms == 2)     
                sql = "and t.BilgilendirmeMail=0";
            else                   
                sql = "and (t.BilgilendirmeSms=0 or t.BilgilendirmeMail=0)";

            try
            {
                var mainSorgu = $@"select 
                p.Email as mail,p.Phone as tel,t.ToplamBorc as borc
                ,t.SonOdemeTarihi as SonOdemeTarihi, t.TahütTarihi as TaTarih,
                pol.Name as polAd, m.MuayeneTarihi as muaTarih,t.Id as taahütnameId
                FROM taahütname t
                INNER JOIN MuayeneKayitlari m on t.MuayeneId=m.Id
                INNER JOIN Patients p on p.Protocol=m.ProtocolNo
                INNER JOIN Poliklinikler pol on m.PolNo=pol.PolNo
                 WHERE  t.SonOdemeTarihi >= DATEADD(DAY, 1, CAST(GETUTCDATE() AS DATE)) AND t.SonOdemeTarihi <  DATEADD(DAY, 2, CAST(GETUTCDATE() AS DATE))
                 AND t.iptal=0 and t.odendi=0 {sql}";
                return await _repositoryContext.Database.SqlQueryRaw<TaahütBilgilendirme>(mainSorgu).ToListAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString(), "Taahütname bilgilendirme job'u patladı.");
                throw;
            }
           
        }
        public async Task<List<RandevuBekleyenHastalar>>RandevuBekleyenHastalariGetirAsync()
        {
            try
            {
                return await _repositoryContext.Database.SqlQueryRaw<RandevuBekleyenHastalar>(@"SELECT 
                    b.Id,b.tcKimlik, protokol, b.doktorNo, b.polNo, b.RandevuTarihi,
                    b.Bilgilendirme, b.RandevuVerildi, b.randevuNotu
                    FROM RandevuBekleyenHastalar b
                    INNER JOIN Randevular r ON r.RandevuTarihi = b.RandevuTarihi AND r.PolNo=b.polNo
                    WHERE r.iptal = 1
                    AND CAST(b.RandevuTarihi AS DATE) BETWEEN CAST(GETDATE() AS DATE) AND CAST(DATEADD(DAY, 1, GETDATE()) AS DATE);"
                ).ToListAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString(), ex.ToString());
                return new List<RandevuBekleyenHastalar>();
            }
        }
        public async Task<Tetkikler> PoliklinikMuaynesiGetir()
        {
            var tetkik = await _repositoryContext.Tetkikler.SingleOrDefaultAsync(t=>t.Kodu=="ASC123" && t.aktifMi==true);
            return tetkik;
        }
        public async Task<Tetkikler> TetkikGetir( string bilgi)
        {
            var tetkik = await _repositoryContext.Tetkikler.FirstOrDefaultAsync(t => t.Kodu.Contains(bilgi) || t.TetikAdi.Contains(bilgi) && t.aktifMi==true);
            return tetkik;
        }
        public async Task<bool>MuayenedeTetkikDahaOncedenIslenmisMı(string tetkik,int muayeneId)
        {
            var result = await _repositoryContext.TedaviKaydi.AnyAsync(t => t.tedaviKodu==tetkik && t.MuyaneId==muayeneId);
            return result;
        }
        public void TedaviKaydiEkle(TedaviKaydi tedaviKaydi)
        {
            _repositoryContext.Add(tedaviKaydi);
        }
        public async Task<List<TedaviKaydi>>OdenmemisTedavileriGetir(int protokol)
        {
            var tedaviler = await _repositoryContext.TedaviKaydi.Where(t => t.prtokol==protokol && t.Odendi==false).ToListAsync();
            return tedaviler;
        }
        public async Task<TedaviKaydi> TedaviKaydiGetir(int dosyaid)
        {
            var tedaviKaydi= await _repositoryContext.TedaviKaydi.SingleOrDefaultAsync(t=>t.MuyaneId==dosyaid);
            return tedaviKaydi;
        }
        public void TahütnameEKle(Taahütname taahütname)
        {
            _repositoryContext.Add(taahütname);
        }

        public async Task<double> MuayeneKaydininToplamBorucunuGetir(int dosyaid)
        {
          
            var borc = await _repositoryContext.TedaviKaydi.Where(b => b.MuyaneId==dosyaid && b.Odendi==false).SumAsync(b => b.fiyat);
            return borc;
        }
        public async Task<List<TedaviKaydi>> MuayeneKaydininOdenecekTedavileri(int muayeneId)
        {
            var tedaviler = await _repositoryContext.TedaviKaydi.Where(t => t.MuyaneId==muayeneId && t.Odendi==false).ToListAsync();
            return tedaviler;
        }
        public async Task<List<TedaviKaydi>> MuayeneKaydininOdemesiIptalEdilecekTedavileri(int muayeneId)
        {
            var tedaviler = await _repositoryContext.TedaviKaydi.Where(t => t.MuyaneId==muayeneId && t.Odendi==true).ToListAsync();
            return tedaviler;
        }
        public async Task<double> TedaviKaydininToplamBorucunuGetir(int dosyaid)
        {

            var borc = await _repositoryContext.TedaviKaydi.Where(b => b.Id==dosyaid && b.Odendi==false).SumAsync(b => b.fiyat);
            return borc;
        }
        public async Task<TedaviKaydi> SingleTedaviKaydiGetir(int dosyaid)
        {
            var tedaviKaydi = await _repositoryContext.TedaviKaydi.SingleOrDefaultAsync(t => t.Id==dosyaid);
            return tedaviKaydi;
        }
        public async Task<double> MuayeneKaydininToplamOdemesiniGetir(int dosyaid)
        {

            var odeme = await _repositoryContext.odeme.Where(b => b.muayeneId==dosyaid).SumAsync(b => b.odemeToplam);
            return odeme;
        }
        public async Task<bool>iptalOlmayanTaahütüVarmi(int dosyaid)
        {
            var result = await _repositoryContext.taahütname.AnyAsync(t=>t.MuayeneId==dosyaid && t.iptal==false);
            return result;
        }
        public void OdemeYap(odeme odeme)
        {
            _repositoryContext.odeme.Add(odeme);
        }

        public async Task<Taahütname> taahütnameGetir(int dosyaid)
        {
            var result = await _repositoryContext.taahütname.SingleOrDefaultAsync(t=>t.MuayeneId==dosyaid);
            return result;
        }

        public async Task<Patient> HastaBilgisiGetir(int protokol)
        {
            var hasta = await _repositoryContext.Patients.SingleOrDefaultAsync(p => p.Protocol==protokol);
            return hasta;
        }

        public void RandevuBekletmeEkke(RandevuBekleyenHastalar randevuBekleyenHastalar)
        {
            _repositoryContext.RandevuBekleyenHastalar.Add(randevuBekleyenHastalar);
        }
        public async Task<RandevuBekleyenHastalar?>RandevuTarihiVePoleGoreRandevuBekleyenHastayiGetir(DateTime date,int pol,int protokol)
        {
            var randevuBekleyen = await _repositoryContext.RandevuBekleyenHastalar.SingleOrDefaultAsync(r => r.RandevuTarihi==date && r.protokol==protokol && r.polNo==pol);
            if(randevuBekleyen!=null)
            {
                return randevuBekleyen;
            }
            else
            {
                return null;
            }
        }
    }

}
