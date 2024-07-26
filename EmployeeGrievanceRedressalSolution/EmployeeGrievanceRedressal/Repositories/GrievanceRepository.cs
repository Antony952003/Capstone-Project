using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces;
using EmployeeGrievanceRedressal.Models;
using EmployeeGrievanceRedressal.Models.DTOs.Grievance;
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

        public async Task<GrievanceDTO> GetGrievanceWithSolver(int id)
        {
            try
            {
                var grievance = await _context.Grievances
                    .Where(g => g.GrievanceId == id)
                    .Select(g => new GrievanceDTO
                    {
                        GrievanceId = g.GrievanceId,
                        EmployeeId = g.EmployeeId,
                        EmployeeName = g.Employee.Name,
                        SolverId = g.SolverId,
                        SolverName = g.Solver != null ? g.Solver.Name : null,
                        Description = g.Description,
                        DateRaised = g.DateRaised,
                        Priority = g.Priority,
                        Type = g.Type.ToString(),
                        Status = g.Status.ToString(),
                        DocumentUrls = g.DocumentUrls.Select(d => d.Url).ToList()
                    })
                    .FirstOrDefaultAsync();

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

        public async Task<List<Grievance>> GetGrievancesWithEmployee()
        {
            try
            {
                var grievances = _context.Grievances
                    .Include(g => g.Employee)
                    .Include(g => g.DocumentUrls)
                    .ToList();

                if (grievances == null)
                {
                    throw new EntityNotFoundException($"No Grievances not found.");
                }

                return grievances;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error getting grievance with histories", ex);
            }
        }
        public async Task<IEnumerable<GrievanceDTO>> GetAllEmployeeGrievancesAsync(int employeeId)
        {
            try
            {
                var employeeExists = await _context.Users.AnyAsync(u => u.UserId == employeeId);

                if (!employeeExists)
                {
                    throw new EntityNotFoundException($"Employee with ID {employeeId} not found.");
                }

                var grievances = await _context.Grievances
                    .Where(g => g.EmployeeId == employeeId)
                    .Select(g => new GrievanceDTO
                    {
                        GrievanceId = g.GrievanceId,
                        EmployeeId = g.EmployeeId,
                        EmployeeName = g.Employee.Name,
                        SolverId = g.SolverId,
                        SolverName = g.Solver != null ? g.Solver.Name : null,
                        Description = g.Description,
                        DateRaised = g.DateRaised,
                        Priority = g.Priority,
                        Type = g.Type.ToString(),
                        Status = g.Status.ToString(),
                        DocumentUrls = g.DocumentUrls.Select(d => d.Url).ToList()
                    })
                    .ToListAsync();

                return grievances;
            }
            catch (EntityNotFoundException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error getting employee grievances", ex);
            }
        }


        public async Task<IEnumerable<GrievanceDTO>> GetAllGrievancesByTypeAsync(string grievanceType)
        {
            try
            {
                if (!Enum.TryParse(grievanceType, true, out GrievanceType parsedGrievanceType))
                {
                    throw new ArgumentException($"Invalid grievance type: {grievanceType}");
                }

                var grievances = await _context.Grievances
                    .Where(g => g.Type == parsedGrievanceType)
                    .Select(g => new GrievanceDTO
                    {
                        GrievanceId = g.GrievanceId,
                        EmployeeId = g.EmployeeId,
                        EmployeeName = g.Employee.Name,
                        SolverId = g.SolverId,
                        SolverName = g.Solver != null ? g.Solver.Name : null,
                        Description = g.Description,
                        DateRaised = g.DateRaised,
                        Priority = g.Priority,
                        Type = g.Type.ToString(),
                        Status = g.Status.ToString(),
                        DocumentUrls = g.DocumentUrls.Select(d => d.Url).ToList()
                    })
                    .ToListAsync();

                if (!grievances.Any())
                {
                    throw new EntityNotFoundException($"No grievances found for type {grievanceType}");
                }

                return grievances;
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
            catch (EntityNotFoundException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error getting grievances by type", ex);
            }
        }

    }

}
