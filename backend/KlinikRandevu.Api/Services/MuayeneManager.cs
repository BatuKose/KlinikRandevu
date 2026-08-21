using Entities.Data_Transfer_Objects.IcdApi;
using Entities.Data_Transfer_Objects.Muayene;
using Entities.Data_Transfer_Objects.Patient;
using Entities.Enums;
using Entities.Exceptions.CustomExceptions;
using Entities.Exeptions.CustomExceptions;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Cryptography;
using Org.BouncyCastle.Bcpg;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Numbers.V1;
using static Entities.Enums.PoliklinikEnum;


namespace Services
{
    public class MuayeneManager:IMuayeneService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger<MuayeneManager> _logger;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _cache;
        private readonly ITwilioSmsManager _twilioSms;
        private readonly IIcdApiManager _icdApiManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MuayeneManager(
      IRepositoryManager repositoryManager,
      ILogger<MuayeneManager> logger,
      IEmailService emailService,
      IMemoryCache memoryCache,
      ITwilioSmsManager twilioSms,
      IIcdApiManager icdApiManager,
      IHttpContextAccessor httpContextAccessor)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
            _emailService = emailService;
            _cache = memoryCache;
            _twilioSms = twilioSms;
            _icdApiManager = icdApiManager;
            _httpContextAccessor = httpContextAccessor;
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
            var uzKod = await _repositoryManager.Muayene.PolGetir(muayene.PolNo);
            if(uzKod is not null)
            {
                if (uzKod.PolUzKod!=UzmanlikBransi.AcilTip)
                {
                    var borcParam = await _repositoryManager.SistemParametresi.GetirAsync("BORCLU_HASTAYA_KAYIT_AC");
                    if(borcParam is null)
                    {
                        parametreEke("BORCLU_HASTAYA_KAYIT_AC");
                    }
                    var borcParamDeger = borcParam?.Deger1?.ToUpper() ??"HAYIR";
                    if(borcParamDeger!="EVET")
                    {
                        var BorcKontrol = await _repositoryManager.Muayene.OdenmemisTedavileriGetir(muayene.ProtocolNo);
                        if (BorcKontrol is not null && BorcKontrol.Count>0)
                        {
                            throw new BadRequestException("Ödenmemiş Borçları Bulunmaktadır. Muayene Açmak için ödeme yapınız");
                        }
                    }
                  
                }
            }
            
            var tatilBlokParam = await _repositoryManager.SistemParametresi.GetirAsync("TATIL_KAYIT_BLOKLA");
            if(tatilBlokParam is null)
            {
                string paramName = "TATIL_KAYIT_BLOKLA";
                parametreEke(paramName);
            }
            if (tatilBlokParam != null && tatilBlokParam.Deger1?.ToUpperInvariant() == "EVET")
            {
                var yil = muayene.MuayeneTarihi.Year;
                var key = $"tatiller:{yil}";

                if (!_cache.TryGetValue(key, out HashSet<DateTime>? set))
                {
                    var tatiller = await _repositoryManager.TatilRepository.TatilleriGetirAsync(yil);
                    set = tatiller.Select(t => t.Tarih.Date).ToHashSet();
                    _cache.Set(key, set, TimeSpan.FromDays(1));  
                }

                if (set.Contains(muayene.MuayeneTarihi.Date))      
                {
                    throw new ParamException("Resmi tatil günlerinde muayene kaydı oluşturamazsınız");
                }
            }
            var randevusuzKayitAcmaParam = await _repositoryManager.SistemParametresi.GetirAsync("RANDEVUSUZ_KAYIT_ACMA");
            if(randevusuzKayitAcmaParam is null)
            {
                string paramName = "RANDEVUSUZ_KAYIT_ACMA";
                parametreEke(paramName);
            }
            if(randevusuzKayitAcmaParam!=null && randevusuzKayitAcmaParam.Deger1=="EVET")
            {
                if (int.TryParse(randevusuzKayitAcmaParam.Deger2, out var hedefPolNo)&& muayene.PolNo == hedefPolNo)
                {
                    bool paramKontrol = await _repositoryManager.Muayene.AyniGünMuayenesiVarmi(muayene.PolNo, muayene.ProtocolNo, muayene.MuayeneTarihi);
                    if (!paramKontrol) throw new ParamException("Bu polikliniğe Randevusuz kayıt açılamaz");
                }
                
            }

