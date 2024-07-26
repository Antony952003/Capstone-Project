using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces;
using EmployeeGrievanceRedressal.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeGrievanceRedressal.Repositories
{
    public class FeedbackRepository : Repository<Feedback>, IFeedbackRepository
    {
        public FeedbackRepository(EmployeeGrievanceContext context) : base(context) { }

        public async Task<Feedback> GetFeedbackWithSolutionAsync(int id)
        {
            try
            {
                var feedback = await _context.Feedbacks
                    .Include(f => f.Solution)
                    .FirstOrDefaultAsync(f => f.FeedbackId == id);

                if (feedback == null)
                {
                    throw new EntityNotFoundException($"Feedback with ID {id} not found.");
                }

                return feedback;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error getting feedback with solution", ex);
            }
        }
    }

}
