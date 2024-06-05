using Microsoft.AspNetCore.Mvc;
using TaskManager.Auth;
using TaskManager.DataAccess;
using TaskManager.Domain;
using TaskManager.Models.User;

namespace TaskManager.Controllers
{
	public class UserController : Controller
	{
		private readonly AuthenticationService _authenticationService;
		private readonly TaskManagerDbContext _dbContext;

		public UserController(AuthenticationService authenticationService, TaskManagerDbContext dbContext)
		{
			_authenticationService = authenticationService;
			_dbContext = dbContext;
		}

		[HttpGet("profile")]
		public async Task<IActionResult> GetProfile()
		{
			var user = _authenticationService.User();

			var userAvatar = _dbContext.UserAvatar.SingleOrDefault(ua => ua.UserId == user.Id);

			return View(new UserProfileViewModel {Email = user.Email, AvatarId = userAvatar?.Id ?? Guid.Empty});
		}

		[HttpPost("profile/upload")]
		public async Task<IActionResult> UploadImage()
		{
			var file = HttpContext.Request.Form.Files.SingleOrDefault();

			if (file is null)
			{
				return RedirectToAction(nameof(GetProfile));
			}

			var userAvatar = _dbContext.UserAvatar.SingleOrDefault(ua => ua.UserId == _authenticationService.User().Id);

			if (userAvatar is not null)
			{
				await using var fileDestStream = System.IO.File.Open(userAvatar.ImagePath, FileMode.Truncate);
				await file.CopyToAsync(fileDestStream);

				return RedirectToAction(nameof(GetProfile));
			}

			var filePath = Path.GetTempFileName();
			await using var stream = System.IO.File.Create(filePath);
			await file.CopyToAsync(stream);

			_dbContext.UserAvatar.Add(
				new UserAvatar()
				{
					Id = Guid.NewGuid(),
					ImagePath = filePath,
					UserId = _authenticationService.User().Id
				}
			);

			await _dbContext.SaveChangesAsync();

			return RedirectToAction(nameof(GetProfile));
		}

		[HttpGet("/image/{id:guid}")]
		public async Task<IActionResult> GetImage(Guid id)
		{
			var file = _dbContext.UserAvatar.Find(id);

			return File(System.IO.File.Open(file.ImagePath, FileMode.Open), "image/jpeg");
		}
	}
}