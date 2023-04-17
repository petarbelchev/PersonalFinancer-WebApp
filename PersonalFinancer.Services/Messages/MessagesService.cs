namespace PersonalFinancer.Services.Messages
{
	using Microsoft.Extensions.Options;

	using MongoDB.Bson.Serialization.Conventions;
	using MongoDB.Driver;
	using MongoDB.Driver.Linq;

	using Services.Messages.Models;

	using Data;
	using Data.Models;
	using AutoMapper;

	public class MessagesService
	{
		private readonly IMongoCollection<Message> messagesCollection;
		private readonly IMapper mapper;

		public MessagesService(
			IOptions<MessagesDatabaseSettings> messagesDatabaseSettings,
			IMapper mapper)
		{
			var camelCaseConvension = new ConventionPack { new CamelCaseElementNameConvention() };
			ConventionRegistry.Register("camelCase", camelCaseConvension, type => true);

			var mongoClient = new MongoClient(
				messagesDatabaseSettings.Value.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(
				messagesDatabaseSettings.Value.DatabaseName);

			messagesCollection = mongoDatabase.GetCollection<Message>(
				messagesDatabaseSettings.Value.MessagesCollectionName);

			this.mapper = mapper;
		}

		public async Task<IEnumerable<MessageOutputServiceModel>> GetAllAsync()
		{
			var messages = await messagesCollection.AsQueryable()
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
			var messages = await messagesCollection.AsQueryable()
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
			MessageDetailsServiceModel message = await messagesCollection.AsQueryable()
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

			await messagesCollection.InsertOneAsync(newMessage);

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

			UpdateResult updateResult = await messagesCollection.UpdateOneAsync(filter, update);

			if (!updateResult.IsAcknowledged)
				throw new InvalidOperationException("Adding a reply was unsuccessful.");
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

			DeleteResult deleteResult = await messagesCollection.DeleteOneAsync(m => m.Id == messageId);

			if (!deleteResult.IsAcknowledged)
				throw new InvalidOperationException("Deleting of the message was unsuccessful.");
		}

		private async Task<bool> IsUserAuthorized(string messageId, string userId)
			=> await messagesCollection.AsQueryable()
				.AnyAsync(m => m.Id == messageId && m.AuthorId == userId);
	}
}
