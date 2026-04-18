namespace DotLearn.Course.Models.Entities;

public class Course
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string Level { get; set; } = null!;
    public decimal Price { get; set; }
    public CourseState State { get; set; } = CourseState.Draft;
    public Guid InstructorId { get; set; }
    public string? ThumbnailS3Key { get; set; }
    public double AverageRating { get; set; } = 0;
    public int EnrollmentCount { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; set; }
    public string? RejectionReason { get; set; }
}

public enum CourseState
{
    Draft = 0,
    Published = 1,
    Archived = 2,
    PendingApproval = 3,
    Rejected = 4
}
