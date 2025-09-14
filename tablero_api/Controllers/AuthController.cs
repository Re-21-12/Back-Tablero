using Microsoft.AspNetCore.Mvc;
using tablero_api.Models.DTOS;
using tablero_api.Services.Interfaces;

namespace tablero_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _authService.AuthenticateAsync(request);
            if (response == null)
                return Unauthorized("Invalid username or password");

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.RegisterAsync(request);
            if (result == null)
                return BadRequest("El usuario ya se encuentra registro o el nombre del perfil no es correcto");

            return Ok(new { message = result });
        }

    }
}
