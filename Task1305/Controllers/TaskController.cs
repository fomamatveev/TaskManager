using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task1305.DataAccess;
using Task1305.Domain;
using Task1305.Models.Task;

namespace Task1305.Controllers
{
    public class TaskController : Controller
    {
        private readonly TaskManagerDbContext _dbContext;

        public TaskController(TaskManagerDbContext dbContext) 
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var tasks = _dbContext.Tasks.ToList();
            return View(new TasksViewModel(tasks));
        }

        [HttpPost]
        public IActionResult Create(string description)
        {
            var task = new Domain.Task
            { 
                Id = Guid.NewGuid(),
                Description = description
            };

            _dbContext.Tasks.Add(task);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("delete/{id}")]
        public IActionResult Delete(Guid id) 
        {
            var task = _dbContext.Tasks.Find(id);
            _dbContext.Tasks.Remove(task!);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
