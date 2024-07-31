using EmployeeGrievanceRedressal.Models;
using System.Security.Claims;

namespace EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces
{
    public interface ITokenService
    {
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        string GenerateRefreshToken();
        string GenerateAccessToken(User user);
    }
}
