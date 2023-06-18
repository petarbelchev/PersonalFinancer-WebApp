namespace PersonalFinancer.Web.EmailSender
{
    public class AuthEmailSenderOptions
    {
        public string EmailSender { get; set; } = null!;

        public string SendGridKey { get; set; } = null!;
    }
}
