using DotLearn.Course.Models.DTOs;

namespace DotLearn.Course.Services;

public interface ICourseService
{
    Task<CourseResponseDto> CreateAsync(CreateCourseRequestDto request, Guid instructorId);
    Task<CourseResponseDto?> GetByIdAsync(Guid id);
    Task<PaginatedResponseDto<CourseResponseDto>> SearchAsync(CourseSearchRequestDto request);
    Task<CourseResponseDto> UpdateAsync(Guid id, UpdateCourseRequestDto request, Guid requesterId, string requesterRole);
    Task<CourseResponseDto> PublishAsync(Guid id, Guid requesterId, string requesterRole);
    Task<CourseResponseDto> ArchiveAsync(Guid id, Guid requesterId, string requesterRole);
    Task<string> GetThumbnailUploadUrlAsync(Guid courseId, Guid requesterId);
    Task ConfirmThumbnailAsync(Guid courseId, string s3Key, Guid requesterId);
    Task<PriceResponseDto> GetPriceAsync(Guid courseId);
}
