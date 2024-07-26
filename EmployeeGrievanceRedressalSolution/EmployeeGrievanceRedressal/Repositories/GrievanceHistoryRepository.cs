using EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces;
using EmployeeGrievanceRedressal.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressal.Repositories
{
    public class GrievanceHistoryRepository : Repository<GrievanceHistory>, IGrievanceHistoryRepository
    {
        public GrievanceHistoryRepository(EmployeeGrievanceContext context) : base(context)
        {
        }

    }
}
