﻿namespace PersonalFinancer.Services.Messages
{
	using AutoMapper;
	using MongoDB.Driver;
	using MongoDB.Driver.Linq;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Repositories;
	using PersonalFinancer.Services.Messages.Models;

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

		public async Task<IEnumerable<MessageOutputDTO>> GetAllAsync()
		{
			return await this.messagesRepo
				.FindAsync(m => new MessageOutputDTO
				{
					Id = m.Id,
					CreatedOn = m.CreatedOn,
					Subject = m.Subject,
					IsSeen = m.IsSeenByAdmin
				});
		}

		public async Task<IEnumerable<MessageOutputDTO>> GetUserMessagesAsync(string userId)
		{
			return await this.messagesRepo.FindAsync(
				x => x.AuthorId == userId,
				m => new MessageOutputDTO
				{
					Id = m.Id,
					CreatedOn = m.CreatedOn,
					Subject = m.Subject,
					IsSeen = m.IsSeenByAuthor
				});
		}

		/// <summary>
		/// Throws Invalid Operation Exception when the message does not exist,
		/// the user is not owner or administrator or message update was unsuccessful.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
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
					CreatedOn = m.CreatedOn,
					Replies = m.Replies.Select(r => new ReplyOutputDTO
					{
						AuthorName = r.AuthorName,
						CreatedOn = r.CreatedOn,
						Content = r.Content
					})
				});

			UpdateResult result = await this.MarkAsSeen(messageId, userId, isUserAdmin);

			return result.IsAcknowledged
				? message
				: throw new InvalidOperationException(ExceptionMessages.UnsuccessfulUpdate);
		}

		public async Task<string> GetMessageAuthorIdAsync(string messageId)
			=> await this.messagesRepo.FindOneAsync(m => m.Id == messageId, m => m.AuthorId);

		public async Task<MessageOutputDTO> CreateAsync(MessageInputDTO model)
		{
			Message newMessage = this.mapper.Map<Message>(model);

			await this.messagesRepo.InsertOneAsync(newMessage);

			return this.mapper.Map<MessageOutputDTO>(newMessage);
		}

		/// <summary>
		/// Throws Argument Exception when the user is unauthorized 
		/// and Invalid Operation Exception when adding a reply was unsuccessful.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<ReplyOutputDTO?> AddReplyAsync(ReplyInputDTO model)
		{
			if (!model.IsAuthorAdmin && !await this.messagesRepo.IsUserDocumentAuthor(model.MessageId, model.AuthorId))
				throw new ArgumentException(ExceptionMessages.UnauthorizedUser);

			Reply reply = this.mapper.Map<Reply>(model);

			UpdateDefinition<Message> update = Builders<Message>.Update
				.Push(x => x.Replies, reply)
				.Set(x => model.IsAuthorAdmin ? x.IsSeenByAuthor : x.IsSeenByAdmin, false);

			UpdateResult result = await this.messagesRepo.UpdateOneAsync(x => x.Id == model.MessageId, update);

			return result.IsAcknowledged 
				? this.mapper.Map<ReplyOutputDTO>(reply) 
				: null;
		}

		public async Task<bool> HasUnseenMessagesByAdminAsync()
			=> await this.messagesRepo.AnyAsync(m => !m.IsSeenByAdmin);

		public async Task<bool> HasUnseenMessagesByUserAsync(string userId)
			=> await this.messagesRepo.AnyAsync(m => m.AuthorId == userId && !m.IsSeenByAuthor);

		/// <summary>
		/// Throws Invalid Operation Exception when the update was unsuccessful.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task MarkMessageAsSeenAsync(string messageId, string userId, bool isUserAdmin)
		{
			UpdateResult updateResult = await this.MarkAsSeen(messageId, userId, isUserAdmin);

			if (!updateResult.IsAcknowledged)
				throw new InvalidOperationException(ExceptionMessages.UnsuccessfulUpdate);
		}

		/// <summary>
		/// Throws Argument Exception when the user is unauthorized 
		/// and Invalid Operation Exception when deleting a message was unsuccessful.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task RemoveAsync(string messageId, string userId, bool isUserAdmin)
		{
			if (!isUserAdmin && !await this.messagesRepo.IsUserDocumentAuthor(messageId, userId))
				throw new ArgumentException(ExceptionMessages.UnauthorizedUser);

			DeleteResult result = await this.messagesRepo.DeleteOneAsync(messageId);

			if (!result.IsAcknowledged)
				throw new InvalidOperationException(ExceptionMessages.UnsuccessfulDelete);
		}

		private async Task<UpdateResult> MarkAsSeen(string messageId, string userId, bool isUserAdmin)
		{
			UpdateDefinition<Message> updateDefinition = Builders<Message>.Update
				.Set(x => isUserAdmin ? x.IsSeenByAdmin : x.IsSeenByAuthor, true);

			UpdateResult result = await this.messagesRepo
				.UpdateOneAsync(x => 
					x.Id == messageId && (isUserAdmin || x.AuthorId == userId), 
					updateDefinition);

			return result;
		}
	}
}
