using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Auth;
using TaskManager.DataAccess;
using TaskManager.Services;

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

	// [Route("Auth/")]
	public class AuthController : Controller
	{
		private readonly TaskManagerDbContext _dbContext;

		private readonly AuthenticationService _authenticationService;
		
		private readonly IOptions<JwtOptions> _jwtOptions;

		private readonly EmailVerification _emailVerification;

		public AuthController(TaskManagerDbContext dbContext, AuthenticationService authenticationService, IOptions<JwtOptions> jwtOptions, EmailVerification emailVerification)
		{
			_dbContext = dbContext;
			_authenticationService = authenticationService;
			_jwtOptions = jwtOptions;
			_emailVerification = emailVerification;
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

		[HttpGet("verify")]
		public IActionResult ConfirmMail(string? token)
		{
			if (token is null)
				return RedirectToAction("GetProfile", nameof(User));

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_jwtOptions.Value.Secret);

			try
			{
				tokenHandler.ValidateToken(
					token,
					new TokenValidationParameters
					{
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(key),
						ValidateIssuer = false,
						ValidateAudience = false,
						ClockSkew = TimeSpan.Zero
					},
					out SecurityToken validatedToken
				);

				var jwtToken = (JwtSecurityToken) validatedToken;
				var email = jwtToken.Claims.First(x => x.Type == "email").Value;

				var user = _dbContext.Users.FirstOrDefault(x => x.Email == email);

				if (user is null)
				{
					return RedirectToAction("GetProfile", nameof(User));
				}
				
				user.EmailConfirm = true;
				_dbContext.Update(user);
				_dbContext.SaveChanges();
				
				return RedirectToAction("GetProfile", nameof(User));
			}
			catch (Exception e)
			{
				return RedirectToAction("GetProfile", nameof(User));
			}
		}

		[HttpGet("sendmail")]
		public RedirectToActionResult SendVerifyMail()
		{
			var user = _authenticationService.User();
			
			_emailVerification.SendEmailConfirm(user);
			
			return RedirectToAction("GetProfile", nameof(User));
		}
	}
}