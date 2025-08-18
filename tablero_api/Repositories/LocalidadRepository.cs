using tablero_api.Data;
using tablero_api.Models;

namespace tablero_api.Repositories
{
    public class LocalidadRepository
    {
        private readonly AppDbContext _context;

        public LocalidadRepository(AppDbContext context)
        {
            _context = context;
        }

        public void AgregarLocalidad(Localidad lc)
        {
            Localidad ll = new Localidad
            {
                
                Nombre = lc.Nombre
            };

            _context.Localidades.Add(ll);
            _context.SaveChanges(); // Guarda en BD
        }

        public void RemoverLocalidad(Localidad lc)
        {
            var localidad = _context.Localidades.Find(lc.id_Localidad);
            if (localidad != null)
            {
                _context.Localidades.Remove(localidad);
                _context.SaveChanges();
            }
        }
    }
}
