using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using tablero_api.Models.DTOS;
using tablero_api.Services.Interfaces;

namespace tablero_api.Controllers
{
    [ApiController]
    [Route("api/email")]
    public class CorreoController : ControllerBase
    {
        private readonly IMailerClient _mailer;
        public CorreoController(IMailerClient mailer) => _mailer = mailer;

        //  POST /api/email/send
        [HttpPost("send")]
        public async Task<ActionResult<EmailResponse>> Send([FromBody] SendEmailRequest req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.To) ||
                string.IsNullOrWhiteSpace(req.Subject) ||
                string.IsNullOrWhiteSpace(req.Body))
                return BadRequest(new EmailResponse { Success = false, Message = "Campos requeridos: to, subject, body" });

            var result = await _mailer.SendAsync(req, ct);
            return Ok(result);
        }

        //  POST /api/email/drafts
        [HttpPost("drafts")]
        public async Task<ActionResult<object>> CreateDraft([FromBody] SendEmailRequest req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.To) ||
                string.IsNullOrWhiteSpace(req.Subject) ||
                string.IsNullOrWhiteSpace(req.Body))
                return BadRequest(new { success = false, error = "Campos requeridos: to, subject, body" });

            var id = await _mailer.CreateDraftAsync(req, ct);
            return Ok(new { success = true, id });
        }

        //  PUT /api/email/drafts/{id}
        [HttpPut("drafts/{id:long}")]
        public async Task<ActionResult<object>> UpdateDraft(long id, [FromBody] SendEmailRequest req, CancellationToken ct)
        {
            if (id <= 0)
                return BadRequest(new { success = false, error = "Id inválido" });

            var ok = await _mailer.UpdateDraftAsync(id, req, ct);
            return ok ? Ok(new { success = true })
                      : BadRequest(new { success = false, error = "No se pudo actualizar el borrador (¿no está en estado queued?)." });
        }

        //  GET /api/email
        [HttpGet]
        public async Task<ActionResult<List<EmailItem>>> List(
            [FromQuery] string? status,
            [FromQuery] int limit = 50,
            [FromQuery] int offset = 0,
            CancellationToken ct = default)
        {
            var items = await _mailer.ListEmailsAsync(status, limit, offset, ct);
            return Ok(items);
        }

        //  GET /api/email/{id}
        [HttpGet("{id:long}")]
        public async Task<ActionResult<EmailItem>> GetById(long id, CancellationToken ct)
        {
            var item = await _mailer.GetEmailByIdAsync(id, ct);
            if (item is null) return NotFound();
            return Ok(item);
        }

        //  DELETE /api/email/{id}
        [HttpDelete("{id:long}")]
        public async Task<ActionResult> DeleteById(long id, CancellationToken ct)
        {
            var ok = await _mailer.DeleteEmailAsync(id, ct);
            return ok ? Ok(new { success = true }) : BadRequest(new { success = false });
        }
    }
}