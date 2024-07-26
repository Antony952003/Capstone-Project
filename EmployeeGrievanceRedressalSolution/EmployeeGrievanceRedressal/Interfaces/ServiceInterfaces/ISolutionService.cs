using EmployeeGrievanceRedressal.Models.DTOs.Solution;

namespace EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces
{
    public interface ISolutionService
    {
        Task<SolutionDTO> ProvideSolutionAsync(ProvideSolutionDTO dto);
        Task<SolutionDTO> GetSolutionByIdAsync(int id);
        Task<IEnumerable<SolutionDTO>> GetSolutionsByGrievanceIdAsync(int grievanceId);
    }
}
