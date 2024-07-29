namespace EmployeeGrievanceRedressal.Models.DTOs.Rating
{
    public class RatingDTO
    {
        public int RatingId { get; set; }
        public int GrievanceId { get; set; }
        public int SolverId { get; set; }
        public int RatingValue { get; set; } 
        public string Comment { get; set; }
    }
}
