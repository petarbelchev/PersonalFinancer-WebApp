namespace PersonalFinancer.Services.Messages
{
	using AutoMapper;
	using MongoDB.Driver;
	using MongoDB.Driver.Linq;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Repositories;
	using PersonalFinancer.Services.Messages.Models;
	using System.Linq.Expressions;

	public class MessagesService : IMessagesService
	{
		private readonly IMongoRepository<Message> messagesRepo;
		private readonly IMapper mapper;

		public MessagesService(
			IMongoRepository<Message> messagesRepo,
			IMapper mapper)
		{
			this.messagesRepo = messagesRepo;
			this.mapper = mapper;
		}

		public async Task<ReplyOutputDTO> AddReplyAsync(ReplyInputDTO model)
		{
			if (!model.IsAuthorAdmin && !await this.messagesRepo.IsUserDocumentAuthor(model.MessageId, model.AuthorId))
				throw new ArgumentException(ExceptionMessages.UnauthorizedUser);

			Reply reply = this.mapper.Map<Reply>(model);

			UpdateDefinition<Message> update = Builders<Message>.Update
				.Push(x => x.Replies, reply)
				.Set(x => model.IsAuthorAdmin ? x.IsSeenByAuthor : x.IsSeenByAdmin, false)
				.Set(x => model.IsAuthorAdmin ? x.IsArchivedByAuthor : x.IsArchivedByAdmin, false);

			UpdateResult result = await this.messagesRepo.UpdateOneAsync(x => x.Id == model.MessageId, update);

			return result.IsAcknowledged
				? this.mapper.Map<ReplyOutputDTO>(reply)
				: throw new InvalidOperationException(ExceptionMessages.UnsuccessfulUpdate);
		}

		public async Task ArchiveAsync(string messageId, string userId, bool isUserAdmin)
		{
			if (!isUserAdmin && !await this.messagesRepo.IsUserDocumentAuthor(messageId, userId))
				throw new ArgumentException(ExceptionMessages.UnauthorizedUser);

			UpdateResult result = await this.messagesRepo.UpdateOneAsync(
				x => x.Id == messageId && (isUserAdmin || x.AuthorId == userId),
				Builders<Message>.Update.Set(x => isUserAdmin ? x.IsArchivedByAdmin : x.IsArchivedByAuthor, true));

			if (!result.IsAcknowledged)
				throw new InvalidOperationException(ExceptionMessages.UnsuccessfulUpdate);
		}

		public async Task<MessageOutputDTO> CreateAsync(MessageInputDTO model)
		{
			// TODO: Add validation for adding a new message!

			Message newMessage = this.mapper.Map<Message>(model);

			await this.messagesRepo.InsertOneAsync(newMessage);

			return this.mapper.Map<MessageOutputDTO>(newMessage);
		}

		public async Task<MessagesDTO> GetAllArchivedAsync(int page = 1)
			=> await this.GetMessagesAsync(m => m.IsArchivedByAdmin, page, isUserAdmin: true);

		public async Task<MessagesDTO> GetAllMessagesAsync(int page = 1)
			=> await this.GetMessagesAsync(m => !m.IsArchivedByAdmin, page, isUserAdmin: true);

		public async Task<MessageDetailsDTO> GetMessageAsync(string messageId, string userId, bool isUserAdmin)
		{
			MessageDetailsDTO message = await this.messagesRepo.FindOneAsync(
				x => x.Id == messageId && (isUserAdmin || x.AuthorId == userId),
				m => new MessageDetailsDTO
				{
					Id = m.Id,
					AuthorId = m.AuthorId,
					AuthorName = m.AuthorName,
					Content = m.Content,
					Subject = m.Subject,
					CreatedOnUtc = m.CreatedOnUtc,
					Replies = m.Replies.Select(r => new ReplyOutputDTO
					{
						AuthorName = r.AuthorName,
						CreatedOnUtc = r.CreatedOnUtc,
						Content = r.Content
					})
				});

			await this.MarkAsSeenAsync(messageId, userId, isUserAdmin);

			return message;
		}

		public async Task<string> GetMessageAuthorIdAsync(string messageId)
			=> await this.messagesRepo.FindOneAsync(m => m.Id == messageId, m => m.AuthorId);

		public async Task<MessagesDTO> GetUserArchivedAsync(string userId, int page = 1)
			=> await this.GetMessagesAsync(m => m.AuthorId == userId && m.IsArchivedByAuthor, page, isUserAdmin: false);

		public async Task<MessagesDTO> GetUserMessagesAsync(string userId, int page = 1)
			=> await this.GetMessagesAsync(m => m.AuthorId == userId && !m.IsArchivedByAuthor, page, isUserAdmin: false);

		public async Task<bool> HasUnseenMessagesByAdminAsync()
			=> await this.messagesRepo.AnyAsync(m => !m.IsSeenByAdmin);

		public async Task<bool> HasUnseenMessagesByUserAsync(string userId)
			=> await this.messagesRepo.AnyAsync(m => m.AuthorId == userId && !m.IsSeenByAuthor);

		public async Task<bool> IsMessageSeenAsync(string messageId, bool isUserAdmin)
		{
			bool result = await this.messagesRepo.FindOneAsync(
				m => m.Id == messageId, 
				m => isUserAdmin ? m.IsSeenByAuthor : m.IsSeenByAdmin);

			return result;
		}

		public async Task MarkMessageAsSeenAsync(string messageId, string userId, bool isUserAdmin)
			=> await this.MarkAsSeenAsync(messageId, userId, isUserAdmin);

		public async Task RemoveAsync(string messageId, string userId, bool isUserAdmin)
		{
			if (!isUserAdmin && !await this.messagesRepo.IsUserDocumentAuthor(messageId, userId))
				throw new ArgumentException(ExceptionMessages.UnauthorizedUser);

			DeleteResult result = await this.messagesRepo.DeleteOneAsync(messageId);

			if (!result.IsAcknowledged)
				throw new InvalidOperationException(ExceptionMessages.UnsuccessfulDelete);
		}

		private async Task<MessagesDTO> GetMessagesAsync(
			Expression<Func<Message, bool>> filterExpression,
			int page,
			bool isUserAdmin)
		{
			var messages = new MessagesDTO
			{
				Messages = await this.messagesRepo.FindAsync(
					filterExpression,
					Builders<Message>.Sort.Descending("CreatedOnUtc"),
					m => new MessageOutputDTO
					{
						Id = m.Id,
						CreatedOnUtc = m.CreatedOnUtc,
						Subject = m.Subject,
						IsSeen = isUserAdmin ? m.IsSeenByAdmin : m.IsSeenByAuthor
					},
					page),

				TotalMessagesCount = await this.messagesRepo.CountAsync(filterExpression)
			};

			return messages;
		}

		/// <exception cref="InvalidOperationException">When the update was unsuccessful.</exception>
		private async Task MarkAsSeenAsync(string messageId, string userId, bool isUserAdmin)
		{
			UpdateDefinition<Message> updateDefinition = Builders<Message>.Update
				.Set(x => isUserAdmin ? x.IsSeenByAdmin : x.IsSeenByAuthor, true);

			UpdateResult result = await this.messagesRepo.UpdateOneAsync(x => 
				x.Id == messageId && (isUserAdmin || x.AuthorId == userId), 
				updateDefinition);

			if (!result.IsAcknowledged)
				throw new InvalidOperationException(ExceptionMessages.UnsuccessfulUpdate);
		}
	}
}
