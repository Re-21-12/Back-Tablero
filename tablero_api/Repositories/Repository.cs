using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using tablero_api.Data;
using tablero_api.Models;
using tablero_api.Repositories.Interfaces;
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
        public async Task<IEnumerable<T>> GetByTwoParameters(int firstParam, int secondParam)
        {
            if (typeof(T) != typeof(Cuarto))
                throw new NotSupportedException("Este método solo es válido para Cuarto.");

            var result = await _context.Cuartos
                .Where(c => c.id_Partido == firstParam && c.id_Equipo == secondParam)
                .ToListAsync();

            return (IEnumerable<T>)result;
        }
        public async Task<T?> GetByPredicateAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public async Task<List<T>> GetValuePerPage(int pageNumber, int pageSize)
        {
          
            return await _context.Set<T>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        }
    }
}
