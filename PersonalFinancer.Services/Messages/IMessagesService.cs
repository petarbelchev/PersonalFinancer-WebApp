namespace PersonalFinancer.Services.Messages
{
	using PersonalFinancer.Services.Messages.Models;

	public interface IMessagesService
	{
		/// <exception cref="UnauthorizedAccessException">When the user is unauthorized.</exception>
		/// <exception cref="InvalidOperationException">When adding a reply was unsuccessful.</exception>
		Task<ReplyOutputDTO> AddReplyAsync(ReplyInputDTO model);

		/// <exception cref="UnauthorizedAccessException">When the user is unauthorized.</exception>
		/// <exception cref="InvalidOperationException">When archiving a message was unsuccessful.</exception>
		Task ArchiveAsync(string messageId, string userId, bool isUserAdmin);

		/// <exception cref="ArgumentException">When the image constraints are not met.</exception>
		Task<string> CreateAsync(MessageInputDTO model);

		Task<MessagesDTO> GetAllArchivedMessagesAsync(int page = 1);

		Task<MessagesDTO> GetAllMessagesAsync(int page = 1);

		/// <exception cref="UnauthorizedAccessException">The user is unauthorized.</exception>
		/// <exception cref="ArgumentException">When marking the message as seen is unsuccessful.</exception>
		/// <exception cref="InvalidOperationException">When the message does not exist.</exception>
		Task<MessageDetailsDTO> GetMessageAsync(string id, string userId, bool isUserAdmin);

		/// <exception cref="InvalidOperationException">When the message does not exist.</exception>
		Task<string> GetMessageAuthorIdAsync(string messageId);

		Task<MessagesDTO> GetUserArchivedMessagesAsync(string userId, int page = 1);

		Task<MessagesDTO> GetUserMessagesAsync(string userId, int page = 1);

		Task<bool> HasUnseenMessagesByAdminAsync();

		Task<bool> HasUnseenMessagesByUserAsync(string userId);

		/// <exception cref="ArgumentException">When the update was unsuccessful.</exception>
		Task MarkMessageAsSeenAsync(string messageId, string userId, bool isUserAdmin);

		/// <exception cref="UnauthorizedAccessException">When the user is unauthorized.</exception>
		/// <exception cref="InvalidOperationException">When deleting a message was unsuccessful.</exception>
		Task RemoveAsync(string messageId, string userId, bool isUserAdmin);
	}
}
