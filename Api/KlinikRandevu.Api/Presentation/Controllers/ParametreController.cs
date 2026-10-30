using Entities.Constants;
using Entities.Data_Transfer_Objects.Email;
using Entities.Data_Transfer_Objects.Muayene;
using Entities.Data_Transfer_Objects.Parametre;
using Entities.Data_Transfer_Objects.Sms;
using Entities.Data_Transfer_Objects.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Presentation.ActionFilters;
using Presentation.Wrappers;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Intelligence.V3;

namespace Presentation.Controllers
{
    [Route("SistemParametreleri")]
    [ApiController]
    // [EnableRateLimiting("RateLimit")]
    public class ParametreController:ControllerBase
    {
        private readonly IServiceManager _ServiceManager;

        public ParametreController(IServiceManager ıserviceManager)
        {
            _ServiceManager=ıserviceManager;
        }
        [HttpGet("hepsinigetir")]
        public async Task<IActionResult> HepsiniGetir()
        {
            var parametreler = await _ServiceManager.SistemParametreService.HepsiniGetirAsync();
            return Ok(parametreler);
        }

        [HttpPost("parametreekle")]
        public async Task<IActionResult> ParamtreEkle([FromBody] ParametreEkleDTO param)
        {
            var parametre =  await _ServiceManager.SistemParametreService.ParametreEkleAsync(param);
            return NoContent();
        }

        [HttpPatch("parametreguncelle/{id}")]
        public async Task<IActionResult> ParamtreGuncelle([FromRoute] int id,[FromBody] ParametreEkleDTO param)
        {
            var parametre = await _ServiceManager.SistemParametreService.ParametreGuncelle(param,id);
            return NoContent();
        }
        [YetkiKontrol(YetkiKodlari.RedisYetki)]
        [HttpPost("cache-temizle")]
        public IActionResult RedisAll()
        {
            _ServiceManager.SistemParametreService.RedisAll();
            return NoContent();
        }
        [HttpGet("getHolidayfromnagerat")]
         public async Task<IActionResult>GetHolidayDataFromNager()
        {
            var result = await _ServiceManager.NagerDateService.GetHolidaysData();
            return Ok(result);
        }
        [HttpPost("icdTokenAl")]
        public async Task<IActionResult> IcdApiTokenAl()
        {
            var result = await _ServiceManager.IcdApiManager.IcdApiTokenAl();
            return Ok(result);
        }
        [HttpGet("loglar")]
        public async Task<IActionResult> LoglariGetir([FromQuery] DateTime baslangic, [FromQuery] DateTime bitis, [FromQuery] string? aksiyonTipi)
        {
            var result = await _ServiceManager.UserLogService.LoglariGetirAsync(baslangic, bitis, aksiyonTipi);
            return Ok(result);
        }
        [HttpGet("log-tipleri")]
        public async Task<IActionResult> LogTipleriniGetir()
        {
            var result = await _ServiceManager.UserLogService.AksiyonTipleriniGetirAsync();
            return Ok(result);
        }
        [HttpPost("kullaniciekle")]
        public IActionResult UserEkle([FromBody] userEkleDTO user)
        {
           _ServiceManager.SistemParametreService.userEkle(user);
            return NoContent();
        }
        [HttpGet("smsayarlari")]
        public async Task<IActionResult> SmsConfigListele()
        {
            var liste = await _ServiceManager.TwilioSmsManager.SmsConfigListeleAsync();
            return Ok(ApiResponse<List<SmsConfigListeDto>>.SuccessResponse(liste));
        }
        [HttpPost("smsayarikaydet")]
        public async Task<IActionResult> SmsConfigEkle([FromBody] InsertSmsConfigDto model)
        {
            var id = await _ServiceManager.TwilioSmsManager.SmsConfigEkleAsync(model);
            return Ok(ApiResponse<int>.SuccessResponse(id));
        }
        [HttpPut("smsayarigüncelle")]
        public async Task<IActionResult> Guncelle(UpdateSmsConfigDto model)
        {
            await _ServiceManager.TwilioSmsManager.SmsConfigGuncelleAsync(model);
            return NoContent();
        }
        [HttpPatch("sms/{id:int}/aktif-pasif")]
        public async Task<IActionResult> AktifPasif(int id)
        {
            var aktifMi = await _ServiceManager.TwilioSmsManager.SmsConfigAktifPasifAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(aktifMi));
        }
        [HttpGet("emailayarlari")]
        public async Task<IActionResult> EmailConfigListele()
        {
            var liste = await _ServiceManager.EmailService.EmailConfigListeleAsync();
            return Ok(ApiResponse<List<EmailConfigListeDto>>.SuccessResponse(liste));
        }
        [HttpPost("emailayarikaydet")]
        public async Task<IActionResult> EmailConfigEkle([FromBody] InsertEmailConfigDto model)
        {
            var id = await _ServiceManager.EmailService.EmailConfigEkleAsync(model);
            return Ok(ApiResponse<int>.SuccessResponse(id));
        }
        [HttpPut("emailayariguncelle")]
        public async Task<IActionResult> EmailConfigGuncelle([FromBody] UpdateEmailConfigDto model)
        {
            await _ServiceManager.EmailService.EmailConfigGuncelleAsync(model);
            return NoContent();
        }
        [HttpPatch("email/{id:int}/aktif-pasif")]
        public async Task<IActionResult> EmailConfigAktifPasif(int id)
        {
            var aktifMi = await _ServiceManager.EmailService.EmailConfigAktifPasifAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(aktifMi));
        }
    }
}
