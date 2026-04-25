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
        
    }
}
