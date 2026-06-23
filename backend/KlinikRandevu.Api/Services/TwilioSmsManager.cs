using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        public TwilioSmsManager(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
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
                var accountSid = _configuration["Twilio:AccountSid"];
            var authToken = _configuration["Twilio:AuthToken"];
            var fromNumber = _configuration["Twilio:FromNumber"];
            try
            {
                TwilioClient.Init(accountSid, authToken);
                var mesaj = await MessageResource.CreateAsync(
                    body: gonderilcekMesaj,
                    from: new Twilio.Types.PhoneNumber(fromNumber),
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
