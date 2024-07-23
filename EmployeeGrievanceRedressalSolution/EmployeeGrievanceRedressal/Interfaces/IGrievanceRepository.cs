using EmployeeGrievanceRedressal.Models;

namespace EmployeeGrievanceRedressal.Interfaces
{
    public interface IGrievanceRepository : IRepository<Grievance>
    {
        Task<Grievance> GetGrievanceWithSolutionsAsync(int id);
        Task<Grievance> GetGrievanceWithHistoriesAsync(int id);
    }
}
