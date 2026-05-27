using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Caching.Memory;
using Repositories.Contracts;
using Repositories.EFCore;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserYetkiManager : IUserYetkiService
    {
        private const string YetkiKodMapCacheKey = "yetki_kod_map";
        private static readonly TimeSpan YetkiKodMapSuresi = TimeSpan.FromHours(1);

        private readonly IRepositoryManager _repositoryManager;
        private readonly IMemoryCache _cache;
        public UserYetkiManager(IRepositoryManager repositoryManager, IMemoryCache memoryCache)
        {
            _repositoryManager = repositoryManager;
            _cache = memoryCache;
        }

        public async Task<HashSet<int>> GetUserYetkiIds(int userId)
        {
            
            string cacheKey = $"user_yetki_{userId}";
            if(_cache.TryGetValue(cacheKey,out HashSet<int> yetkiler))
            {
                return yetkiler;
            }
            yetkiler = await _repositoryManager.UserYetkiRepository.GetUserYetkiId(userId);
            _cache.Set(cacheKey, yetkiler, TimeSpan.FromMinutes(30));
            return yetkiler;
        }


        public void InvalidateUserCache(int userId)
        {
            string cacheKey = $"user_yetki_{userId}";
            _cache.Remove(cacheKey);
        }

        private async Task<Dictionary<string,int>>GetYetkiKontrolMapInternal()
        {
            if(_cache.TryGetValue(YetkiKodMapCacheKey,out Dictionary<string,int>map))
            {
                return map;
            }
            map = await _repositoryManager.UserYetkiRepository.GetYetkiKodMap();
            _cache.Set(YetkiKodMapCacheKey,map,TimeSpan.FromHours(3));
            return map;
        }

        public async Task<int?> GetYetkiIdByKod(string kod)
        {
            var map = await GetYetkiKontrolMapInternal();
            if(map.TryGetValue(kod,out var yetkiid))
            {
                return yetkiid;
            }
            return null;
        }

        
        public void InvalidateYetkiKodMap()
        {
            _cache.Remove(YetkiKodMapCacheKey);
        }
    }
}
