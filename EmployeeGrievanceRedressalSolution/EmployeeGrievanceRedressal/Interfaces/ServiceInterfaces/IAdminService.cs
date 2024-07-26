using EmployeeGrievanceRedressal.Models;
using EmployeeGrievanceRedressal.Models.DTOs.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<IEnumerable<UserDTO>> GetAllApprovedUsersAsync();
        Task<IEnumerable<SolverDTO>> GetAllSolversAsync();
        Task<IEnumerable<UserDTO>> GetAllEmployeesAsync();
        Task<UserDTO> AssignRoleAsync(AssignRoleDTO assignRoleDTO);
    }
}
