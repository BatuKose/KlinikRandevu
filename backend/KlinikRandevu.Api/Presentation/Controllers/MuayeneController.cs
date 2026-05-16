using Entities.Data_Transfer_Objects.Muayene;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/Poliklinik")]
    public class MuayeneController:ControllerBase
    {
        private readonly IServiceManager _ServiceManager;

        public MuayeneController(IServiceManager ıserviceManager)
        {
            _ServiceManager=ıserviceManager;
        }
        [HttpPost("calismaplaniolustur")]
        public async Task<IActionResult> DoktorCalismaPlaniOlustur([FromBody] CalismaPlaniOlusturDTO plan)
        {
            var result = await _ServiceManager.MuayeneService.CalismaPlaniOlusturAsync(plan);
            return Ok(result);
        }
        [HttpPost("randevuolustur")]
        public async Task<IActionResult> RandevuOlusturAsync([FromBody] RandevuOlusturDTO plan)
        {
            var result = await _ServiceManager.MuayeneService.RandevuOlusturAsync(plan);
            return Ok(result);
        }
        [HttpPost("muayeneolustur")]
        public async Task<IActionResult> MuayeneOlusturAsync([FromBody] MuayeneKayitiOlusturDTO muayene)
        {
            var result = await _ServiceManager.MuayeneService.MuayeneKayitiOlustur(muayene);
            return Ok(result);
        }
        [HttpGet("randevularigetir")]
        public async Task <IActionResult> HastaRandevulariniGetir([FromQuery] DateTime baslangic , DateTime bitis)
        {
            var result = await _ServiceManager.MuayeneService.HastaRandevulariniGetir(baslangic , bitis);
            return Ok(result);
        }
        [HttpGet("hastaninrandevusunugetir")]
        public async Task<IActionResult> HastaninRandevulariniGetir([FromQuery(Name = "protokol")] int protokol)
        {
            var result = await _ServiceManager.MuayeneService.HastanınRandevulariniGetir(protokol);
            return Ok(result);
        }
        [HttpPatch("{doktorId:int}/docpasif")]
        public async Task<IActionResult> DoktoruPasifeAl([FromRoute] int doktorId)
        {
            await _ServiceManager.MuayeneService.DoktoruPasifeAl(doktorId);
            return NoContent();
        }
        [HttpPatch("{polIid:int}/polpasif")]
        public async Task<IActionResult> PoluPasifeAl([FromRoute] int polIid)
        {
            await _ServiceManager.MuayeneService.PoluPasifeAl(polIid);
            return NoContent();
        }
        [HttpPost("doktor/{doktorNo}/randevu-hatirlatma-mail")]
        public async Task<IActionResult> RandevuHatirlatmaMailGonder(int doktorNo)
        {
            await _ServiceManager.MuayeneService.DoktorGunlukProgramMailiGonderAsync(doktorNo);

            return Ok(new { mesaj = "Mail başarıyla gönderildi." });
        }
    }
}
