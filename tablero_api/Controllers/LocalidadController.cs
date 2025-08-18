using Microsoft.AspNetCore.Mvc;
using tablero_api.Models;
using tablero_api.Repositories;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace tablero_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalidadController : ControllerBase
    {
        private readonly LocalidadRepository ct;

        public LocalidadController(LocalidadRepository repo)
        {
            ct = repo;
        }
        // GET: api/<LocalidadController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<LocalidadController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<LocalidadController>
        [HttpPost]
        public IActionResult Post(Localidad lc)
        {
            //Localidad localidad = (Localidad)lrequest;
            ct.AgregarLocalidad(lc);
            return Ok("Localidad agregada");
        }

        // PUT api/<LocalidadController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LocalidadController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
