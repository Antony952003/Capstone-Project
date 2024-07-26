using System.ComponentModel.DataAnnotations;

namespace EmployeeGrievanceRedressal.Models.DTOs.Grievance
{
    public class CreateGrievanceDTO
    {
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Priority is required.")]
        public string Priority { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        public string Type { get; set; }
        public List<string> DocumentUrls { get; set; }
    }
}
