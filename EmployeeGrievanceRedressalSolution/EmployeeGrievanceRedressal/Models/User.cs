namespace EmployeeGrievanceRedressal.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string UserImage { get; set; }
        public DateTime DOB { get; set; }
        public UserRole Role { get; set; }
        public bool IsApproved { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public GrievanceType? GrievanceType { get; set; } // Nullable property for solvers
        public List<Grievance> RaisedGrievances { get; set; }
        public List<Feedback> Feedbacks { get; set; }
        public List<Solution> Solutions { get; set; }
    }

    public enum UserRole
    {
        Employee,
        Admin,  
        Solver
    }

    public enum GrievanceType
    {
        HR,
        IT,
        Facilities,
        ProjectManagement,
        Administration,
    }
}
