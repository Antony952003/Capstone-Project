namespace EmployeeGrievanceRedressal.Models.DTOs.Solution
{
    public class SolutionDTO
    {
        public int SolutionId { get; set; }
        public string SolutionTitle { get; set; }
        public string Description { get; set; }
        public DateTime DateProvided { get; set; }
        public int GrievanceId { get; set; }
        public string? SolverName { get; set; }
        public int? SolverId { get; set; }
        public List<string>? DocumentUrls { get; set; }
    }
}
