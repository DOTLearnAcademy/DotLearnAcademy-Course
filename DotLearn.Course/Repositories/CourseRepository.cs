using DotLearn.Course.Data;
using DotLearn.Course.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotLearn.Course.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly CourseDbContext _context;

    public CourseRepository(CourseDbContext context)
    {
        _context = context;
    }

    public async Task<Models.Entities.Course?> GetByIdAsync(Guid id) =>
        await _context.Courses.FindAsync(id);

    public async Task<(List<Models.Entities.Course> Items, int Total)> SearchAsync(
        string? q, string? category, string? level,
        decimal? minPrice, decimal? maxPrice,
        string? sortBy, bool instructorOnly, Guid? instructorId, int page, int pageSize)
    {
        IQueryable<Models.Entities.Course> query;

        if (instructorOnly && instructorId.HasValue)
        {
            // Instructor view: show ALL their courses (Draft, Published, Archived)
            query = _context.Courses
                .Where(c => c.InstructorId == instructorId.Value);
        }
        else
        {
            // Public view: only Published courses
            query = _context.Courses
                .Where(c => c.State == CourseState.Published);
        }

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(c => c.Title.Contains(q) || c.Description.Contains(q));

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(c => c.Category == category);

        if (!string.IsNullOrWhiteSpace(level))
            query = query.Where(c => c.Level == level);

        if (minPrice.HasValue)
            query = query.Where(c => c.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(c => c.Price <= maxPrice.Value);

        query = sortBy switch
        {
            "price_asc"  => query.OrderBy(c => c.Price),
            "price_desc" => query.OrderByDescending(c => c.Price),
            "rating"     => query.OrderByDescending(c => c.AverageRating),
            _            => query.OrderByDescending(c => c.EnrollmentCount)
        };

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task AddAsync(Models.Entities.Course course)
    {
        await _context.Courses.AddAsync(course);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Models.Entities.Course course)
    {
        _context.Courses.Update(course);
        await _context.SaveChangesAsync();
    }
}
