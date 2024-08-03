using EmployeeGrievanceRedressal.Models.DTOs.ApprovalRequest;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces
{
    public interface IApprovalRequestService
    {
        Task<ApprovalRequestDTO> CreateApprovalRequestAsync(CreateApprovalRequestDTO model, int employeeId);
        Task<IEnumerable<ApprovalRequestDTO>> GetAllApprovalRequestsAsync();
        Task<IEnumerable<ApprovalRequestDTO>> GetAllApprovedRequestsAsync();
        Task<IEnumerable<ApprovalRequestDTO>> GetAllRejectedRequestsAsync();
        Task<IEnumerable<ApprovalRequestDTO>> GetAllPendingRequestsAsync();
        Task<ApprovalRequestDTO> GetApprovalRequestByIdAsync(int id);
        Task<ApprovalRequestResponseDTO> ApproveRequestAsync(int id);
        Task<ApprovalRequestResponseDTO> RejectRequestAsync(int id);
        Task<IEnumerable<ApprovalRequestDTO>> GetAllUserApprovalRequests(int userid);
    }
}
