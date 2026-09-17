using Entities.Data_Transfer_Objects.Muayene;
using Entities.Exeptions.CustomExceptions;
using Entities.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class JobManger : IJobService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IEmailService _emailManager;
        private readonly ITwilioSmsManager _twilioSmsManager;
        private readonly ILogger<JobManger> _logger;

        public JobManger(IRepositoryManager repositoryManager, IEmailService emailManager, ITwilioSmsManager twilioSmsManager, ILogger<JobManger> logger)
        {
            _repositoryManager=repositoryManager;
            _emailManager=emailManager;
            _twilioSmsManager=twilioSmsManager;
            _logger=logger;
        }

        public async Task HatirlatmalariGonderAsync()
        {
            var ozellikAcikMi = await _repositoryManager.SistemParametresi.GetirAsync("JOB_HATIRLATMA_MAIL_SMS_GONDER");
            if(ozellikAcikMi == null )
            {
                string paramName = "JOB_HATIRLATMA_MAIL_SMS_GONDER";
                parametreEke(paramName);
            }
            var paramdeger = ozellikAcikMi?.Deger1?.ToUpper() ??"HAYIR";
            if(paramdeger !="EVET")
            {
                return;
            }
            var baslangic = DateTime.UtcNow.Date.AddDays(1);
            var bitis = baslangic.AddDays(1);
            var randevular= await _repositoryManager.Muayene.JobYarininRandevuluHastalari(baslangic, bitis);
            if(randevular is null|| randevular.Count==0)
            {
                return;
            }
            var basariliIdler = new List<int>();
            foreach(var randevu in randevular)
            {
                try
                {
                    var mesaj = $"{randevu.randevutarihi} tarihininde {randevu.poliklinik} {randevu.doktorad} adlı polinkliniğe randevunuz bulunmaktadır.";
                    var konu = "Yaklaşan Randevunuz";
                    var mailJobDeger = ozellikAcikMi?.Deger3?.ToUpper() ?? "HAYIR";
                    if(mailJobDeger=="EVET")
                    {

                        await _emailManager.MailGonderAsync(randevu.email, konu, mesaj);
                    }
                    var smsJobDeger = ozellikAcikMi?.Deger2?.ToUpper() ?? "HAYIR";
                    if(smsJobDeger=="EVET")
                    {
                        await _twilioSmsManager.SmsGonderAsync(randevu.numara,mesaj);
                    }
                    basariliIdler.Add(randevu.randevuId);
                }
                catch(Exception ex)
                {
                    _logger.LogWarning($"{randevu.randevuId} randevu idli Jobtan randevu hatırlatıcı mail gönderilemedi \n {ex}");
                }
            }
            await _repositoryManager.Muayene.HatirlatmaMilUpte(basariliIdler);
        }
        public async Task DoktorGunlukProgramHatirlatmaGonderAsync()
        {
            var ozellikAcikMi = await _repositoryManager.SistemParametresi.GetirAsync("JOB_DOKTOR_HATIRLATMA_GONDER");
            if(ozellikAcikMi is null)
            {
                string paramName = "JOB_DOKTOR_HATIRLATMA_GONDER";
                parametreEke(paramName);
            }
            var paramdeger = ozellikAcikMi?.Deger1?.ToUpper() ??"HAYIR";
            if (paramdeger !="EVET")
            {
                return;
            }
            var doktorIdler = await _repositoryManager.Muayene.DoktorIdleriniGetir();
            if(doktorIdler==null  || doktorIdler.Count==0)
            {
                return;
            }
            
            foreach(var doktor in doktorIdler)
            {
                
               var bilgilendir = await _repositoryManager.Muayene.DoktorRandevuHatirlatma(doktor);
               
                if (!bilgilendir.Any())
                {
                    return;
                }
                try
                {
                    var ilkKayit = bilgilendir.First();
                    var doktorAd = ilkKayit.doktorad;
                    var doktorEmail = ilkKayit.doktormail;
                    var htmlIcerik = MailIcerikOlustur(doktorAd, bilgilendir);
                    var konu = $"Günlük randevu programınız-{DateTime.Today:dd:MM:yyyy}";
                    await _emailManager.MailGonderAsync(doktorEmail, konu, htmlIcerik);
                }
                catch(Exception ex)
                {
                    _logger.LogWarning($"{ex}");
                }

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

        public async Task MuayeneOnayiVerilmemisKayitlariKapat()
        {
            var özellikAcikMi = await _repositoryManager.SistemParametresi.GetirAsync("JOB_OTOMATİK_MUAYENE_KAPAT");
            if(özellikAcikMi is null)
            {
                string paramName = "JOB_OTOMATİK_MUAYENE_KAPAT";
                parametreEke(paramName);
            }
            var özekkikD1 = özellikAcikMi?.Deger1?.ToUpper() ?? "HAYIR";
            var özellikD2 = özellikAcikMi?.Deger2;
            if (!int.TryParse(özellikD2, out var dakika))
                dakika = 15;

            var etkilenen = await _repositoryManager.Muayene
                .MuayeneKapatmaUpdate(TimeSpan.FromMinutes(dakika));

         //  Console.WriteLine("{Adet} muayene otomatik kapatıldı.", etkilenen); 
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

        public async Task OtomatikMuayeneAc()
        {
            var ozellikAcikMi = await _repositoryManager.SistemParametresi.GetirAsync("JOB_BELIRLI_POLLERE_OTO_MUAYENE_AC");
            if( ozellikAcikMi == null)
            {
                parametreEke("JOB_BELIRLI_POLLERE_OTO_MUAYENE_AC");
            }
            var paramDeger1 = ozellikAcikMi?.Deger1?.ToUpper()??"HAYIR";
            var paramDeger2 = ozellikAcikMi?.Deger2;
            if( paramDeger1!="EVET" && paramDeger2 != null)
            {
                return;
            }
            
            var hastalar = await _repositoryManager.Muayene.YariniHastalariniGetir();
            if( hastalar!=null && hastalar.Count>0)
            {
                var polNo = Convert.ToInt32(paramDeger2);
                foreach (var hasta in hastalar)
                {
                    if(hasta.polno!=polNo)
                    {
                        return;
                    }
                    var muayeneeKaydi = new MuayeneKaydi()
                    {
                        PolNo=polNo,
                        HastaTc=hasta.tc,
                        ProtocolNo=hasta.protokol,
                        DoktorNo=hasta.doktor,
                        MuayeneTarihi=hasta.tarih,
                        BaslangicSaati=hasta.muayenesaati,
                        RandevuId=hasta.randevuid


                    };
                    _repositoryManager.Muayene.MuayeneKaydiOlustur(muayeneeKaydi);
                    var log = new UserLog()
                    {
                        UserId=null,
                        IpAdresi=null,
                        Detay="JOB TARAFINDAN OTOMATİK OLARAK MUAYENE OLUŞTURULDU",
                        EntityTipi="Muayene",
                        AksiyonTipi="INSERT",
                        EntityId=muayeneeKaydi.ProtocolNo
                    };
                }
                await _repositoryManager.saveAsyc();
            }

        }
        public async Task TaahütnameBilgilendirme()
        {
            var jobAcikMi = await _repositoryManager.SistemParametresi.GetirAsync("JOB_TAAHUTNAME_BILGILENDIRME_ACIK");
            if (jobAcikMi == null)
            {
                parametreEke("JOB_TAAHUTNAME_BILGILENDIRME_ACIK");
                return;   
            }

            var paramDeger1 = jobAcikMi?.Deger1?.ToUpper() ?? "HAYIR";
            var paramDeger2 = jobAcikMi?.Deger2?.ToUpper() ?? "HAYIR";
            var paramDeger3 = jobAcikMi?.Deger3?.ToUpper() ?? "HAYIR";

            if (paramDeger1 != "EVET")
                return;

            int smsRequest;
            if (paramDeger2 == "EVET" && paramDeger3 == "HAYIR")
                smsRequest = 1;
            else if (paramDeger2 == "HAYIR" && paramDeger3 == "EVET")
                smsRequest = 2;
            else if (paramDeger2 == "EVET" && paramDeger3 == "EVET")
                smsRequest = 3;                          
            else
                return;                                  

            var borcluHastalar = await _repositoryManager.Muayene.YaklasanTahütBilgilendirme(smsRequest);
            if (borcluHastalar == null || borcluHastalar.Count == 0)
                return;

            var smsGidenler = new List<int>();
            var mailGidenler = new List<int>();

            foreach (var hasta in borcluHastalar)
            {
                var icerik = $"sayın hastamız {hasta.muaTarih} tarihli {hasta.polAd} poliklinik muayeneniz sonucu {hasta.TaTarih} tarihli " +
                             $"taahütnameniz bulunmaktadır. Borcunuz {hasta.borc} TL'dir. Son ödeme tarihi {hasta.SonOdemeTarihi}'dir geçmiş olsun";

                try
                {
                    if (smsRequest == 1)
                    {
                        await _twilioSmsManager.SmsGonderAsync(hasta.tel, icerik);
                        smsGidenler.Add(hasta.taahütnameId);
                    }
                    else if (smsRequest == 2)
                    {
                        await _emailManager.MailGonderAsync(hasta.mail, "Taahütname Hk.", icerik);
                        mailGidenler.Add(hasta.taahütnameId);
                    }
                    else if (smsRequest == 3)
                    {
                        await _emailManager.MailGonderAsync(hasta.mail, "Taahütname Hk.", icerik);
                        await _twilioSmsManager.SmsGonderAsync(hasta.tel, icerik);
                        smsGidenler.Add(hasta.taahütnameId);
                        mailGidenler.Add(hasta.taahütnameId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Taahütname bilgilendirme gönderilemedi. TaahütnameId: {Id}", hasta.taahütnameId);
                }
            }

            if (smsGidenler.Count > 0)
                await _repositoryManager.Muayene.HatirlatmaTaahütnameUpdateSMS(smsGidenler);

            if (mailGidenler.Count > 0)
                await _repositoryManager.Muayene.HatirlatmaTaahütnameUpdateMAIL(mailGidenler);
        }

        public async Task RandevuBekletenHastalariBildir()
        {
            var ozellikAcikMi = await _repositoryManager.SistemParametresi.GetirAsync("RANDEVU_BEKLEYEN_HASTALARI_BILDIR");
            if (ozellikAcikMi is null)
            {
                string paramName = "RANDEVU_BEKLEYEN_HASTALARI_BILDIR";
                parametreEke(paramName);
            }
            var paramdeger = ozellikAcikMi?.Deger1?.ToUpper() ??"HAYIR";
            if (paramdeger !="EVET")
            {
                return;
            }
            var hastalar =  await _repositoryManager.Muayene.RandevuBekleyenHastalariGetirAsync();
            if(hastalar is null || hastalar.Count<0)
            {
                return;
            }
            var paramDeger2 = ozellikAcikMi?.Deger2?.ToUpper() ?? "HAYIR";
            var paramDeger3 = ozellikAcikMi?.Deger3?.ToUpper() ?? "HAYIR";
            int smsRequest;
            if (paramDeger2 == "EVET" && paramDeger3 == "HAYIR")
                smsRequest = 1;
            else if (paramDeger2 == "HAYIR" && paramDeger3 == "EVET")
                smsRequest = 2;
            else if (paramDeger2 == "EVET" && paramDeger3 == "EVET")
                smsRequest = 3;
            else
                return;
            var BilgilendirmeGidenler = new List<int>();
            foreach (var hasta in hastalar)
            {
                var hastaBilgi = await _repositoryManager.Muayene.HastaBilgisiGetir(hasta.protokol);
                if(hastaBilgi is null)
                {
                    return;
                }
                string bilgilendir = $"{hasta.RandevuTarihi} tarihli oluşturduğunuz randevu Bekleme istemi olumlu sonuclandı randevu alabilirsiniz";
                if(smsRequest== 1 && hastaBilgi.Phone is not null)
                {
                   await _twilioSmsManager.SmsGonderAsync(hastaBilgi.Phone, bilgilendir);
                    BilgilendirmeGidenler.Add(hasta.Id);
                }
                else if(smsRequest== 2 && hastaBilgi.Email is not null)
                {
                    await _emailManager.MailGonderAsync(hastaBilgi.Email, "Bekleyen Randevu", bilgilendir);
                    BilgilendirmeGidenler.Add(hasta.Id);
                }
                else if(smsRequest== 3 && hastaBilgi.Email is not null && hastaBilgi.Phone is not null)
                {
                    await _emailManager.MailGonderAsync(hastaBilgi.Email, "Bekleyen Randevu", bilgilendir);
                    await _twilioSmsManager.SmsGonderAsync(hastaBilgi.Phone, bilgilendir);
                    BilgilendirmeGidenler.Add(hasta.Id);
                }
                else
                {
                    return;
                }
                if(BilgilendirmeGidenler.Count>0)
                {
                     await _repositoryManager.Muayene.HatirlatmaBekleyenRandevuUpdate(BilgilendirmeGidenler);
                }

            }
        }

        public async Task RandevuOzelMesajGonder()
        {
            var ozellikAcikMi = await _repositoryManager.SistemParametresi.GetirAsync("SERVISE_OZEL_RANDEVU_SONRASI_OZEL_MESAJ_GONDER");
            if( ozellikAcikMi == null ) 
            {
                parametreEke("SERVISE_OZEL_RANDEVU_SONRASI_OZEL_MESAJ_GONDER");
                return; 
            }
            var ParamD1= ozellikAcikMi.Deger1?.ToUpper() ?? "HAYIR";
            if( ParamD1 =="HAYIR")
            {
                return;
            }
            var paramD2 = int.TryParse(ozellikAcikMi.Deger2, out var parsed) ? parsed : 0;
            if( paramD2 == 0 )
            {
                return;
            }
            var paramD3=ozellikAcikMi.Deger3;
            if(string.IsNullOrEmpty(paramD3))
            {
                return;
            }
            var param4=int.TryParse(ozellikAcikMi.Deger4,out var parsed2) ? parsed2 : 0;
            if(parsed2== 0)
            {
                return;
            }
            var hastalar = await _repositoryManager.Muayene.JobRandevuluHastalaraOzelMesajGonderilcekleriGetir(paramD2);
            if(hastalar == null || hastalar.Count<1)
            {
                return;
            }
            int bilgilendirmeRequest;
            if(param4 == 1)
            {
                bilgilendirmeRequest=1;
            }
            else if(param4==2)
            {
                bilgilendirmeRequest=2;
            }
            else if( param4==3)
            {
                bilgilendirmeRequest=3;
            }
            else
            {
                return;
            }
            var BilgilendirmeGidenler = new List<int>();
            foreach(var hasta in hastalar)
            {
                var hastaBilgi = await _repositoryManager.Muayene.HastaBilgisiGetir(hasta.protokol);
                if (hastaBilgi is null)
                {
                    return;
                }
                string mesaj = paramD3;
                if(bilgilendirmeRequest==1 && hastaBilgi.Email is not null)
                {
                    await _emailManager.MailGonderAsync(hastaBilgi.Email, "Bilgilendirme", mesaj);
                    BilgilendirmeGidenler.Add(hasta.randevuid);
                }
                else if(bilgilendirmeRequest==2 && hastaBilgi.Phone is not null)
                {
                    await _twilioSmsManager.SmsGonderAsync(hastaBilgi.Phone, mesaj);
                    BilgilendirmeGidenler.Add(hasta.randevuid);
                }
                else if(bilgilendirmeRequest==3 && hastaBilgi.Email is not null && hastaBilgi.Phone is not null)
                {
                    await _emailManager.MailGonderAsync(hastaBilgi.Email, "Bilgilendirme", mesaj);
                    await _twilioSmsManager.SmsGonderAsync(hastaBilgi.Phone, mesaj);
                    BilgilendirmeGidenler.Add(hasta.randevuid);
                }
                else
                {
                    return;
                }
                if(BilgilendirmeGidenler.Count > 0)
                {
                    await _repositoryManager.Muayene.OzelMesajGonderilenlerUpdate(BilgilendirmeGidenler);
                }
                else
                {
                    return;
                }
            }

        }
    }
}
