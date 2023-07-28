namespace PersonalFinancer.Services.EmailSender
{
	using Microsoft.AspNetCore.Identity.UI.Services;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using SendGrid;
	using SendGrid.Helpers.Mail;

	public class EmailSender : IEmailSender
	{
		private readonly AuthEmailSenderOptions options;
		private readonly ILogger<EmailSender> logger;

		public EmailSender(
			IOptions<AuthEmailSenderOptions> optionsAccessor,
			ILogger<EmailSender> logger)
		{
			this.options = optionsAccessor.Value;
			this.logger = logger;
		}

		public async Task SendEmailAsync(string email, string subject, string message)
		{
			if (string.IsNullOrEmpty(this.options.SendGridKey)
				|| string.IsNullOrEmpty(this.options.EmailSender))
			{
				throw new Exception("Null Message Sender Options");
			}

			var client = new SendGridClient(this.options.SendGridKey);

			var msg = new SendGridMessage()
			{
				From = new EmailAddress(this.options.EmailSender),
				Subject = subject,
				PlainTextContent = message,
				HtmlContent = message
			};

			msg.AddTo(new EmailAddress(email));
			msg.SetClickTracking(false, false);

			Response response = await client.SendEmailAsync(msg);

			if (response.IsSuccessStatusCode)
				this.logger.LogInformation("Email to {email} queued successfully!", email);
			else
				this.logger.LogError("Failure Email to {email}", email);
		}
	}
}
