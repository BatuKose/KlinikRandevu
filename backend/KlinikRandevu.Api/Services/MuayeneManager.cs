using Entities.Data_Transfer_Objects.Muayene;
using Entities.Enums;
using Entities.Exceptions.CustomExceptions;
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

            var pediyatriYasKontrol = await _repositoryManager.SistemParametresi.GetirAsync("PEDIATRI_YAS_LIMITI");
            if(pediyatriYasKontrol!= null && pediyatriYasKontrol.Deger1?.ToUpper()=="EVET")
            {
                var uzmanlik = await _repositoryManager.Muayene.PolUzmanlikKoduAsync(muayene.PolNo);
                if(uzmanlik==PoliklinikEnum.UzmanlikBransi.Pedodonti)
                {
                    var hasta =  await _repositoryManager.Patient.GetPatientByProtokolASycn(muayene.ProtocolNo);
                    if(hasta!=null)
                    {
                        var currdate = DateTime.Now.Year;
                        var yas = currdate-hasta.BirthDate.Year;
                        if (yas < int.Parse(pediyatriYasKontrol.Deger2) || yas > int.Parse(pediyatriYasKontrol.Deger3))
                        {
                            throw new ParamException("Pedodonti polikliniğine 0 yaşından küçük " +
                                "16 yaşından büyük hasta kaydı açamazsınız");
                        }
                    }
                }
            }
           
            var cinsiyetKurali = await _repositoryManager.SistemParametresi.GetirAsync("KADIN_DOGUM_ERKEK_YASAKLA");

            if(cinsiyetKurali != null && cinsiyetKurali.Deger1?.ToUpper()=="EVET")
            {
                var uzmanlik= await _repositoryManager.Muayene.PolUzmanlikKoduAsync(muayene.PolNo);

                if(uzmanlik==PoliklinikEnum.UzmanlikBransi.KadinHastaliklariVedogum)
                {
                    var hasta = await _repositoryManager.Patient.GetPatientByProtokolASycn(muayene.ProtocolNo);
                    if (hasta.Gender==GenderEnum.male ||hasta.Gender== GenderEnum.none)
                    {
                        throw new ParamException("Bu polikliniğe cinsiyeti kadın harici hasta açılmaz");
                    }
                }
                
            }         

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
        public async Task<List<HastaRandevulariniGetirDTO>> HastaRandevulariniGetir(DateTime baslangic, DateTime bitis)
        {
            if (baslangic < new DateTime(1900, 1, 1) || bitis < new DateTime(1900, 1, 1))
            {
                throw new ArgumentException("Geçersiz tarih aralığı.");
            }
            if (baslangic > bitis)
            {
                throw new ArgumentException("Başlangıç tarihi bitişten büyük olamaz.");
            }

            var result = await _repositoryManager.Muayene.HastaRandevulariniGetir(baslangic, bitis);

            if (!result.Any())
                throw new NotFoundException("Seçilen tarih aralığında randevu bulunmamaktadır.");

            return result;
        }
        public async Task<List<HastaRandevulariniGetirDTO>> HastanınRandevulariniGetir(int protokol)
        {
            if (protokol<=0) throw new BadRequestException("Protokolü sıfırdan büyük olmalıdır");
            var hasta = await _repositoryManager.Muayene.hastaVarmiProtokol(protokol);
            if (!hasta) throw new NotFoundException("Hasta bulunamadı");

            var result = await _repositoryManager.Muayene.HastanınRandevulariniGetir(protokol);

            if (!result.Any())
                throw new NotFoundException("Seçilen hastanın  randevusu bulunmamaktadır.");

            return result;
        }
        public async Task<Doctor>DoktoruPasifeAl(int doktor)
        {
            var doktorVarMi= await _repositoryManager.Muayene.doktorVarMI(doktor);
            if (!doktorVarMi) throw new NotFoundException("Doktor bulunamadı");
            var doktorBul = await _repositoryManager.Muayene.DoktoruGetir(doktor);
        
            if(doktorBul.isActive==true)
            {
                var kontrol = await _repositoryManager.Muayene.DoktorIleriRandevuSorgula(doktor);
                if (kontrol!=0) throw new BadRequestException("Doktorun ileriki tarihte randevuları bulunmak önce onları kontrol ediniz");
                doktorBul.isActive=false;
            }
            else
            {
                doktorBul.isActive=true;
            }

                await _repositoryManager.saveAsyc();
            return doktorBul;
        }
        public async Task<Poliklinik> PoluPasifeAl(int polno)
        {
            var polVarMi = await _repositoryManager.Muayene.doktorVarMI(polno);
            if (!polVarMi) throw new NotFoundException("Doktor bulunamadı");
            var polBul = await _repositoryManager.Muayene.PolGetir(polno);

            if (polBul.isActive==true)
            {
                var kontrol = await _repositoryManager.Muayene.PolIleriRandevuSorgula(polno);
                if (kontrol!=0) throw new BadRequestException("Doktorun ileriki tarihte randevuları bulunmak önce onları kontrol ediniz");
                polBul.isActive=false;
            }
            else
            {
                polBul.isActive=true;
            }

            await _repositoryManager.saveAsyc();
            return polBul;
        }
    }
}
