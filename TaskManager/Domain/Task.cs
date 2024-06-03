using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Domain
{
	public class Task
	{
		public Guid Id { get; set; }

		public string Description { get; set; }

		public Guid UserId { get; set; }

		[ForeignKey(nameof(UserId))]
		public User User { get; set; }
	}
}