using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces;
using EmployeeGrievanceRedressal.Models.DTOs.Solution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class SolutionController : ControllerBase
    {
        private readonly ISolutionService _solutionService;

        public SolutionController(ISolutionService solutionService)
        {
            _solutionService = solutionService;
        }

        [Authorize(Roles = "Solver")]
        [HttpPost("provide-solution")]
        public async Task<IActionResult> ProvideSolution([FromBody] ProvideSolutionDTO model)
        {
            try
            {
                var result = await _solutionService.ProvideSolutionAsync(model);
                return Ok(result);
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

        [Authorize(Roles = "Solver, Employee")]
        [HttpGet("solution/{id}")]
        public async Task<IActionResult> GetSolutionById(int id)
        {
            try
            {
                var solution = await _solutionService.GetSolutionByIdAsync(id);
                return Ok(solution);
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
        [Authorize(Roles = "Solver, Employee")]
        [HttpGet("solutions/{grievanceId}")]
        public async Task<IActionResult> GetSolutionsByGrievanceId(int grievanceId)
        {
            try
            {
                var solutions = await _solutionService.GetSolutionsByGrievanceIdAsync(grievanceId);
                return Ok(solutions);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }
    }
}
