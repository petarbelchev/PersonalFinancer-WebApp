namespace PersonalFinancer.Services.Messages
{
    using AutoMapper;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
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

        public async Task<IEnumerable<MessageOutputServiceModel>> GetAllAsync()
        {
            return await this.messagesRepo
                .FindAsync(m => new MessageOutputServiceModel
                {
                    Id = m.Id,
                    CreatedOn = m.CreatedOn,
                    Subject = m.Subject
                });
        }

        public async Task<IEnumerable<MessageOutputServiceModel>> GetUserMessagesAsync(string userId)
        {
            return await this.messagesRepo.FindAsync(
                x => x.AuthorId == userId,
                m => new MessageOutputServiceModel
                {
                    Id = m.Id,
                    CreatedOn = m.CreatedOn,
                    Subject = m.Subject
                });
        }

        /// <summary>
        /// Throws InvalidOperationException when Message does not exist
        /// or User is not owner or Administrator
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<MessageDetailsServiceModel> GetMessageAsync(
            string messageId, string userId, bool isUserAdmin)
        {
            return await this.messagesRepo.FindOneAsync(
                x => x.Id == messageId && (isUserAdmin || x.AuthorId == userId),
                m => new MessageDetailsServiceModel
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
                });
        }

        public async Task<string> CreateAsync(MessageInputServiceModel model)
        {
            Message newMessage = this.mapper.Map<Message>(model);
            newMessage.CreatedOn = DateTime.UtcNow;

            await this.messagesRepo.InsertOneAsync(newMessage);

            return newMessage.Id;
        }

        /// <summary>
        /// Throws ArgumentException when the User is not Authorized 
        /// and InvalidOperationException when adding a reply was unsuccessful.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<UpdateResult> AddReplyAsync(ReplyInputServiceModel model)
        {
            if (!model.IsAuthorAdmin && !await this.messagesRepo.IsUserDocumentAuthor(model.MessageId, model.AuthorId))
                throw new ArgumentException("The User is not Authorized to make replies.");

            Reply reply = this.mapper.Map<Reply>(model);
            reply.CreatedOn = DateTime.UtcNow;

            UpdateDefinition<Message> update = Builders<Message>.Update.Push(x => x.Replies, reply);

            UpdateResult result = await this.messagesRepo.UpdateOneAsync(x => x.Id == model.MessageId, update);

            return result;
        }

        /// <summary>
        /// Throws ArgumentException when the User is not Authorized 
        /// and InvalidOperationException when deleting a message was unsuccessful.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task RemoveAsync(string messageId, string userId, bool isUserAdmin)
        {
            if (!isUserAdmin && !await this.messagesRepo.IsUserDocumentAuthor(messageId, userId))
                throw new ArgumentException("The User is not Authorized to delete the message.");

            DeleteResult result = await this.messagesRepo.DeleteOneAsync(messageId);

            if (!result.IsAcknowledged)
                throw new InvalidOperationException("Delete a message was unsuccessful.");
        }
    }
}
