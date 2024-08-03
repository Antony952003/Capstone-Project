using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces;
using EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces;
using EmployeeGrievanceRedressal.Models;
using EmployeeGrievanceRedressal.Models.DTOs.Grievance;
using Microsoft.AspNetCore.Identity;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EmployeeGrievanceContext _context;

        public GrievanceService(IGrievanceRepository grievanceRepository, IUserRepository userRepository,
            EmployeeGrievanceContext context,
            IGrievanceHistoryService grievanceHistoryService,
            IHttpContextAccessor httpContextAccessor)
        {
            _grievanceRepository = grievanceRepository;
            _userRepository = userRepository;
            _grievanceHistoryService = grievanceHistoryService;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<GrievanceDTO>> GetAllEmployeeGrievancesAsync(int employeeId)
        {
            try
            {
                var employeegrievances = await _grievanceRepository.GetAllEmployeeGrievancesAsync(employeeId);
                return employeegrievances;
            }
            catch (Exception ex)
            {
                // Log exception details here if needed
                throw ex;
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
            try
            {
                var employee = await _userRepository.GetByIdAsync(employeeId);
                if (employee == null)
                {
                    throw new EntityNotFoundException("Employee not found.");
                }
                var grievances = await _grievanceRepository.GetAllEmployeeGrievancesAsync(employeeId);
                var pendinggrievance = grievances.FirstOrDefault(x => x.Status != GrievanceStatus.Closed.ToString());
                if (pendinggrievance != null)
                {
                    throw new Exception("Cannot raise a new grievance when there is a grievance not Closed !!");
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
                    Title = model.Title,
                    Type = grievanceType,
                    DocumentUrls = model?.DocumentUrls?.Select(url => new DocumentUrl { Url = url }).ToList()
                };

                await _grievanceRepository.Add(grievance);

                var user = _httpContextAccessor.HttpContext.User;
                var userId = Convert.ToInt32(user.FindFirst("uid")?.Value);
                if (userId == null)
                {
                    throw new EntityNotFoundException("User not found!!");
                }
                var Employeeuser = await _userRepository.GetByIdAsync(userId);

                var history = new GrievanceHistory
                {
                    GrievanceId = grievance.GrievanceId,
                    HistoryType = "Raised Grievance",
                    RelatedEntityId = null,
                    DateChanged = DateTime.UtcNow,
                    StatusChange = $"Grievance is Raised by {Employeeuser.Name}",

                };
                var historyDto = await _grievanceHistoryService.RecordHistoryAsync(history);

                return MapToGrievanceDTO(grievance);
            }
            catch(EntityNotFoundException ex)
            {
                throw ex;
            }
            catch(Exception ex)
            {
                throw ex;
            }
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

            if(solver.IsAvailable == false)
            {
                throw new Exception("Solver already assigned to a grievance");
            }
            if(grievance.Status == GrievanceStatus.Escalated)
            {
                var existingsolver = await _userRepository.GetByIdAsync((int)grievance.SolverId);
                existingsolver.IsAvailable = true;
                _userRepository.Update(existingsolver);
            }

            grievance.SolverId = solverId;
            grievance.Status = GrievanceStatus.InProgress;
            _grievanceRepository.Update(grievance);

            solver.IsAvailable = false;
            _userRepository.Update(solver);
            var grievanceDTO = await _grievanceRepository.GetGrievanceWithSolver(grievanceId);
            var history = new GrievanceHistory
            {
                GrievanceId = grievance.GrievanceId,
                HistoryType = "Assign Grievance",
                RelatedEntityId = null,
                DateChanged = DateTime.UtcNow,
                StatusChange = $"Grievance is Assigned to {grievanceDTO.SolverName}",
            };
            var historyDto = await _grievanceHistoryService.RecordHistoryAsync(history);

            return grievanceDTO;
        }
        private GrievanceDTO MapToGrievanceDTO(Grievance grievance)
        {
            return new GrievanceDTO
            {
                GrievanceId = grievance.GrievanceId,
                EmployeeId = grievance.EmployeeId,
                EmployeeName = grievance.Employee?.Name,
                EmployeeImage= grievance.Employee?.UserImage,
                Title = grievance.Title,
                SolverId = grievance.SolverId, 
                SolverName = grievance.Solver?.Name,  
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
                var grievanceDTO = await _grievanceRepository.GetGrievanceWithSolver(grievanceId);
                var history = new GrievanceHistory
                {
                    GrievanceId = grievance.GrievanceId,
                    HistoryType = "Grievance Resolved",
                    RelatedEntityId = null,
                    DateChanged = DateTime.UtcNow,
                    StatusChange = $"Grievance has been resolved by {grievanceDTO.SolverName}",
                };
                var historyDto = await _grievanceHistoryService.RecordHistoryAsync(history);

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

        public async Task<GrievanceDTO> CloseGrievanceAsync(CloseGrievanceDTO closeGrievanceDTO)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var grievance = await _grievanceRepository.GetByIdAsync(closeGrievanceDTO.GrievanceId);
                    if (grievance == null)
                    {
                        throw new EntityNotFoundException($"Grievance with ID {closeGrievanceDTO.GrievanceId} not found.");
                    }

                    if(grievance.Status == GrievanceStatus.Closed)
                    {
                        throw new ServiceException("Grievance is already been closed, make another grievance request");
                    }

                    if (grievance.Status != GrievanceStatus.Resolved)
                    {
                        if(closeGrievanceDTO.ForceClose == false)
                            throw new ServiceException("Grievance cannot be closed before getting resolved");
                    }

                    grievance.Status = GrievanceStatus.Closed;
                    _grievanceRepository.Update(grievance);

                    var solver = await _userRepository.GetByIdAsync((int)grievance.SolverId);
                    solver.IsAvailable = true;
                    _userRepository.Update(solver);
                    var grievancedto = await _grievanceRepository.GetGrievanceWithSolver(closeGrievanceDTO.GrievanceId);
                    var user = _httpContextAccessor.HttpContext.User;
                    var userId = Convert.ToInt32(user.FindFirst("uid")?.Value);
                    if (userId == null)
                    {
                        throw new EntityNotFoundException("User not found!!");
                    }
                    var adminuser = await _userRepository.GetByIdAsync(userId);

                    var history = new GrievanceHistory
                    {
                        GrievanceId = grievance.GrievanceId,
                        HistoryType = "Closed Grievance",
                        RelatedEntityId = null,
                        DateChanged = DateTime.UtcNow,
                        StatusChange = $"Grievance is Closed by {adminuser.Name}",
                    };
                    var historyDto = await _grievanceHistoryService.RecordHistoryAsync(history);

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


                    var user = _httpContextAccessor.HttpContext.User;
                    var userId = Convert.ToInt32(user.FindFirst("uid")?.Value);
                    if (userId == null)
                    {
                        throw new EntityNotFoundException("User not found!!");
                    }
                    var solveruser = await _userRepository.GetByIdAsync(userId);

                    var history = new GrievanceHistory
                    {
                        GrievanceId = grievance.GrievanceId,
                        HistoryType = "Escalated Grievance",
                        RelatedEntityId = null,
                        DateChanged = DateTime.UtcNow,
                        StatusChange = $"Grievance is Escalated by {solveruser.Name}",
                    };
                    var historyDto = await _grievanceHistoryService.RecordHistoryAsync(history);
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

        public async Task<IEnumerable<GrievanceDTO>> GetAllGrievancesBySolver(int solverId)
        {
            var grievances = await _grievanceRepository.GetGrievancesWithEmployee();
            grievances = grievances.FindAll(x => x.SolverId == solverId);
            return grievances.Select(MapToGrievanceDTO);

        }
    }
}
