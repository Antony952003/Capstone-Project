using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces;
using EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces;
using EmployeeGrievanceRedressal.Models;
using EmployeeGrievanceRedressal.Models.DTOs.Login;
using EmployeeGrievanceRedressal.Models.DTOs.Register;
using EmployeeGrievanceRedressal.Models.DTOs.User;
using Microsoft.IdentityModel.Tokens;
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
                var accessToken = _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();
                user.RefreshTokens.Add(new RefreshToken
                {
                    Token = refreshToken,
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow
                });
                 _userRepository.Update(user);
                return new AuthResponse
                {
                    IsSuccessful = true,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
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

                var accessToken = _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();
                user.RefreshTokens.Add(new RefreshToken
                {
                    Token = refreshToken,
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow
                });
                _userRepository.Update(user);
                return new AuthResponse
                {
                    IsSuccessful = true,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    User = MapToUserDTO(user)
                };
            }
            catch (UserNotFoundException ex)
            {
                throw;
            }
            catch (InvalidPasswordException ex)
            {
                throw new InvalidPasswordException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred during login: " + ex.Message);
            }
        }

        public async Task<AuthResponse> RefreshTokenAsync(string token)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(token);
            if (user == null)
            {
                throw new UserNotFoundException("Invalid refresh token.");
            }

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive)
            {
                throw new SecurityTokenException("Invalid refresh token.");
            }

            // Replace old refresh token with a new one (rotate token)
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            refreshToken.Revoked = DateTime.UtcNow;
            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow
            });
             _userRepository.Update(user);

            var newAccessToken = _tokenService.GenerateAccessToken(user);

            return new AuthResponse
            {
                IsSuccessful = true,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                User = MapToUserDTO(user)
            };
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
