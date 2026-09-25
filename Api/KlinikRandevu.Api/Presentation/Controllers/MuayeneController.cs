using Entities.Constants;
using Entities.Data_Transfer_Objects.Muayene;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Presentation.ActionFilters;
using Presentation.Wrappers;
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
   // [EnableRateLimiting("RateLimit")]
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
       // [YetkiKontrol(YetkiKodlari.RandevuAcma)]
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
        [HttpPost("icdara")]
        public async Task<IActionResult> ICDBul([FromBody] string icd)
        {
            var result = await _ServiceManager.IcdApiManager.TaniAraAsync(icd);

            return Ok(result);
        }
        [HttpPatch("randevuiptalet")]
        public async Task<IActionResult> RandevuIptalEt([FromQuery] int id)
        {
            var result = await _ServiceManager.MuayeneService.RandevuIptalAsync(id);
            return Ok("Randevu iptal edilmiştir");
        }
        [HttpPost("randevuluHastalarinBilgileri")]
        public async Task<IActionResult> RandevuluHastalarinBilgileriGetir([FromBody] RandevuluHastaFiltreDto filtre)
        {
            var result = await _ServiceManager.MuayeneService.RandevuluHastaBilgileriniGetir(
                filtre.Basla, filtre.Bitis, filtre.MuayeneOldumu);

            return Ok(result);
        }
        [HttpPost("{muayeneid}/teshisekle")]
        public async Task<IActionResult> MuayeneTeshisEkle([FromRoute] int muayeneid, [FromBody] string teshis)
        {
            var result= await _ServiceManager.MuayeneService.TeshisEkle(muayeneid, teshis);
            return NoContent();
        }
        [HttpPatch("muayenebitis")]
        public async Task<IActionResult> MuayeneKapat([FromQuery] int id)
        {
            var muayene = await _ServiceManager.MuayeneService.MuayeneKapat(id);
            return NoContent();
        }
        [HttpPost("taahütnameEKle")]
        public async Task<IActionResult> TaahütnameEKle([FromBody] TaahütnameEkleDTO taahütname)
        {
            var result= await _ServiceManager.MuayeneService.TaahütnameEkleAsync(taahütname);
            return Ok(ApiResponse<TaahütnameDTO>.SuccessResponse(result, "Tahütname Başarıyla eklendi"));
        }
        [HttpGet("hastaninTaahutnameleriniGetir")]
        public async Task<IActionResult> HastaninTaahutnameleriniGetir([FromQuery] int protokol)
        {
            var result = await _ServiceManager.MuayeneService.HastaninTaahütnameleriniGetirAsync(protokol);
            return Ok(ApiResponse<List<HastaTaahütnameListeDTO>>.SuccessResponse(result));
        }
        [HttpPut("taahutnameGuncelle")]
        public async Task<IActionResult> TaahutnameGuncelle([FromBody] TaahütnameGuncelleDTO model)
        {
            var result = await _ServiceManager.MuayeneService.TaahütnameGuncelleAsync(model);
            return Ok(ApiResponse<TaahütnameGuncelleDTO>.SuccessResponse(result));
        }
        [HttpPatch("taahutnameIptalEt")]
        public async Task<IActionResult> TaahutnameIptalEt([FromQuery] int id)
        {
            await _ServiceManager.MuayeneService.TaahütnameIptalAsync(id);
            return NoContent();
        }
        [HttpPost("odenemeYap")]
        public async Task<IActionResult> OdemeYap([FromBody] OdemeYapDTO odeneme)
        {
            var result = await _ServiceManager.MuayeneService.OdemeYap(odeneme);
            return Ok(ApiResponse<OdemeYapDTO>.SuccessResponse(result));
        }
        [HttpPost("tedaviEkle")]
        public async Task<IActionResult> TedaviEkle([FromBody] TedaviEkleDTO tedavi)
        {
            var result = await _ServiceManager.MuayeneService.MuayeneyeTedaviEKle(tedavi);
            return Ok(result);
        }
        [HttpPatch("OdemeIptal")]
        public async Task<IActionResult> OdemeIptalET([FromBody]OdemeIptalDTO iptal)
        {
            var result = await _ServiceManager.MuayeneService.OdemeIade(iptal);
            return NoContent();
        }
        [HttpPost("DoktorIzınEkle")]
        public async Task<IActionResult> DoktorIzınEkle([FromBody] DoktorIzınOlusturDTO model)
        {
            var result = await _ServiceManager.MuayeneService.DoktorIzınOlusturAsync(model);
            return Ok(ApiResponse<DoktorIzınOlusturDTO>.SuccessResponse(result));
        }
        [HttpPost("CalismaPlaniKopyala")]
        public async Task<IActionResult> CalismaPlaniKopyala([FromBody] CalismaPlaniKopyala model)
        {
            var result =  await _ServiceManager.MuayeneService.CalismaPlaniKopyala(model);
            return Ok(ApiResponse<CalismaPlaniKopyala>.SuccessResponse(result));
        }
        [HttpPost("bransBazliCalismaPlaniKopyala")]
        public async Task<IActionResult> BransBazliCalismaPlaniKopyala([FromBody] CalismaPlaniKopyalaBransBazliDTO model)
        {
            var result = await _ServiceManager.MuayeneService.BransBazliCalismaPlaniKopyalaAsync(model);
            return Ok(ApiResponse<CalismaPlaniKopyalaBransBazliDTO>.SuccessResponse(result));
        }
        [HttpPost("PoliklinikYesilAlanEKle")]
        public async Task<IActionResult> PolYesilAlanAyarEkle([FromBody] PoliklinikYesilAlanAyarlarıEkleDto model)
        {
            var result= await _ServiceManager.MuayeneService.PoliklinikYesilAlanAyariEkle(model);
            return Ok(ApiResponse<PoliklinikYesilAlanAyarlarıEkleDto>.SuccessResponse(result));
        }
        [HttpPost("HastaYesilAlanEKle")]
        public async Task<IActionResult> HastaYesilAlanEKleAsync([FromBody] YesilListeyeHastaEkleDTO model)
        {
            var result= await _ServiceManager.MuayeneService.YesilListeyeHastaEkleAsync(model);
            return Ok(ApiResponse<YesilListeyeHastaEkleDTO>.SuccessResponse(model));
        }
        [HttpPost("doktorEkle")]
        public async Task<IActionResult> DoktorEkle([FromBody] DoktorEkleDTO model)
        {
            var result = await _ServiceManager.MuayeneService.DoktorEkleAsync(model);
            return Ok(ApiResponse<YeniKayitSonucDTO>.SuccessResponse(result, "Doktor başarıyla eklendi"));
        }
        [HttpPost("servisEkle")]
        public async Task<IActionResult> ServisEkle([FromBody] ServisEkleDTO model)
        {
            var result = await _ServiceManager.MuayeneService.ServisEkleAsync(model);
            return Ok(ApiResponse<YeniKayitSonucDTO>.SuccessResponse(result, "Poliklinik başarıyla eklendi"));
        }
        [HttpGet("uzmanlikBranslariniGetir")]
        public async Task<IActionResult> UzmanlikBranslariniGetir()
        {
            var result = await _ServiceManager.MuayeneService.UzmanlikBranslariniGetirAsync();
            return Ok(ApiResponse<List<UzmanlikBransiDTO>>.SuccessResponse(result));
        }
        [HttpGet("doktorListesiGetir")]
        public IActionResult AktifDoktorlariGetir()
        {
            var doktorList = _ServiceManager.MuayeneService.AktifDoktorlariGetir();
            return Ok(ApiResponse<List<AktifDoktorlariGetirDTO>>.SuccessResponse(doktorList));
        }
        [HttpGet("ServisListesiGetir")]
        public IActionResult AktifServisListesiniGetir()
        {
            var servisList = _ServiceManager.MuayeneService.AktifServisleriGetir();
            return Ok(ApiResponse<List<AktifServisListesiGetirDto>>.SuccessResponse(servisList));
        }
        [HttpGet("muayeneGetir")]
        public async Task<IActionResult> MuayeneGetir([FromQuery] int id)
        {
            var result = await _ServiceManager.MuayeneService.MuayeneDetayGetir(id);
            return Ok(ApiResponse<MuayeneKaydiDetayDTO>.SuccessResponse(result));
        }
        [HttpGet("muayeneRandevuIleGetir")]
        public async Task<IActionResult> MuayeneRandevuIleGetir([FromQuery] int randevuId)
        {
            var result = await _ServiceManager.MuayeneService.MuayeneDetayRandevuIleGetir(randevuId);
            return Ok(ApiResponse<MuayeneKaydiDetayDTO>.SuccessResponse(result));
        }
        [HttpGet("teshisleriGetir")]
        public async Task<IActionResult> TeshisleriGetir([FromQuery] int muayeneId)
        {
            var result = await _ServiceManager.MuayeneService.TeshisleriGetir(muayeneId);
            return Ok(ApiResponse<List<teshisler>>.SuccessResponse(result));
        }
        [HttpGet("tedavileriGetir")]
        public async Task<IActionResult> TedavileriGetir([FromQuery] int muayeneId)
        {
            var result = await _ServiceManager.MuayeneService.TedavileriGetir(muayeneId);
            return Ok(ApiResponse<List<TedaviKaydi>>.SuccessResponse(result));
        }
        [HttpGet("odemeleriGetir")]
        public async Task<IActionResult> OdemeleriGetir([FromQuery] int muayeneId)
        {
            var result = await _ServiceManager.MuayeneService.OdemeleriGetir(muayeneId);
            return Ok(ApiResponse<List<odeme>>.SuccessResponse(result));
        }
        [HttpGet("muayeneBorcGetir")]
        public async Task<IActionResult> MuayeneBorcGetir([FromQuery] int muayeneId)
        {
            var result = await _ServiceManager.MuayeneService.MuayeneBorcGetir(muayeneId);
            return Ok(ApiResponse<double>.SuccessResponse(result));
        }
        [HttpGet("muayeneOdemeToplamGetir")]
        public async Task<IActionResult> MuayeneOdemeToplamGetir([FromQuery] int muayeneId)
        {
            var result = await _ServiceManager.MuayeneService.MuayeneOdemeToplamGetir(muayeneId);
            return Ok(ApiResponse<double>.SuccessResponse(result));
        }
        [HttpGet("poliklinikHastaListesiGetir")]
        public async Task<IActionResult> PoliklinikHastaListesiGetir([FromQuery] int polNo, [FromQuery] DateTime baslangic, [FromQuery] DateTime bitis)
        {
            var result = await _ServiceManager.MuayeneService.PoliklinikHastaListesiGetir(polNo, baslangic, bitis);
            return Ok(ApiResponse<List<PoliklinikHastaListesiDTO>>.SuccessResponse(result));
        }
    }
}
