using DotLearn.Course.Models.DTOs;
using DotLearn.Course.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DotLearn.Course.Controllers;

[ApiController]
[Route("api/admin/courses")]
[Authorize(Roles = "Admin")]
public class AdminCoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public AdminCoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingCourses()
    {
        var courses = await _courseService.GetPendingCoursesAsync();
        return Ok(courses);
    }

    [HttpPut("{id:guid}/approve")]
    public async Task<IActionResult> ApproveCourse(Guid id)
    {
        try
        {
            var adminIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(adminIdStr, out var adminId)) return Unauthorized();

            var result = await _courseService.ApproveCourseAsync(id, adminId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Course not found." });
        }
    }

    [HttpPut("{id:guid}/reject")]
    public async Task<IActionResult> RejectCourse(Guid id, [FromBody] RejectCourseRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
                return BadRequest(new { message = "A rejection reason must be provided." });

            var adminIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(adminIdStr, out var adminId)) return Unauthorized();

            var result = await _courseService.RejectCourseAsync(id, adminId, request.Reason);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Course not found." });
        }
    }
}
