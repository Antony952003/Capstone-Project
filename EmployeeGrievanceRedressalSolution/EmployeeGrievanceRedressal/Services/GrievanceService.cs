using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces;
using EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces;
using EmployeeGrievanceRedressal.Models;
using EmployeeGrievanceRedressal.Models.DTOs.Grievance;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressal.Services
{
    public class GrievanceService : IGrievanceService
    {
        private readonly IGrievanceRepository _grievanceRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGrievanceHistoryService _grievanceHistoryService;
        private readonly EmployeeGrievanceContext _context;

        public GrievanceService(IGrievanceRepository grievanceRepository, IUserRepository userRepository,
            EmployeeGrievanceContext context,
            IGrievanceHistoryService grievanceHistoryService)
        {
            _grievanceRepository = grievanceRepository;
            _userRepository = userRepository;
            _grievanceHistoryService = grievanceHistoryService;
            _context = context;
        }

        public async Task<IEnumerable<GrievanceDTO>> GetAllEmployeeGrievancesAsync(int employeeId)
        {
            try
            {
                var employeegrievances = await _grievanceRepository.GetAllEmployeeGrievancesAsync(employeeId);
                if(employeegrievances.Count() == 0)
                {
                    throw new EntityNotFoundException("No Employee Grievances are found !!");
                }
                return employeegrievances;
            }
            catch (Exception ex)
            {
                // Log exception details here if needed
                throw new ServiceException("Error retrieving employee grievances", ex);
            }
        }

        public async Task<IEnumerable<GrievanceDTO>> GetAllGrievancesByTypeAsync(string grievanceType)
        {
            try
            {
                return await _grievanceRepository.GetAllGrievancesByTypeAsync(grievanceType);
            }
            catch (Exception ex)
            {
                // Log exception details here if needed
                throw new ServiceException("Error retrieving grievances by type", ex);
            }
        }

        public async Task<GrievanceDTO> RaiseGrievanceAsync(CreateGrievanceDTO model, int employeeId)
        {
            var employee = await _userRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                throw new EntityNotFoundException("Employee not found.");
            }

            if (!Enum.TryParse<GrievanceType>(model.Type, out var grievanceType))
            {
                throw new InvalidOperationException("Invalid grievance type.");
            }

            var grievance = new Grievance
            {
                EmployeeId = employeeId,
                Description = model.Description,
                DateRaised = DateTime.UtcNow,
                Priority = model.Priority,
                Status = GrievanceStatus.Open,
                Type = grievanceType,
                DocumentUrls = model.DocumentUrls.Select(url => new DocumentUrl { Url = url }).ToList()
            };

            await _grievanceRepository.Add(grievance);

            return MapToGrievanceDTO(grievance);
        }

        public async Task<IEnumerable<GrievanceDTO>> GetAllGrievancesAsync()
        {
            var grievances = await _grievanceRepository.GetGrievancesWithEmployee();
            return grievances.Select(MapToGrievanceDTO);
        }

        public async Task<GrievanceDTO> AssignGrievanceAsync(int grievanceId, int solverId)
        {
            var grievance = await _grievanceRepository.GetByIdAsync(grievanceId);
            if (grievance == null)
            {
                throw new EntityNotFoundException("Grievance not found.");
            }

            var solver = await _userRepository.GetByIdAsync(solverId);
            if (solver == null || solver.Role != UserRole.Solver)
            {
                throw new InvalidOperationException("Invalid solver.");
            }

            grievance.SolverId = solverId;
            grievance.Status = GrievanceStatus.InProgress;
            _grievanceRepository.Update(grievance);
            var grievanceDTO = await _grievanceRepository.GetGrievanceWithSolver(grievanceId);
            var historyDto = await _grievanceHistoryService.RecordHistoryAsync(grievanceId, $"Grievance By : {grievanceDTO.EmployeeName} is assigned to solver : {grievanceDTO.SolverName}");


            return grievanceDTO;
        }
        private GrievanceDTO MapToGrievanceDTO(Grievance grievance)
        {
            return new GrievanceDTO
            {
                GrievanceId = grievance.GrievanceId,
                EmployeeId = grievance.EmployeeId,
                EmployeeName = grievance.Employee?.Name,  // Safe navigation operator
                SolverId = grievance.SolverId, // No need to check if SolverId is null
                SolverName = grievance.Solver?.Name,  // Safe navigation operator
                Description = grievance.Description,
                DateRaised = grievance.DateRaised,
                Priority = grievance.Priority,
                Type = grievance.Type.ToString(), // Assuming Type is an enum
                Status = grievance.Status.ToString(), // Assuming Status is an enum
                DocumentUrls = grievance.DocumentUrls?.Select(d => d.Url).ToList() // Safe navigation operator
            };
        }

        public async Task<GrievanceDTO> GetGrievanceByIdAsync(int id)
        {
            var grievanceDTO = await _grievanceRepository.GetGrievanceWithSolver(id);

            return grievanceDTO;
        }
        public async Task<GrievanceDTO> MarkAsResolvedAsync(int grievanceId)
        {
            try
            {
                var grievance = await _grievanceRepository.GetByIdAsync(grievanceId);
                if (grievance == null)
                {
                    throw new EntityNotFoundException($"Grievance with ID {grievanceId} not found.");
                }

                if(grievance.Status == GrievanceStatus.Resolved)
                {
                    throw new ServiceException("Grievance is already resolved !!");
                }

                grievance.Status = GrievanceStatus.Resolved;
                _grievanceRepository.Update(grievance);
                await _grievanceHistoryService.RecordHistoryAsync(grievanceId, $"Grievance has been resolved");

                var returngrievance = await _grievanceRepository.GetGrievanceWithSolver(grievanceId);
                return returngrievance;
            }
            catch(ServiceException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new ServiceException("Error marking grievance as resolved", ex);
            }
        }

        public async Task<GrievanceDTO> CloseGrievanceAsync(int grievanceId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var grievance = await _grievanceRepository.GetByIdAsync(grievanceId);
                    if (grievance == null)
                    {
                        throw new EntityNotFoundException($"Grievance with ID {grievanceId} not found.");
                    }

                    if(grievance.Status == GrievanceStatus.Closed)
                    {
                        throw new ServiceException("Grievance is already been closed, make another grievance request");
                    }

                    if (grievance.Status != GrievanceStatus.Resolved)
                    {
                        throw new ServiceException("Grievance cannot be closed before getting resolved");
                    }

                    grievance.Status = GrievanceStatus.Closed;
                    _grievanceRepository.Update(grievance);

                    var grievancedto = await _grievanceRepository.GetGrievanceWithSolver(grievanceId);

                    await _grievanceHistoryService.RecordHistoryAsync(grievanceId, "Grievance closed by admin");

                    // Commit transaction
                    await transaction.CommitAsync();

                    return grievancedto;
                }
                catch (ServiceException ex)
                {
                    // Rollback transaction
                    await transaction.RollbackAsync();
                    throw ex;
                }
                catch (Exception ex)
                {
                    // Rollback transaction
                    await transaction.RollbackAsync();
                    throw new ServiceException("Error closing grievance", ex);
                }
            }
        }
        public async Task<GrievanceDTO> EscalateGrievanceAsync(EscalateGrievanceDTO model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var grievance = await _grievanceRepository.GetByIdAsync(model.GrievanceId);
                    if (grievance == null)
                    {
                        throw new EntityNotFoundException($"Grievance with ID {model.GrievanceId} not found.");
                    }

                    if (grievance.Status != GrievanceStatus.InProgress)
                    {
                        throw new ServiceException("Only grievances in progress can be escalated.");
                    }

                    grievance.Status = GrievanceStatus.Escalated;
                    _grievanceRepository.Update(grievance);
                    var grievancedto = await _grievanceRepository.GetGrievanceWithSolver(grievance.GrievanceId);


                    await _grievanceHistoryService.RecordHistoryAsync(grievance.GrievanceId, $"Grievance escalated by solver({grievancedto.SolverName}). Reason: " + model.Reason);

                    await transaction.CommitAsync();

                    return grievancedto;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new ServiceException("Error escalating grievance", ex);
                }
            }
        }

        public async Task<IEnumerable<GrievanceDTO>> GetAllOpenGrievancesAsync()
        {
            var grievances = await _grievanceRepository.GetGrievancesWithEmployee();
            grievances = grievances.FindAll(x => x.Status == GrievanceStatus.Open);
            return grievances.Select(MapToGrievanceDTO);
        }
    }
}
