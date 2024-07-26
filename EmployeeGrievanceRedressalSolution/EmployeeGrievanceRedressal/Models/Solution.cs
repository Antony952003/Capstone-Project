namespace EmployeeGrievanceRedressal.Models
{
    public class Solution
    {
        public int SolutionId { get; set; }
        public int GrievanceId { get; set; }
        public Grievance Grievance { get; set; }
        public int? SolverId { get; set; }
        public User Solver { get; set; }
        public string SolutionTitle { get; set; }
        public string Description { get; set; }
        public DateTime DateProvided { get; set; }
        public Feedback Feedback { get; set; }
        public List<DocumentUrl> DocumentUrls { get; set; } // Changed to string URLs
    }


}
