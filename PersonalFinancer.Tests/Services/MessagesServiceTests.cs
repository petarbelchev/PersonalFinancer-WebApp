using MongoDB.Driver;
using MongoDB.Driver.Linq;

using NUnit.Framework;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Infrastructure;
using PersonalFinancer.Services.Messages;
using PersonalFinancer.Services.Messages.Models;

namespace PersonalFinancer.Tests.Services
{
	[TestFixture]
	internal class MessagesServiceTests : UnitTestsBase
	{
		private MessagesService messagesService;

		[SetUp]
		public void SetUp()
		{
			IMongoRepository<Message> repo = new MongoRepository<Message>(this.mongoDbContext);
			this.messagesService = new MessagesService(repo, this.mapper);
		}

		[Test]
		public async Task GelAllAsync_ShouldReturnCorrectData()
		{
			//Arrange
			var expect = await messagesCollection.AsQueryable()
				.Select(m => new MessageOutputServiceModel
				{
					Id = m.Id,
					CreatedOn = m.CreatedOn,
					Subject = m.Subject
				})
				.ToListAsync();

			//Act
			var actual = await messagesService.GetAllAsync();

			//Assert
			Assert.That(actual.Count(), Is.EqualTo(expect.Count));

			for (int i = 0; i < expect.Count; i++)
			{
				Assert.That(actual.ElementAt(i).Id,
					Is.EqualTo(expect[i].Id));
				Assert.That(actual.ElementAt(i).CreatedOn,
					Is.EqualTo(expect[i].CreatedOn));
				Assert.That(actual.ElementAt(i).Subject,
					Is.EqualTo(expect[i].Subject));
			}
		}

		[Test]
		public async Task GetUserMessagesAsync_ShouldReturnCorrectData()
		{
			//Arrange
			var expect = await messagesCollection.AsQueryable()
				.Where(m => m.AuthorId == User1.Id)
				.Select(m => new MessageOutputServiceModel
				{
					Id = m.Id,
					Subject = m.Subject,
					CreatedOn = m.CreatedOn
				})
				.ToListAsync();

			//Act
			var actual = await messagesService.GetUserMessagesAsync(User1.Id);

			//Assert
			Assert.That(actual.Count(), Is.EqualTo(expect.Count));

			for (int i = 0; i < expect.Count; i++)
			{
				Assert.That(actual.ElementAt(i).Id,
					Is.EqualTo(expect[i].Id));
				Assert.That(actual.ElementAt(i).CreatedOn,
					Is.EqualTo(expect[i].CreatedOn));
				Assert.That(actual.ElementAt(i).Subject,
					Is.EqualTo(expect[i].Subject));
			}
		}

		[Test]
		public async Task GetMessageAsync_ShouldReturnCorrectData_WhenUserIsAuthor()
		{
			//Arrange
			var expect = await messagesCollection.AsQueryable()
				.Where(m => m.Id == Message1User1.Id)
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

			//Act
			var actual = await messagesService
				.GetMessageAsync(Message1User1.Id, User1.Id, isUserAdmin: false);

			//Assert
			Assert.That(actual.Id, Is.EqualTo(expect.Id));
			Assert.That(actual.Subject, Is.EqualTo(expect.Subject));
			Assert.That(actual.Content, Is.EqualTo(expect.Content));
			Assert.That(actual.AuthorName, Is.EqualTo(expect.AuthorName));
			Assert.That(actual.CreatedOn, Is.EqualTo(expect.CreatedOn));
			Assert.That(actual.Replies.Count(), Is.EqualTo(expect.Replies.Count()));

			for (int i = 0; i < expect.Replies.Count(); i++)
			{
				Assert.That(actual.Replies.ElementAt(i).Content,
					Is.EqualTo(expect.Replies.ElementAt(i).Content));
				Assert.That(actual.Replies.ElementAt(i).AuthorName,
					Is.EqualTo(expect.Replies.ElementAt(i).AuthorName));
				Assert.That(actual.Replies.ElementAt(i).CreatedOn,
					Is.EqualTo(expect.Replies.ElementAt(i).CreatedOn));
			}
		}
		
