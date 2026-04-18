using Microsoft.EntityFrameworkCore;

namespace DotLearn.Course.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Add DbSets here
}
