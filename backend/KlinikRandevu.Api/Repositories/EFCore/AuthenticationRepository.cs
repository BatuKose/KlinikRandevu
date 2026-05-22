using Entities.Data_Transfer_Objects.Authentication;
using Entities.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public AuthenticationRepository(RepositoryContext repositoryContext)
        {
            _repositoryContext=repositoryContext;
        }

        public async Task<User> Login(string username, string password)
        {
            var user = await _repositoryContext.Users.FirstOrDefaultAsync(u => u.UserName==username && u.Password==password);
            return user;
        }
    }
}
