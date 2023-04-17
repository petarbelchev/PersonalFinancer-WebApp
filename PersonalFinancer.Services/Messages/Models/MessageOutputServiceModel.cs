namespace PersonalFinancer.Services.Messages.Models
{
	public class MessageOutputServiceModel
	{
        public string Id { get; set; } = null!;

        public DateTime CreatedOn { get; set; }

        public string Subject { get; set; } = null!;
	}
}
