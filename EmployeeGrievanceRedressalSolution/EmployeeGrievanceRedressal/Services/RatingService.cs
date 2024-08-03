using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces;
using EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces;
using EmployeeGrievanceRedressal.Models.DTOs.Rating;
using EmployeeGrievanceRedressal.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressal.Services
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGrievanceRepository _grievanceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RatingService(IRatingRepository ratingRepository, IUserRepository userRepository, IGrievanceRepository grievanceRepository, IHttpContextAccessor httpContextAccessor)
        {
            _ratingRepository = ratingRepository;
            _userRepository = userRepository;
            _grievanceRepository = grievanceRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<RatingReturnDTO> ProvideRatingAsync(RatingDTO ratingDto)
        {
            var grievance = await _grievanceRepository.GetByIdAsync(ratingDto.GrievanceId);
            if (grievance == null)
            {
                throw new EntityNotFoundException($"Grievance with ID {ratingDto.GrievanceId} not found.");
            }

            var solver = await _userRepository.GetByIdAsync(ratingDto.SolverId);
            var employee = await _userRepository.GetByIdAsync(grievance.EmployeeId);
            if (solver == null || solver.Role != UserRole.Solver)
            {
                throw new EntityNotFoundException($"Solver with ID {ratingDto.SolverId} not found.");
            }

            var rating = new Rating
            {
               Comment = ratingDto.Comment,
               DateProvided = DateTime.UtcNow,
               GrievanceId = ratingDto.GrievanceId,
               RatingValue = ratingDto.RatingValue,
               SolverId = ratingDto.SolverId,
               
            };

            await _ratingRepository.AddRatingAsync(rating);

            // Update solver's average rating
            solver.AverageRating = await CalculateAverageRatingAsync(solver.UserId);
            _userRepository.Update(solver);

            // Return the rating as DTO
            return new RatingReturnDTO
            {
               Comment = rating.Comment,
               EmployeeImage =employee.UserImage,
               EmployeeName = employee.Name,
               GrievanceTitle = grievance.Title,
               Rating = rating.RatingValue
            };
        }

        private async Task<decimal> CalculateAverageRatingAsync(int solverId)
        {
            var ratings = await _ratingRepository.GetRatingsBySolverIdAsync(solverId);

            if (!ratings.Any())
            {
                return 0;
            }

            double averageRating = ratings.Average(r => (double)r.RatingValue);
            return (decimal)averageRating;
        }

        public async Task<IEnumerable<RatingReturnDTO>> GetRatingsBySolverIdAsync(int solverId)
        {
            var ratings = await _ratingRepository.GetRatingsBySolverIdAsync(solverId);
            return ratings.Select(r => new RatingReturnDTO
            {
                Comment = r.Comment,
                EmployeeImage = r.Grievance.Employee.UserImage,
                EmployeeName = r.Grievance.Employee.Name,
                GrievanceTitle = r.Grievance.Title,
                Rating = r.RatingValue,
            });
        }
    }
}
