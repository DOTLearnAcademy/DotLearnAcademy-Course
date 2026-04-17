using DotLearn.Course.Models.Entities;

namespace DotLearn.Course.Repositories;

public interface ICourseRepository
{
    Task<Models.Entities.Course?> GetByIdAsync(Guid id);
    Task<(List<Models.Entities.Course> Items, int Total)> SearchAsync(string? q, string? category,
        string? level, decimal? minPrice, decimal? maxPrice,
        string? sortBy, bool instructorOnly, Guid? instructorId, int page, int pageSize);
    Task AddAsync(Models.Entities.Course course);
    Task UpdateAsync(Models.Entities.Course course);
    Task IncrementEnrollmentCountAsync(Guid courseId);
}
