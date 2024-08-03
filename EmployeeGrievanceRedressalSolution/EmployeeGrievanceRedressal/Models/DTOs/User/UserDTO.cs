namespace EmployeeGrievanceRedressal.Models.DTOs.User
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string UserImage { get; set; }
        public DateTime DOB { get; set; }
        public string Role { get; set; }
        public string GrievanceDept { get; set; }
        public bool IsApproved { get; set; }
    }
}
