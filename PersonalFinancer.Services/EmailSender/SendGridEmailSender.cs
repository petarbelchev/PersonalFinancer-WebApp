namespace PersonalFinancer.Services.EmailSender
{
	using Microsoft.AspNetCore.Identity.UI.Services;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using SendGrid;
	using SendGrid.Helpers.Mail;

	public class SendGridEmailSender : IEmailSender
	{
		private readonly ISendGridClient sendGridClient;
		private readonly AuthEmailSenderOptions options;
		private readonly ILogger<SendGridEmailSender> logger;

		public SendGridEmailSender(
			ISendGridClient sendGridClient,
			IOptions<AuthEmailSenderOptions> optionsAccessor,
			ILogger<SendGridEmailSender> logger)
		{
			this.sendGridClient = sendGridClient;
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

			var msg = new SendGridMessage()
			{
				From = new EmailAddress(this.options.EmailSender),
				Subject = subject,
				HtmlContent = message
			};

			msg.AddTo(new EmailAddress(email));
			msg.SetClickTracking(false, false);

			Response response = await this.sendGridClient.SendEmailAsync(msg);

			if (response.IsSuccessStatusCode)
				this.logger.LogInformation("Email to {email} queued successfully!", email);
			else
				this.logger.LogError("Failure Email to {email}", email);
		}
	}
}
