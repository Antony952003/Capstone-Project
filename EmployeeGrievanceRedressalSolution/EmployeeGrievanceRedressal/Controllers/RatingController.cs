using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces;
using EmployeeGrievanceRedressal.Models.DTOs.Rating;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RatingsController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingsController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    [HttpPost]
    [Route("ProvideRating")]
    public async Task<ActionResult<RatingDTO>> ProvideRating(RatingDTO ratingDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _ratingService.ProvideRatingAsync(ratingDto);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(401, new { Message = "Unauthorized access error occurred.", Details = ex.Message });
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ServiceException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
