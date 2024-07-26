using EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces;
using EmployeeGrievanceRedressal.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressal.Repositories
{
    public class ApprovalRequestRepository : IApprovalRequestRepository
    {
        private readonly EmployeeGrievanceContext _context;

        public ApprovalRequestRepository(EmployeeGrievanceContext context)
        {
            _context = context;
        }

        public async Task<ApprovalRequest> GetByIdAsync(int id)
        {
            return await _context.ApprovalRequests
                .Include(ar => ar.Employee)
                .FirstOrDefaultAsync(ar => ar.ApprovalRequestId == id);
        }

        public async Task<IEnumerable<ApprovalRequest>> GetAllAsync()
        {
            return await _context.ApprovalRequests
                .Include(ar => ar.Employee)
                .ToListAsync();
        }

        public async Task AddAsync(ApprovalRequest approvalRequest)
        {
            _context.ApprovalRequests.Add(approvalRequest);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ApprovalRequest approvalRequest)
        {
            _context.ApprovalRequests.Update(approvalRequest);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var approvalRequest = await GetByIdAsync(id);
            if (approvalRequest != null)
            {
                _context.ApprovalRequests.Remove(approvalRequest);
                await _context.SaveChangesAsync();
            }
        }
    }
}