		[Test]
		public async Task GetMessageAsync_ShouldReturnCorrectData_WhenUserIsAdmin()
		{
			//Arrange
			var expect = await messagesCollection.AsQueryable()
				.Where(m => m.Id == Message1User1.Id)
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

			//Act
			var actual = await messagesService
				.GetMessageAsync(Message1User1.Id, User2.Id, isUserAdmin: true);

			//Assert
			Assert.That(actual.Id, Is.EqualTo(expect.Id));
			Assert.That(actual.Subject, Is.EqualTo(expect.Subject));
			Assert.That(actual.Content, Is.EqualTo(expect.Content));
			Assert.That(actual.AuthorName, Is.EqualTo(expect.AuthorName));
			Assert.That(actual.CreatedOn, Is.EqualTo(expect.CreatedOn));
			Assert.That(actual.Replies.Count(), Is.EqualTo(expect.Replies.Count()));

			for (int i = 0; i < expect.Replies.Count(); i++)
			{
				Assert.That(actual.Replies.ElementAt(i).Content,
					Is.EqualTo(expect.Replies.ElementAt(i).Content));
				Assert.That(actual.Replies.ElementAt(i).AuthorName,
					Is.EqualTo(expect.Replies.ElementAt(i).AuthorName));
				Assert.That(actual.Replies.ElementAt(i).CreatedOn,
					Is.EqualTo(expect.Replies.ElementAt(i).CreatedOn));
			}
		}
				
