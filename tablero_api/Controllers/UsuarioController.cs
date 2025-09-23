using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tablero_api.Models;
using tablero_api.Models.DTOS;
using tablero_api.Services;
using tablero_api.Services.Interfaces;

namespace tablero_api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UsuarioController(IAuthService authService, IService<Usuario> usuarioService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly IService<Usuario> _usuarioService = usuarioService;


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var usuarios = await _usuarioService.GetAllAsync();
            if (usuarios == null)
                return NotFound("No existen usuarios");

            return Ok(new { usuarios = usuarios});
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var usuario = await _usuarioService.GetByIdAsync(id);
            if (usuario == null)
                return NotFound("No existen usuarios");

            return Ok(new { usuario = usuario });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Usuario request)
        {

            var usuario = await _usuarioService.GetByIdAsync(id);
            if (usuario == null)
                return BadRequest("El ID no coincide");
            var usuarioUpt = new Usuario()
            {   Id_Usuario = id,
                Nombre = request.Nombre,
                Id_Rol = request.Id_Rol,
                UpdatedAt = DateTime.UtcNow,
                Contrasena = request.Contrasena,
            };
            var actualizado = await _usuarioService.UpdateAsync(usuarioUpt);
            return Ok(actualizado);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        { var foundedUsuario = _usuarioService.GetByIdAsync(id);
            if (foundedUsuario == null) return NotFound("Usuario no encontrado");  
            var usuario = _usuarioService.DeleteAsync(id);
            return Ok("Usuario Eliminado");
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
