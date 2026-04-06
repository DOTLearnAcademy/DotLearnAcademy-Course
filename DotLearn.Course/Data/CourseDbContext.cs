using DotLearn.Course.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotLearn.Course.Data;

public class CourseDbContext : DbContext
{
    public CourseDbContext(DbContextOptions<CourseDbContext> options) : base(options) { }

    public DbSet<Models.Entities.Course> Courses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Models.Entities.Course>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Title).IsRequired().HasMaxLength(300);
            entity.Property(c => c.Description).IsRequired();
            entity.Property(c => c.Category).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Level).IsRequired().HasMaxLength(50);
            entity.Property(c => c.Price).HasColumnType("decimal(18,2)");
            entity.Property(c => c.State).HasDefaultValue(CourseState.Draft);
        });
    }
}
