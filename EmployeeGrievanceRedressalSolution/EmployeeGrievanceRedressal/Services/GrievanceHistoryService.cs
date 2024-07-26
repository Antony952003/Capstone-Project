using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces;
using EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces;
using EmployeeGrievanceRedressal.Models;
using EmployeeGrievanceRedressal.Models.DTOs.GrievanceHistory;
using System;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressal.Services
{
    public class GrievanceHistoryService : IGrievanceHistoryService
    {
        private readonly IGrievanceHistoryRepository _grievanceHistoryRepository;

        public GrievanceHistoryService(IGrievanceHistoryRepository grievanceHistoryRepository)
        {
            _grievanceHistoryRepository = grievanceHistoryRepository;
        }

        public async Task<GrievanceHistoryDTO> RecordHistoryAsync(int grievanceId, string statusChange)
        {
            try
            {
                var history = new GrievanceHistory
                {
                    GrievanceId = grievanceId,
                    StatusChange = statusChange,
                    DateChanged = DateTime.UtcNow,

                };

                await _grievanceHistoryRepository.Add(history);

                return new GrievanceHistoryDTO
                {
                    GrievanceHistoryId = history.GrievanceHistoryId,
                    StatusChange = statusChange,
                    DateChanged = history.DateChanged,
                    GrievanceId = history.GrievanceId,
                };
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error in Grievance History", ex);
            }
        }
        public async Task<IEnumerable<GrievanceHistoryDTO>> GetGrievanceHistoryAsync(int grievanceId)
        {
            try
            {
                var histories = await _grievanceHistoryRepository.GetAllAsync();
                if(!histories.Any())
                {
                    throw new EntityNotFoundException("There is no Grievance history found!!");
                }

                return histories.Where(x => x.GrievanceId == grievanceId).OrderBy(h => h.DateChanged)
                                .Select(h => new GrievanceHistoryDTO
                                {
                                    GrievanceHistoryId = h.GrievanceHistoryId,
                                    GrievanceId = h.GrievanceId,
                                    StatusChange = h.StatusChange,
                                    DateChanged = h.DateChanged
                                });
            }
            catch(EntityNotFoundException ex)
            {
                throw ex;
            }
            catch(Exception ex)
            {
                throw new RepositoryException("Error in fetching the GrievanceHistory", ex);
            }
        }
    }
}
