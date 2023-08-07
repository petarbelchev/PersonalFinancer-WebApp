namespace PersonalFinancer.Services.Messages
{
	using AutoMapper;
	using Microsoft.AspNetCore.Http;
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
			await this.AuthorizeUser(model.MessageId, model.AuthorId, model.IsAuthorAdmin);

			Reply reply = this.mapper.Map<Reply>(model);

			UpdateDefinition<Message> updateDefinition = Builders<Message>.Update
				.Push(x => x.Replies, reply)
				.Set(x => model.IsAuthorAdmin ? x.IsSeenByAuthor : x.IsSeenByAdmin, false)
				.Set(x => model.IsAuthorAdmin ? x.IsArchivedByAuthor : x.IsArchivedByAdmin, false);

			UpdateResult result = await this.messagesRepo
				.UpdateOneAsync(x => x.Id == model.MessageId, updateDefinition);

			return result.IsAcknowledged
				? this.mapper.Map<ReplyOutputDTO>(reply)
				: throw new InvalidOperationException(ExceptionMessages.UnsuccessfulUpdate);
		}

		public async Task ArchiveAsync(string messageId, string userId, bool isUserAdmin)
		{
			await this.AuthorizeUser(messageId, userId, isUserAdmin);

			UpdateResult result = await this.messagesRepo.UpdateOneAsync(
				x => x.Id == messageId && (isUserAdmin || x.AuthorId == userId),
				Builders<Message>.Update.Set(x => isUserAdmin ? x.IsArchivedByAdmin : x.IsArchivedByAuthor, true));

			if (!result.IsAcknowledged)
				throw new InvalidOperationException(ExceptionMessages.UnsuccessfulUpdate);
		}

		public async Task<string> CreateAsync(MessageInputDTO model)
		{
			var newMessage = this.mapper.Map<Message>(model);
			newMessage.Image = await GetImageByteArray(model.Image);

			await this.messagesRepo.InsertOneAsync(newMessage);

			return newMessage.Id;
		}

		public async Task<MessagesDTO> GetAllArchivedMessagesAsync(int page, string? search)
		{
			Expression<Func<Message, bool>> filterExpression = 
				GetMessagesFilterExpression(isUserAdmin: true, archivedMessages: true, search);

			return await this.GetMessagesAsync(filterExpression, page, isUserAdmin: true);
		}

		public async Task<MessagesDTO> GetAllMessagesAsync(int page, string? search)
		{
			Expression<Func<Message, bool>> filterExpression =
				GetMessagesFilterExpression(isUserAdmin: true, archivedMessages: false, search);

			return await this.GetMessagesAsync(filterExpression, page, isUserAdmin: true);
		}

		public async Task<MessageDetailsDTO> GetMessageAsync(string messageId, string userId, bool isUserAdmin)
		{
			MessageDetailsDTO message = await this.messagesRepo.FindOneAsync(
				x => x.Id == messageId,
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
					}),
					Image = m.Image
				});

			if (!isUserAdmin && message.AuthorId != userId)
				throw new UnauthorizedAccessException(ExceptionMessages.UnauthorizedUser);

			await this.MarkAsSeenAsync(messageId, userId, isUserAdmin);

			return message;
		}

		public async Task<string> GetMessageAuthorIdAsync(string messageId)
			=> await this.messagesRepo.FindOneAsync(m => m.Id == messageId, m => m.AuthorId);

		public async Task<MessagesDTO> GetUserArchivedMessagesAsync(string userId, int page, string? search)
		{
			Expression<Func<Message, bool>> filterExpression =
				GetMessagesFilterExpression(isUserAdmin: false, archivedMessages: true, search, userId);

			return await this.GetMessagesAsync(filterExpression, page, isUserAdmin: false);
		}

		public async Task<MessagesDTO> GetUserMessagesAsync(string userId, int page, string? search)
		{
			Expression<Func<Message, bool>> filterExpression =
				GetMessagesFilterExpression(isUserAdmin: false, archivedMessages: false, search, userId);

			return await this.GetMessagesAsync(filterExpression, page, isUserAdmin: false);
		}

		public async Task<bool> HasUnseenMessagesByAdminAsync()
			=> await this.messagesRepo.AnyAsync(m => !m.IsSeenByAdmin);

		public async Task<bool> HasUnseenMessagesByUserAsync(string userId)
			=> await this.messagesRepo.AnyAsync(m => m.AuthorId == userId && !m.IsSeenByAuthor);

		public async Task MarkMessageAsSeenAsync(string messageId, string userId, bool isUserAdmin)
			=> await this.MarkAsSeenAsync(messageId, userId, isUserAdmin);

		public async Task RemoveAsync(string messageId, string userId, bool isUserAdmin)
		{
			await this.AuthorizeUser(messageId, userId, isUserAdmin);
			DeleteResult result = await this.messagesRepo.DeleteOneAsync(messageId);

			if (!result.IsAcknowledged)
				throw new InvalidOperationException(ExceptionMessages.UnsuccessfulDelete);
		}

		/// <exception cref="UnauthorizedAccessException">When the user is unauthorized.</exception>
		private async Task AuthorizeUser(string messageId, string userId, bool isUserAdmin)
		{
			if (!isUserAdmin && !await this.messagesRepo.IsUserDocumentAuthor(messageId, userId))
				throw new UnauthorizedAccessException(ExceptionMessages.UnauthorizedUser);
		}

		/// <exception cref="ArgumentException">When the image constraints are not met.</exception>
		private static async Task<byte[]?> GetImageByteArray(IFormFile? formFile)
		{
			if (formFile != null)
			{
				string[] validImageTypes = { "image/jpeg", "image/png" };

				if (!validImageTypes.Contains(formFile.ContentType))
					throw new ArgumentException(ValidationMessages.InvalidImageFileType);

				if (formFile.Length > 200 * 1024)
					throw new ArgumentException(ValidationMessages.InvalidImageSize);

				using var memoryStream = new MemoryStream();
				await formFile.CopyToAsync(memoryStream);
				return memoryStream.ToArray();
			}

			return null;
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

		private static Expression<Func<Message, bool>> GetMessagesFilterExpression(
			bool isUserAdmin, 
			bool archivedMessages, 
			string? search, 
			string? userId = null)
		{
			if (search != null)
				search = search.ToLower();

			Expression<Func<Message, bool>> expression = (m) =>
				(
					(isUserAdmin && archivedMessages && m.IsArchivedByAdmin) ||
					(isUserAdmin && !archivedMessages && !m.IsArchivedByAdmin) ||
					(!isUserAdmin && archivedMessages && m.IsArchivedByAuthor && m.AuthorId == userId) ||
					(!isUserAdmin && !archivedMessages && !m.IsArchivedByAuthor && m.AuthorId == userId)
				)
				&&
				(
					search == null ||
					m.Subject.ToLower().Contains(search) ||
					m.Content.ToLower().Contains(search) ||
					m.AuthorName.ToLower().Contains(search)
				);

			return expression;
		}

		/// <exception cref="ArgumentException">When the update was unsuccessful.</exception>
		private async Task MarkAsSeenAsync(string messageId, string userId, bool isUserAdmin)
		{
			UpdateDefinition<Message> updateDefinition = Builders<Message>.Update
				.Set(x => isUserAdmin ? x.IsSeenByAdmin : x.IsSeenByAuthor, true);

			UpdateResult result = await this.messagesRepo.UpdateOneAsync(x =>
				x.Id == messageId && (isUserAdmin || x.AuthorId == userId),
				updateDefinition);

			if (!result.IsAcknowledged)
				throw new ArgumentException(ExceptionMessages.UnsuccessfulUpdate);
		}
	}
}
