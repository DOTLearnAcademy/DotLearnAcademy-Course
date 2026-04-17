namespace DotLearn.Course.Models.DTOs;

public class EnrollmentCompletedEventDto
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
}
