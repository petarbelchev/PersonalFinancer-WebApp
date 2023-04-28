namespace PersonalFinancer.Web.Infrastructure.EmailSender
{
    public class AuthMessageSenderOptions
    {
        public string EmailSender { get; set; } = null!;

        public string SendGridKey { get; set; } = null!;
    }
}
