using Entities.Data_Transfer_Objects.Parametre;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Route("SistemParametreleri")]
    [ApiController]
    public class ParametreController:ControllerBase
    {
        private readonly IServiceManager _ServiceManager;

        public ParametreController(IServiceManager ıserviceManager)
        {
            _ServiceManager=ıserviceManager;
        }
        [HttpPost("parametreekle")]
        public async Task<IActionResult> ParamtreEkle([FromBody] ParametreEkleDTO param)
        {
            var parametre =  await _ServiceManager.SistemParametreService.ParametreEkleAsync(param);
            return NoContent();
        }
        [HttpPatch("parametreguncelle")]
        public async Task<IActionResult> ParamtreGuncelle([FromRoute] int id,[FromBody] ParametreEkleDTO param)
        {
            var parametre = await _ServiceManager.SistemParametreService.ParametreGuncelle(param,id);
            return NoContent();
        }
    }
}
