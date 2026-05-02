using Entities.Data_Transfer_Objects.Patient;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/Patient")]
    public class PatientController: ControllerBase
    {
        private readonly IServiceManager _ServiceManager;

        public PatientController(IServiceManager ıserviceManager)
        {
            _ServiceManager=ıserviceManager;
        }
 
        [HttpPost("hastakayit")]
        public async Task<IActionResult> InsertPatientAsync([FromBody] CreatePatientDto patient)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _ServiceManager.PatientService.CreatePatientAsync(patient);
            return NoContent();
        }
        [HttpGet("hastakayithastagetir")]
        public async Task<List<GetPatientDTO>> GetPatient([FromQuery] string arama)
        {
            var result= await _ServiceManager.PatientService.getPatientAsync(arama);
            return result;
        }
        [HttpPut("hastakayithastagüncelle")]
        public async Task<IActionResult> UpdatePatientAsync([FromBody] UpdatePatientDTO patient,[FromQuery]int protokol)
        {
            var result= await _ServiceManager.PatientService.UpdatePatient(patient,protokol);
            return Ok(result);
        }
        [HttpPatch("hastakayithastasil")]
        public async Task<IActionResult> DeletePatientAsync([FromQuery] int protkol)
        {
            await _ServiceManager.PatientService.DeletePatient(protkol);
            return NoContent();
        }
    }
}
