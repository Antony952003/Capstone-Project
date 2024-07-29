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

        public RatingService(IRatingRepository ratingRepository, IUserRepository userRepository, IGrievanceRepository grievanceRepository)
        {
            _ratingRepository = ratingRepository;
            _userRepository = userRepository;
            _grievanceRepository = grievanceRepository;
        }

        public async Task<RatingDTO> ProvideRatingAsync(RatingDTO ratingDto)
        {
            var grievance = await _grievanceRepository.GetByIdAsync(ratingDto.GrievanceId);
            if (grievance == null)
            {
                throw new EntityNotFoundException($"Grievance with ID {ratingDto.GrievanceId} not found.");
            }

            var solver = await _userRepository.GetByIdAsync(ratingDto.SolverId);
            if (solver == null || solver.Role != UserRole.Solver)
            {
                throw new EntityNotFoundException($"Solver with ID {ratingDto.SolverId} not found.");
            }

            var rating = new Rating
            {
                GrievanceId = ratingDto.GrievanceId,
                SolverId = ratingDto.SolverId,
                RatingValue = ratingDto.RatingValue,
                Comment = ratingDto.Comment,
                DateProvided = DateTime.UtcNow,
            };

            await _ratingRepository.AddRatingAsync(rating);

            // Update solver's average rating
            solver.AverageRating = await CalculateAverageRatingAsync(solver.UserId);
            _userRepository.Update(solver);

            // Return the rating as DTO
            return new RatingDTO
            {
                RatingId = rating.RatingId,
                GrievanceId = rating.GrievanceId,
                SolverId = rating.SolverId,
                RatingValue = rating.RatingValue,
                Comment = rating.Comment,
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

        public async Task<IEnumerable<RatingDTO>> GetRatingsBySolverIdAsync(int solverId)
        {
            var ratings = await _ratingRepository.GetRatingsBySolverIdAsync(solverId);
            return ratings.Select(r => new RatingDTO
            {
                RatingId = r.RatingId,
                GrievanceId = r.GrievanceId,
                SolverId = r.SolverId,
                RatingValue = r.RatingValue,
                Comment = r.Comment,
            });
        }
    }
}
