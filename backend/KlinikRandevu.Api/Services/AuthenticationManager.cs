using BCrypt.Net;
using Entities.Data_Transfer_Objects.Authentication;
using Entities.Data_Transfer_Objects.UserLog;
using Entities.Exeptions.CustomExceptions;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache _memoryCache;
        public AuthenticationManager(IRepositoryManager repositoryManager, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache)
        {
            _repositoryManager = repositoryManager;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache= memoryCache;
        }

        public async Task<TokenResponseDTO> Login(LoginDTO loginDTO)
        {
              
            if (loginDTO is null
                || string.IsNullOrWhiteSpace(loginDTO.username)
                || string.IsNullOrWhiteSpace(loginDTO.password))
                throw new BadRequestException("Kullanıcı adı ve şifre boş olamaz");

            if (loginDTO.username.Trim().Length <= 1 || loginDTO.password.Trim().Length <= 1)
                throw new BadRequestException("Kullanıcı adı ya da şifre bir karakterden büyük olmalıdır");

            var loginEngelliParam = await _repositoryManager.SistemParametresi.GetirAsync("HATALI_LOGIN_BLOK");
            bool blokAktif = loginEngelliParam != null && loginEngelliParam.Deger1?.ToUpper() == "EVET";

            int maxDeneme = 4;
            int blokSure = 5;
            if (blokAktif)
            {
                if (int.TryParse(loginEngelliParam?.Deger2, out var parsedMax)) maxDeneme = parsedMax;
                if (int.TryParse(loginEngelliParam?.Deger3, out var parsedSure)) blokSure  = parsedSure;
            }

            var blokCache = $"hatali_login_{loginDTO.username.Trim().ToLowerInvariant()}";

            if (blokAktif)
            {
                var mevcutSayac = _memoryCache.Get<int?>(blokCache) ?? 0;
                if (mevcutSayac >= maxDeneme)
                    throw new BadRequestException($"Çok fazla hatalı giriş. Lütfen {blokSure} dk sonra tekrardan deneyiniz");
            }

            var user = await _repositoryManager.Authentication.GetByUsernameAsync(loginDTO.username.Trim());

            bool girisBasarisiz;
            if (user is null)
            {
                girisBasarisiz = true;
            }
            else
            {
                try
                {
                   
                    girisBasarisiz = !BCrypt.Net.BCrypt.Verify(loginDTO.password, user.Password);
                }
                catch (SaltParseException ex)
                {
                  
                    Console.WriteLine( $" {ex} Bozuk hash formatı. Kullanıcı: {user.UserName}", user.UserName);
                    girisBasarisiz = true;
                }
            }

            if (girisBasarisiz)
            {
                if (blokAktif)
                {
                    var sayac = (_memoryCache.Get<int?>(blokCache) ?? 0) + 1;
                    _memoryCache.Set(blokCache, sayac, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(blokSure)
                    });
                }
                throw new BadRequestException("Kullanıcı adı veya şifre hatalı");
            }

            if (blokAktif) _memoryCache.Remove(blokCache);

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
