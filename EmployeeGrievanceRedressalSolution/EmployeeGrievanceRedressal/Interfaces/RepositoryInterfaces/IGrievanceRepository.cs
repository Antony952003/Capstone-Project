using EmployeeGrievanceRedressal.Models;
using EmployeeGrievanceRedressal.Models.DTOs.Grievance;

namespace EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces
{
    public interface IGrievanceRepository : IRepository<Grievance>
    {
        Task<Grievance> GetGrievanceWithSolutionsAsync(int id);
        Task<Grievance> GetGrievanceWithHistoriesAsync(int id);
        Task<GrievanceDTO> GetGrievanceWithSolver(int id);
        Task<List<Grievance>> GetGrievancesWithEmployee();

        Task<IEnumerable<GrievanceDTO>> GetAllEmployeeGrievancesAsync(int employeeId);
        Task<IEnumerable<GrievanceDTO>> GetAllGrievancesByTypeAsync(string grievanceType);
    }
}
