namespace EmployeeGrievanceRedressal.Models.DTOs.User
{
    public class UpdateUserDTO
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string UserImage { get; set; }
    }
}
