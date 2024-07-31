using EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces;
using EmployeeGrievanceRedressal.Models.DTOs.GrievanceHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GrievanceHistoryController : ControllerBase
    {
        private readonly IGrievanceHistoryService _grievanceHistoryService;

        public GrievanceHistoryController(IGrievanceHistoryService grievanceHistoryService)
        {
            _grievanceHistoryService = grievanceHistoryService;
        }

        [HttpGet("GetGrievanceHistoryById")]
        [Authorize(Roles = "Admin, Employee, Solver")]
        public async Task<IActionResult> GetGrievanceHistory(int grievanceId)
        {
            try
            {
                var history = await _grievanceHistoryService.GetGrievanceHistoryAsync(grievanceId);
                if (history == null || !history.Any())
                {
                    return NotFound(new { Message = "No history found for the specified grievance." });
                }
                return Ok(history);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, new { Message = "Unauthorized access error occurred.", Details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }
    }
}
