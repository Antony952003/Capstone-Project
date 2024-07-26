using System.ComponentModel.DataAnnotations;

namespace EmployeeGrievanceRedressal.Models.DTOs.Grievance
{
    public class AssignGrievanceDTO
    {
        [Required(ErrorMessage = "Grievance ID is required.")]
        public int GrievanceId { get; set; }

        [Required(ErrorMessage = "Solver ID is required.")]
        public int SolverId { get; set; }
    }
}
