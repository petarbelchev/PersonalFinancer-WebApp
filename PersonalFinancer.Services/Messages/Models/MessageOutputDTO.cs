namespace PersonalFinancer.Services.Messages.Models
{
	public class MessageOutputDTO
	{
        public string Id { get; set; } = null!;

        public DateTime CreatedOnUtc { get; set; }

        public string Subject { get; set; } = null!;

        public bool IsSeen { get; set; }
    }
}
