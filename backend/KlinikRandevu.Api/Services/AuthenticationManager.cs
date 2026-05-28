using Entities.Data_Transfer_Objects.Authentication;
using Entities.Data_Transfer_Objects.UserLog;
using Entities.Exeptions.CustomExceptions;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repositories.Contracts;
using Services.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Services
{
    public class AuthenticationManager : IAuthService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationManager(IRepositoryManager repositoryManager, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _repositoryManager = repositoryManager;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TokenResponseDTO> login(LoginDTO loginDTO)
        {
            if (loginDTO == null) throw new BadRequestException("Kullanıcı bilgileri dolu olmak zorundadır");
            if (loginDTO.username.Trim().Length <= 1 || loginDTO.password.Trim().Length <= 1)
                throw new BadRequestException("Kullanıcı adı ya da şifre bir karakterden büyük olmalıdır");

            var user = await _repositoryManager.Authentication.Login(loginDTO.username, loginDTO.password);
            if (user is null) throw new NotFoundException("Kullanıcı bilgilerine ulaşılamadı");

            var loginLogParametre = await _repositoryManager.SistemParametresi.GetirAsync("LOGIN_LOG_TUTULSUN");
            if (loginLogParametre != null && loginLogParametre.Deger1?.ToUpper() == "EVET")
            {
                var ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "İp adresi bulunamadı";
                var log = new UserLog
                {
                    UserId = user.UserID,
                    AksiyonTipi = "Sisteme giriş",
                    IpAdresi = ip,
                    EntityTipi = "users"
                };
                _repositoryManager.UserLogRepository.LoginLogYaz(log);
            }

            return await GenerateTokenResponseAsync(user);
        }

        public async Task<TokenResponseDTO> RefreshTokenAsync(string refreshToken)
        {
            var user = await _repositoryManager.Authentication.GetUserByRefreshTokenAsync(refreshToken);
            if (user is null) throw new BadRequestException("Geçersiz refresh token");
            if (user.RefreshTokenExpiry < DateTime.UtcNow) throw new BadRequestException("Refresh token süresi dolmuş, lütfen tekrar giriş yapın");

            return await GenerateTokenResponseAsync(user);
        }

        private async Task<TokenResponseDTO> GenerateTokenResponseAsync(User user)
        {
            var accessToken = GenerateAccessToken(user.UserName, user.UserID);
            var newRefreshToken = GenerateRefreshToken();

            await _repositoryManager.Authentication.UpdateRefreshTokenAsync(user.UserID, newRefreshToken, DateTime.UtcNow.AddDays(7));
            await _repositoryManager.saveAsyc();

            return new TokenResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            };
        }

        private string GenerateAccessToken(string username, int userId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:Key"]));
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
                expires: DateTime.UtcNow.AddHours(5),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
