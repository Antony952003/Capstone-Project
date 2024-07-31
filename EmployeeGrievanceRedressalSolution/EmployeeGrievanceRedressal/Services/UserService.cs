using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces;
using EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces;
using EmployeeGrievanceRedressal.Models;
using EmployeeGrievanceRedressal.Models.DTOs.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressal.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly EmployeeGrievanceContext _context;

        public UserService(IUserRepository userRepository,
            EmployeeGrievanceContext context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<UserDTO> UpdateUserAsync(UpdateUserDTO model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = await _userRepository.GetByIdAsync(model.UserId);
                    if (user == null)
                    {
                        throw new EntityNotFoundException($"User with ID {model.UserId} not found.");
                    }

                    // Update user properties
                    user.Name = model.Name ?? user.Name;
                    user.Phone = model.Phone ?? user.Phone;
                    user.Email = model.Email ?? user.Email;
                    user.UserImage = model.UserImage ?? user.UserImage;

                    _userRepository.Update(user);
                    await transaction.CommitAsync();

                    return new UserDTO
                    {
                        UserId = user.UserId,
                        Name = user.Name,
                        Phone = user.Phone,
                        Email = user.Email,
                        UserImage = user.UserImage,
                        DOB = user.DOB
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new ServiceException("Error updating user", ex);
                }
            }
        }
    }
}
