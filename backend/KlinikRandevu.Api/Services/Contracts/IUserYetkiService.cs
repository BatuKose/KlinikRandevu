using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IUserYetkiService
    {
        Task<HashSet<int>> GetUserYetkiIds(int userId);
        void InvalidateUserCache(int userId);
    }
}
