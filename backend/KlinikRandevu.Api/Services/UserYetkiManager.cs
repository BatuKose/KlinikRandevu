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

    }
}
