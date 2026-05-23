using Entities.Data_Transfer_Objects.Authentication;
using Entities.Exeptions.CustomExceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AuthenticationManager : IAuthService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IConfiguration _configuration;
        public AuthenticationManager(IRepositoryManager repositoryManager, IConfiguration configuration)
        {
            _repositoryManager=repositoryManager;
            _configuration=configuration;
        }

        public async Task<string> login(LoginDTO loginDTO)
        {
            if (loginDTO == null) throw new BadRequestException("Kullanıcı bilgleri dolu olmak zorundadır");
            if ((loginDTO.username.Trim().Length<=1 ||  loginDTO.password.Trim().Length<=1))
                throw new BadRequestException("Kullanıcı adı yada şifre bir karekterden büyük olmalıdır");
            var user= await _repositoryManager.Authentication.Login(loginDTO.username, loginDTO.password);
            if (user is null) throw new NotFoundException("Kullanıcı bilgilerine ulaşılamadı");
            string tokenAd = user.UserName;
            int tokenUserId=user.UserID;
            return await GenerateToken(tokenAd, tokenUserId);
        }
        public async Task<string> GenerateToken(string username, int userId)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["jwt:Key"])
            );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, username),
        new Claim("UserID", userId.ToString())
    };

            var token = new JwtSecurityToken(
                issuer: _configuration["jwt:Issuer"],
                audience: _configuration["jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(5),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
