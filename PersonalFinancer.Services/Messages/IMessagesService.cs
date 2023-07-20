namespace PersonalFinancer.Services.Messages
{
	using PersonalFinancer.Services.Messages.Models;

	public interface IMessagesService
	{
		/// <exception cref="ArgumentException">When the user is unauthorized.</exception>
		/// <exception cref="InvalidOperationException">When adding a reply was unsuccessful.</exception>
		Task<ReplyOutputDTO> AddReplyAsync(ReplyInputDTO model);

		/// <exception cref="ArgumentException">When the user is unauthorized.</exception>
		/// <exception cref="InvalidOperationException">When archiving a message was unsuccessful.</exception>
		Task ArchiveAsync(string messageId, string userId, bool isUserAdmin);

		Task<MessageOutputDTO> CreateAsync(MessageInputDTO model);

		Task<MessagesDTO> GetAllArchivedMessagesAsync(int page = 1);

		Task<MessagesDTO> GetAllMessagesAsync(int page = 1);

		/// <exception cref="InvalidOperationException">When the message does not exist, 
		/// the user is not owner or administrator or message update was unsuccessful.</exception>
		Task<MessageDetailsDTO> GetMessageAsync(string id, string userId, bool isUserAdmin);

		Task<string> GetMessageAuthorIdAsync(string messageId);

		Task<MessagesDTO> GetUserArchivedMessagesAsync(string userId, int page = 1);

		Task<MessagesDTO> GetUserMessagesAsync(string userId, int page = 1);

		Task<bool> HasUnseenMessagesByAdminAsync();

		Task<bool> HasUnseenMessagesByUserAsync(string userId);

		/// <exception cref="InvalidOperationException">When the update was unsuccessful.</exception>
		Task MarkMessageAsSeenAsync(string messageId, string userId, bool isUserAdmin);

		/// <exception cref="ArgumentException">When the user is unauthorized.</exception>
		/// <exception cref="InvalidOperationException">When deleting a message was unsuccessful.</exception>
		Task RemoveAsync(string messageId, string userId, bool isUserAdmin);
	}
}
