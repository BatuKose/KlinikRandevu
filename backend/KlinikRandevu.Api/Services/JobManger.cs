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
    }
}
