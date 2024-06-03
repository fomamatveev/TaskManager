using TaskManager.Auth;
using TaskManager.DataAccess;

namespace TaskManager.Services;

public class FileStorage
{
	private readonly TaskManagerDbContext _dbContext;

	private readonly AuthenticationService _authenticationService;

	public FileStorage(TaskManagerDbContext dbContext, AuthenticationService authenticationService)
	{
		_dbContext = dbContext;
		_authenticationService = authenticationService;
	}

	public FileStream CreateImageFile(string imagePath)
	{
		return File.Create(imagePath);
	}

	public FileStream GetImageFile(string imagePath)
	{
		return File.Open(imagePath, FileMode.Open);
	}

	public void DeleteImageFile(string imagePath)
	{
		File.Delete(imagePath);
	}
}