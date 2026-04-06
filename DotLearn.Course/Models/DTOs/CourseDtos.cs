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
    DateTime? PublishedAt
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
    int Page = 1,
    int PageSize = 12
);

public record ThumbnailConfirmDto(string S3Key);

public record PriceResponseDto(Guid CourseId, decimal Price);
