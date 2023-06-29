namespace PersonalFinancer.Services.Messages
{
    using MongoDB.Driver;
    using PersonalFinancer.Services.Messages.Models;

    public interface IMessagesService
    {
        Task<IEnumerable<MessageOutputDTO>> GetAllAsync();

        Task<IEnumerable<MessageOutputDTO>> GetUserMessagesAsync(string userId);

		/// <summary>
		/// Throws Invalid Operation Exception when the message does not exist
		/// or the user is not owner or administrator
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<MessageDetailsDTO> GetMessageAsync(string id, string userId, bool isUserAdmin);

        Task<string> CreateAsync(MessageInputDTO model);

		/// <summary>
		/// Throws Argument Exception when the user is unauthorized 
		/// and Invalid Operation Exception when adding a reply was unsuccessful.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task<UpdateResult> AddReplyAsync(ReplyInputDTO model);

		/// <summary>
		/// Throws Argument Exception when the user is unauthorized 
		/// and Invalid Operation Exception when deleting a message was unsuccessful.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task RemoveAsync(string messageId, string userId, bool isUserAdmin);
    }
}
