using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces;
using EmployeeGrievanceRedressal.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeGrievanceRedressal.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(EmployeeGrievanceContext context) : base(context) { }

        public async Task<User> GetUserWithGrievancesAsync(int id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.RaisedGrievances)
                    .FirstOrDefaultAsync(u => u.UserId == id);

                if (user == null)
                {
                    throw new EntityNotFoundException($"User with ID {id} not found.");
                }

                return user;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error getting user with grievances", ex);
            }
        }

        public async Task<User> GetUserWithFeedbacksAsync(int id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Feedbacks)
                    .FirstOrDefaultAsync(u => u.UserId == id);

                if (user == null)
                {
                    throw new EntityNotFoundException($"User with ID {id} not found.");
                }

                return user;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error getting user with feedbacks", ex);
            }
        }
    }

}
