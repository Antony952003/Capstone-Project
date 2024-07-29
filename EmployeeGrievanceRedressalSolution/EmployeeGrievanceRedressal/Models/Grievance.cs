namespace EmployeeGrievanceRedressal.Models
{
    public class Grievance
    {
        public int GrievanceId { get; set; }
        public int EmployeeId { get; set; }
        public User Employee { get; set; }
        public int? SolverId { get; set; }
        public User Solver { get; set; }
        public string  Title { get; set; }
        public string Description { get; set; }
        public DateTime DateRaised { get; set; }
        public string Priority { get; set; }
        public GrievanceType Type { get; set; }
        public GrievanceStatus Status { get; set; }
        public List<Solution> Solutions { get; set; }
        public List<DocumentUrl> DocumentUrls { get; set; }
        public List<GrievanceHistory> GrievanceHistories { get; set; }
    }

    public enum GrievancePriority
    {
        Low,
        Medium,
        High
    }

    public enum GrievanceStatus
    {
        Open,
        InProgress,
        Resolved,
        Closed,
        Escalated
    }


}
