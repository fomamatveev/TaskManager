using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Auth;
using TaskManager.DataAccess;
using TaskManager.Models.Task;

namespace TaskManager.Controllers
{
	[Authorize]
	public class TaskController : Controller
	{
		private readonly TaskManagerDbContext _dbContext;

		private readonly AuthenticationService _authenticationService;

		private readonly ILogger<TaskController> _logger;

		public TaskController(
			TaskManagerDbContext dbContext,
			AuthenticationService authenticationService,
			ILogger<TaskController> logger)
		{
			_dbContext = dbContext;
			_authenticationService = authenticationService;
			_logger = logger;
		}

		[HttpGet]
		public IActionResult Index()
		{
			var tasks = _dbContext.Tasks;
			return View(new TasksViewModel(tasks.Where(t => t.UserID == _authenticationService.User.Id).ToList()));
		}

		[HttpPost]
		public IActionResult Create(string description)
		{
			var task = new TaskManager.Domain.Task
			{
				Id = Guid.NewGuid(),
				Description = description,
				UserID = _authenticationService.User.Id
			};

			_dbContext.Tasks.Add(task);
			_dbContext.SaveChanges();

			_logger.LogInformation("{user} created task {id}", _authenticationService.User.Id, task.Id);

			return RedirectToAction(nameof(Index));
		}

		[HttpGet("delete/{id}")]
		public IActionResult Delete(Guid id)
		{
			var task = _dbContext.Tasks.Find(id);

			if (task.UserID != _authenticationService.User.Id)
				return Forbid();

			_dbContext.Tasks.Remove(task!);
			_dbContext.SaveChanges();

			_logger.LogInformation("{user} deleted task {id}", _authenticationService.User.Id, task.Id);

			return RedirectToAction(nameof(Index));
		}
	}
}