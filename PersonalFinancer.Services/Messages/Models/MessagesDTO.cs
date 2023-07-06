namespace PersonalFinancer.Services.Messages.Models
{
	public class MessagesDTO
	{
		public IEnumerable<MessageOutputDTO> Messages { get; set; } = null!;

        public int TotalMessagesCount { get; set; }
    }
}
