using System;
using System.Linq;
using System.Threading.Tasks;
using tablero_api.Models;

namespace tablero_api.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            // Roles to ensure
            var roles = new[] { "Admin", "Localidad", "Equipo", "Partido", "Jugador", "Cuarto", "Imagen", "Usuario", "Rol", "Permiso", "Cliente" };
            foreach (var r in roles)
            {
                if (!db.Roles.Any(x => x.Nombre == r))
                {
                    db.Roles.Add(new Rol { Nombre = r, CreatedAt = DateTime.UtcNow, CreatedBy = 0 });
                }
            }
            await db.SaveChangesAsync();

            // Permisos (idempotente)
            var permisosToEnsure = new (string Nombre, string RolName)[] {
                ("Localidad:Agregar","Admin"),("Localidad:Editar","Admin"),("Localidad:Eliminar","Admin"),("Localidad:Consultar","Admin"),
                ("Localidad:Consultar","Cliente"),("Equipo:Consultar","Cliente"),("Partido:Consultar","Cliente"),
                ("Jugador:Consultar","Cliente"),("Cuarto:Consultar","Cliente"),("Imagen:Consultar","Cliente")
            };

            foreach (var (permNombre, rolNombre) in permisosToEnsure)
            {
                var rol = db.Roles.FirstOrDefault(r => r.Nombre == rolNombre);
                if (rol == null) continue;

                var exists = db.Permisos.Any(p => p.Nombre == permNombre && p.Id_Rol == rol.Id_Rol);
                if (!exists)
                {
                    db.Permisos.Add(new Permiso { Nombre = permNombre, Id_Rol = rol.Id_Rol, CreatedAt = DateTime.UtcNow, CreatedBy = 0 });
                }
            }

            await db.SaveChangesAsync();
        }
    }
}