using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces;
using EmployeeGrievanceRedressal.Models.DTOs.Grievance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GrievanceController : ControllerBase
    {
        private readonly IGrievanceService _grievanceService;

        public GrievanceController(IGrievanceService grievanceService)
        {
            _grievanceService = grievanceService;
        }

        [HttpPut("ResolveGrievance")]
        [Authorize(Roles = "Solver")]
        public async Task<IActionResult> MarkAsResolved(int id)
        {
            try
            {
                var grievance = await _grievanceService.MarkAsResolvedAsync(id);
                return Ok(grievance);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ServiceException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }

        [HttpGet("GetAllGrievances")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllGrievances()
        {
            try
            {
                var grievances = await _grievanceService.GetAllGrievancesAsync();
                return Ok(grievances);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, new { Message = "Unauthorized access error occurred.", Details = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ServiceException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }

        [HttpGet("GetGrievanceById")]
        [Authorize(Roles = "Admin, Employee, Solver")]
        public async Task<IActionResult> GetGrievanceById(int id)
        {
            try
            {
                var grievances = await _grievanceService.GetGrievanceByIdAsync(id);
                return Ok(grievances);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, new { Message = "Unauthorized access error occurred.", Details = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ServiceException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }

        [HttpPut("CloseGrievance")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CloseGrievance(CloseGrievanceDTO closeGrievanceDTO)
        {
            try
            {
                var grievance = await _grievanceService.CloseGrievanceAsync(closeGrievanceDTO);
                return Ok(grievance);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, new { Message = "Unauthorized access error occurred.", Details = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ServiceException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }
        [HttpPost("EscalateGrievance")]
        [Authorize(Roles = "Solver")]
        public async Task<IActionResult> EscalateGrievance([FromBody] EscalateGrievanceDTO model)
        {
            try
            {
                var grievance = await _grievanceService.EscalateGrievanceAsync(model);
                return Ok(grievance);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, new { Message = "Unauthorized access error occurred.", Details = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ServiceException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }
    }
}
