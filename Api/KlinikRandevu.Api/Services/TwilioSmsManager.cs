using Entities.Data_Transfer_Objects.Sms;
using Entities.Exeptions.CustomExceptions;
using Entities.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Repositories.Contracts;
using Repositories.EFCore;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Services
{
    public class TwilioSmsManager: ITwilioSmsManager
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly RepositoryContext _repositoryManager;
        public TwilioSmsManager(IConfiguration configuration, ILogger<TwilioSmsManager> logger, RepositoryContext manager)
        {
            _configuration = configuration;
            _logger = logger;
            _repositoryManager = manager;
        }

        public async Task<int> SmsConfigEkleAsync(InsertSmsConfigDto model)
        {
            if (model == null)
                throw new BadRequestException("Sms bilgilerini kontrol ediniz");

            var aktifler = await _repositoryManager.SmsConfig
                .Where(s => s.IsActive)
                .ToListAsync();

            foreach (var sms in aktifler)
                sms.IsActive = false;

            var yeniConfig = new SmsConfig
            {
                AccountId = model.AccountId,
                Token = model.Token,
                FromNumber = model.FromNumber,
                IsActive = true
            };

            _repositoryManager.SmsConfig.Add(yeniConfig);

            try
            {
                await _repositoryManager.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sms bilgisi kayıt edilemedi.");
                throw;
            }

            return yeniConfig.Id;
        }
        public async Task SmsConfigGuncelleAsync(UpdateSmsConfigDto model)
        {
            if (model == null)
                throw new BadRequestException("Sms bilgilerini kontrol ediniz");

            var config = await _repositoryManager.SmsConfig
                .FirstOrDefaultAsync(s => s.Id == model.Id);

            if (config == null)
                throw new NotFoundException("Güncellenecek sms ayarı bulunamadı");

            config.AccountId = model.AccountId;
            config.FromNumber = model.FromNumber;

            if (!string.IsNullOrWhiteSpace(model.Token))
                config.Token = model.Token;

            try
            {
                await _repositoryManager.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sms bilgisi güncellenemedi. Id: {Id}", model.Id);
                throw;
            }
        }

        public async Task<bool> SmsConfigAktifPasifAsync(int id)
        {
            var config = await _repositoryManager.SmsConfig
                .FirstOrDefaultAsync(s => s.Id == id);

            if (config == null)
                throw new NotFoundException("Sms ayarı bulunamadı");

            if (config.IsActive)
            {
                config.IsActive = false;
            }
            else
            {
                var digerAktifler = await _repositoryManager.SmsConfig
                    .Where(s => s.IsActive && s.Id != id)
                    .ToListAsync();

                foreach (var sms in digerAktifler)
                    sms.IsActive = false;

                config.IsActive = true;
            }

            try
            {
                await _repositoryManager.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sms ayarı aktif/pasif yapılamadı. Id: {Id}", id);
                throw;
            }

            return config.IsActive;
        }
        public async Task<bool> SmsGonderAsync(string telefonNumarası, string gonderilcekMesaj)
        {
            string twilioCepStandart;
            if(telefonNumarası.StartsWith("0"))
            {
                twilioCepStandart = "+90" + telefonNumarası.Substring(1);
            }
            else if(telefonNumarası.StartsWith("5"))
            {
                twilioCepStandart = "+90" + telefonNumarası;
            }
            else
            {
                twilioCepStandart = telefonNumarası;
            }
                var smsModel = await _repositoryManager.SmsConfig.Where(s => s.IsActive)
                    .Select(s => new GetSmsConfigDto
                    {
                        AccountId=s.AccountId,
                        FromNumber=s.FromNumber,
                        Token=s.Token
                    }).SingleOrDefaultAsync();
                if (smsModel == null)
                    throw new Exception("Kurumunuza ait sms firma bilgileri tanımlı değildir kontrol ediniz");
            
            try
            {
                
                TwilioClient.Init(smsModel.AccountId, smsModel.Token);
                var mesaj = await MessageResource.CreateAsync(
                    body: gonderilcekMesaj,
                    from: new Twilio.Types.PhoneNumber(smsModel.FromNumber),
                    to: new Twilio.Types.PhoneNumber(twilioCepStandart)
                );

                return mesaj.Status != MessageResource.StatusEnum.Failed;
            }
            catch (Twilio.Exceptions.ApiException ex)
            {

                _logger.LogError($"TWILIO Api hata-----> Kodu: {ex.Code}, Mesaj: {ex.Message}, Detay: {ex.MoreInfo}");

                throw new Exception($"TWILIO Api hata Kodu: {ex.Code}, Mesaj: {ex.Message}, Detay: {ex.MoreInfo}");
            }

        }
    }
}
