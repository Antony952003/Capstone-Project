using EmployeeGrievanceRedressal.Models;

namespace EmployeeGrievanceRedressal.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserWithGrievancesAsync(int id);
        Task<User> GetUserWithFeedbacksAsync(int id);
    }
}
