using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces;
using EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces;
using EmployeeGrievanceRedressal.Migrations;
using EmployeeGrievanceRedressal.Models;
using EmployeeGrievanceRedressal.Models.DTOs.User;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressal.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;

        public AdminService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            if (users == null || !users.Any())
            {
                throw new EntityNotFoundException("No users found.");
            }
            return users.Select(MapToUserDTO);
        }

        public async Task<IEnumerable<UserDTO>> GetAllApprovedUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var approvedUsers = users.Where(u => u.IsApproved);
            if (!approvedUsers.Any())
            {
                throw new EntityNotFoundException("No approved users found.");
            }
            return approvedUsers.Select(MapToUserDTO);
        }

        public async Task<IEnumerable<SolverDTO>> GetAllSolversAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var solvers = users.Where(u => u.Role == UserRole.Solver);
            if (!solvers.Any())
            {
                throw new EntityNotFoundException("No solvers found.");
            }
            return solvers.Select(MapToSolverDTO);
        }

        public async Task<IEnumerable<UserDTO>> GetAllEmployeesAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var employees = users.Where(u => u.Role == UserRole.Employee);
            if (!employees.Any())
            {
                throw new EntityNotFoundException("No employees found.");
            }
            return employees.Select(MapToUserDTO);
        }

        public async Task<UserDTO> AssignRoleAsync(AssignRoleDTO assignRoleDTO)
        {
            var user = await _userRepository.GetByIdAsync(assignRoleDTO.UserId);
            if (user == null)
            {
                throw new EntityNotFoundException("User not found.");
            }

            if (assignRoleDTO.Role == UserRole.Solver.ToString())
            {
                if (Enum.TryParse(assignRoleDTO.GrievanceType, true, out GrievanceType parsedGrievanceType))
                {
                    user.Role = UserRole.Solver;
                    user.GrievanceType = parsedGrievanceType;
                }
                else
                {
                    throw new InvalidOperationException("Invalid grievance type.");
                }
            }
            else
            {
                if (Enum.TryParse(assignRoleDTO.Role, true, out UserRole parsedRole))
                {
                    user.Role = parsedRole;
                }
                else
                {
                    throw new InvalidOperationException("Invalid role.");
                }
            }

            _userRepository.Update(user);
            return MapToUserDTO(user);
        }

        // Mapping methods
        private UserDTO MapToUserDTO(User user)
        {
            return new UserDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                UserImage = user.UserImage,
                DOB = user.DOB,
                Role = user.Role.ToString(),
                IsApproved = user.IsApproved
            };
        }
        private SolverDTO MapToSolverDTO(User user)
        {
            return new SolverDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                UserImage = user.UserImage,
                DOB = user.DOB,
                Role = user.Role.ToString(),
                IsApproved = user.IsApproved,
                GrievanceDepartmentType = user.GrievanceType != null ? user.GrievanceType.ToString() : ""
            };
        }
    }
}
