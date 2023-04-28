namespace PersonalFinancer.Web.Infrastructure.EmailSender
{
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.Extensions.Logging;
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
            if (string.IsNullOrEmpty(Options.SendGridKey) 
                || string.IsNullOrEmpty(Options.EmailSender))
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

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);
            
            var response = await client.SendEmailAsync(msg);

            logger.LogInformation(response.IsSuccessStatusCode
                ? $"Email to {email} queued successfully!"
                : $"Failure Email to {email}");
        }
    }
}
