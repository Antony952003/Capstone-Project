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
            var employees = users.Where(u => u.Role == UserRole.Employee && u.IsApproved == true);
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

            if(user.IsApproved == false)
            {
                throw new Exception("The user is not approved yet!!");
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
                    user.GrievanceType = null;
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
                IsApproved = user.IsApproved,
                GrievanceDept = user.GrievanceType.ToString(),
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
                GrievanceDepartmentType = user.GrievanceType != null ? user.GrievanceType.ToString() : "",
                AverageRating = user.AverageRating,
                IsAvailable = user.IsAvailable,
            };
        }

        public async Task<UserDTO> GetEmployeeById(int employeeid)
        {
            try
            {
                var employee = await _userRepository.GetByIdAsync(employeeid);
                if(employee != null)
                {
                    return MapToUserDTO(employee);
                }
                throw new EntityNotFoundException("Employee not found");
            }
            catch(EntityNotFoundException e)
            {
                throw e;
            }
            catch(Exception e)
            {
                throw new RepositoryException("Error in fetching employee", e);
            }
        }

        public async Task<UserDTO> DisApproveEmployeeById(int employeeid)
        {
            try
            {
                var employee = await _userRepository.GetByIdAsync(employeeid);
                if (employee != null)
                {
                    employee.IsApproved = false;
                    _userRepository.Update(employee);
                    return MapToUserDTO(employee);
                }
                throw new EntityNotFoundException("Employee not found");
            }
            catch (EntityNotFoundException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new RepositoryException("Error in fetching employee", e);
            }
        }

        public async Task<UserDTO> DeleteEmployeeById(int employeeid)
        {
            try
            {
                var employee = await _userRepository.GetByIdAsync(employeeid);
                if (employee != null)
                {
                    _userRepository.RemoveUserById(employeeid);
                    return MapToUserDTO(employee);
                }
                throw new EntityNotFoundException("Employee not found");
            }
            catch (EntityNotFoundException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new RepositoryException("Error in fetching employee", e);
            }
        }

        public async Task<SolverDTO> GetSolverById(int solverid)
        {
            try
            {
                var employee = await _userRepository.GetByIdAsync(solverid);
                if (employee != null && employee.Role == UserRole.Solver)
                {
                    return MapToSolverDTO(employee);
                }
                throw new EntityNotFoundException("Employee not found");
            }
            catch (EntityNotFoundException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new RepositoryException("Error in fetching employee", e);
            }
        }

        public async Task<SolverDTO> ChangeDepartmentBySolverId(int solverid, string grievancedepartmenttype)
        {
            var employee = await _userRepository.GetByIdAsync(solverid);
            if(employee != null && employee.Role == UserRole.Solver)
            {
                if (Enum.TryParse(grievancedepartmenttype, true, out GrievanceType parsedGrievanceType)){
                    employee.GrievanceType = parsedGrievanceType;
                    _userRepository.Update(employee);
                    return MapToSolverDTO(employee);
                }
            }
            throw new EntityNotFoundException("Solver not found");
        }

        public async Task<IEnumerable<SolverDTO>> GetSolversByType(string grievanceType)
        {
            try
            {
                var solvers = await GetAllSolversAsync();
                solvers = solvers.Where(x => x.GrievanceDepartmentType == grievanceType && x.IsAvailable == true);
                return solvers.ToList();
            }
            catch(EntityNotFoundException e)
            {
                throw e;
            }
        }
    }
}
