namespace PersonalFinancer.Services.Messages.Models
{
    public class ReplyOutputDTO
    {
        public string AuthorName { get; set; } = null!;

        public DateTime CreatedOnUtc { get; set; }

        public string Content { get; set; } = null!;
    }
}
