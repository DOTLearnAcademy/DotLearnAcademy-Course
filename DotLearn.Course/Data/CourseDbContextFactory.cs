using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DotLearn.Course.Data;

public class CourseDbContextFactory : IDesignTimeDbContextFactory<CourseDbContext>
{
    public CourseDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CourseDbContext>();
        optionsBuilder.UseSqlServer("Server=placeholder;Database=CourseDb;");
        return new CourseDbContext(optionsBuilder.Options);
    }
}
