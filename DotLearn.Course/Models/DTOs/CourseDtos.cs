namespace DotLearn.Course.Models.DTOs;

public record CreateCourseRequestDto(
    string Title,
    string Description,
    string Category,
    string Level,
    decimal Price
);

public record UpdateCourseRequestDto(
    string? Title,
    string? Description,
    string? Category,
    string? Level,
    decimal? Price
);

public record CourseResponseDto(
    Guid Id,
    string Title,
    string Description,
    string Category,
    string Level,
    decimal Price,
    string State,
    Guid InstructorId,
    string? ThumbnailS3Key,
    double AverageRating,
    int EnrollmentCount,
    DateTime CreatedAt,
    DateTime? PublishedAt,
    string? RejectionReason = null
);

public record PaginatedResponseDto<T>(
    List<T> Items,
    int Total,
    int Page,
    int PageSize,
    int TotalPages
);

public record CourseSearchRequestDto(
    string? Q,
    string? Category,
    string? Level,
    decimal? MinPrice,
    decimal? MaxPrice,
    string? SortBy,
    bool InstructorOnly = false,
    Guid? InstructorId = null,
    int Page = 1,
    int PageSize = 12
);

public record ThumbnailConfirmDto(string S3Key);

public record PriceResponseDto(
    Guid CourseId, 
    string Title,
    decimal Price, 
    string Currency,
    bool IsPublished,
    bool IsFree
);

public record RejectCourseRequestDto(string Reason);
