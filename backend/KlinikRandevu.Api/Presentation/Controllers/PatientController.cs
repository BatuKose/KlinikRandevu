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
        [HttpPost]
        public async Task<IActionResult> InsertPatientAsync([FromBody] CreatePatientDto patient)
        {
            await _ServiceManager.PatientService.CreatePatientAsync(patient);
            return NoContent();
        }
    }
}
