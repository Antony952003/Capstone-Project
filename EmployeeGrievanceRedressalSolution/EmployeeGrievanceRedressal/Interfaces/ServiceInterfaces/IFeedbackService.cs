using EmployeeGrievanceRedressal.Models;
using EmployeeGrievanceRedressal.Models.DTOs.Feedback;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces
{
    public interface IFeedbackService
    {
        Task<FeedbackDTO> ProvideFeedbackAsync(ProvideFeedbackDTO dto);
        Task<FeedbackDTO> GetFeedbackByIdAsync(int id);
    }
}
