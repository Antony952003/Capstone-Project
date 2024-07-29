using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces;
using EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces;
using EmployeeGrievanceRedressal.Models;
using EmployeeGrievanceRedressal.Models.DTOs.Login;
using EmployeeGrievanceRedressal.Models.DTOs.Register;
using EmployeeGrievanceRedressal.Models.DTOs.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EmployeeGrievanceRedressal.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<AuthResponse> RegisterAsync(UserRegisterDTO model)
        {
            var user = MapToUser(model);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            try
            {
                await _userRepository.Add(user);
                var token = _tokenService.GenerateToken(user);
                return new AuthResponse
                {
                    IsSuccessful = true,
                    Token = token,
                    User = MapToUserDTO(user)
                };
            }
            catch (Exception ex)
            {
                throw new RegistrationException("An error occurred while registering the user: " + ex.Message);
            }
        }

        public async Task<AuthResponse> LoginAsync(UserLoginDTO model)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(model.Email);
                if (user == null)
                {
                    throw new UserNotFoundException("User not found.");
                }
                if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    throw new InvalidPasswordException("Invalid password.");
                }

                var token = _tokenService.GenerateToken(user);
                return new AuthResponse
                {
                    IsSuccessful = true,
                    Token = token,
                    User = MapToUserDTO(user)
                };
            }
            catch (UserNotFoundException ex)
            {
                throw;
            }
            catch (InvalidPasswordException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred during login: " + ex.Message);
            }
        }


        // Mapping methods
        private User MapToUser(UserRegisterDTO model)
        {
            return new User
            {
                Name = model.Name,
                Phone = model.Phone,
                Email = model.Email,
                UserImage = model.UserImage,
                DOB = model.DOB,
                Role = UserRole.Employee,  // Default role
                IsApproved = false,  // Default approval status
                RaisedGrievances = new List<Grievance>(),
                Feedbacks = new List<Feedback>(),
                Solutions = new List<Solution>()
            };
        }

        private UserDTO MapToUserDTO(User user)
        {
            return new UserDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Phone = user.Phone,
                Email = user.Email,
                UserImage = user.UserImage,
                DOB = user.DOB,
                Role = user.Role.ToString(),
                IsApproved = user.IsApproved
            };
        }

        public async Task<string> CheckUsernameAvailablity(string name)
        {
            var user = await _userRepository.GetByNameAsync(name);
            if(user != null)
            {
                return "NotAvailable";
            }
            return "Available";
        }

        public async Task<string> CheckUsermailAvailablity(string mail)
        {
            var user = await _userRepository.GetByEmailAsync(mail);
            if (user != null)
            {
                return "NotAvailable";
            }
            return "Available";
        }
    }
}
