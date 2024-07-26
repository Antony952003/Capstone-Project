using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces;
using EmployeeGrievanceRedressal.Models;
using EmployeeGrievanceRedressal.Models.DTOs.Solution;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressal.Repositories
{
    public class SolutionRepository : Repository<Solution>, ISolutionRepository
    {
        public SolutionRepository(EmployeeGrievanceContext context) : base(context)
        {
        }

        public async Task<List<Solution>> GetAllSolutionswithSolver()
        {
            try
            {
                var solutions = await _context.Solutions
                    .Include(x => x.DocumentUrls)
                    .Include(x => x.Solver)
                    .ToListAsync();
                if(!solutions.Any()) {
                    throw new EntityNotFoundException("No Solutions are found !!");
                }
                return solutions;
            }
            catch(EntityNotFoundException ex)
            {
                throw ex;
            }
            catch(Exception ex)
            {
                throw new RepositoryException("Error in fetching the solutions !!", ex);
            }
        }

        public async Task<Solution> GetSolutionByIdwithSolver(int id)
        {
            try
            {
                var solution = _context.Solutions
                    .Include(x => x.DocumentUrls)
                    .Include(x => x.Solver)
                    .FirstOrDefault(x => x.SolutionId == id);


                if (solution == null)
                {
                    throw new EntityNotFoundException("No Solutions are found !!");
                }
                return solution;
            }
            catch (EntityNotFoundException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error in fetching the solutions !!", ex);
            }
        }
    }
}
