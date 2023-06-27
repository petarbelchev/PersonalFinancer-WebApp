namespace PersonalFinancer.Services.Messages
{
    using MongoDB.Driver;
    using PersonalFinancer.Services.Messages.Models;

    public interface IMessagesService
    {
        Task<IEnumerable<MessageOutputDTO>> GetAllAsync();

        Task<IEnumerable<MessageOutputDTO>> GetUserMessagesAsync(string userId);

        /// <summary>
        /// Throws InvalidOperationException when Message does not exist
        /// or User is not owner or Administrator
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        Task<MessageDetailsDTO> GetMessageAsync(string id, string userId, bool isUserAdmin);

        Task<string> CreateAsync(MessageInputDTO model);

        /// <summary>
        /// Throws ArgumentException when the User is not Authorized 
        /// and InvalidOperationException when adding a reply was unsuccessful.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        Task<UpdateResult> AddReplyAsync(ReplyInputDTO model);

        /// <summary>
        /// Throws ArgumentException when the User is not Authorized 
        /// and InvalidOperationException when deleting a message was unsuccessful.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        Task RemoveAsync(string messageId, string userId, bool isUserAdmin);
    }
}
