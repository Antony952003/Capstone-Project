using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeGrievanceRedressal.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly EmployeeGrievanceContext _context;

        public Repository(EmployeeGrievanceContext context)
        {
            _context = context;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _context.Set<T>().FindAsync(id);
                if (entity == null)
                {
                    throw new EntityNotFoundException($"{typeof(T).Name} with ID {id} not found.");
                }
                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                var allentities = await _context.Set<T>().ToListAsync();
                if (allentities.Count == 0)
                {
                    throw new EntityNotFoundException("No Entities are found !!");
                }
                return allentities;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error getting all entities", ex);
            }
        }

        public async Task Add(T entity)
        {
            try
            {
                _context.Set<T>().Add(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error adding entity", ex);
            }
        }

        public void Update(T entity)
        {
            try
            {
                _context.Set<T>().Update(entity);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating entity", ex);
            }
        }

        public void Remove(T entity)
        {
            try
            {
                _context.Set<T>().Remove(entity);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error removing entity", ex);
            }
        }
    }
}
