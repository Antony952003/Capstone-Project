using EmployeeGrievanceRedressal.Models;
using EmployeeGrievanceRedressal.Models.DTOs.GrievanceHistory;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces
{
    public interface IGrievanceHistoryService
    {
        Task<GrievanceHistoryDTO> RecordHistoryAsync(GrievanceHistory history);
        Task<IEnumerable<GrievanceHistoryDTO>> GetGrievanceHistoryAsync(int grievanceId);
    }
}
