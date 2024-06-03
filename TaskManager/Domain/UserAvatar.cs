using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Domain;

public class UserAvatar
{
	public Guid Id { get; set; }
	
	public string ImagePath { get; set; }
	
	public Guid UserId { get; set; }
	
	[ForeignKey(nameof(UserId))]
	public User User { get; set; }
}