using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces;
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

        public async Task<User> GetByNameAsync(string name)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Name == name);
        }
        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> RemoveUserById(int id)
        {
            var employee = await _context.Users
        .FirstOrDefaultAsync(e => e.UserId == id);
            var approvalrequestsofemployee = await _context.ApprovalRequests.ToListAsync();
            approvalrequestsofemployee = approvalrequestsofemployee.FindAll(x => x.EmployeeId == id);
            foreach (var item in approvalrequestsofemployee)
            {
                _context.ApprovalRequests.Remove(item);
                _context.SaveChanges();
            }
            _context.Users.Remove(employee);
            _context.SaveChanges();
            return employee;

        }
    }

}
