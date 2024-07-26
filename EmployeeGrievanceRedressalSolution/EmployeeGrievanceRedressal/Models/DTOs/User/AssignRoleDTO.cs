using System.ComponentModel.DataAnnotations;

namespace EmployeeGrievanceRedressal.Models.DTOs.User
{
    public class AssignRoleDTO
    {
        public int UserId { get; set; }
        public string Role { get; set; } // Use string for the role
        public string? GrievanceType { get; set; } // Use string for the grievance type
    }
}
