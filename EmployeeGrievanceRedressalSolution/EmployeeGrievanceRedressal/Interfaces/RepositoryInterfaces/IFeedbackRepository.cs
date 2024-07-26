using EmployeeGrievanceRedressal.Models;

namespace EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces
{
    public interface IFeedbackRepository : IRepository<Feedback>
    {
        Task<Feedback> GetFeedbackWithSolutionAsync(int id);
    }
}
