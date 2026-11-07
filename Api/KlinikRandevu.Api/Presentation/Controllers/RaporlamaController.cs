using Entities.Data_Transfer_Objects.Raporlama;
using Microsoft.AspNetCore.Mvc;
using Presentation.Wrappers;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/Raporlama")]
    // [EnableRateLimiting("RateLimit")]
    public class RaporlamaController:ControllerBase
    {
        private readonly IServiceManager _ServiceManager;

        public RaporlamaController(IServiceManager ıserviceManager)
        {
            _ServiceManager=ıserviceManager;
        }
        [HttpGet("uzmanlikbazlihastagelisleri")]
        public async Task<IActionResult> UzmanlikBazliHastaGelisleri([FromQuery] DateTime baslangic, [FromQuery] DateTime bitis)
        {
            var result= await _ServiceManager.RaporlamaService.UzmanlikBazliHastaBasvuru(baslangic, bitis);
            return Ok(ApiResponse<List<UzamanlikBazliRaporDto?>>.SuccessResponse(result));
        }
        [HttpGet("teshisraporu")]
        public async Task<IActionResult> TeshisRaporu([FromQuery] DateTime baslangic, [FromQuery] DateTime bitis)
        {
            var result = await _ServiceManager.RaporlamaService.TeshisRaporu(baslangic, bitis);
            return Ok(ApiResponse<List<TeshisRaporDto>>.SuccessResponse(result));
        }
    }
}
