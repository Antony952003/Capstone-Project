using EmployeeGrievanceRedressal.Models;

namespace EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces
{
    public interface IApprovalRequestRepository
    {
        Task<ApprovalRequest> GetByIdAsync(int id);
        Task<IEnumerable<ApprovalRequest>> GetAllAsync();
        Task AddAsync(ApprovalRequest approvalRequest);
        Task UpdateAsync(ApprovalRequest approvalRequest);
        Task DeleteAsync(int id);
    }
}
