namespace EmployeeGrievanceRedressal.Models.DTOs.ApprovalRequest
{
    public class ApprovalRequestDTO
    {
        public int ApprovalRequestId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime RequestDate { get; set; }
        public string Reason { get; set; }
        public ApprovalStatus Status { get; set; }
    }
}
