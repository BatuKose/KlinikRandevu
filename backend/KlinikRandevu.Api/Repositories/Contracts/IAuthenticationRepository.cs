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
        Task<User> Login(string username, string password);
    }
}
