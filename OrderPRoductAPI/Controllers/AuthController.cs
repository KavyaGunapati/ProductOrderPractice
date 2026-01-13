using Interfaces.IManager;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;

namespace OrderPRoductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        public AuthController(IAuthManager authManager)
        {
            _authManager = authManager;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register register)
        {
            var result=await _authManager.RegisterAsync(register);
            if(result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            var result=await _authManager.LoginAsync(login);
            if(result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}