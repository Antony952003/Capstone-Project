using EmployeeGrievanceRedressal.Models.DTOs.Grievance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces
{
    public interface IGrievanceService
    {
        Task<GrievanceDTO> RaiseGrievanceAsync(CreateGrievanceDTO model, int employeeId);
        Task<GrievanceDTO> EscalateGrievanceAsync(EscalateGrievanceDTO model);
        Task<IEnumerable<GrievanceDTO>> GetAllGrievancesAsync();
        Task<IEnumerable<GrievanceDTO>> GetAllOpenGrievancesAsync();
        Task<GrievanceDTO> GetGrievanceByIdAsync(int id);
        Task<GrievanceDTO> AssignGrievanceAsync(int grievanceId, int solverId);
        Task<IEnumerable<GrievanceDTO>> GetAllEmployeeGrievancesAsync(int employeeId);
        Task<IEnumerable<GrievanceDTO>> GetAllGrievancesByTypeAsync(string grievanceType);
        Task<GrievanceDTO> MarkAsResolvedAsync(int grievanceId);
        Task<GrievanceDTO> CloseGrievanceAsync(CloseGrievanceDTO closeGrievanceDTO);
    }
}
