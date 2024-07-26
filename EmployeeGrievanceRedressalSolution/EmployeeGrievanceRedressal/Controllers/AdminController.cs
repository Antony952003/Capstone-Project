using EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces;
using EmployeeGrievanceRedressal.Models.DTOs.User;
using EmployeeGrievanceRedressal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Models.DTOs.Grievance;
using EmployeeGrievanceRedressal.Services;

namespace EmployeeGrievanceRedressal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IGrievanceService _grievanceService;

        public AdminController(IAdminService adminService, IGrievanceService grievanceService)
        {
            _adminService = adminService;
            _grievanceService = grievanceService;
        }

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("GetAllApprovedUsers")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllApprovedUsers()
        {
            try
            {
                var users = await _adminService.GetAllApprovedUsersAsync();
                return Ok(users);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("GetAllSolvers")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllSolvers()
        {
            try
            {
                var users = await _adminService.GetAllSolversAsync();
                return Ok(users);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("GetAllEmployees")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllEmployees()
        {
            try
            {
                var users = await _adminService.GetAllEmployeesAsync();
                return Ok(users);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("AssignRoleToUser")]
        public async Task<ActionResult<UserDTO>> AssignRole(AssignRoleDTO assignRoleDTO)
        {
            try
            {
                var user = await _adminService.AssignRoleAsync(assignRoleDTO);
                return Ok(user);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetAllGrievances")]
        public async Task<IActionResult> GetAllGrievances()
        {
            try
            {
                var grievances = await _grievanceService.GetAllGrievancesAsync();
                return Ok(grievances);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log exception details here if needed
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }

        [HttpGet("GetAllOpenGrievances")]
        public async Task<IActionResult> GetAllOpenGrievances()
        {
            try
            {
                var grievances = await _grievanceService.GetAllOpenGrievancesAsync();
                return Ok(grievances);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log exception details here if needed
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }

        [HttpPost("AssignOpenGrievances")]
        public async Task<IActionResult> AssignGrievance([FromBody] AssignGrievanceDTO model)
        {
            try
            {
                var updatedGrievance = await _grievanceService.AssignGrievanceAsync(model.GrievanceId, model.SolverId);
                return Ok(updatedGrievance);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log exception details here if needed
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }
        [HttpGet("GetAllGrievancesByType")]
        public async Task<IActionResult> GetAllGrievancesByType(string type)
        {
            try
            {
                var grievances = await _grievanceService.GetAllGrievancesByTypeAsync(type);
                return Ok(grievances);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log exception details here if needed
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }
    }
}
