namespace EmployeeGrievanceRedressal.Models.DTOs.Feedback
{
    public class FeedbackDTO
    {
        public int FeedbackId { get; set; }
        public string SolutionTitle { get; set; }
        public string SolverName { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Comments { get; set; }
        public DateTime DateProvided { get; set; }
    }
}
