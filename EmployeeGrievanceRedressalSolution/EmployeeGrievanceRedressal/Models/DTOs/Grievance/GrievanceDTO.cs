namespace EmployeeGrievanceRedressal.Models.DTOs.Grievance
{
    public class GrievanceDTO
    {
        public int GrievanceId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int? SolverId { get; set; }
        public string SolverName { get; set; }
        public string Description { get; set; }
        public DateTime DateRaised { get; set; }
        public string Priority { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public List<string> DocumentUrls { get; set; }
    }
}
