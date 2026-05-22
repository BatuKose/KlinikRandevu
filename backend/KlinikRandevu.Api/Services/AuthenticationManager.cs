using Entities.Data_Transfer_Objects.Authentication;
using Entities.Exeptions.CustomExceptions;
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

        public async Task<LoginDTO> login(LoginDTO loginDTO)
        {
            if (loginDTO == null) throw new BadRequestException("Kullanıcı bilgleri dolu olmak zorundadır");
            if ((loginDTO.username.Trim().Length<=1 ||  loginDTO.password.Trim().Length<=1))
                throw new BadRequestException("Kullanıcı adı yada şifre bir karekterden büyük olmalıdır");
            var user= await _repositoryManager.Authentication.Login(loginDTO.username, loginDTO.password);
            if (user is null) throw new NotFoundException("Kullanıcı bilgilerine ulaşılamadı");
            return new LoginDTO
            {
                username=user.UserName,
                password=user.Password
            };

        }
    }
}
