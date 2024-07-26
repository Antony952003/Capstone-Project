using EmployeeGrievanceRedressal.Models;

namespace EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
