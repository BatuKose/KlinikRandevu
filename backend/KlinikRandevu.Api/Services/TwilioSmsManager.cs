using Microsoft.Extensions.Configuration;
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

        public TwilioSmsManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SmsGonderAsync(string telefonNumarası, string gonderilcekMesaj)
        {
            var accountSid = _configuration["Twilio:AccountSid"];
            var authToken = _configuration["Twilio:AuthToken"];
            var fromNumber = _configuration["Twilio:FromNumber"];
            //if (string.IsNullOrEmpty(accountSid) || string.IsNullOrEmpty(authToken))
            //    throw new Exception($"Twilio config boş! SID null mu: {accountSid is null}, Token null mu: {authToken is null}");
            //TwilioClient.Init(accountSid, authToken);

            //var mesaj = await MessageResource.CreateAsync(
            //    body: gonderilcekMesaj,
            //    from: new Twilio.Types.PhoneNumber(fromNumber),
            //    to: new Twilio.Types.PhoneNumber(telefonNumarası)
            //    );
            //return mesaj.Status!=MessageResource.StatusEnum.Failed;
           

            try
            {
                TwilioClient.Init(accountSid, authToken);
                var mesaj = await MessageResource.CreateAsync(
                    body: gonderilcekMesaj,
                    from: new Twilio.Types.PhoneNumber(fromNumber),
                    to: new Twilio.Types.PhoneNumber(telefonNumarası)
                );

                return mesaj.Status != MessageResource.StatusEnum.Failed;
            }
            catch (Twilio.Exceptions.ApiException ex)
            {
                
                throw new Exception($"TWILIO HATASI → Kod: {ex.Code}, Mesaj: {ex.Message}, Detay: {ex.MoreInfo}");
            }

        }
    }
}
