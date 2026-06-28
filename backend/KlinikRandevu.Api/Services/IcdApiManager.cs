using Entities.Data_Transfer_Objects.IcdApi;
using Entities.Exeptions.CustomExceptions;
using Entities.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
                var tokenVarmi = await _repositoryManager.IcdApiRepository.DbApiTokenGetir();
                if(tokenVarmi is null)
                {
                    var Dbyekaydet = new IcdApiEntegrasyon
                    {
                        TokenType=response.TokenType,
                        Token=response.AccessToken,
                        GecerlilikSüresi=DateTime.UtcNow.AddSeconds(response.ExpiresIn)
                    };
                    _repositoryManager.IcdApiRepository.TokenKayet(Dbyekaydet);
                   
                }
                else
                {
                    tokenVarmi.Token= response.AccessToken;
                    tokenVarmi.TokenType= response.TokenType;
                    tokenVarmi.GecerlilikSüresi=DateTime.UtcNow.AddSeconds(response.ExpiresIn);
                }
                await _repositoryManager.saveAsyc();
                return response?.AccessToken;
            }
            catch(Exception ex)
            {
                _logger.LogWarning("ICD token alınırken beklenmeyen bir hata oluştu.");
                return null;
            }
        }
        public async Task<List<TaniDto>> TaniAraAsync(string aranan)
        {
            string token = "";
            var tokenGetir = await _repositoryManager.IcdApiRepository.DbApiTokenGetir();
            if (tokenGetir is null) throw new BadRequestException("Token bulunamadı");
            if(tokenGetir.GecerlilikSüresi>DateTime.UtcNow.AddMinutes(1))
            { 
              token= tokenGetir.Token;
            }
            else
            {
                var tokenAl = await IcdApiTokenAl();
                token=tokenAl;
            }
            var client = _httpClientFactory.CreateClient();
            var url= $"https://id.who.int/icd/release/11/2025-01/mms/search" +
              $"?q={Uri.EscapeDataString(aranan)}" +
              $"&flatResults=true" +
              $"&highlightingEnabled=false" +
              $"&medicalCodingMode=true";

            var istek = new HttpRequestMessage(HttpMethod.Get, url);
            istek.Headers.Authorization= new AuthenticationHeaderValue("Bearer",token);
            istek.Headers.Add("API-Version", "v2");
            istek.Headers.Add("Accept-Language", "tr");
            istek.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var cevap= await client.SendAsync(istek);

            if(!cevap.IsSuccessStatusCode)
            {
                _logger.LogWarning($"ICD kodu bulunamadı{cevap.StatusCode}, aranan {aranan}");
                return new List<TaniDto>();
            }
            var data = await cevap.Content.ReadFromJsonAsync<IcdAramaCevabiDto>();
            return data?.DestinationEntities??new List<TaniDto>();

        }
    }
}

