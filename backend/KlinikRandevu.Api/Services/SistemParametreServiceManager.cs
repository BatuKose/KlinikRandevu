using Entities.Models;
using Microsoft.Extensions.Caching.Memory;
using Repositories.Contracts;
using Services.Contracts;

namespace Services
{
    public class SistemParametreServiceManager : ISistemParametreService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMemoryCache _cache;

        public SistemParametreServiceManager(IRepositoryManager repositoryManager, IMemoryCache cache)
        {
            _repositoryManager = repositoryManager;
            _cache = cache;
        }


        public async Task<SistemParametresi?> GetirAsync(string parametreAdi)
        {
            var cacheKey = $"sysparam_{parametreAdi}";

            if (_cache.TryGetValue(cacheKey, out SistemParametresi? cached))
                return cached;
            var param =await _repositoryManager.SistemParametresi.GetirAsync(parametreAdi);
            if(param != null)
            {
                _cache.Set(cacheKey, param,TimeSpan.FromMinutes(30));
            }
            return param;
                
        }

        public async Task<bool> AktifMi(string parametreAdi)
        {
           var param= await GetirAsync(parametreAdi);
            return param != null &&param.Deger1?.ToUpper()=="EVET";
        }
    }
}