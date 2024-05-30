using Microsoft.EntityFrameworkCore;
using Task = TaskManager.Domain.Task;

namespace TaskManager.DataAccess
{
    public class TaskManagerDbContext : DbContext
    {
        public DbSet<Domain.Task> Tasks { get; set; } = null!;
        public DbSet<Domain.User> Users { get; set; } = null!;

        public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Task>()
                .HasOne(t => t.User)
                .WithMany();
        }
    }
}
