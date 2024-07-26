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
            // Validate if the solution exists
            var solution = await _solutionRepository.GetByIdAsync(dto.SolutionId);
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

            // Create feedback
            var feedback = new Feedback
            {
                SolutionId = dto.SolutionId,
                EmployeeId = dto.EmployeeId,
                Comments = dto.Comments,
                DateProvided = DateTime.Now,
            };

            try
            {
                await _feedbackRepository.Add(feedback);

                var feedbackdto =  new FeedbackDTO
                {
                    FeedbackId = feedback.FeedbackId,
                    SolutionId = feedback.SolutionId,
                    EmployeeId = feedback.EmployeeId,
                    EmployeeName = employee.Name,
                    Comments = feedback.Comments,
                    DateProvided = feedback.DateProvided
                };
                var historyDto = await _grievanceHistoryService.RecordHistoryAsync(solution.GrievanceId, $"Feedback for solution {solution.SolutionTitle} has been provided " +
                    $"Feedback : {feedback.Comments} By {employee.Name}");
                return feedbackdto;
            }
            catch (Exception ex)
            {
                throw new ServiceException("Error providing feedback", ex);
            }
        }

        public async Task<FeedbackDTO> GetFeedbackByIdAsync(int id)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(id);
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
                SolutionId = feedback.SolutionId,
                EmployeeId = feedback.EmployeeId,
                EmployeeName = employee.Name,
                Comments = feedback.Comments,
                DateProvided = feedback.DateProvided
            };
        }
    }
}
