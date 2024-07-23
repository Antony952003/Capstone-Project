using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces;
using EmployeeGrievanceRedressal.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeGrievanceRedressal.Repositories
{
    public class GrievanceRepository : Repository<Grievance>, IGrievanceRepository
    {
        public GrievanceRepository(EmployeeGrievanceContext context) : base(context) { }

        public async Task<Grievance> GetGrievanceWithSolutionsAsync(int id)
        {
            try
            {
                var grievance = await _context.Grievances
                    .Include(g => g.Solutions)
                    .FirstOrDefaultAsync(g => g.GrievanceId == id);

                if (grievance == null)
                {
                    throw new EntityNotFoundException($"Grievance with ID {id} not found.");
                }

                return grievance;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error getting grievance with solutions", ex);
            }
        }

        public async Task<Grievance> GetGrievanceWithHistoriesAsync(int id)
        {
            try
            {
                var grievance = await _context.Grievances
                    .Include(g => g.GrievanceHistories)
                    .FirstOrDefaultAsync(g => g.GrievanceId == id);

                if (grievance == null)
                {
                    throw new EntityNotFoundException($"Grievance with ID {id} not found.");
                }

                return grievance;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error getting grievance with histories", ex);
            }
        }
    }

}
