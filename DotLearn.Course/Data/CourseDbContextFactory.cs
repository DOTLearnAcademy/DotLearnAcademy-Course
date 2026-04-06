using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DotLearn.Course.Data;

public class CourseDbContextFactory : IDesignTimeDbContextFactory<CourseDbContext>
{
    public CourseDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CourseDbContext>();
        var connStr = "Server=dotlearn-db.c7ge68ueyfep.ap-southeast-2.rds.amazonaws.com,1433;Database=CourseDb;User Id=admin;Password=DOTLearn@123;TrustServerCertificate=True";
        optionsBuilder.UseSqlServer(connStr);
        return new CourseDbContext(optionsBuilder.Options);
    }
}
