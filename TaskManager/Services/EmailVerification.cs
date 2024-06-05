using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Domain;

namespace TaskManager.Services;

public class JwtOptions
{
	public string Secret { get; set; }

	public string Issuer { get; set; }

	public string Audience { get; set; }

	public int ExpirationTime { get; set; }
}

public class SmtpOptions
{
	public string SMTPKey {get;set;}

	public string EmailAddress {get;set;}
}

public class EmailVerification
{
	private readonly IOptions<JwtOptions> _jwtOptions;

	private readonly IOptions<SmtpOptions> _smtpOptions;

	public EmailVerification(IOptions<JwtOptions> jwtOptions, IOptions<SmtpOptions> smtpOptions)
	{
		_jwtOptions = jwtOptions;
		_smtpOptions = smtpOptions;
	}

	public string GenerateToken(User user)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.ASCII.GetBytes(_jwtOptions.Value.Secret);

		var tokenDescriptor = new SecurityTokenDescriptor()
		{
			Subject = new ClaimsIdentity(new[] {new Claim(ClaimTypes.Email, user.Email)}),
			Expires = DateTime.UtcNow.AddDays(7),
			SigningCredentials = new SigningCredentials(
				new SymmetricSecurityKey(key),
				SecurityAlgorithms.HmacSha256Signature
			)
		};

		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}

	public void SendEmailConfirm(User user)
	{
		const string server = "smtp.yandex.ru";

		var to = user.Email;
		string from = _smtpOptions.Value.EmailAddress;

		var message = new MailMessage(from, to);

		message.Subject = "Email verify topic";
		message.Body = $"URL-validation: <a href=\"http://localhost:5034/verify/?token={GenerateToken(user)}\">url</a>";
		message.IsBodyHtml = true;

		var client = new SmtpClient(server);
		client.Credentials = new NetworkCredential(from, _smtpOptions.Value.SMTPKey);
		client.Port = 587;
		client.EnableSsl = true;

		try
		{
			client.Send(message);
		}
		catch (Exception ex)
		{
			Console.WriteLine("Exception caught in SendEmailConfirm(): {0}",
				ex.ToString());
		}
	}
}