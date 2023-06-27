namespace PersonalFinancer.Services.Messages.Models
{
    public class MessageInputDTO
    {
        public string AuthorId { get; set; } = null!;

        public string AuthorName { get; set; } = null!;

        public string Subject { get; set; } = null!;

        public string Content { get; set; } = null!;
    }
}
