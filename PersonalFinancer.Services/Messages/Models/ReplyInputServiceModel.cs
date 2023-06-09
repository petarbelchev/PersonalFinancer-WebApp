namespace PersonalFinancer.Services.Messages.Models
{
	public class ReplyInputServiceModel
	{
        public string MessageId { get; set; } = null!;

        public Guid AuthorId { get; set; }

        public bool IsAuthorAdmin { get; set; }

        public string AuthorName { get; set; } = null!;

        public string Content { get; set; } = null!;
    }
}
