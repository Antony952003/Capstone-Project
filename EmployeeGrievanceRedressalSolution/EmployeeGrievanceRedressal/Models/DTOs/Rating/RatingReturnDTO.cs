namespace EmployeeGrievanceRedressal.Models.DTOs.Rating
{
    public class RatingReturnDTO
    {
        public string EmployeeName { get; set; }
        public string EmployeeImage { get; set; }
        public string GrievanceTitle { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
