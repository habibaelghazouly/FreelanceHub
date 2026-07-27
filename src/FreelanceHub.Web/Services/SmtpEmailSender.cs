using FreelanceHub.Application.Services.Abstractions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace FreelanceHub.Web.Services
{
	public class SmtpEmailSender : IEmailSender
	{
		private readonly SmtpOptions _options;

		public SmtpEmailSender(IOptions<SmtpOptions> options)
		{
			_options = options.Value;
		}

		public async Task SendAsync(string recipientEmail, string subject, string body)
		{
			if (string.IsNullOrWhiteSpace(_options.Host) || string.IsNullOrWhiteSpace(_options.FromEmail))
			{
				throw new InvalidOperationException("SMTP host and sender email must be configured.");
			}

			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));
			message.To.Add(MailboxAddress.Parse(recipientEmail));
			message.Subject = subject;
			message.Body = new TextPart("plain") { Text = body };

			using var client = new SmtpClient();
			await client.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls);
			if (!string.IsNullOrWhiteSpace(_options.Username))
			{
				await client.AuthenticateAsync(_options.Username, _options.Password);
			}

			await client.SendAsync(message);
			await client.DisconnectAsync(true);
		}
	}
}
