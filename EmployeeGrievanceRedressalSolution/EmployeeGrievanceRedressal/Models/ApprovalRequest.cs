namespace EmployeeGrievanceRedressal.Models
{
    public class ApprovalRequest
    {
        public int ApprovalRequestId { get; set; }
        public int EmployeeId { get; set; }
        public User Employee { get; set; }
        public DateTime DateRequested { get; set; }
        public ApprovalStatus Status { get; set; }
    }

    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Rejected
    }

}
