using Microsoft.EntityFrameworkCore;
using TaskManager.Domain;
using Task = TaskManager.Domain.Task;

namespace TaskManager.DataAccess
{
	public class TaskManagerDbContext : DbContext
	{
		public DbSet<Task> Tasks { get; set; } = null!;

		public DbSet<User> Users { get; set; } = null!;

		public DbSet<UserAvatar> UserAvatar { get; set; } = null!;

		public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options) : base(options)
		{
			Database.EnsureCreated();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Task>()
				.HasOne(t => t.User)
				.WithMany();

			modelBuilder.Entity<UserAvatar>()
				.HasOne(ua => ua.User)
				.WithOne();
		}
	}
}