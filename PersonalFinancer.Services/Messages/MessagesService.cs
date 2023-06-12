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
        private readonly IMongoRepository<Message> repo;
        private readonly IMapper mapper;

        public MessagesService(
            IMongoRepository<Message> repo,
            IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<MessageOutputServiceModel>> GetAllAsync()
        {
            IEnumerable<MessageOutputServiceModel> messages = await this.repo
                .FindAsync(m => new MessageOutputServiceModel
                {
                    Id = m.Id,
                    CreatedOn = m.CreatedOn,
                    Subject = m.Subject
                });

            return messages;
        }

        public async Task<IEnumerable<MessageOutputServiceModel>> GetUserMessagesAsync(string userId)
        {
            IEnumerable<MessageOutputServiceModel> messages = await this.repo.FindAsync(
                x => x.AuthorId == userId,
                m => new MessageOutputServiceModel
                {
                    Id = m.Id,
                    CreatedOn = m.CreatedOn,
                    Subject = m.Subject
                });

            return messages;
        }

        /// <summary>
        /// Throws InvalidOperationException when Message does not exist
        /// or User is not owner or Administrator
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<MessageDetailsServiceModel> GetMessageAsync(
            string messageId, string userId, bool isUserAdmin)
        {
            MessageDetailsServiceModel message = await this.repo.FindOneAsync(
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

            return message;
        }

        public async Task<string> CreateAsync(MessageInputServiceModel model)
        {
            Message newMessage = this.mapper.Map<Message>(model);
            newMessage.CreatedOn = DateTime.UtcNow;

            await this.repo.InsertOneAsync(newMessage);

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
            if (!model.IsAuthorAdmin && !await this.repo.IsUserDocumentAuthor(model.MessageId, model.AuthorId))
                throw new ArgumentException("The User is not Authorized to make replies.");

            Reply reply = this.mapper.Map<Reply>(model);
            reply.CreatedOn = DateTime.UtcNow;

            UpdateDefinition<Message> update = Builders<Message>.Update.Push(x => x.Replies, reply);

            UpdateResult result = await this.repo.UpdateOneAsync(x => x.Id == model.MessageId, update);

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
            if (!isUserAdmin && !await this.repo.IsUserDocumentAuthor(messageId, userId))
                throw new ArgumentException("The User is not Authorized to delete the message.");

            DeleteResult result = await this.repo.DeleteOneAsync(messageId);

            if (!result.IsAcknowledged)
                throw new InvalidOperationException("Delete a message was unsuccessful.");
        }
    }
}
