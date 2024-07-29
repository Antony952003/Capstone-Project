using EmployeeGrievanceRedressal.Models.DTOs.Rating;
using EmployeeGrievanceRedressal.Models;

namespace EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces
{
    public interface IRatingService
    {
        Task<RatingDTO> ProvideRatingAsync(RatingDTO ratingDto);
        Task<IEnumerable<RatingDTO>> GetRatingsBySolverIdAsync(int solverId);
    }

}
