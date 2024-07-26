using EmployeeGrievanceRedressal.Models;
using EmployeeGrievanceRedressal.Models.DTOs.Solution;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces
{
    public interface ISolutionRepository : IRepository<Solution>
    {
        Task<List<Solution>> GetAllSolutionswithSolver();
        Task<Solution> GetSolutionByIdwithSolver(int id);
    }
}
