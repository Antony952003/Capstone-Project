namespace EmployeeGrievanceRedressal.Models
{
    public class DocumentUrl
    {
        public int DocumentUrlId { get; set; }
        public int GrievanceId { get; set; }
        public string Url { get; set; }
        public Grievance Grievance { get; set; }
    }
}
