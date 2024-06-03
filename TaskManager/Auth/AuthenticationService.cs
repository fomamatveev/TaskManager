using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using TaskManager.DataAccess;
using TaskManager.Domain;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Auth
{
	public class AuthenticationService
	{
		private readonly TaskManagerDbContext _dbContext;

		private readonly IHttpContextAccessor _contextAccessor;


		public bool IsAuthenticated => _contextAccessor.HttpContext.User.Identity?.IsAuthenticated ?? false;

		public AuthenticationService(TaskManagerDbContext dbContext, IHttpContextAccessor contextAccessor)
		{
			_dbContext = dbContext;
			_contextAccessor = contextAccessor;
		}

		public User User()
		{
			var user = _contextAccessor.HttpContext.User.ToUser(_dbContext);

			if (user is null && IsAuthenticated)
			{
				Logout();
			}

			return user;
		}

		public async Task<bool> Login(string email, string password)
		{
			var user = _dbContext.Users.SingleOrDefault(user => user.Email == email && user.Password == password);

			if (user is null) return false;

			var claims = new List<Claim> {new Claim(ClaimTypes.Email, user.Email)};

			var claimsIdentity = new ClaimsIdentity(claims, "Cookies");

			await _contextAccessor.HttpContext.SignInAsync(
				CookieAuthenticationDefaults.AuthenticationScheme,
				new ClaimsPrincipal(claimsIdentity)
			);

			return true;
		}

		public Task Logout()
		{
			return _contextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		}

		public async Task<User?> Register(string email, string password)
		{
			if (_dbContext.Users.Any(e => e.Email == email))
				return null;

			var user = new User
			{
				Id = Guid.NewGuid(),
				Email = email,
				Password = password
			};

			await _dbContext.AddAsync(user);
			await _dbContext.SaveChangesAsync();

			return user;
		}
	}
}