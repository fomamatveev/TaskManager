namespace TaskManager.Domain
{
	public class Task
	{
		public Guid Id { get; set; }

		public string Description { get; set; }

		public Guid UserID { get; set; }
		
		public User User { get; set; }
	}
}