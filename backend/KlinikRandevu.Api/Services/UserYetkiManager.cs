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
    }
}
