using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces;
using EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces;
using EmployeeGrievanceRedressal.Models;
using EmployeeGrievanceRedressal.Models.DTOs.Feedback;
using EmployeeGrievanceRedressal.Models.DTOs.Grievance;
using System;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressal.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly ISolutionRepository _solutionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGrievanceHistoryService _grievanceHistoryService;

        public FeedbackService(
            IFeedbackRepository feedbackRepository,
            ISolutionRepository solutionRepository,
            IUserRepository userRepository,
            IGrievanceHistoryService grievanceHistoryService)
        {
            _feedbackRepository = feedbackRepository;
            _solutionRepository = solutionRepository;
            _userRepository = userRepository;
            _grievanceHistoryService = grievanceHistoryService;
        }

        public async Task<FeedbackDTO> ProvideFeedbackAsync(ProvideFeedbackDTO dto)
        {
            try
            {
                // Validate if the solution exists
                var solution = await _solutionRepository.GetByIdAsync(dto.SolutionId);
            var solver = await _userRepository.GetByIdAsync((int)solution.SolverId);
            if (solution == null)
            {
                throw new EntityNotFoundException($"Solution with ID {dto.SolutionId} not found.");
            }

            // Validate if the employee exists
            var employee = await _userRepository.GetByIdAsync(dto.EmployeeId);
            if (employee == null)
            {
                throw new EntityNotFoundException($"Employee with ID {dto.EmployeeId} not found.");
            }
            var feedbacks = await _feedbackRepository.GetAllAsync();
            var existingfeedback = feedbacks.FirstOrDefault(x => x.SolutionId == solution.SolutionId);
            if(existingfeedback != null) {
                throw new Exception("There is already a feedback given for this solution!!");
            }
            var feedback = new Feedback
            {
                SolutionId = dto.SolutionId,
                EmployeeId = dto.EmployeeId,
                Comments = dto.Comments,
                DateProvided = DateTime.Now,
            };

                await _feedbackRepository.Add(feedback);

                var feedbackdto =  new FeedbackDTO
                {
                    FeedbackId = feedback.FeedbackId,
                    SolutionTitle = solution.SolutionTitle,
                    SolverName = solver.Name,
                    EmployeeId = feedback.EmployeeId,
                    EmployeeName = employee.Name,
                    Comments = feedback.Comments,
                    DateProvided = feedback.DateProvided
                };
                var history = new GrievanceHistory
                {
                    GrievanceId = solution.GrievanceId,
                    HistoryType = "Feedback",
                    RelatedEntityId = feedback.FeedbackId,
                    DateChanged = DateTime.UtcNow,
                    StatusChange = $"Feedback Description : {feedback.Comments}",
                };
                var historyDto = await _grievanceHistoryService.RecordHistoryAsync(history);
               
                return feedbackdto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<FeedbackDTO> GetFeedbackByIdAsync(int id)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(id);
            var solution = await _solutionRepository.GetByIdAsync(feedback.SolutionId);
            var solver = await _userRepository.GetByIdAsync((int)solution.SolverId);
            if (feedback == null)
            {
                throw new EntityNotFoundException($"Feedback with ID {id} not found.");
            }

            var employee = await _userRepository.GetByIdAsync(feedback.EmployeeId);
            if (employee == null)
            {
                throw new EntityNotFoundException($"Employee with ID {feedback.EmployeeId} not found.");
            }

            return new FeedbackDTO
            {
                FeedbackId = feedback.FeedbackId,
                SolutionTitle = solution.SolutionTitle,
                SolverName = solver.Name,
                EmployeeId = feedback.EmployeeId,
                EmployeeName = employee.Name,
                Comments = feedback.Comments,
                DateProvided = feedback.DateProvided
            };
        }
    }
}
