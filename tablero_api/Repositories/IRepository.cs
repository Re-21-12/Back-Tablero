namespace tablero_api.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task PostAsync(T entity);
        Task PutAsync(T entity);
        Task DeleteAsync(int id);
    }

}
