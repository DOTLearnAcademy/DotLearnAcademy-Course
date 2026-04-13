using Amazon.S3;
using Amazon.S3.Model;
using DotLearn.Course.Models.DTOs;
using DotLearn.Course.Models.Entities;
using DotLearn.Course.Repositories;

namespace DotLearn.Course.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _repo;
    private readonly IAmazonS3 _s3;
    private readonly IConfiguration _config;

    public CourseService(ICourseRepository repo, IAmazonS3 s3, IConfiguration config)
    {
        _repo = repo;
        _s3 = s3;
        _config = config;
    }

    public async Task<CourseResponseDto> CreateAsync(
        CreateCourseRequestDto request, Guid instructorId)
    {
        var course = new Models.Entities.Course
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Category = request.Category,
            Level = request.Level,
            Price = request.Price,
            InstructorId = instructorId,
            State = CourseState.Draft
        };

        await _repo.AddAsync(course);
        return MapToDto(course);
    }

    public async Task<CourseResponseDto?> GetByIdAsync(Guid id)
    {
        var course = await _repo.GetByIdAsync(id);
        return course == null ? null : MapToDto(course);
    }

    public async Task<PaginatedResponseDto<CourseResponseDto>> SearchAsync(
        CourseSearchRequestDto request)
    {
        var (items, total) = await _repo.SearchAsync(
            request.Q, request.Category, request.Level,
            request.MinPrice, request.MaxPrice,
            request.SortBy, request.InstructorOnly, request.InstructorId,
            request.Page, request.PageSize);

        var totalPages = (int)Math.Ceiling((double)total / request.PageSize);

        return new PaginatedResponseDto<CourseResponseDto>(
            items.Select(MapToDto).ToList(),
            total, request.Page, request.PageSize, totalPages);
    }

    public async Task<CourseResponseDto> UpdateAsync(
        Guid id, UpdateCourseRequestDto request,
        Guid requesterId, string requesterRole)
    {
        var course = await GetCourseOrThrow(id);
        ValidateOwnership(course, requesterId, requesterRole);

        if (course.State == CourseState.Archived)
            throw new InvalidOperationException("Cannot edit an archived course.");

        if (request.Title != null) course.Title = request.Title;
        if (request.Description != null) course.Description = request.Description;
        if (request.Category != null) course.Category = request.Category;
        if (request.Level != null) course.Level = request.Level;
        if (request.Price.HasValue) course.Price = request.Price.Value;

        await _repo.UpdateAsync(course);
        return MapToDto(course);
    }

    public async Task<CourseResponseDto> PublishAsync(
        Guid id, Guid requesterId, string requesterRole)
    {
        var course = await GetCourseOrThrow(id);
        ValidateOwnership(course, requesterId, requesterRole);

        if (course.State != CourseState.Draft)
            throw new InvalidOperationException("Only Draft courses can be published.");

        course.State = CourseState.Published;
        course.PublishedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(course);
        // TODO: Publish CoursePublished SQS event in Phase 4
        return MapToDto(course);
    }

    public async Task<CourseResponseDto> ArchiveAsync(
        Guid id, Guid requesterId, string requesterRole)
    {
        var course = await GetCourseOrThrow(id);
        ValidateOwnership(course, requesterId, requesterRole);

        course.State = CourseState.Archived;
        await _repo.UpdateAsync(course);
        return MapToDto(course);
    }

    public async Task<string> GetThumbnailUploadUrlAsync(
        Guid courseId, Guid requesterId)
    {
        var course = await GetCourseOrThrow(courseId);
        var bucket = _config["AWS:AssetsBucket"] ?? "dotlearn-assets-dev";
        var key = $"thumbnails/{courseId}/{Guid.NewGuid()}.jpg";

        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucket,
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(5)
        };

        return _s3.GetPreSignedURL(request);
    }

    public async Task ConfirmThumbnailAsync(
        Guid courseId, string s3Key, Guid requesterId)
    {
        var course = await GetCourseOrThrow(courseId);
        course.ThumbnailS3Key = s3Key;
        await _repo.UpdateAsync(course);
    }

    public async Task<PriceResponseDto> GetPriceAsync(Guid courseId)
    {
        var course = await GetCourseOrThrow(courseId);
        return new PriceResponseDto(course.Id, course.Price);
    }

    // ── Helpers ──────────────────────────────────────────────────
    private async Task<Models.Entities.Course> GetCourseOrThrow(Guid id)
    {
        var course = await _repo.GetByIdAsync(id);
        if (course == null)
            throw new KeyNotFoundException($"Course {id} not found.");
        return course;
    }

    private static void ValidateOwnership(
        Models.Entities.Course course, Guid requesterId, string requesterRole)
    {
        if (requesterRole != "Admin" && course.InstructorId != requesterId)
            throw new UnauthorizedAccessException("You do not own this course.");
    }

    private static CourseResponseDto MapToDto(Models.Entities.Course c) => new(
        c.Id, c.Title, c.Description, c.Category, c.Level,
        c.Price, c.State.ToString(), c.InstructorId,
        c.ThumbnailS3Key, c.AverageRating, c.EnrollmentCount,
        c.CreatedAt, c.PublishedAt
    );
}
