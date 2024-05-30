using System.Security.Claims;
using TaskManager.DataAccess;
using TaskManager.Domain;

namespace TaskManager.Auth;

public static class ClaimsPrincipalExtensions
{
	public static User? ToUser(this ClaimsPrincipal claimsPrincipal, TaskManagerDbContext dbContext)
	{
		var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

		if (email is null)
		{
			return null;
		}

		return dbContext.Users.SingleOrDefault(e => e.Email == email);
	}
}