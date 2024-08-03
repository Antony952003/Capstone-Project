using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces;
using EmployeeGrievanceRedressal.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeGrievanceRedressal.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly EmployeeGrievanceContext _context;

        public RatingRepository(EmployeeGrievanceContext context)
        {
            _context = context;
        }

        public async Task AddRatingAsync(Rating rating)
        {
            await _context.Ratings.AddAsync(rating);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Rating>> GetRatingsBySolverIdAsync(int solverId)
        {

            var ratings = await _context.Ratings.
            Include(x => x.Grievance)
                .ThenInclude(g => g.Employee)
            .Where(r => r.SolverId == solverId).ToListAsync();

                return ratings;
            
        }
    }

}
