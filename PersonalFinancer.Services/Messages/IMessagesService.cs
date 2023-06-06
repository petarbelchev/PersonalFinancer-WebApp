using MongoDB.Driver;
using PersonalFinancer.Services.Messages.Models;

namespace PersonalFinancer.Services.Messages
{
	public interface IMessagesService
	{
		Task<IEnumerable<MessageOutputServiceModel>> GetAllAsync();

		Task<IEnumerable<MessageOutputServiceModel>> GetUserMessagesAsync(string userId);

		/// <summary>
		/// Throws InvalidOperationException when Message does not exist
		/// or User is not owner or Administrator
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<MessageDetailsServiceModel> GetMessageAsync(string id, string userId, bool isUserAdmin);

		Task<string> CreateAsync(MessageInputServiceModel model);

		/// <summary>
		/// Throws ArgumentException when the User is not Authorized 
		/// and InvalidOperationException when adding a reply was unsuccessful.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task<UpdateResult> AddReplyAsync(ReplyInputServiceModel model);

		/// <summary>
		/// Throws ArgumentException when the User is not Authorized 
		/// and InvalidOperationException when deleting a message was unsuccessful.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task RemoveAsync(string messageId, string userId, bool isUserAdmin);
	}
}
