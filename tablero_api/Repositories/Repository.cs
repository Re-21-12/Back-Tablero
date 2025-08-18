using tablero_api.Repositories;
using tablero_api.Data;
using Microsoft.EntityFrameworkCore;
namespace tablero_api.Repositories
{
    // El repositorio es una clase generica que llama a la base de datos
    public class Repository<T> : IRepository<T> where T :class
    {
        //Inyecction de dependencias
        private readonly AppDbContext _context;
        public Repository(AppDbContext context)
        {
            // Verifica que el contexto no sea nulo, si no lanza una excepcion 
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<T?> GetByIdAsync(int id)
        {
            // Busca un elemento por su ID
            return await _context.Set<T>().FindAsync(id);
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            // Devuelve todos los elementos de la tabla
            return await _context.Set<T>().ToListAsync();
        }
        public async Task PostAsync(T entity)
        {
           
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task PutAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {

            var entity = await GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException($"Entidad con ID {id} no encontrada.");
            

            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
