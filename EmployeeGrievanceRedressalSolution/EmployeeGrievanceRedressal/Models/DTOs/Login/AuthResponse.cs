using EmployeeGrievanceRedressal.Models.DTOs.User;

namespace EmployeeGrievanceRedressal.Models.DTOs.Login
{
    public class AuthResponse
    {
        public bool IsSuccessful { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public UserDTO User { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
