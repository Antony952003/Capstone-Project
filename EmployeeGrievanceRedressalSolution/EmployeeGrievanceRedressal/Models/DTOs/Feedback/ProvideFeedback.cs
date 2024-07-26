namespace EmployeeGrievanceRedressal.Models.DTOs.Feedback
{
    public class ProvideFeedbackDTO
    {
        public int SolutionId { get; set; }
        public int EmployeeId { get; set; }
        public string Comments { get; set; }
        public DateTime DateProvided { get; set; }
    }
}
