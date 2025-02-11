﻿using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces;
using EmployeeGrievanceRedressal.Models;
using EmployeeGrievanceRedressal.Models.DTOs.Grievance;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IGrievanceService _grievanceService;
        private readonly BlobStorageService _blobStorageService;

        public EmployeeController(IGrievanceService grievanceService, BlobStorageService blobStorageService)
        {
            _grievanceService = grievanceService;
            _blobStorageService = blobStorageService;
        }

        [HttpPost("raise-grievance")]
        public async Task<IActionResult> RaiseGrievance([FromForm] CreateGrievanceDTO grievanceDto, [FromForm] IFormFile[] documents)
        {
            try
            {
                if (documents != null && documents.Length > 0)
                {
                    var documentUrls = await _blobStorageService.UploadFilesAsync(documents);
                    grievanceDto.DocumentUrls = documentUrls;
                }
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "uid").Value.ToString();
                var grievance = await _grievanceService.RaiseGrievanceAsync(grievanceDto, Convert.ToInt32(userIdClaim));
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

        [HttpGet("grievance/{id}")]
        public async Task<IActionResult> GetGrievanceById(int id)
        {
            try
            {
                var grievance = await _grievanceService.GetGrievanceByIdAsync(id);
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
            catch (Exception ex)
            {
                // Log exception details here if needed
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }
        [HttpGet("grievances/{employeeId}")]
        public async Task<IActionResult> GetAllEmployeeGrievances(int employeeId)
        {
            try
            {
                var grievances = await _grievanceService.GetAllEmployeeGrievancesAsync(employeeId);
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
            catch (Exception ex)
            {
                // Log exception details here if needed
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }
    }
}
