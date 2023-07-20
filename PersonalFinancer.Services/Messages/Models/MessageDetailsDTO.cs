namespace PersonalFinancer.Services.Messages.Models
{
	public class MessageDetailsDTO : MessageInputDTO
	{
        public string Id { get; set; } = null!;

        public DateTime CreatedOnUtc { get; set; }

		public IEnumerable<ReplyOutputDTO> Replies { get; set; } = null!;
	}
}
