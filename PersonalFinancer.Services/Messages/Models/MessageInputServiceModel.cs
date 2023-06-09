namespace PersonalFinancer.Services.Messages.Models
{
    public class MessageInputServiceModel
    {
        public Guid AuthorId { get; set; }

        public string AuthorName { get; set; } = null!;

        public string Subject { get; set; } = null!;

        public string Content { get; set; } = null!;
    }
}
