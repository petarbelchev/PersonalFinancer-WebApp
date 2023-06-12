namespace PersonalFinancer.Web.Infrastructure.EmailSender
{
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.Extensions.Options;
    using SendGrid;
    using SendGrid.Helpers.Mail;

    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> logger;

        public EmailSender(
            IOptions<AuthMessageSenderOptions> optionsAccessor,
            ILogger<EmailSender> logger)
        {
            this.Options = optionsAccessor.Value;
            this.logger = logger;
        }

        public AuthMessageSenderOptions Options { get; set; }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            if (string.IsNullOrEmpty(this.Options.SendGridKey)
                || string.IsNullOrEmpty(this.Options.EmailSender))
            {
                throw new Exception("Null Message Sender Options");
            }

            var client = new SendGridClient(this.Options.SendGridKey);
            
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(this.Options.EmailSender),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };

            msg.AddTo(new EmailAddress(email));
            msg.SetClickTracking(false, false);

            Response response = await client.SendEmailAsync(msg);

            this.logger.LogInformation(response.IsSuccessStatusCode
                ? $"Email to {email} queued successfully!"
                : $"Failure Email to {email}");
        }
    }
}
