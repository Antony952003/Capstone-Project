using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces;
using EmployeeGrievanceRedressal.Models.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPut("UpdateUserDetails")]
        [Authorize(Roles = "Employee, Admin, Solver")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO model)
        {
            try
            {
                var updatedUser = await _userService.UpdateUserAsync(model);
                return Ok(updatedUser);
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
