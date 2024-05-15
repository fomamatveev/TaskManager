using Microsoft.EntityFrameworkCore;

namespace Task1305.DataAccess
{
    public class TaskManagerDbContext : DbContext
    {
        public DbSet<Domain.Task> Tasks { get; set; } = null!;

        public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options) : base(options) 
        {
            Database.EnsureCreated();
        }
    }
}
