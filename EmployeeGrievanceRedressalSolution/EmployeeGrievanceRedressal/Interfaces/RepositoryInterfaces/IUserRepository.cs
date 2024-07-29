using EmployeeGrievanceRedressal.Models;

namespace EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserWithGrievancesAsync(int id);
        Task<User> GetUserWithFeedbacksAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByNameAsync(string name);
        Task<User> RemoveUserById(int id);
    }
}
