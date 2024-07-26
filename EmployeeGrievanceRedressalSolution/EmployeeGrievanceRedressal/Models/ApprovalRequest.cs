namespace EmployeeGrievanceRedressal.Models
{
    public class ApprovalRequest
    {
        public int ApprovalRequestId { get; set; }
        public int EmployeeId { get; set; }
        public User Employee { get; set; }
        public DateTime RequestDate { get; set; }
        public string Reason { get; set; }
        public ApprovalStatus Status { get; set; } // Pending, Approved, Rejected
    }

    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Rejected
    }

}
