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
            var response = await _authService.RegisterAsync(model);
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response.Errors);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDTO model)
        {
            var response = await _authService.LoginAsync(model);
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response.Errors);
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
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RequestToken requestToken)
        {
            var response = await _authService.RefreshTokenAsync(requestToken.token);
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response.Errors);
        }
    }
}
