using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tablero_api.Data;
using tablero_api.Models;
using tablero_api.Models;
using tablero_api.Models.DTOS;
using tablero_api.Services.Interfaces;

namespace tablero_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailsController(IMailerServiceClient mailer, AppDbContext db) : ControllerBase
    {
        // Crea y envía en el mismo paso
        // POST /api/emails
        [HttpPost]
        [AllowAnonymous] // quítalo si exiges JWT
        public async Task<IActionResult> CreateAndSend([FromBody] EmailRequestDto dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(dto.To) ||
                string.IsNullOrWhiteSpace(dto.Subject) ||
                string.IsNullOrWhiteSpace(dto.Body))
                return BadRequest(new { success = false, message = "Campos requeridos: to, subject, body" });

            var entity = new EmailMessage
            {
                To = dto.To,
                Subject = dto.Subject,
                Body = dto.Body,
                Status = EmailStatus.Draft
            };

            db.Emails.Add(entity);
            await db.SaveChangesAsync(ct);

            var res = await mailer.SendAsync(new MailerSendRequest
            {
                To = dto.To,
                Subject = dto.Subject,
                Body = dto.Body
            }, ct);

            if (res.Success)
            {
                entity.Status = EmailStatus.Sent;
                entity.SentAt = DateTime.UtcNow;
                entity.Error = null;
                await db.SaveChangesAsync(ct);
                return Ok(new { success = true, id = entity.Id, message = "Correo enviado" });
            }
            else
            {
                entity.Status = EmailStatus.Failed;
                entity.Error = res.Error ?? res.Message;
                await db.SaveChangesAsync(ct);
                return StatusCode(502, new { success = false, id = entity.Id, message = "Fallo al enviar", error = entity.Error });
            }
        }

        // Crea borrador (no envía)
        // POST /api/emails/draft
        [HttpPost("draft")]
        public async Task<IActionResult> CreateDraft([FromBody] EmailRequestDto dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(dto.To) ||
                string.IsNullOrWhiteSpace(dto.Subject) ||
                string.IsNullOrWhiteSpace(dto.Body))
                return BadRequest(new { success = false, message = "Campos requeridos: to, subject, body" });

            var entity = new EmailMessage { To = dto.To, Subject = dto.Subject, Body = dto.Body, Status = EmailStatus.Draft };
            db.Emails.Add(entity);
            await db.SaveChangesAsync(ct);
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity);
        }

        // Lista paginada
        // GET /api/emails?page=1&size=20
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int size = 20, CancellationToken ct = default)
        {
            page = page <= 0 ? 1 : page;
            size = size <= 0 ? 20 : size;

            var q = db.Emails.AsNoTracking().OrderByDescending(e => e.CreatedAt);
            var total = await q.CountAsync(ct);
            var items = await q.Skip((page - 1) * size).Take(size).ToListAsync(ct);
            return Ok(new { total, page, size, items });
        }

        // Detalle
        // GET /api/emails/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id, CancellationToken ct)
        {
            var e = await db.Emails.FindAsync([id], ct);
            return e is null ? NotFound() : Ok(e);
        }

        // Editar borrador
        // PUT /api/emails/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateDraft(int id, [FromBody] EmailRequestDto dto, CancellationToken ct)
        {
            var e = await db.Emails.FindAsync([id], ct);
            if (e is null) return NotFound();
            if (e.Status != EmailStatus.Draft) return BadRequest(new { message = "Solo se pueden editar borradores" });

            if (string.IsNullOrWhiteSpace(dto.To) ||
                string.IsNullOrWhiteSpace(dto.Subject) ||
                string.IsNullOrWhiteSpace(dto.Body))
                return BadRequest(new { success = false, message = "Campos requeridos: to, subject, body" });

            e.To = dto.To;
            e.Subject = dto.Subject;
            e.Body = dto.Body;

            await db.SaveChangesAsync(ct);
            return Ok(e);
        }

        // Enviar un borrador
        // POST /api/emails/{id}/send
        [HttpPost("{id:int}/send")]
        public async Task<IActionResult> SendDraft(int id, CancellationToken ct)
        {
            var e = await db.Emails.FindAsync([id], ct);
            if (e is null) return NotFound();
            if (e.Status != EmailStatus.Draft) return BadRequest(new { message = "No es borrador" });

            var res = await mailer.SendAsync(new MailerSendRequest
            {
                To = e.To,
                Subject = e.Subject,
                Body = e.Body
            }, ct);

            if (res.Success)
            {
                e.Status = EmailStatus.Sent;
                e.SentAt = DateTime.UtcNow;
                e.Error = null;
                await db.SaveChangesAsync(ct);
                return Ok(new { success = true });
            }

            e.Status = EmailStatus.Failed;
            e.Error = res.Error ?? res.Message;
            await db.SaveChangesAsync(ct);
            return StatusCode(502, new { success = false, error = e.Error });
        }

        // Eliminar
        // DELETE /api/emails/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var e = await db.Emails.FindAsync([id], ct);
            if (e is null) return NotFound();
            db.Emails.Remove(e);
            await db.SaveChangesAsync(ct);
            return NoContent();
        }
    }
}