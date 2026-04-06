using DotLearn.Course.Models.DTOs;
using DotLearn.Course.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DotLearn.Course.Controllers;

[ApiController]
[Route("api/courses")]
public class CourseController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CourseController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    // GET /api/courses
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Search([FromQuery] CourseSearchRequestDto request)
    {
        var result = await _courseService.SearchAsync(request);
        return Ok(result);
    }

    // GET /api/courses/{id}
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course == null) return NotFound(new { error = "Course not found." });
        return Ok(course);
    }

    // POST /api/courses
    [HttpPost]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequestDto request)
    {
        var instructorId = GetUserId();
        var result = await _courseService.CreateAsync(request, instructorId);
        return StatusCode(201, result);
    }

    // PUT /api/courses/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<IActionResult> Update(Guid id,
        [FromBody] UpdateCourseRequestDto request)
    {
        try
        {
            var result = await _courseService.UpdateAsync(
                id, request, GetUserId(), GetUserRole());
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    // PUT /api/courses/{id}/publish
    [HttpPut("{id}/publish")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<IActionResult> Publish(Guid id)
    {
        try
        {
            var result = await _courseService.PublishAsync(
                id, GetUserId(), GetUserRole());
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    // DELETE /api/courses/{id} → archives (soft delete)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<IActionResult> Archive(Guid id)
    {
        try
        {
            var result = await _courseService.ArchiveAsync(
                id, GetUserId(), GetUserRole());
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    // POST /api/courses/{id}/thumbnail-upload-url
    [HttpPost("{id}/thumbnail-upload-url")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<IActionResult> GetThumbnailUploadUrl(Guid id)
    {
        try
        {
            var url = await _courseService.GetThumbnailUploadUrlAsync(id, GetUserId());
            return Ok(new { uploadUrl = url });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    // PUT /api/courses/{id}/thumbnail-confirm
    [HttpPut("{id}/thumbnail-confirm")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<IActionResult> ConfirmThumbnail(Guid id,
        [FromBody] ThumbnailConfirmDto request)
    {
        try
        {
            await _courseService.ConfirmThumbnailAsync(id, request.S3Key, GetUserId());
            return Ok(new { message = "Thumbnail updated." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    // GET /internal/courses/{id}/price  ← internal only
    [HttpGet("/internal/courses/{id}/price")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPrice(Guid id)
    {
        try
        {
            var result = await _courseService.GetPriceAsync(id);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    // ── Helpers ──────────────────────────────────────────────────
    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User ID not found in token."));

    private string GetUserRole() =>
        User.FindFirstValue(ClaimTypes.Role)
            ?? throw new UnauthorizedAccessException("Role not found in token.");
}
