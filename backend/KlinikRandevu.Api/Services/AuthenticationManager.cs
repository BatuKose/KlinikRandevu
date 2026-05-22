using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AuthenticationManager : IAuthService
    {
        private readonly IRepositoryManager _repositoryManager;

        public AuthenticationManager(IRepositoryManager repositoryManager)
        {
            _repositoryManager=repositoryManager;
        }
    }
}
