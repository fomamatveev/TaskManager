namespace TaskManager.Models.User;

public class UserProfileViewModel
{
	public string Email { get; set; }

	public IFormFile File { get; set; }

	public Guid AvatarId { get; set; }
}