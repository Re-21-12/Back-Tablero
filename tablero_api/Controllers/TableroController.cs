using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tablero_api.Data;
using tablero_api.Models;
using tablero_api.Models.DTOS;
using tablero_api.Repositories;

namespace tablero_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableroController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TableroController(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateTableroDto dto)

        {
            if (dto == null) return BadRequest("Body invalido");
            if (dto.Partido == null) return BadRequest("Partido es invalido");
            if (dto.Local == null) return BadRequest("El equipo local es invalido");
            if (dto.Visitante == null) return BadRequest("El equipo visitante es invalido");
            if (dto.Localidad == null) return BadRequest("Localidad es invalido");

            if (dto == null)
                return BadRequest("El request está vacío");


            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var local = await _context.Equipos.FindAsync(dto.Partido.id_Local);
                var visitante = await _context.Equipos.FindAsync(dto.Partido.id_Visitante);
                var localidad = await _context.Localidades.FindAsync(dto.Partido.id_Localidad);
                if (local == null || visitante == null|| localidad == null)
                    throw new InvalidOperationException("Equipos o localidades no encontrados");
                // Crear partido y asignar relaciones
                var partido = new Partido
                {
                    FechaHora = dto.Partido.FechaHora,
                    Local = local,
                    Visitante = visitante,
                    localidad = localidad
                };

                _context.Partidos.Add(partido);

                // Crear cuartos usando los mismos objetos que EF ya está trackeando
                
                var cuartos = dto.Cuartos.Select(c =>
                {
                    if (c.id_Cuarto != 0)
                        throw new InvalidOperationException("Datos del cuarto invalidos");

                    if (c.Total_Punteo < 0 || c.Total_Faltas < 0)
                    {
                        throw new InvalidDataException("Puntaje o faltas invalido");
                    }
                    return new Cuarto
                    {
                        No_Cuarto = c.No_Cuarto,
                        duenio = c.duenio,
                        Total_Punteo = c.Total_Punteo,
                        
                        Total_Faltas = c.Total_Faltas,
                        
                        Partido = partido,
                        Equipo = c.duenio.ToLower() switch
                        {
                            "v" => visitante,
                            "l" => local,
                            _ => throw new InvalidOperationException("Dueño invalido")
                        }
                    };
                    
                }).ToList();


                _context.Cuartos.AddRange(cuartos);

                await _context.SaveChangesAsync();

                //Calculo de Ganador

                return Ok(new
                {
                    PartidoId = partido.id_Partido,
                    CuartosGuardados = cuartos.Count
                });
            }
            catch(Exception ex)
            {
                // Si hay cualquier error, deshacer transacción
                await transaction.RollbackAsync();
                return BadRequest(new { error = ex.Message });
            }
        }
       
    }
}
