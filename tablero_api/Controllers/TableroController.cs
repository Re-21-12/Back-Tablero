using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tablero_api.Models;
using tablero_api.Repositories;

namespace tablero_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableroController : ControllerBase
    {
        private readonly TableroRepository ct;
        [HttpPost]
        public IActionResult Post(List<Cuarto> cuartos, Partido pt, Equipo local,Equipo visitante, Localidad lc)
        {
            pt.localidad = lc;
            pt.Local = local;
            pt.Visitante = visitante;
            ct.EnviarPartido(pt);
            foreach (Cuarto c in cuartos)
            {
                c.Partido = pt;
                if (c.duenio == "v") {
                    c.Equipo = visitante;
                }
                else
                {
                    c.Equipo = local;
                }
                ct.EnviarCuarto(c);
            }
            
           

            return Ok("nada");
        }
    }
}
