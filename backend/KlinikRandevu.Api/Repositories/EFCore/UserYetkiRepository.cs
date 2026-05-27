using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    public class UserYetkiRepository: IUserYetkiRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public UserYetkiRepository(RepositoryContext repositoryContext)
        {
            _repositoryContext=repositoryContext;
        }

        public async Task<HashSet<int>> GetUserYetkiId(int userId)
        {
            var yetkiIds = await _repositoryContext.UserYetkiler.Where(uy => uy.UserId==userId)
                .Select(uy => uy.YetkiId).ToListAsync();
            return yetkiIds.ToHashSet();
        }
    }
}
