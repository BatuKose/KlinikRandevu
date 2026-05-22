using Entities.Data_Transfer_Objects.Authentication;
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
    [Route("Authentication")]
    // [EnableRateLimiting("RateLimit")]
    public class AuthenticationController:ControllerBase
    {
        private readonly IServiceManager _ServiceManager;

        public AuthenticationController(IServiceManager ıserviceManager)
        {
            _ServiceManager=ıserviceManager;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _ServiceManager.AuthenticationService.login(login);
            return Ok(result);
        }
    }
}
