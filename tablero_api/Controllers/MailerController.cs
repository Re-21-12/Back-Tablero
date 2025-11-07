using Microsoft.AspNetCore.Mvc;
using tablero_api.Services;
using tablero_api.DTOs;

namespace tablero_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MailerController : ControllerBase
    {
        private readonly MailerService _mailerService;

        public MailerController(MailerService mailerService)
        {
            _mailerService = mailerService;
        }

        // ============================================================
        // 📬 CORREOS
        // ============================================================

        /// <summary>
        /// Envía un nuevo correo electrónico.
        /// </summary>
        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _mailerService.SendEmailAsync(dto);
                return Ok(new { success = true, data = result });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(503, new
                {
                    success = false,
                    message = "No se pudo conectar al microservicio de correos",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error enviando correo",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene la lista paginada de correos.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllEmails([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _mailerService.GetPaginatedEmailsAsync(page, pageSize);
                return Ok(new
                {
                    success = true,
                    page = result.Page,
                    pageSize = result.PageSize,
                    total = result.TotalCount,
                    data = result.Items
                });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(503, new
                {
                    success = false,
                    message = "Microservicio no disponible",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error obteniendo correos",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene un correo por su ID.
        /// </summary>
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetEmailById(long id)
        {
            try
            {
                var email = await _mailerService.GetEmailByIdAsync(id);

                if (email == null)
                    return NotFound(new { success = false, message = $"Correo con ID {id} no encontrado" });

                return Ok(new { success = true, data = email });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(503, new
                {
                    success = false,
                    message = "Microservicio no disponible",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error obteniendo correo",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Elimina un correo existente.
        /// </summary>
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteEmail(long id)
        {
            try
            {
                var deleted = await _mailerService.DeleteEmailAsync(id);
                if (!deleted)
                    return NotFound(new { success = false, message = $"Correo con ID {id} no encontrado" });

                return Ok(new { success = true, message = "Correo eliminado correctamente" });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(503, new
                {
                    success = false,
                    message = "Microservicio no disponible",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error eliminando correo",
                    error = ex.Message
                });
            }
        }

        // ============================================================
        // 🧩 PLANTILLAS
        // ============================================================

        /// <summary>
        /// Crea una nueva plantilla de correo.
        /// </summary>
        [HttpPost("templates")]
        public async Task<IActionResult> CreateTemplate([FromBody] MailTemplateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _mailerService.CreateTemplateAsync(dto);
                return Ok(new { success = true, data = result });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(503, new
                {
                    success = false,
                    message = "Microservicio no disponible",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error creando plantilla",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Actualiza una plantilla existente.
        /// </summary>
        [HttpPut("templates/{id:int}")]
        public async Task<IActionResult> UpdateTemplate(int id, [FromBody] MailTemplateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _mailerService.UpdateTemplateAsync(id, dto);
                return Ok(new { success = true, data = result });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(503, new
                {
                    success = false,
                    message = "Microservicio no disponible",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error actualizando plantilla",
                    error = ex.Message
                });
            }
        }
        [HttpGet("templates")]
        public async Task<IActionResult> GetAllTemplates()
        {
            try
            {
                var response = await _mailerService.GetAllTemplatesAsync();
                return Ok(response);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(503, new
                {
                    success = false,
                    message = "Microservicio no disponible",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error obteniendo plantillas",
                    error = ex.Message
                });
            }
        }

            /// <summary>
            /// Elimina una plantilla existente.
            /// </summary>
            [HttpDelete("templates/{id:int}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            try
            {
                var deleted = await _mailerService.DeleteTemplateAsync(id);
                if (!deleted)
                    return NotFound(new { success = false, message = $"Plantilla con ID {id} no encontrada" });

                return Ok(new { success = true, message = "Plantilla eliminada correctamente" });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(503, new
                {
                    success = false,
                    message = "Microservicio no disponible",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error eliminando plantilla",
                    error = ex.Message
                });
            }
        }
    }
}
