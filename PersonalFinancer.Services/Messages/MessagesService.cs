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
                    Subject = m.Subject
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
                    Subject = m.Subject
                });
        }

        /// <summary>
        /// Throws Invalid Operation Exception when the message does not exist
        /// or the user is not owner or administrator
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<MessageDetailsDTO> GetMessageAsync(
            string messageId, string userId, bool isUserAdmin)
        {
            return await this.messagesRepo.FindOneAsync(
                x => x.Id == messageId && (isUserAdmin || x.AuthorId == userId),
                m => new MessageDetailsDTO
                {
                    Id = m.Id,
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
        }

        public async Task<string> CreateAsync(MessageInputDTO model)
        {
            Message newMessage = this.mapper.Map<Message>(model);
            newMessage.CreatedOn = DateTime.UtcNow;

            await this.messagesRepo.InsertOneAsync(newMessage);

            return newMessage.Id;
        }

        /// <summary>
        /// Throws Argument Exception when the user is unauthorized 
        /// and Invalid Operation Exception when adding a reply was unsuccessful.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<UpdateResult> AddReplyAsync(ReplyInputDTO model)
        {
            if (!model.IsAuthorAdmin && !await this.messagesRepo.IsUserDocumentAuthor(model.MessageId, model.AuthorId))
                throw new ArgumentException(ExceptionMessages.UnauthorizedUser);

            Reply reply = this.mapper.Map<Reply>(model);
            reply.CreatedOn = DateTime.UtcNow;

            UpdateDefinition<Message> update = Builders<Message>.Update.Push(x => x.Replies, reply);

            UpdateResult result = await this.messagesRepo.UpdateOneAsync(x => x.Id == model.MessageId, update);

            return result;
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
    }
}
