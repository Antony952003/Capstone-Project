using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces;
using EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces;
using EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces;
using EmployeeGrievanceRedressal.Models;
using EmployeeGrievanceRedressal.Models.DTOs.ApprovalRequest;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressal.Services
{
    public class ApprovalRequestService : IApprovalRequestService
    {
        private readonly IApprovalRequestRepository _approvalRequestRepository;
        private readonly IUserRepository _userRepository;

        public ApprovalRequestService(IApprovalRequestRepository approvalRequestRepository, IUserRepository userRepository)
        {
            _approvalRequestRepository = approvalRequestRepository;
            _userRepository = userRepository;
        }

        public async Task<ApprovalRequestDTO> CreateApprovalRequestAsync(CreateApprovalRequestDTO model, int employeeId)
        {
            var employee = await _userRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                throw new EntityNotFoundException("Employee not found.");
            }

            // Check if there's already a pending or approved approval request for this employee
            var existingRequest = await _approvalRequestRepository.GetAllAsync();
            if (employee.IsApproved == true)
            {
                throw new InvalidOperationException("An approval request is already pending or approved for this employee.");
            }

            var approvalRequest = new ApprovalRequest
            {
                EmployeeId = employeeId,
                RequestDate = DateTime.UtcNow,
                Reason = model.Reason,
                Status = ApprovalStatus.Pending
            };

            await _approvalRequestRepository.AddAsync(approvalRequest);

            // Update employee's isApproved status
            employee.IsApproved = false;
            _userRepository.Update(employee);

            return MapToApprovalRequestDTO(approvalRequest);
        }

        public async Task<IEnumerable<ApprovalRequestDTO>> GetAllApprovalRequestsAsync()
        {
            var requests = await _approvalRequestRepository.GetAllAsync();
            return requests.Select(MapToApprovalRequestDTO);
        }

        public async Task<IEnumerable<ApprovalRequestDTO>> GetAllApprovedRequestsAsync()
        {
            var requests = await _approvalRequestRepository.GetAllAsync();
            return requests
                .Where(r => r.Status == ApprovalStatus.Approved)
                .Select(MapToApprovalRequestDTO);
        }

        public async Task<IEnumerable<ApprovalRequestDTO>> GetAllRejectedRequestsAsync()
        {
            var requests = await _approvalRequestRepository.GetAllAsync();
            return requests
                .Where(r => r.Status == ApprovalStatus.Rejected)
                .Select(MapToApprovalRequestDTO);
        }

        public async Task<IEnumerable<ApprovalRequestDTO>> GetAllPendingRequestsAsync()
        {
            var requests = await _approvalRequestRepository.GetAllAsync();
            return requests
                .Where(r => r.Status == ApprovalStatus.Pending)
                .Select(MapToApprovalRequestDTO);
        }

        public async Task<ApprovalRequestDTO> GetApprovalRequestByIdAsync(int id)
        {
            var request = await _approvalRequestRepository.GetByIdAsync(id);
            if (request == null)
            {
                throw new EntityNotFoundException("Approval request not found.");
            }
            return MapToApprovalRequestDTO(request);
        }

        public async Task<ApprovalRequestResponseDTO> ApproveRequestAsync(int id)
        {
            var request = await _approvalRequestRepository.GetByIdAsync(id);
            if (request == null)
            {
                throw new EntityNotFoundException("Approval request not found.");
            }

            request.Status = ApprovalStatus.Approved;
            await _approvalRequestRepository.UpdateAsync(request);

            // Update employee's isApproved status
            var employee = await _userRepository.GetByIdAsync(request.EmployeeId);
            if (employee == null)
            {
                throw new EntityNotFoundException("Employee not found.");
            }

            employee.IsApproved = true;
            _userRepository.Update(employee);

            return new ApprovalRequestResponseDTO
            {
                ApprovalRequest = MapToApprovalRequestDTO(request),
                Message = "Approval request has been approved."
            };
        }

        public async Task<ApprovalRequestResponseDTO> RejectRequestAsync(int id)
        {
            var request = await _approvalRequestRepository.GetByIdAsync(id);
            if (request == null)
            {
                throw new EntityNotFoundException("Approval request not found.");
            }

            request.Status = ApprovalStatus.Rejected;
            await _approvalRequestRepository.UpdateAsync(request);

            // Update employee's isApproved status
            var employee = await _userRepository.GetByIdAsync(request.EmployeeId);
            if (employee == null)
            {
                throw new EntityNotFoundException("Employee not found.");
            }

            employee.IsApproved = false;
            _userRepository.Update(employee);

            return new ApprovalRequestResponseDTO
            {
                ApprovalRequest = MapToApprovalRequestDTO(request),
                Message = "Approval request has been rejected."
            };
        }

        // Mapping methods
        private ApprovalRequestDTO MapToApprovalRequestDTO(ApprovalRequest request)
        {
            return new ApprovalRequestDTO
            {
                ApprovalRequestId = request.ApprovalRequestId,
                EmployeeId = request.EmployeeId,
                EmployeeName = request.Employee.Name,
                RequestDate = request.RequestDate,
                Reason = request.Reason,
                Status = request.Status
            };
        }
    }
}
