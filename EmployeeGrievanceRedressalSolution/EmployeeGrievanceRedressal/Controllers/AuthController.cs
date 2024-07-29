using EmployeeGrievanceRedressal.Models.DTOs;
using EmployeeGrievanceRedressal.Models.DTOs.Login;
using EmployeeGrievanceRedressal.Models.DTOs.Register;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces;

namespace EmployeeGrievanceRedressal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterDTO model)
        {
            var result = await _authService.RegisterAsync(model);
            if (!result.IsSuccessful)
            {
                return BadRequest(result.Errors);
            }
            return Ok(new { Token = result.Token, User = result.User });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDTO model)
        {
            var result = await _authService.LoginAsync(model);
            if (!result.IsSuccessful)
            {
                return BadRequest(result.Errors);
            }
            return Ok(new { Token = result.Token, User = result.User });
        }
        [HttpPost("CheckUsernameAvailablity")]
        public async Task<string> checknameavailable(string name)
        {
            var result = await _authService.CheckUsernameAvailablity(name);
            return result;
        }
        [HttpPost("CheckUsermailAvailablity")]
        public async Task<string> checkmailavailable(string mail)
        {
            var result = await _authService.CheckUsermailAvailablity(mail);
            return result;
        }
    }
}
