using Entities.Data_Transfer_Objects.Authentication;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IAuthenticationRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
        Task UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime expiry);
    }
}