            var pediyatriYasKontrol = await _repositoryManager.SistemParametresi.GetirAsync("PEDIATRI_YAS_LIMITI");
            if(pediyatriYasKontrol is null)
            {
                string paramName = "PEDIATRI_YAS_LIMITI";
                parametreEke(paramName);
            }
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
            if(cinsiyetKurali is null)
            {
                string paramName = "KADIN_DOGUM_ERKEK_YASAKLA";
                parametreEke(paramName);
            }

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

            string aksiyonTipi = $"muayene oluşturma {kayit.HastaTc} tcli hastaya {kayit.MuayeneTarihi} tarihli " +
                $"{kayit.PolNo} numaralı pole muayene oluşturuldu";
            string EntityTipi = "MuayeneKayitlari";
            int entityId = kayit.ProtocolNo;
            logYaz(aksiyonTipi, entityId, EntityTipi);
            _repositoryManager.Muayene.MuayeneKaydiOlustur(kayit);
             await _repositoryManager.saveAsyc();
            var tetkik = await _repositoryManager.Muayene.PoliklinikMuaynesiGetir();
            if(tetkik != null)
            {
                var TedaviKaydiEkle = new TedaviKaydi()
                {
                    MuyaneId=kayit.Id,
                    doktorId=kayit.DoktorNo,
                    fiyat=tetkik.Fiyat,
                    tedaviAdi=tetkik.TetikAdi,
                    Odendi=false,
                    tedaviKodu=tetkik.Kodu,
                    prtokol=kayit.ProtocolNo
                };
                _repositoryManager.Muayene.TedaviKaydiEkle(TedaviKaydiEkle);
                await _repositoryManager.saveAsyc();
            }


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

