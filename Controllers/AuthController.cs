using EnvaTest.DTO;
using EnvaTest.Result;
using EnvaTest.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace EnvaTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<Result<TokenDTO>>> Login(LoginDTO loginDTO)
        {
            var result = await _authService.LoginAsync(loginDTO);
            return StatusCode(result.StatusCode, result);
        }
    }
} 