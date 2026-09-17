using Entities.Data_Transfer_Objects.Nager;
using Entities.Models;
using Repositories.Contracts;
using Repositories.EFCore;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Services
{
    public class NagerDateManager : INagerDateService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IHttpClientFactory _httpClientFactory;

        public NagerDateManager(IRepositoryManager repositoryManager, IHttpClientFactory httpClientFactory)
        {
            _repositoryManager = repositoryManager;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<NagerGetDataDTO>> GetHolidaysData()
        {
            var client = _httpClientFactory.CreateClient();   
            var year = DateTime.Now.Year;

            var liste = await client.GetFromJsonAsync<List<NagerGetDataDTO>>(
                $"https://date.nager.at/api/v3/PublicHolidays/{year}/TR");
            if (liste is { Count: > 0 })
            {
              // Transaction eklenecek
                bool dataKontrol = await _repositoryManager.TatilRepository.ApiVeriVarmı();
                if(dataKontrol)
                {
                  await   _repositoryManager.TatilRepository.ApiVerileriniSil();
                
                }
                foreach(var item in liste)
                {
                    var tatil = new Tatil
                    {
                        ApiMi = true,
                        CreatedAt = DateTime.Now,
                        TatilAdi = item.localName,
                        Tarih = item.date
                    };
                    _repositoryManager.TatilRepository.TatilGünleriEkle(tatil);

                }
                await _repositoryManager.saveAsyc();
                // Transaction bitecek
            }
            return liste ?? new();
        }
    }
}
