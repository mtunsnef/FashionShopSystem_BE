using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace FashionShopSystem.Service.Services.UserService
{
	public class MailService
	{
		private readonly IConfiguration _configuration;
		public MailService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task SendEmailAsync(string toEmail, string subject, string body)
		{
			var mailSettings = _configuration.GetSection("MailSettings");
			var email = new MimeMessage();
			email.From.Add(new MailboxAddress(mailSettings["DisplayName"], mailSettings["From"]));
			email.To.Add(MailboxAddress.Parse(toEmail));
			email.Subject = subject;
			email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

			using var smtp = new SmtpClient();
			await smtp.ConnectAsync(mailSettings["Host"], int.Parse(mailSettings["Port"]), false);
			await smtp.AuthenticateAsync(mailSettings["UserName"], mailSettings["Password"]);
			await smtp.SendAsync(email);
			await smtp.DisconnectAsync(true);
		}
	}
}