            var tatilBlokParam = await _repositoryManager.SistemParametresi.GetirAsync("TATIL_KAYIT_BLOKLA");
            if(tatilBlokParam is null)
            {
                string paramName = "TATIL_KAYIT_BLOKLA";
                parametreEke(paramName);
            }
            if (tatilBlokParam != null && tatilBlokParam.Deger1?.ToUpperInvariant() == "EVET")
            {
                var yil = plan.RandevuTarihi.Year;
                var key = $"tatiller:{yil}";

                if (!_cache.TryGetValue(key, out HashSet<DateTime> set))
                {
                    var tatiller = await _repositoryManager.TatilRepository.TatilleriGetirAsync(yil);
                    set = tatiller.Select(t => t.Tarih.Date).ToHashSet();
                    _cache.Set(key, set, TimeSpan.FromDays(1));
                }

                if (set.Contains(plan.RandevuTarihi.Date))
                {
                    throw new ParamException("Resmi tatil günlerinde randevu kaydı oluşturamazsınız");
                }
            }
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
            {
                var randevuBekletmeParam = await _repositoryManager.SistemParametresi.GetirAsync("RANDEVU_BEKLETMEYI_KAPAT");
                if(randevuBekletmeParam is null)
                {
                    parametreEke("RANDEVU_BEKLETMEYI_KAPAT");
                }
                var randevuBekletmeParamDeger= randevuBekletmeParam?.Deger1?.ToString() ?? "HAYIR";
                if (plan.RandevuBekleme==true && randevuBekletmeParamDeger=="HAYIR")
                {
                    RandevuBekleyenHastalar randevuBekleyen = new RandevuBekleyenHastalar()
                    {
                        Bilgilendirme=false,
                        RandevuVerildi=false,
                        doktorNo=plan.DoktorNo,
                        polNo=plan.PolNo,
                        protokol=plan.ProtocolNo,
                        RandevuTarihi=plan.RandevuTarihi,
                        tcKimlik=plan.HastaTc
                    };
                    _repositoryManager.Muayene.RandevuBekletmeEkke(randevuBekleyen);
                    await _repositoryManager.saveAsyc();
                    throw new BadRequestException($"Bu saatte doktorun başka bir randevusu bulunmaktadır. {plan.RandevuTarihi} tarihli bekleme listesine alındınız" +
                        $"müsaitlik olursa size  bilgilendirme mail/sms gönderilcektir.");
                }
                else
                {
                    throw new BadRequestException("Bu saatte doktorun başka bir randevusu bulunmaktadır.");
                }
                    
            }
               

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
            string aksiyonTipi = $"Randevu oluşturma {plan.HastaTc} tcli hastaya {plan.RandevuTarihi} tarihli randevu oluşturuldu";
            string EntityTipi = "randevular";
            int entityId = plan.ProtocolNo;
            logYaz(aksiyonTipi, entityId, EntityTipi);
            await _repositoryManager.saveAsyc();
            //randevu mail
            var mailParametre = await _repositoryManager.SistemParametresi.GetirAsync("EMAIL_GONDERME");
            if(mailParametre is null)
            {
                string paramName = "EMAIL_GONDERME";
                parametreEke(paramName);
            }
            if(mailParametre?.Deger1?.ToUpper()=="EVET" && int.Parse(mailParametre.Deger3)!=plan.PolNo)
            {
                return plan;
            }
            var mailHasta = await _repositoryManager.Patient.GetPatientByProtokolASycn(plan.ProtocolNo);
            if (mailHasta is null) throw new NotFoundException("Hastanın mailini bulunamadı");
            var mailDoktor = await _repositoryManager.Muayene.DoktoruGetir(plan.DoktorNo); 
            if (mailDoktor is null) throw new NotFoundException("doktora ait mail bulunamadı");
            if(!string.IsNullOrWhiteSpace(mailHasta.Email) && !string.IsNullOrWhiteSpace(mailDoktor.DoktorAd))
            {
                try
                {
                 
                    await _emailService.RandevuOnayMailiGonder(mailHasta.Email,
                        $"{mailHasta.Name} {mailHasta.Surname}",
                        $"{mailDoktor.DoktorAd}", plan.RandevuTarihi
                        );

                }
                catch(Exception ex)
                {
                    _logger.LogWarning(ex, "Randevu oluştu fakat mail gönderilemedi. ProtokolNo: {ProtokolNo}", mailHasta.Protocol);
                }
            }
            var smsRandevuParam = await _repositoryManager.SistemParametresi.GetirAsync("RANDEVU_SMS_BILGISI");
            if(smsRandevuParam is null)
            {
                string paramName = "RANDEVU_SMS_BILGISI";
                parametreEke(paramName);
            }
            if(smsRandevuParam != null && smsRandevuParam.Deger1?.ToUpper()=="EVET")
            {
                var hastaCepTelVarmi = await _repositoryManager.Muayene.hastaTelNoVarmi(plan.ProtocolNo);
                if(hastaCepTelVarmi)
                {
                    var mesaj = $"sayın hastamız {plan.RandevuTarihi} tarihine başarılı şekilde randevunuz alınmıştır.";
                    var cepNo = await _repositoryManager.Muayene.HastaCepTelefonGetir(plan.ProtocolNo);
                    if(cepNo!=null)
                    {
                        try
                        {
                            await _twilioSms.SmsGonderAsync(cepNo, mesaj);
                        }
                        catch(Exception ex)
                        {
                            _logger.LogInformation(ex, "Randevu oluştu fakat SMS gönderilemedi. Protokol: {Protokol}", plan.ProtocolNo);
                        }
                        
                    }
                }
            }
            return plan;
        }
        private async void logYaz(string aksiyonTipi, int entityid,string entitytipi)
        {
            var ipAdress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "İp adresi bulunamadı";
            var userStringId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserID")?.Value;
            int? userId = int.TryParse(userStringId, out int parsedId) ? parsedId : null;
            var LoguYaz = new UserLog()
            {
                AksiyonTipi = aksiyonTipi,
                EntityTipi = entitytipi,
                UserId= userId,
                IpAdresi=ipAdress,
                EntityId=entityid
            };
            _repositoryManager.UserLogRepository.LoginLogYaz(LoguYaz);
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
            var doktorBul = await _repositoryManager.Muayene.DoktoruGetir(doktor) ?? throw new NotFoundException("Doktor bilgisi bulunamadı");
            
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
            if (polBul is null) throw new NotFoundException("Poliklinik bulunamadı");

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
        public async Task DoktorGunlukProgramMailiGonderAsync(int doktorNo)
        {
         
            var randevular = await _repositoryManager.Muayene.DoktorRandevuHatirlatma(doktorNo);
            if (!randevular.Any()) throw new NotFoundException("Doktorun bugüne ait aktif randevusu bulunmamaktadır");

            var ilkKayit = randevular.First();
            var doktorAd = ilkKayit.doktorad;
            var doktorEmail = ilkKayit.doktormail;

            var htmlIcerik = MailIcerikOlustur(doktorAd, randevular);
            var konu = $"Günlük randevu programınız-{DateTime.Today:dd:MM:yyyy}";
            try
            {
                var mailGondermeParametre = await _repositoryManager.SistemParametresi.GetirAsync("EMAIL_GONDERME");
                if(mailGondermeParametre is null)
                {
                    string paramName = "EMAIL_GONDERME";
                    parametreEke(paramName);
                }
                if (mailGondermeParametre != null
                    && mailGondermeParametre.Deger1?.ToUpper() == "EVET"
                    && int.TryParse(mailGondermeParametre.Deger4, out var hedefDoktorNo)
                    && hedefDoktorNo != doktorNo)

                {
                    return;
                }
                    await _emailService.MailGonderAsync(doktorEmail, konu, htmlIcerik);
            }
            catch(Exception ex)
            {
                _logger.LogWarning($"doktor randevu programı bilgileri mail olarak gönderilemedi \n {ex} "+DateTime.Now);
            }
           
            
        }
        private string MailIcerikOlustur(string doktorAd, List<DoktorRandevuHatirlatmaEmailDTO> randevular)
        {
            var satirlar = string.Join("", randevular.Select((r, i) => $@"
            <tr style='background:{(i % 2 == 0 ? "#f9f9f9" : "#ffffff")};'>
                <td style='padding:10px;border:1px solid #ddd;'>{r.randevutarihi:HH:mm}</td>
                <td style='padding:10px;border:1px solid #ddd;'>{r.hastaad} {r.hastsoyad}</td>
                <td style='padding:10px;border:1px solid #ddd;'>{r.polad}</td>
            </tr>"));

            return $@"
            <div style='font-family:Arial,sans-serif;max-width:700px;'>
                <h2>Sayın Dr. {doktorAd}</h2>
                <p>{DateTime.Today:dd MMMM yyyy} tarihli randevu programınız:</p>
                <table style='border-collapse:collapse;width:100%;'>
                    <thead>
                        <tr style='background:#4a90e2;color:white;'>
                            <th style='padding:12px;text-align:left;'>Saat</th>
                            <th style='padding:12px;text-align:left;'>Hasta</th>
                            <th style='padding:12px;text-align:left;'>Poliklinik</th>
                        </tr>
                    </thead>
                    <tbody>{satirlar}</tbody>
                </table>
                <p>Toplam: <strong>{randevular.Count} randevu</strong></p>
            </div>";
        }
        public async Task<Randevu>RandevuIptalAsync(int randevuId)
        {
            var randevu= await _repositoryManager.Muayene.GetRandevuById(randevuId);
            if (randevu == null) throw new NotFoundException("Ranevu bilgileri bulunamadı");
            if (randevu.RandevuTarihi<DateTime.Now.Date) throw new BadRequestException("Randevu tarihi geçmiş olan randevu iptal edilemez");
             randevu.iptal=true;
            await _repositoryManager.saveAsyc();
            var randevuIptalMailParam = await _repositoryManager.SistemParametresi.GetirAsync("RANDEVU_IPTAL_MAILI_GONDER");
            if(randevuIptalMailParam is null)
            {
                string paramName = "RANDEVU_IPTAL_MAILI_GONDER";
                parametreEke(paramName);
            }
            if(randevuIptalMailParam != null && randevuIptalMailParam.Deger1?.ToUpper()=="EVET")
            {
                var hasta = await _repositoryManager.Patient.GetPatientByProtokolASycn(randevu.ProtocolNo);
                string mesaj = $"Sayın hastamız{hasta.Name} {hasta.Surname} {randevu.RandevuTarihi} tarihli randevunuz iptal edilmiştir. Sağlıklı günler dileriz ";
                string konu = "Randevu iptali";
                if(!string.IsNullOrWhiteSpace(hasta.Email))
                {
                    try
                    {
                        await _emailService.MailGonderAsync(hasta.Email, konu, mesaj);
                    }
                    catch
                    {
                        _logger.LogWarning("Randevu iptal edildi mail gönderilemedi");
                    }
                }

            }
            var randevuIptalSmsParam = await _repositoryManager.SistemParametresi.GetirAsync("RANDEVU_IPTAL_SMS");
            if(randevuIptalSmsParam != null && randevuIptalSmsParam.Deger1?.ToUpper()=="EVET")
            {
                var hasta = await _repositoryManager.Patient.GetPatientByProtokolASycn(randevu.ProtocolNo);
                if(!string.IsNullOrWhiteSpace(hasta.Phone))
                {
                    try
                    {
                        string mesaj = $"Sayın hastamız{hasta.Name} {hasta.Surname} {randevu.RandevuTarihi} tarihli randevunuz iptal edilmiştir. Sağlıklı günler dileriz ";
                        await _twilioSms.SmsGonderAsync(hasta.Phone, mesaj);
                    }
                    catch
                    {
                        _logger.LogWarning("Randevu iptal edildi sms gönderilemedi");
                    }
                }
            }
            return randevu;
        }
        public async Task<List<RandevuluHastalarinBilgilerDTO>> RandevuluHastaBilgileriniGetir(
            DateTime basla, DateTime bitis, bool muayeneOldumu)
        {
            if (basla == default || bitis == default)
                throw new BadRequestException("Başlangıç ve bitiş tarihi zorunludur.");

            if (bitis < basla)
                throw new BadRequestException("Bitiş tarihi başlangıç tarihinden küçük olamaz.");

            if ((bitis - basla).TotalDays > 366)
                throw new BadRequestException("Tarih aralığı en fazla 1 yıl olabilir.");

            var list = await _repositoryManager.Muayene.RandevuluHastaBilgileri(basla, bitis, muayeneOldumu);

            if (list == null || !list.Any())
                throw new NotFoundException("Randevulu hasta bilgileri bulunamadı.");

            return list;
        }
        public async Task<teshisler>TeshisEkle(int muayeneId,string teshis)
        {
            var muayene = await _repositoryManager.Muayene.GetMuayeneById(muayeneId);
            if (muayene == null) throw new NotFoundException("Muayene kaydı bulunmadı");
            if (muayene.BitisSaati is not null) throw new BadRequestException("Muayene onay verilmiştir teşhis eklenemez");
            var muayeneTeshis= await _icdApiManager.TaniAraAsync(teshis);
            if (muayeneTeshis == null || muayeneTeshis.Count == 0) throw new NotFoundException("Teşhis bulunamadı");
            var ilkTeshis = muayeneTeshis.First();
            bool teshisDahaOnceEklenmisMi = await _repositoryManager.Muayene.MuayenedeAyniTeshisdenVarmi(muayeneId, ilkTeshis.TheCode);
            if (!teshisDahaOnceEklenmisMi) throw new BadRequestException($"{ilkTeshis.TheCode} {ilkTeshis.Title} bu teşhis hastanın " +
                $"{muayeneId} numaralı sıra  kaydında mevcut.");

            var muayeneTeshisEkle = new teshisler()
            {
                muayeneId=muayene.Id,
                teshisAd=ilkTeshis.Title,
                teshisKod=ilkTeshis.TheCode
            };
            
            _repositoryManager.Muayene.teshisEkle(muayeneTeshisEkle);
            logYaz("TeshisEkleme", muayeneId, "Teşhis");
            await _repositoryManager.saveAsyc();
            return muayeneTeshisEkle;
        }
        public async Task<int>MuayeneKapat(int id)
        {
            var teshisZorunluParam = await _repositoryManager.SistemParametresi.GetirAsync("TESHIS_OLMADAN_MUAYENE_KAPATMA");
            if(teshisZorunluParam is null)
            {
                string paramName = "TESHIS_OLMADAN_MUAYENE_KAPATMA";
                parametreEke(paramName);
            }
            var teshisParamDeger = teshisZorunluParam?.Deger1?.ToUpper() ?? "HAYIR";
            if (id<=0) throw new BadRequestException("Muayene başlık id'si giriniz");
            var muayene = await _repositoryManager.Muayene.GetMuayeneById(id);
            if (muayene == null) throw new BadRequestException("Muayene Kaydı bulunamadı");
            if(muayene.BitisSaati is null)
            {
                if(teshisParamDeger=="EVET")
                {
                    bool teshishVarmı =  await _repositoryManager.Muayene.MuayenedeTeshisVarMı(muayene.Id);
                    if(!teshishVarmı==true)
                    {
                        throw new BadRequestException("Muayene'ye teşhis eklemeden sonlandıramazsınız");
                    }
                }
                var now = DateTime.Now;
                TimeSpan bitisSaati = new TimeSpan(now.Hour, now.Minute, now.Second);
                muayene.BitisSaati= bitisSaati;
                logYaz($"muayeneOnay bitis Saati: {bitisSaati}", muayene.Id, "muayene");
            }
            else
            {
                muayene.BitisSaati=null;
            }

              await _repositoryManager.saveAsyc();
             return id;
        }
        private void parametreEke(string paramName)
        {
            var paramEkle = new SistemParametresi()
            {
                ParametreAdi=paramName,
                Deger1="HAYIR",
                Deger2=null,
                Deger3=null,
                Deger4=null,
                Deger5= null,
                Aktif=true
            };
            _repositoryManager.SistemParametresi.Ekle(paramEkle);
            _repositoryManager.Save();
        }
        public async Task<Taahütname>TaahütnameEkleAsync(TaahütnameEkleDTO taahütname)
        {
            if (taahütname is null) throw new BadRequestException("Taahütname bilgileri boş olamaz");
            var tedaviKaydi= await _repositoryManager.Muayene.TedaviKaydiGetir(taahütname.MuayeneId);
            if (tedaviKaydi==null) throw new NotFoundException("Tedavi Kaydı bulunamadı");
            double toplamBorc = await _repositoryManager.Muayene.MuayeneKaydininToplamBorucunuGetir(taahütname.MuayeneId);
            if (toplamBorc<=0) throw new BadRequestException($"Hastanın {taahütname.MuayeneId} numaralı muayene kaydında borç bulunmamaktadır.");
            bool taahütKontrol = await _repositoryManager.Muayene.iptalOlmayanTaahütüVarmi(taahütname.MuayeneId);
            if(taahütKontrol is true)
            {
                throw new BadRequestException("İlgili kaydın iptal edilmemiş taahütnamesi bulunmaktadır önce onu iptal ediniz");
            }
            var result = new Taahütname
            {
                TahütTarihi=DateTime.UtcNow,
                BilgilendirmeMail=false,
                BilgilendirmeSms=false,
                SonOdemeTarihi=taahütname.SonOdemeTarihi,
                MuayeneId=taahütname.MuayeneId,
                ToplamBorc=toplamBorc,
                iptal=false,
                odendi=false
            };
            _repositoryManager.Muayene.TahütnameEKle(result);
            await _repositoryManager.saveAsyc();
            return result;
        }
        public async Task<OdemeYapDTO> OdemeYap(OdemeYapDTO odeme)
        {
            if (odeme is null) throw new BadRequestException("Odeme bilgileri boş olamaz");
            if (odeme.odemeToplam<=0) throw new BadRequestException("Ödeme fiyat bilgisi sıfırdan büyük olmalıdır");
            if (odeme.odeme==odemeTipiEnum.OdemeEnum.tedaviBazli) 
            {
                var toplamBorc = await _repositoryManager.Muayene.TedaviKaydininToplamBorucunuGetir(odeme.TedaviId);
                if (odeme.odemeToplam>toplamBorc) throw new BadRequestException("Ödeme toplam borçtan büyük olamaz");
                if (toplamBorc <=0) throw new BadRequestException("Tedavini Borcu bulunmamaktadır");
                if (odeme.odemeToplam!=toplamBorc)
                {
                    throw new BadRequestException("Odeme Fiyat bilgisi eşit değildir");
                }
                var odemeUpdate = await _repositoryManager.Muayene.SingleTedaviKaydiGetir(odeme.TedaviId);
                if (odemeUpdate!= null)
                {
                    odemeUpdate.Odendi=true;
                }
                var InsertOdeme = new odeme
                {
                    muayeneId=odeme.muayeneId,
                    odemeToplam=odeme.odemeToplam,
                    odemeTarihi=DateTime.UtcNow
                };
                _repositoryManager.Muayene.OdemeYap(InsertOdeme);
            }
            else if(odeme.odeme==odemeTipiEnum.OdemeEnum.toplamMuayene)
            {
                var toplamBorc = await _repositoryManager.Muayene.MuayeneKaydininToplamBorucunuGetir(odeme.muayeneId);
                if(toplamBorc<=0)
                {
                    throw new BadRequestException("Muayene Kaydının borcu bulunmamaktadır");

                }
                if (odeme.odemeToplam>toplamBorc) throw new BadRequestException("Ödeme toplam borçtan büyük olamaz");
                if (odeme.odemeToplam<toplamBorc) throw new BadRequestException("Ödeme muayenenin toplam borcundan küçük olamaz");
                if(odeme.odemeToplam==toplamBorc)
                {
                    var InsertOdeme = new odeme
                    {
                        muayeneId=odeme.muayeneId,
                        odemeToplam=odeme.odemeToplam,
                        odemeTarihi=DateTime.UtcNow
                    };
                    var odenecekTedaviler = await _repositoryManager.Muayene.MuayeneKaydininOdenecekTedavileri(odeme.muayeneId);
                    foreach(var tedavi in odenecekTedaviler)
                    {
                        tedavi.Odendi=true;
                    }
                }
            }

            var otoTaahütnameOdemeParam = await _repositoryManager.SistemParametresi.GetirAsync("ODEME_SONRASI_OTOMATIK_TAAHUTNAME_ODENDI_YAP");
            if (otoTaahütnameOdemeParam is null)
            {
                parametreEke("ODEME_SONRASI_OTOMATIK_TAAHUTNAME_ODENDI_YAP");
            }
            var paramDeger = otoTaahütnameOdemeParam?.Deger1?.ToUpper() ?? "HAYIR";
            if (paramDeger =="EVET")

            {
                var taahütname = await _repositoryManager.Muayene.taahütnameGetir(odeme.muayeneId);
                if (taahütname != null && taahütname.odendi==false)
                {
                    if (taahütname.ToplamBorc==odeme.odemeToplam)
                    {
                        taahütname.odendi=true;
                    }
                }
            }
            await _repositoryManager.saveAsyc();
            return new OdemeYapDTO
            {
                muayeneId=odeme.muayeneId,
                odemeToplam=odeme.odemeToplam
            };
            
        }
        public async Task<TedaviEkleDTO> MuayeneyeTedaviEKle(TedaviEkleDTO giris)
        {
            var muayene = await _repositoryManager.Muayene.GetMuayeneById(giris.MuyaneId);
            if (muayene == null) throw new NotFoundException("Muayene Kaydı Bulunamadı");
            if (muayene.BitisSaati is not null) throw new BadRequestException("Muayene onayı verilmiş kayda tetkik eklenemez");
            var tedavi = new Tetkikler();
            if(!string.IsNullOrEmpty(giris.tedaviAdi))
            {
                 tedavi = await _repositoryManager.Muayene.TetkikGetir(giris.tedaviAdi);
            }
            else if(!string.IsNullOrEmpty(giris.tedaviKodu))
            {
                tedavi = await _repositoryManager.Muayene.TetkikGetir(giris.tedaviKodu);
            }
            if (tedavi is null) throw new NotFoundException("tetkik bilgisi bulunamadı");
            var mükerrerTetkikGirisParam = await _repositoryManager.SistemParametresi.GetirAsync("MUKERRER_TETKIK_GIRISI");
            if(mükerrerTetkikGirisParam is null)
            {
                parametreEke("MUKERRER_TETKIK_GIRISI");
            }
            var mükerrerParamD1= mükerrerTetkikGirisParam?.Deger1?.ToUpper() ?? "HAYIR";
            if(mükerrerParamD1=="HAYIR")
            {
                bool tetkikDahaOncedenIslenmiMı = await _repositoryManager.Muayene.MuayenedeTetkikDahaOncedenIslenmisMı(tedavi.Kodu, giris.MuyaneId);
                if (tetkikDahaOncedenIslenmiMı)
                {
                    throw new BadRequestException($"Tetkik {giris.MuyaneId} idli muayene'ye daha önceden işlenmiştir. Yenisi eklenemez");
                }
            }
            var taahütnameVarMi = await _repositoryManager.Muayene.taahütnameGetir(giris.MuyaneId);
            if (taahütnameVarMi is not null)
            {
              var taahütnameliKaydaTedaviEkleParam = await _repositoryManager.SistemParametresi.GetirAsync("TAAHUTNAMELI_MUAYENEYE_TEDAVI_GIRISI");
                if (taahütnameliKaydaTedaviEkleParam is null)
                {
                    parametreEke("TAAHUTNAMELI_MUAYENEYE_TEDAVI_GIRISI");
                }
                var taahütnameParamDeger = taahütnameliKaydaTedaviEkleParam?.Deger1?.ToUpper() ?? "HAYIR";
                if (taahütnameParamDeger=="EVET")
                {
                    var toplamBorc = taahütnameVarMi.ToplamBorc;
                    var yeniBorc = toplamBorc+tedavi.Fiyat;
                    taahütnameVarMi.ToplamBorc = yeniBorc;
                    string konu = "Taahütname Fiyat değişikliği hk.";
                    string mesaj = $"{muayene.MuayeneTarihi} tarihli muayenenize yeni tetkikler eklenmiştir.  {taahütnameVarMi.TahütTarihi} tarihli taahütname" +
                        $"toplam borcu {taahütnameVarMi.ToplamBorc} olmuştur.";
                    var hasta = await _repositoryManager.Muayene.HastaBilgisiGetir(muayene.ProtocolNo);
                    var paramD2 = taahütnameliKaydaTedaviEkleParam?.Deger2?.ToUpper() ?? "HAYIR";
                    var paramD3 = taahütnameliKaydaTedaviEkleParam?.Deger3?.ToUpper() ?? "HAYIR";
                    if(paramD2 =="EVET" && hasta.Email is not null)
                    {
                        try
                        {
                            await _emailService.MailGonderAsync(hasta.Email, konu, mesaj);
                        }
                        catch(Exception ex)
                        {
                            _logger.LogWarning("HATA---------------------------");
                            _logger.LogWarning($"{ex}");
                        }
                    }
                    else if(paramD3=="EVET" && hasta.Phone is not null)
                    {
                       try
                        {
                            await _twilioSms.SmsGonderAsync(hasta.Phone, mesaj);
                        }
                          catch(Exception ex)
                        {
                            _logger.LogWarning($"{ex}");
                        }
                    }
                }
                else
                {
                    throw new BadRequestException("Taahütnamesi olan kayıta tedavi ekleyemezsiniz önce onu iptal edin");
                }
            }
          
            var insertTedavi = new TedaviKaydi()
            {
                doktorId=muayene.DoktorNo,
                MuyaneId=muayene.Id,
                fiyat=tedavi.Fiyat,
                Odendi=false,
                prtokol=muayene.ProtocolNo,
                tedaviAdi=tedavi.TetikAdi,
                tedaviKodu=tedavi.Kodu
            };
            _repositoryManager.Muayene.TedaviKaydiEkle(insertTedavi);
            await _repositoryManager.saveAsyc();
            return new TedaviEkleDTO
            {
                MuyaneId=insertTedavi.MuyaneId,
                tedaviAdi=insertTedavi.tedaviAdi,
                tedaviKodu=insertTedavi.tedaviKodu
            };
        }
    }
}
