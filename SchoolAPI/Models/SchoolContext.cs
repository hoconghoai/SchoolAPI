using Microsoft.EntityFrameworkCore;

namespace SchoolAPI.Models
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<UserRefreshTokens> UserRefreshTokens { get; set; }
    }
}
