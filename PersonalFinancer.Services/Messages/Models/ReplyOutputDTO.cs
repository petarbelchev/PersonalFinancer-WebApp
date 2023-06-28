namespace PersonalFinancer.Services.Messages.Models
{
    public class ReplyOutputDTO
    {
        public string AuthorName { get; set; } = null!;

        public DateTime CreatedOn { get; set; }

        public string Content { get; set; } = null!;
    }
}
