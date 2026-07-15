using Entities.Data_Transfer_Objects.Muayene;
using Entities.Exeptions.CustomExceptions;
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
    }
}
