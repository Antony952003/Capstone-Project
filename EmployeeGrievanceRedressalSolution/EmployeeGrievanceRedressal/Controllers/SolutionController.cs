using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces;
using EmployeeGrievanceRedressal.Models.DTOs.Grievance;
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
        private readonly BlobStorageService _blobStorageService;

        public SolutionController(ISolutionService solutionService, BlobStorageService blobStorageService)
        {
            _solutionService = solutionService;
            _blobStorageService = blobStorageService;
        }

        [HttpPost("provide-solution")]
        [Authorize(Roles = "Solver")]
        public async Task<IActionResult> ProvideSolution([FromForm] ProvideSolutionDTO model, [FromForm] IFormFile[] documents)
        {
            try
            {
                if (documents != null && documents.Length > 0)
                {
                    var documentUrls = await _blobStorageService.UploadFilesAsync(documents);
                    model.DocumentUrls = documentUrls;
                }
                var result = await _solutionService.ProvideSolutionAsync(model);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, new { Message = "Unauthorized access error occurred.", Details = ex.Message });
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

        [Authorize(Roles = "Employee, Solver, Admin")]
        [HttpGet("solution/{id}")]
        public async Task<IActionResult> GetSolutionById(int id)
        {
            try
            {
                var solution = await _solutionService.GetSolutionByIdAsync(id);
                return Ok(solution);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, new { Message = "Unauthorized access error occurred.", Details = ex.Message });
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
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, new { Message = "Unauthorized access error occurred.", Details = ex.Message });
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
