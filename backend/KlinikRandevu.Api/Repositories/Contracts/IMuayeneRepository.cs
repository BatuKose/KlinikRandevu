using Entities.Data_Transfer_Objects.Muayene;
using Entities.Enums;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IMuayeneRepository
    {
        public void CalismaPlaniOlustur(DoktorCalismaPlani plan);
        public Task<bool> doktorVarMI(int number);
        public Task<bool> polVarMI(int number);
        public Task<bool> hastaVarmi(long number);
        public Task<bool> PolRandevuMüsaitMi(int number);
        Task<int?> PolMaxSüre(int number);
        Task<int?> PolMaxRanevu(int number);
        public void RandevuOlustur(Randevu randevu);
        public Task<bool> randevuVarmı(int dnumber ,int pnumber, DayOfWeek day);
        Task<bool> randevuSaatiVarmı(int dnumber, int pnumber, TimeSpan baslangic, TimeSpan randevuBitis);
        Task<bool> CakisanRandevuVarMi(int doktorNo, int polNo, DateTime yeniBaslangic, DateTime yeniBitis);
        Task<bool> HastaAyniGunRandevusuVarMi(long hastaTc, int doktorNo, DateTime tarih);
        Task<DoktorCalismaPlani?> CalismaPlaniGetirAsync(int doktorNo, int polNo, DayOfWeek gun,
        TimeSpan baslangic, TimeSpan bitis);
        public void MuayeneKaydiOlustur(MuayeneKaydi muayene);
        Task<Randevu?> HastanınRanevusunuGetir(long hastaTc, int doktorNo, DateTime tarih);
        Task<bool> AyniGünMuayenesiVarmi(int pol, int protokol, DateTime muayenetarihi);
        Task<List<HastaRandevulariniGetirDTO>> HastaRandevulariniGetir(DateTime baslangic, DateTime bitis);
        Task<List<HastaRandevulariniGetirDTO>> HastanınRandevulariniGetir(int protokol);
        Task<bool> hastaVarmiProtokol(int number);
        Task<Doctor?> DoktoruGetir(int number);
        Task<Poliklinik?> PolGetir(int number);
        Task<int> DoktorIleriRandevuSorgula(int number);
        Task<int> PolIleriRandevuSorgula(int number);
        Task<PoliklinikEnum.UzmanlikBransi> PolUzmanlikKoduAsync(int polNo);
        Task<List<DoktorRandevuHatirlatmaEmailDTO>> DoktorRandevuHatirlatma(int doktorno);
        Task<bool> hastaTelNoVarmi(int number);
        Task<string?> HastaCepTelefonGetir(int protokol);
        Task<Randevu?> GetRandevuById(int id);
        Task<List<RandevuluHastalarinBilgilerDTO>> RandevuluHastaBilgileri(DateTime baslangic, DateTime bitis, bool muayeneOlduMu);
        Task<int> ileriTarihliRandevuVarmi(int protokol);
        Task<int> HastaninHicAktifMuayenesiOlduMU(int protokol);
        void teshisEkle(teshisler teshisler);
        Task<MuayeneKaydi?> GetMuayeneById(int id);
        Task<bool> MuayenedeAyniTeshisdenVarmi(int sira, string tcode);
        Task<bool> MuayenedeTeshisVarMı(int muayeneid);
        Task<List<JobHatirlatmaSorguDTO>> JobYarininRandevuluHastalari(DateTime basla, DateTime bitis);
        Task HatirlatmaMilUpte(IEnumerable<int> randevuIdler);
        Task<List<int>> DoktorIdleriniGetir();
        Task<List<(int id, TimeSpan baslangicSaati)>> MuayenesiKapanmamisMuayeneIdleriniGetir();
        Task<int> MuayeneKapatmaUpdate(TimeSpan sure);
        Task<List<YariniHastalariniGetirDTO>> YariniHastalariniGetir();
        Task<Tetkikler> PoliklinikMuaynesiGetir();
        public void TedaviKaydiEkle(TedaviKaydi tedaviKaydi);
        Task<List<TedaviKaydi>> OdenmemisTedavileriGetir(int protokol);
        public void TahütnameEKle(Taahütname taahütname);
        Task<TedaviKaydi> TedaviKaydiGetir(int dosyaid);
        Task<double> MuayeneKaydininToplamBorucunuGetir(int dosyaid);
        Task<bool> iptalOlmayanTaahütüVarmi(int dosyaid);
        Task<List<TaahütBilgilendirme>> YaklasanTahütBilgilendirme(int sms);
        Task HatirlatmaTaahütnameUpdateALL(IEnumerable<int> taahütnameIdler);
        Task HatirlatmaTaahütnameUpdateSMS(IEnumerable<int> taahütnameIdler);
        Task HatirlatmaTaahütnameUpdateMAIL(IEnumerable<int> taahütnameIdler);
        public void OdemeYap(odeme odeme);
        Task<double> MuayeneKaydininToplamOdemesiniGetir(int dosyaid);
        Task<double> TedaviKaydininToplamBorucunuGetir(int dosyaid);
        Task<TedaviKaydi> SingleTedaviKaydiGetir(int dosyaid);
        Task<Taahütname>taahütnameGetir(int dosyaid);
        Task<List<TedaviKaydi>> MuayeneKaydininOdenecekTedavileri(int muayeneId);
        Task<Tetkikler> TetkikGetir(string bilgi);
        Task<Patient> HastaBilgisiGetir(int protokol);
        Task<bool> MuayenedeTetkikDahaOncedenIslenmisMı(string tetkik, int muayeneId);
        public void RandevuBekletmeEkke(RandevuBekleyenHastalar randevuBekleyenHastalar);
        Task<List<RandevuBekleyenHastalar>> RandevuBekleyenHastalariGetirAsync();
        Task HatirlatmaBekleyenRandevuUpdate(IEnumerable<int> bekleyenId);
    }
}
