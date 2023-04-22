namespace PersonalFinancer.Services.Messages
{
	using MongoDB.Driver;

	using Services.Messages.Models;

	public interface IMessagesService
	{
		Task<IEnumerable<MessageOutputServiceModel>> GetAllAsync();

		Task<IEnumerable<MessageOutputServiceModel>> GetUserMessagesAsync(string userId);

		Task<MessageDetailsServiceModel> GetMessageAsync(string id, string userId, bool isUserAdmin);

		Task<string> CreateAsync(MessageInputServiceModel model);

		Task<UpdateResult> AddReplyAsync(ReplyInputServiceModel model);

		Task RemoveAsync(string messageId, string userId, bool isUserAdmin);
	}
}
