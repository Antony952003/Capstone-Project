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

        public async Task<GrievanceHistoryDTO> RecordHistoryAsync(GrievanceHistory history)
        {
            try
            {
                var grievancehistory = new GrievanceHistory
                {
                    GrievanceId = history.GrievanceId,
                    StatusChange = history.StatusChange,
                    DateChanged = DateTime.UtcNow,
                    HistoryType = history.HistoryType,
                    RelatedEntityId = history.RelatedEntityId,
                };

                await _grievanceHistoryRepository.Add(grievancehistory);

                return new GrievanceHistoryDTO
                {
                    GrievanceId = grievancehistory.GrievanceId,
                    StatusChange = grievancehistory.StatusChange,
                    DateChanged = grievancehistory.DateChanged,
                    HistoryType = grievancehistory.HistoryType,
                    RelatedEntityId = grievancehistory.RelatedEntityId,
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

                return histories.Where(x => x.GrievanceId == grievanceId)
                                .OrderBy(h => h.DateChanged)
                                .Select(h => new GrievanceHistoryDTO
                                {
                                    GrievanceHistoryId = h.GrievanceHistoryId,
                                    GrievanceId = h.GrievanceId,
                                    StatusChange = h.StatusChange,
                                    DateChanged = h.DateChanged,
                                    RelatedEntityId= h.RelatedEntityId,
                                    HistoryType = h.HistoryType,
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
