namespace PersonalFinancer.Services.Messages
{
	using PersonalFinancer.Services.Messages.Models;

	public interface IMessagesService
    {
        Task<IEnumerable<MessageOutputDTO>> GetAllAsync();

        Task<IEnumerable<MessageOutputDTO>> GetUserMessagesAsync(string userId);

		/// <summary>
		/// Throws Invalid Operation Exception when the message does not exist,
		/// the user is not owner or administrator or message update was unsuccessful.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<MessageDetailsDTO> GetMessageAsync(string id, string userId, bool isUserAdmin);

		Task<string> GetMessageAuthorIdAsync(string messageId);

        Task<MessageOutputDTO> CreateAsync(MessageInputDTO model);

		/// <summary>
		/// Throws Argument Exception when the user is unauthorized 
		/// and returns null when adding a reply was unsuccessful.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		Task<ReplyOutputDTO?> AddReplyAsync(ReplyInputDTO model);

		Task<bool> HasUnseenMessagesByAdminAsync();

		Task<bool> HasUnseenMessagesByUserAsync(string userId);

		Task<bool> IsMessageSeenAsync(string messageId, bool isUserAdmin);

		/// <summary>
		/// Throws Invalid Operation Exception when the update was unsuccessful.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task MarkMessageAsSeenAsync(string messageId, string userId, bool isUserAdmin);

		/// <summary>
		/// Throws Argument Exception when the user is unauthorized 
		/// and Invalid Operation Exception when deleting a message was unsuccessful.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task RemoveAsync(string messageId, string userId, bool isUserAdmin);
	}
}
