namespace EmployeeGrievanceRedressal.Models.DTOs.GrievanceHistory
{
    public class GrievanceHistoryDTO
    {
        public int GrievanceHistoryId { get; set; }
        public int GrievanceId { get; set; }
        public string StatusChange { get; set; }
        public DateTime DateChanged { get; set; }
        public string HistoryType { get; set; }
        public int? RelatedEntityId { get; set; }
    }
}
