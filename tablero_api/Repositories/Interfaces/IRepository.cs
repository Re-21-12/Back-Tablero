using System.Linq.Expressions;

namespace tablero_api.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task PostAsync(T entity);
        Task PutAsync(T entity);
        Task DeleteAsync(int id);
        Task<IEnumerable<T>> GetByTwoParameters(int firstParam, int twoParam);
        Task<T?> GetByPredicateAsync(Expression<Func<T, bool>> predicate);

    }

}
