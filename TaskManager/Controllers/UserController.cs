using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Auth;

namespace TaskManager.Controllers
{
	public class UserLoginRequest
	{
		public string Email { get; set; }

		public string Password { get; set; }
	}

	public class UserRegisterRequest
	{
		public string Email { get; set; }

		public string Password { get; set; }
	}

	public class UserController : Controller
	{
		private readonly AuthenticationService _authenticationService;

		public UserController(AuthenticationService authenticationService)
		{
			_authenticationService = authenticationService;
		}

		[HttpGet("login")]
		public IActionResult Login()
		{
			if (_authenticationService.IsAuthenticated)
			{
				return RedirectToAction("Index", "Task");
			}

			return View();
		}

		[Authorize]
		[HttpGet("logout")]
		public async Task<IActionResult> Logout()
		{
			await _authenticationService.Logout();

			return RedirectToAction(nameof(Login));
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(UserLoginRequest loginRequest)
		{
			if (_authenticationService.IsAuthenticated)
			{
				return RedirectToAction("Index", "Task");
			}

			if (await _authenticationService.Login(loginRequest.Email, loginRequest.Password))
			{
				return RedirectToAction("Index", "Task");
			}

			return RedirectToAction(nameof(Login));
		}

		[HttpGet("register")]
		public IActionResult Register()
		{
			if (_authenticationService.IsAuthenticated)
			{
				return RedirectToAction("Index", "Task");
			}

			return View();
		}
		
		[HttpPost("register")]
		public async Task<IActionResult> Register(UserRegisterRequest registerRequest)
		{
			if (await _authenticationService.Register(registerRequest.Email, registerRequest.Password) is not null)
			{
				return await Login(
					new UserLoginRequest() {Email = registerRequest.Email, Password = registerRequest.Password}
				);
			}

			return RedirectToAction(nameof(Register));
		}
	}
}