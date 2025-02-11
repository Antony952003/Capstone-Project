﻿namespace EmployeeGrievanceRedressal.Models
{
    public class GrievanceHistory
    {
        public int GrievanceHistoryId { get; set; }
        public int GrievanceId { get; set; }
        public Grievance Grievance { get; set; }
        public string StatusChange { get; set; }
        public DateTime DateChanged { get; set; }

        public string HistoryType { get; set; } // e.g., "Solution", "Feedback"
        public int? RelatedEntityId { get; set; }
    }


}
