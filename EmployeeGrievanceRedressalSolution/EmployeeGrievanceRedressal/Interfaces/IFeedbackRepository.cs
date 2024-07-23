using EmployeeGrievanceRedressal.Models;

namespace EmployeeGrievanceRedressal.Interfaces
{
    public interface IFeedbackRepository : IRepository<Feedback>
    {
        Task<Feedback> GetFeedbackWithSolutionAsync(int id);
    }
}
