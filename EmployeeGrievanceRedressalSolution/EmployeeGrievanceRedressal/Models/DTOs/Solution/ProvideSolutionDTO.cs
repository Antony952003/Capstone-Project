using System.ComponentModel.DataAnnotations;

namespace EmployeeGrievanceRedressal.Models.DTOs.Solution
{
    public class ProvideSolutionDTO
    {
        [Required]
        public int GrievanceId { get; set; }

        [Required]
        public int SolverId { get; set; }

        [Required]
        public string SolutionTitle { get; set; }

        [Required]
        public string Description { get; set; }

        public List<string> DocumentUrls { get; set; }
    }
}
