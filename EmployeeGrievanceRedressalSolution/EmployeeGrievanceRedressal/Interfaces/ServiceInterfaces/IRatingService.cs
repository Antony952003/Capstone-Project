using EmployeeGrievanceRedressal.Models.DTOs.Rating;
using EmployeeGrievanceRedressal.Models;

namespace EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces
{
    public interface IRatingService
    {
        Task<RatingReturnDTO> ProvideRatingAsync(RatingDTO ratingDto);
        Task<IEnumerable<RatingReturnDTO>> GetRatingsBySolverIdAsync(int solverId);
    }

}
