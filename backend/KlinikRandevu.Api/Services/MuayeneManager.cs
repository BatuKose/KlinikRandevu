using Entities.Data_Transfer_Objects.Muayene;
using Entities.Exeptions.CustomExceptions;
using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class MuayeneManager:IMuayeneService
    {
        private readonly IRepositoryManager _repositoryManager;

        public MuayeneManager(IRepositoryManager repositoryManager)
        {
            _repositoryManager=repositoryManager;
        }

        public async Task<CalismaPlaniOlusturDTO> CalismaPlaniOlusturAsync(CalismaPlaniOlusturDTO plan)
        {
            if (plan == null) throw new BadRequestException("Çalıma planındaki bütün bilgilerin girilmesi gerekmektedir");
            var doctorExists = await _repositoryManager.Muayene.doktorVarMI(plan.DoktorNo);
            if (!doctorExists) throw new NotFoundException("Doktor bilgisi bulunamadı");
            var polExists = await _repositoryManager.Muayene.polVarMI(plan.PolNo);
            if (!polExists) throw new NotFoundException("poliklinik bilgisi bulunamadı");
            var onlineMüsaitlik = await _repositoryManager.Muayene.PolRandevuMüsaitMi(plan.PolNo);
            if (!onlineMüsaitlik) throw new NotFoundException("Randevu vermeye çalıştığını poliklinik randevuya kapatılmıştır");
            int? randevuSüresi = await _repositoryManager.Muayene.PolMaxSüre(plan.PolNo);
            if(randevuSüresi.HasValue)
            {
                if (plan.RandevuSuresiDk>randevuSüresi) throw new BadRequestException("Slotalarda bulunan randevu süresi aşıldı");
            }
            var toplamSüre = (int)(plan.BitisSaati - plan.BaslangicSaati).TotalMinutes;
            var hesaplananRandevuSayisi = toplamSüre / plan.RandevuSuresiDk;

            int? maksRandevuSayisi = await _repositoryManager.Muayene.PolMaxRanevu(plan.PolNo);
            if (maksRandevuSayisi.HasValue)
            {
                if (hesaplananRandevuSayisi > maksRandevuSayisi.Value)
                    throw new BadRequestException("Slottaki toplam randevu sayısı günlük maksimumu aşmaktadır.");
            }
            var CalismaPlani = new DoktorCalismaPlani()
            {
                BaslangicSaati=plan.BaslangicSaati,
                BitisSaati=plan.BitisSaati,
                DoktorNo=plan.DoktorNo,
                GunAdi=plan.GunAdi,
                PolNo=plan.PolNo,
                RandevuSuresiDk=plan.RandevuSuresiDk
            };
             _repositoryManager.Muayene.CalismaPlaniOlustur(CalismaPlani);
            await _repositoryManager.saveAsyc();
            var result = new CalismaPlaniOlusturDTO()
            {
                BaslangicSaati = CalismaPlani.BaslangicSaati,
                BitisSaati=CalismaPlani.BitisSaati,
                DoktorNo=CalismaPlani.DoktorNo,
                GunAdi=CalismaPlani.GunAdi,
                PolNo=CalismaPlani.PolNo,
                RandevuSuresiDk=CalismaPlani.RandevuSuresiDk
            };
            return result;
        }

        public async Task<MuayeneKayitiOlusturDTO> MuayeneKayitiOlustur(MuayeneKayitiOlusturDTO muayene)
        {
            if (muayene == null) throw new BadRequestException("Muayene bilgilerini kontrol ediniz");

            var doctorExists = await _repositoryManager.Muayene.doktorVarMI(muayene.DoktorNo);
            if (!doctorExists)
                throw new NotFoundException("Doktor bilgisi bulunamadı");

            var polExists = await _repositoryManager.Muayene.polVarMI(muayene.PolNo);
            if (!polExists)
                throw new NotFoundException("Poliklinik bilgisi bulunamadı");

            var patientExists = await _repositoryManager.Muayene.hastaVarmi(muayene.HastaTc);
            if (!patientExists)
                throw new NotFoundException("Hasta bilgisi bulunamadı");

            if (muayene.MuayeneTarihi<DateTime.UtcNow) 
                throw new BadRequestException("Muayene tarihi geçmiş tarihli olamaz");

            int? randevuid=null;
            DateTime randevuTarihi = muayene.MuayeneTarihi.Date + muayene.BaslangicSaati;
            DateTime tarihKontrol = muayene.MuayeneTarihi.Date;

            var aynigünMuayene = await _repositoryManager.Muayene.AyniGünMuayenesiVarmi(muayene.PolNo,muayene.ProtocolNo, tarihKontrol);

            if (aynigünMuayene) throw new BadRequestException("Hastanın aynı gün aynı polikliniğe randevusu bulunmaktadır");

            var mevcutRandevu = await _repositoryManager.Muayene.HastanınRanevusunuGetir(muayene.HastaTc, muayene.DoktorNo, randevuTarihi);
            if(mevcutRandevu is not null)
            {
                if (mevcutRandevu.RandevuTarihi==randevuTarihi)
                {
                    randevuid=mevcutRandevu?.Id;
                }
            }
           

            var kayit = new MuayeneKaydi
            {
                BaslangicSaati=muayene.BaslangicSaati,
                ProtocolNo=muayene.ProtocolNo,
                DoktorNo=muayene.DoktorNo,
                PolNo= muayene.PolNo,
                HastaTc=muayene.HastaTc,
                MuayeneTarihi= muayene.MuayeneTarihi,
                RandevuId=randevuid
            };
             _repositoryManager.Muayene.MuayeneKaydiOlustur(kayit);
             await _repositoryManager.saveAsyc();
            return new MuayeneKayitiOlusturDTO
            {
                BaslangicSaati=kayit.BaslangicSaati,
                ProtocolNo=kayit.ProtocolNo,
                DoktorNo=kayit.DoktorNo,
                PolNo=kayit.PolNo,
                HastaTc=kayit.HastaTc,
                MuayeneTarihi=kayit.MuayeneTarihi
            };
        }

        public async Task<RandevuOlusturDTO> RandevuOlusturAsync(RandevuOlusturDTO plan)
        {
            if (plan == null)
                throw new BadRequestException("Randevu bilgilerini kontrol ediniz");

            if (plan.RandevuTarihi < DateTime.Now)
                throw new BadRequestException("Geçmiş tarihe randevu oluşturulamaz.");
            var doctorExists = await _repositoryManager.Muayene.doktorVarMI(plan.DoktorNo);
            if (!doctorExists)
                throw new NotFoundException("Doktor bilgisi bulunamadı");

            var polExists = await _repositoryManager.Muayene.polVarMI(plan.PolNo);
            if (!polExists)
                throw new NotFoundException("Poliklinik bilgisi bulunamadı");

            var patientExists = await _repositoryManager.Muayene.hastaVarmi(plan.HastaTc);
            if (!patientExists)
                throw new NotFoundException("Hasta bilgisi bulunamadı");

            DayOfWeek randevuGunu = plan.RandevuTarihi.DayOfWeek;
            TimeSpan randevuSaati = plan.RandevuTarihi.TimeOfDay;
            TimeSpan randevuBitis = randevuSaati.Add(TimeSpan.FromMinutes(plan.SureDakika));

            var calismaPlani = await _repositoryManager.Muayene.CalismaPlaniGetirAsync(
                plan.DoktorNo, plan.PolNo, randevuGunu, randevuSaati, randevuBitis);

            if (calismaPlani == null)
                throw new BadRequestException("Uygun randevu saati bulunmamaktadır.");

            var calismaBaslangictanGecenDk = (randevuSaati - calismaPlani.BaslangicSaati).TotalMinutes;

            if (calismaBaslangictanGecenDk % calismaPlani.RandevuSuresiDk != 0)
                throw new BadRequestException(
                    $"Randevu saati uygun slotta değil. Lütfen {calismaPlani.RandevuSuresiDk} dakikalık aralıklarla seçim yapınız.");

            if (plan.SureDakika != calismaPlani.RandevuSuresiDk)
                throw new BadRequestException(
                    $"Randevu süresi {calismaPlani.RandevuSuresiDk} dakika olmalıdır.");

            DateTime yeniBaslangic = plan.RandevuTarihi;
            DateTime yeniBitis = plan.RandevuTarihi.AddMinutes(plan.SureDakika);

            var cakismaVar = await _repositoryManager.Muayene.CakisanRandevuVarMi(
                plan.DoktorNo, plan.PolNo, yeniBaslangic, yeniBitis);

            if (cakismaVar)
                throw new BadRequestException("Bu saatte doktorun başka bir randevusu bulunmaktadır.");

            var hastaAyniGunRandevu = await _repositoryManager.Muayene.HastaAyniGunRandevusuVarMi(
                plan.HastaTc, plan.DoktorNo, plan.RandevuTarihi);

            if (hastaAyniGunRandevu)
                throw new BadRequestException("Bu tarihte hastanın aynı doktora başka bir randevusu bulunmaktadır.");


            var randevuOlustur = new Randevu
            {
                DoktorNo = plan.DoktorNo,
                PolNo = plan.PolNo,
                HastaTc = plan.HastaTc,
                ProtocolNo = plan.ProtocolNo,
                RandevuTarihi = plan.RandevuTarihi,
                SureDakika = plan.SureDakika,
                Notlar = plan.Notlar,
            };

            _repositoryManager.Muayene.RandevuOlustur(randevuOlustur);
            await _repositoryManager.saveAsyc();

            return plan;
        }

    }
}
