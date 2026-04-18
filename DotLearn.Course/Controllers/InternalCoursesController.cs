using DotLearn.Course.Data;
using DotLearn.Course.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotLearn.Course.Services;

namespace DotLearn.Course.Controllers;

[ApiController]
[Route("api/internal/courses")]
public class InternalCoursesController : ControllerBase
{
    private readonly CourseDbContext _db;
    private readonly ICourseRepository _repo;
    private readonly ICourseService _courseService;

    public InternalCoursesController(CourseDbContext db, ICourseRepository repo, ICourseService courseService)
    {
        _db = db;
        _repo = repo;
        _courseService = courseService;
    }

    [HttpGet("{id:guid}/price")]
    public async Task<IActionResult> GetPrice(Guid id)
    {
        try
        {
            var priceData = await _courseService.GetPriceAsync(id);
            return Ok(priceData);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Course not found." });
        }
    }

    [HttpPost("{id:guid}/increment-enrollment")]
    [AllowAnonymous]
    public async Task<IActionResult> IncrementEnrollment(Guid id)
    {
        await _repo.IncrementEnrollmentCountAsync(id);
        return Ok(new { message = "Enrollment count incremented." });
    }
}
