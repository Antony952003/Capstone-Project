using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces;
using EmployeeGrievanceRedressal.Models.DTOs.ApprovalRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalRequestController : ControllerBase
    {
        private readonly IApprovalRequestService _approvalRequestService;

        public ApprovalRequestController(IApprovalRequestService approvalRequestService)
        {
            _approvalRequestService = approvalRequestService;
        }

        [HttpPost("CreateApprovalRequest")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> CreateApprovalRequest([FromBody] CreateApprovalRequestDTO model)
        {
            try
            {
                int employeeId = GetAuthenticatedEmployeeId(); // Implement this method based on your authentication setup

                var result = await _approvalRequestService.CreateApprovalRequestAsync(model, employeeId);
                return CreatedAtAction(nameof(GetApprovalRequestById), new { id = result.ApprovalRequestId }, result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, new { Message = "Unauthorized access error occurred.", Details = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An unexpected error occurred.", Details = ex.Message });
            }
        }

        [HttpGet("GetAllApprovalRequests")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllApprovalRequests()
        {
            try
            {
                var result = await _approvalRequestService.GetAllApprovalRequestsAsync();
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, new { Message = "Unauthorized access error occurred.", Details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An unexpected error occurred.", Details = ex.Message });
            }
        }

        [HttpGet("GetApprovalRequestById")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetApprovalRequestById(int id)
        {
            try
            {
                var result = await _approvalRequestService.GetApprovalRequestByIdAsync(id);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, new { Message = "Unauthorized access error occurred.", Details = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An unexpected error occurred.", Details = ex.Message });
            }
        }

        [HttpPost("ApproveRequest")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveRequest(int id)
        {
            try
            {
                var result = await _approvalRequestService.ApproveRequestAsync(id);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, new { Message = "Unauthorized access error occurred.", Details = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An unexpected error occurred.", Details = ex.Message });
            }
        }

        [HttpPost("RejectRequest")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectRequest(int id)
        {
            try
            {
                var result = await _approvalRequestService.RejectRequestAsync(id);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, new { Message = "Unauthorized access error occurred.", Details = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An unexpected error occurred.", Details = ex.Message });
            }
        }

        [HttpGet("GetAllApprovedRequests")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllApprovedRequests()
        {
            try
            {
                var result = await _approvalRequestService.GetAllApprovedRequestsAsync();
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, new { Message = "Unauthorized access error occurred.", Details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An unexpected error occurred.", Details = ex.Message });
            }
        }
        [HttpGet("GetAllRejectedRequests")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllRejectedRequests()
        {
            try
            {
                var result = await _approvalRequestService.GetAllRejectedRequestsAsync();
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, new { Message = "Unauthorized access error occurred.", Details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An unexpected error occurred.", Details = ex.Message });
            }
        }

        [HttpGet("GetAllPendingRequests")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ApprovalRequestDTO>>> GetAllPendingRequests()
        {
            try
            {
                var requests = await _approvalRequestService.GetAllPendingRequestsAsync();
                return Ok(requests);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, new { Message = "Unauthorized access error occurred.", Details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An unexpected error occurred.", Details = ex.Message });

            }
        }

        // Dummy method for fetching the authenticated user's employee ID
        private int GetAuthenticatedEmployeeId()
        {
            // Get the user claims from the HttpContext
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "uid");

            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("User ID claim not found.");
            }

            if (!int.TryParse(userIdClaim.Value, out var employeeId))
            {
                throw new UnauthorizedAccessException("Invalid User ID claim format.");
            }

            return employeeId;
        }

    }
}
