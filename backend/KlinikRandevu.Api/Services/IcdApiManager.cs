using Entities.Data_Transfer_Objects.IcdApi;
using Entities.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Voice;
using Twilio.Types;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
namespace Services
{
    public class IcdApiManager: IIcdApiManager
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;
        private readonly IRepositoryManager _repositoryManager;
        public IcdApiManager(IConfiguration configuration, IHttpClientFactory httpClientFactory,ILogger logger,IRepositoryManager manager)
        {
            _configuration = configuration;
            _httpClientFactory=httpClientFactory;
            _logger = logger;
            _repositoryManager=manager;
        }
        public async Task<string> IcdApiTokenAl()
        {
            var client = _httpClientFactory.CreateClient();
            var clientId = _configuration["Icd:ClientId"];
            var clientSecret = _configuration["Icd:ClientSecret"];

            var postDetay= new Dictionary<string, string>
            {
                ["client_id"]     = clientId,
                ["client_secret"] = clientSecret,
                ["scope"]         = "icdapi_access",
                ["grant_type"]    = "client_credentials"
            };

            var body= new FormUrlEncodedContent(postDetay);

             try
            {
                var istek = await client.PostAsync(
                "https://icdaccessmanagement.who.int/connect/token", body);
                if (!istek.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                       "ICD token alınamadı. Status: {Status}",
                       istek.StatusCode);
                    return null;

                }

                var response = await istek.Content.ReadFromJsonAsync<IcdTokenCevabiDto>();
                if (string.IsNullOrEmpty(response?.AccessToken))
                {
                    _logger.LogWarning("ICD cevabı geldi ama access_token boş.");
                    return null;
                }

                _logger.LogInformation("ICD token başarıyla alındı.");
                var Dbyekaydet = new IcdApiEntegrasyon
                {
                    TokenType=response.TokenType,
                    Token=response.AccessToken,
                    GecerlilikSüresi=DateTime.UtcNow.AddSeconds(response.ExpiresIn)
                };
                _repositoryManager.IcdApiRepository.TokenKayet(Dbyekaydet);
                await _repositoryManager.saveAsyc();
                return response?.AccessToken;
            }
            catch(Exception ex)
            {
                _logger.LogWarning("ICD token alınırken beklenmeyen bir hata oluştu.");
                return null;
            }
        }
    }
}