		[Test]
		public void GetMessageAsync_ShouldThrowException_WhenUserIsNotAuthor()
		{
			//Act & Assert
			Assert.That(async () => await messagesService
				.GetMessageAsync(Message1User1.Id, User2.Id, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}
		
		[Test]
		public void GetMessageAsync_ShouldThrowException_WhenMessageDoesNotExist()
		{
			//Arrange
			string invalidId = "DFFCFE4DB5712BAC41909A7E";

			//Act & Assert
			Assert.That(async () => await messagesService
				.GetMessageAsync(invalidId, User2.Id, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task CreateAsync_ShouldCreateNewMessage_WithValidInput()
		{
			//Arrange
			long totalCountBefore = await messagesCollection
				.CountDocumentsAsync(_ => true);

			long userCountBefore = await messagesCollection
				.CountDocumentsAsync(d => d.AuthorId == User1.Id);

			//Act
			var inputModel = new MessageInputServiceModel
			{
				AuthorId = User1.Id,
				AuthorName = $"{User1.FirstName} {User1.LastName}",
				Subject = "Test Subject",
				Content = "Test Content"
			};
			string newMessageId = await messagesService.CreateAsync(inputModel);

			var newMessage = await messagesCollection.AsQueryable()
				.FirstAsync(d => d.Id == newMessageId);

			long totalCountAfter = await messagesCollection
				.CountDocumentsAsync(_ => true);

			long userCountAfter = await messagesCollection
				.CountDocumentsAsync(d => d.AuthorId == User1.Id);

			//Assert
			Assert.That(newMessageId, Is.Not.Null);
			Assert.That(newMessage.Id, Is.EqualTo(newMessageId));
			Assert.That(newMessage.AuthorId, Is.EqualTo(inputModel.AuthorId));
			Assert.That(newMessage.AuthorName, Is.EqualTo(inputModel.AuthorName));
			Assert.That(newMessage.Subject, Is.EqualTo(inputModel.Subject));
			Assert.That(newMessage.Content, Is.EqualTo(inputModel.Content));
			Assert.That(userCountAfter, Is.EqualTo(userCountBefore + 1));
			Assert.That(totalCountAfter, Is.EqualTo(totalCountBefore + 1));
		}

		[Test]
		public async Task AddReplyAsync_ShouldAddNewReplyToMessage_WhenUserIsMessageAuthor()
		{
			//Arrange
			int repliesCountBefore = await messagesCollection.AsQueryable()
				.Where(m => m.Id == Message1User1.Id)
				.Select(m => m.Replies.Count)
				.FirstAsync();

			//Act
			var inputModel = new ReplyInputServiceModel
			{
				AuthorId = User1.Id,
				AuthorName = $"{User1.FirstName} {User1.LastName}",
				MessageId = Message1User1.Id,
				Content = "New test reply",
				IsAuthorAdmin = false
			};
			await messagesService.AddReplyAsync(inputModel);
			
			int repliesCountAfter = await messagesCollection.AsQueryable()
				.Where(m => m.Id == Message1User1.Id)
				.Select(m => m.Replies.Count)
				.FirstAsync();

			Reply reply = await messagesCollection.AsQueryable()
				.Where(m => m.Id == inputModel.MessageId)
				.Select(m => m.Replies.Last())
				.FirstAsync();

			//Assert
			Assert.That(repliesCountAfter, Is.EqualTo(repliesCountBefore + 1));
			Assert.That(reply.Content, Is.EqualTo(inputModel.Content));
			Assert.That(reply.AuthorId, Is.EqualTo(inputModel.AuthorId));
			Assert.That(reply.AuthorName, Is.EqualTo(inputModel.AuthorName));
		}
		
		[Test]
		public async Task AddReplyAsync_ShouldAddNewReplyToMessage_WhenUserIsAdmin()
		{
			//Arrange
			int repliesCountBefore = await messagesCollection.AsQueryable()
				.Where(m => m.Id == Message1User1.Id)
				.Select(m => m.Replies.Count)
				.FirstAsync();

			//Act
			var inputModel = new ReplyInputServiceModel
			{
				AuthorId = User2.Id,
				AuthorName = $"{User2.FirstName} {User2.LastName}",
				MessageId = Message1User1.Id,
				Content = "New test reply",
				IsAuthorAdmin = true
			};
			await messagesService.AddReplyAsync(inputModel);
			
			int repliesCountAfter = await messagesCollection.AsQueryable()
				.Where(m => m.Id == Message1User1.Id)
				.Select(m => m.Replies.Count)
				.FirstAsync();

			Reply reply = await messagesCollection.AsQueryable()
				.Where(m => m.Id == inputModel.MessageId)
				.Select(m => m.Replies.Last())
				.FirstAsync();

			//Assert
			Assert.That(repliesCountAfter, Is.EqualTo(repliesCountBefore + 1));
			Assert.That(reply.Content, Is.EqualTo(inputModel.Content));
			Assert.That(reply.AuthorId, Is.EqualTo(inputModel.AuthorId));
			Assert.That(reply.AuthorName, Is.EqualTo(inputModel.AuthorName));
		}

		[Test]
		public void AddReplyAsync_ShouldThrowException_WhenUserIsNotAuthorized()
		{
			//Act & Assert
			var inputModel = new ReplyInputServiceModel
			{
				AuthorId = User2.Id,
				AuthorName = $"{User2.FirstName} {User2.LastName}",
				MessageId = Message1User1.Id,
				Content = "New test reply",
				IsAuthorAdmin = false
			};
			Assert.That(async () => await messagesService.AddReplyAsync(inputModel),
			Throws.TypeOf<ArgumentException>().With.Message
				.EqualTo("The User is not Authorized to make replies."));
		}

		[Test]
		public async Task RemoveAsync_ShouldRemoveMessage_WhenUserIsAuthor()
		{
			//Arrange
			Message message = new Message
			{
				AuthorId = User1.Id,
				AuthorName = $"{User1.FirstName} {User1.LastName}",
				CreatedOn = DateTime.UtcNow,
				Subject = "Test Message",
				Content = "This is a test message"
			};
			await messagesCollection.InsertOneAsync(message);

			long messagesCountBefore = await messagesCollection
				.CountDocumentsAsync(_ => true);

			//Act
			await messagesService.RemoveAsync(message.Id, User1.Id, isUserAdmin: false);
			
			long messagesCountAfter = await messagesCollection
				.CountDocumentsAsync(_ => true);
			
			Message? messageInDb = await messagesCollection
				.AsQueryable()
				.FirstOrDefaultAsync(m => m.Id == message.Id);

			//Assert
			Assert.That(messagesCountAfter, Is.EqualTo(messagesCountBefore - 1));
			Assert.That(messageInDb, Is.Null);
		}
		
		[Test]
		public async Task RemoveAsync_ShouldRemoveMessage_WhenUserIsAdmin()
		{
			//Arrange
			Message message = new Message
			{
				AuthorId = User1.Id,
				AuthorName = $"{User1.FirstName} {User1.LastName}",
				CreatedOn = DateTime.UtcNow,
				Subject = "Test Message",
				Content = "This is a test message"
			};
			await messagesCollection.InsertOneAsync(message);

			long messagesCountBefore = await messagesCollection
				.CountDocumentsAsync(_ => true);

			//Act
			await messagesService.RemoveAsync(message.Id, User2.Id, isUserAdmin: true);
			
			long messagesCountAfter = await messagesCollection
				.CountDocumentsAsync(_ => true);
			
			Message? messageInDb = await messagesCollection
				.AsQueryable()
				.FirstOrDefaultAsync(m => m.Id == message.Id);

			//Assert
			Assert.That(messagesCountAfter, Is.EqualTo(messagesCountBefore - 1));
			Assert.That(messageInDb, Is.Null);
		}

		[Test]
		public async Task RemoveAsync_ShouldThrowException_WhenUserIsNotAuthorized()
		{
			//Arrange
			Message message = new Message
			{
				AuthorId = User1.Id,
				AuthorName = $"{User1.FirstName} {User1.LastName}",
				CreatedOn = DateTime.UtcNow,
				Subject = "Test Message",
				Content = "This is a test message"
			};
			await messagesCollection.InsertOneAsync(message);

			//Act & Assert
			Assert.That(async () => await messagesService
				.RemoveAsync(message.Id, User2.Id, isUserAdmin: false),
			Throws.TypeOf<ArgumentException>().With.Message
				.EqualTo("The User is not Authorized to make replies."));
		}
	}
}
