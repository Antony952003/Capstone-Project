using EmployeeGrievanceRedressal.Models;

namespace EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces
{
    public interface IRatingRepository
    {
        Task AddRatingAsync(Rating rating);
        Task<IEnumerable<Rating>> GetRatingsBySolverIdAsync(int solverId);
    }
}
