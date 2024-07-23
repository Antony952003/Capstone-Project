namespace EmployeeGrievanceRedressal.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public int SolutionId { get; set; }
        public Solution Solution { get; set; }
        public int EmployeeId { get; set; }
        public User Employee { get; set; }
        public string Comments { get; set; }
        public DateTime DateProvided { get; set; }
    }


}
