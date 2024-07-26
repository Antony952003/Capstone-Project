namespace EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task Add(T entity);
        void Update(T entity);
        void Remove(T entity);
    }
}
