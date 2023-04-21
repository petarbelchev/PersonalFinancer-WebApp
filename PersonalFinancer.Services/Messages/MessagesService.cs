namespace PersonalFinancer.Services.Messages
{
	using AutoMapper;

	//using Microsoft.EntityFrameworkCore;
	
	using MongoDB.Driver;
	using MongoDB.Driver.Linq;
	
	using Data.Models;
	
	using Services.Infrastructure;
	using Services.Messages.Models;

	public class MessagesService : IMessagesService
	{
		private readonly IMongoRepository<Message> messagesRepo;
		private readonly IMapper mapper;

		public MessagesService(
			IMongoRepository<Message> repo,
			IMapper mapper)
		{
			this.messagesRepo = repo;
			this.mapper = mapper;
		}

		public async Task<IEnumerable<MessageOutputServiceModel>> GetAllAsync()
		{
			var messages = await messagesRepo.All()
				.Select(m => new MessageOutputServiceModel
				{
					Id = m.Id,
					CreatedOn = m.CreatedOn,
					Subject = m.Subject
				})
				.ToListAsync();

			return messages;
		}

		public async Task<IEnumerable<MessageOutputServiceModel>> GetUserMessagesAsync(string userId)
		{
			var messages = await messagesRepo.All()
				.Where(m => m.AuthorId == userId)
				.Select(m => new MessageOutputServiceModel
				{
					Id = m.Id,
					CreatedOn = m.CreatedOn,
					Subject = m.Subject
				})
				.ToListAsync();

			return messages;
		}

		/// <summary>
		/// Throws InvalidOperationException when Message does not exist
		/// or User is not owner or Administrator
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<MessageDetailsServiceModel> GetMessageAsync(string id, string userId, bool isUserAdmin)
		{
			MessageDetailsServiceModel message = await messagesRepo.All()
				.Where(m => m.Id == id && (isUserAdmin || m.AuthorId == userId))
				.Select(m => new MessageDetailsServiceModel
				{
					Id = m.Id,
					AuthorName = m.AuthorName,
					Content = m.Content,
					Subject = m.Subject,
					CreatedOn = m.CreatedOn,
					Replies = m.Replies.Select(r => new ReplyOutputServiceModel
					{
						AuthorName = r.AuthorName,
						CreatedOn = r.CreatedOn,
						Content = r.Content
					})
				})
				.FirstAsync();

			return message;
		}

		public async Task<string> CreateAsync(MessageInputServiceModel model)
		{
			var newMessage = mapper.Map<Message>(model);
			newMessage.CreatedOn = DateTime.UtcNow;

			await messagesRepo.AddAsync(newMessage);

			return newMessage.Id;
		}

		/// <summary>
		/// Throws ArgumentException when the User is not Authorized 
		/// and InvalidOperationException when adding a reply was unsuccessful.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task AddReplyAsync(ReplyInputServiceModel model)
		{
			if (!model.IsAuthorAdmin && !await IsUserAuthorized(model.MessageId, model.AuthorId))
				throw new ArgumentException("The User is not Authorized to make replies.");

			var reply = mapper.Map<Reply>(model);
			reply.CreatedOn = DateTime.UtcNow;

			var filter = Builders<Message>.Filter.Eq(x => x.Id, model.MessageId);
			var update = Builders<Message>.Update.Push(x => x.Replies, reply);

			await messagesRepo.UpdateAsync(filter, update);
		}
		
		/// <summary>
		/// Throws ArgumentException when the User is not Authorized 
		/// and InvalidOperationException when deleting a message was unsuccessful.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task RemoveAsync(string messageId, string userId, bool isUserAdmin)
		{
			if (!isUserAdmin && !await IsUserAuthorized(messageId, userId))
				throw new ArgumentException("The User is not Authorized to make replies.");

			var filter = Builders<Message>.Filter.Eq(x => x.Id, messageId);

			await messagesRepo.Remove(filter);
		}

		private async Task<bool> IsUserAuthorized(string messageId, string userId)
			=> await messagesRepo.All().AnyAsync(m => m.Id == messageId && m.AuthorId == userId);
	}
}
