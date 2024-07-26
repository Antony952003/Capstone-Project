using EmployeeGrievanceRedressal.Models.DTOs.User;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces
{
    public interface IUserService
    {
        Task<UserDTO> UpdateUserAsync(UpdateUserDTO model);
    }
}
