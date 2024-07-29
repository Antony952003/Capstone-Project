namespace EmployeeGrievanceRedressal.Models
{
    public class Rating
    {
        public int RatingId { get; set; }
        public int GrievanceId { get; set; }
        public Grievance Grievance { get; set; }
        public int SolverId { get; set; }
        public User Solver { get; set; }
        public int RatingValue { get; set; } // Assuming a scale from 1 to 5
        public string Comment { get; set; }
        public DateTime DateProvided { get; set; } = DateTime.UtcNow;
    }
}
