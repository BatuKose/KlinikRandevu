using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Services.Contracts;

namespace Presentation.ActionFilters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class YetkiKontrolAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string _yetkiKodu;

        public YetkiKontrolAttribute(string yetkiKodu)
        {
            _yetkiKodu = yetkiKodu;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var userIdClaim = context.HttpContext.User.FindFirst("UserID")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var yetkiService = context.HttpContext.RequestServices
                .GetRequiredService<IUserYetkiService>();

            var gerekliId = await yetkiService.GetYetkiIdByKod(_yetkiKodu);
            if (gerekliId == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            var kullaniciYetkileri = await yetkiService.GetUserYetkiIds(userId);
            if (!kullaniciYetkileri.Contains(gerekliId.Value))
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}