using AutoMapper;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Moq;
using NUnit.Framework;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Data.Repositories;
using PersonalFinancer.Services.Messages;
using PersonalFinancer.Services.Messages.Models;
using PersonalFinancer.Tests.Mocks;

namespace PersonalFinancer.Tests.Services
{
	[TestFixture]
	internal class MessagesServiceTests
	{
		private Mock<IMongoRepository<Message>> repoMock;
		private MessagesService service;
		private readonly IMapper mapper = ServicesMapperMock.Instance;

		[SetUp]
		public void SetUp()
		{
			repoMock = new Mock<IMongoRepository<Message>>();
			service = new MessagesService(repoMock.Object, this.mapper);
		}

		[Test]
		public async Task GelAllAsync_ShouldReturnCorrectData()
		{
			//Arrange
			var expect = new List<MessageOutputServiceModel>
			{
				new MessageOutputServiceModel
				{
					Id = "1",
					CreatedOn = DateTime.UtcNow,
					Subject = "Test Subject 1"
				},
				new MessageOutputServiceModel
				{
					Id = "2",
					CreatedOn = DateTime.UtcNow,
					Subject = "Test Subject 2"
				}
			};
			
			repoMock.Setup(x => x.FindAsync(
				m => new MessageOutputServiceModel
				{
					Id = m.Id,
					CreatedOn = m.CreatedOn,
					Subject = m.Subject
				})).ReturnsAsync(expect);
			
			//Act
			var actual = await service.GetAllAsync();

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
			var expect = new List<MessageOutputServiceModel>
			{
				new MessageOutputServiceModel
				{
					Id = "1",
					CreatedOn = DateTime.UtcNow,
					Subject = "Test Subject 1"
				}
			};
			
			string userId = "user id";
			
			repoMock.Setup(x => x.FindAsync(
				m => m.AuthorId == userId, 
				m => new MessageOutputServiceModel
				{
					Id = m.Id,
					CreatedOn = m.CreatedOn,
					Subject = m.Subject
				})).ReturnsAsync(expect);

			//Act
			var actual = await service.GetUserMessagesAsync(userId);

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
			var expect = new MessageDetailsServiceModel
			{
				Id = "1",
				AuthorName = "Test Author Name",
				Content = "Test Content",
				Subject = "Test Subject",
				CreatedOn = DateTime.UtcNow,
				Replies = new List<ReplyOutputServiceModel>
				{
					new ReplyOutputServiceModel
					{
						AuthorName = "Test Reply Author",
						CreatedOn = DateTime.UtcNow,
						Content = "Test Reply Content"
					}
				}
			};

			string messageId = "messageId";
			bool isUserAdmin = false;
			string userId = "userId";

			repoMock.Setup(x => x.FindOneAsync(
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
				})).ReturnsAsync(expect);

			//Act
			var actual = await service.GetMessageAsync(messageId, userId, isUserAdmin);

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
			var expect = new MessageDetailsServiceModel
			{
				Id = "1",
				AuthorName = "Test Author Name",
				Content = "Test Content",
				Subject = "Test Subject",
				CreatedOn = DateTime.UtcNow,
				Replies = new List<ReplyOutputServiceModel>
				{
					new ReplyOutputServiceModel
					{
						AuthorName = "Test Reply Author",
						CreatedOn = DateTime.UtcNow,
						Content = "Test Reply Content"
					}
				}
			};

			string messageId = "messageId";
			bool isUserAdmin = true;
			string userId = "userId";

			repoMock.Setup(x => x.FindOneAsync(
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
				})).ReturnsAsync(expect);

			//Act
			var actual = await service.GetMessageAsync(messageId, userId, isUserAdmin);

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
		public void GetMessageAsync_ShouldThrowException_WhenUserIsNotAuthorOrAdmin()
		{
			//Arrange
			string messageId = "messageId";
			bool isUserAdmin = false;
			string notAuthorId = "notAuthorId";

			repoMock.Setup(x => x.FindOneAsync(
				x => x.Id == messageId && (isUserAdmin || x.AuthorId == notAuthorId),
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
				})).Throws<InvalidOperationException>();

			//Act & Assert
			Assert.That(async () => await service
				.GetMessageAsync(messageId, notAuthorId, isUserAdmin),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task CreateAsync_ShouldCreateNewMessage_WithValidInput()
		{
			//Arrange
			var fakeCollection = new List<Message>();

			var inputModel = new MessageInputServiceModel
			{
				AuthorId = "Test Author Id",
				AuthorName = "Test Author Name",
				Subject = "Test Subject",
				Content = "Test Content"
			};

			string newMessageId = "New Message Id";

			repoMock.Setup(x => x.InsertOneAsync(It.IsAny<Message>()))
				.Callback((Message message) =>
				{
					message.Id = newMessageId;
					fakeCollection.Add(message);
				});

			//Act
			string returnedId = await service.CreateAsync(inputModel);
			var msgInCollection = fakeCollection.First();

			//Assert
			Assert.That(returnedId, Is.Not.Null);
			Assert.That(returnedId, Is.EqualTo(newMessageId));
			Assert.That(fakeCollection.Count, Is.EqualTo(1));
			Assert.That(msgInCollection.AuthorId, Is.EqualTo(inputModel.AuthorId));
			Assert.That(msgInCollection.AuthorName, Is.EqualTo(inputModel.AuthorName));
			Assert.That(msgInCollection.Subject, Is.EqualTo(inputModel.Subject));
			Assert.That(msgInCollection.Content, Is.EqualTo(inputModel.Content));
		}

		[Test]
		public async Task AddReplyAsync_ShouldAddNewReplyToMessage_WhenUserIsMessageAuthor()
		{
			//Arrange
			var inputModel = new ReplyInputServiceModel
			{
				AuthorId = "Test Author Id",
				AuthorName = "Test Author Name",
				MessageId = "Test Message Id",
				Content = "Test Content",
				IsAuthorAdmin = false
			};

			var updateResultMock = new Mock<UpdateResult>();
			updateResultMock.Setup(x => x.IsAcknowledged).Returns(true);

			repoMock.Setup(x => x
				.IsUserDocumentAuthor(inputModel.MessageId, inputModel.AuthorId))
				.ReturnsAsync(true);

			repoMock.Setup(x => x
				.UpdateOneAsync(
					x => x.Id == inputModel.MessageId,
					It.IsAny<UpdateDefinition<Message>>()))
				.ReturnsAsync(updateResultMock.Object);

			//Act
			var result = await service.AddReplyAsync(inputModel);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.IsAcknowledged, Is.True);
		}

		[Test]
		public async Task AddReplyAsync_ShouldAddNewReplyToMessage_WhenUserIsAdmin()
		{
			//Arrange
			var inputModel = new ReplyInputServiceModel
			{
				AuthorId = "Test Author Id",
				AuthorName = "Test Author Name",
				MessageId = "Test Message Id",
				Content = "Test Content",
				IsAuthorAdmin = true
			};

			var updateResultMock = new Mock<UpdateResult>();
			updateResultMock.Setup(x => x.IsAcknowledged).Returns(true);

			repoMock.Setup(x => x
				.IsUserDocumentAuthor(inputModel.MessageId, inputModel.AuthorId))
				.ReturnsAsync(false);

			repoMock.Setup(x => x
				.UpdateOneAsync(
					x => x.Id == inputModel.MessageId,
					It.IsAny<UpdateDefinition<Message>>()))
				.ReturnsAsync(updateResultMock.Object);

			//Act
			var result = await service.AddReplyAsync(inputModel);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.IsAcknowledged, Is.True);
		}

		[Test]
		public void AddReplyAsync_ShouldThrowException_WhenUserIsNotAuthorized()
		{
			//Arrange
			var inputModel = new ReplyInputServiceModel
			{
				AuthorId = "Test Author Id",
				AuthorName = "Test Author Name",
				MessageId = "Test Message Id",
				Content = "Test Content",
				IsAuthorAdmin = false
			};

			repoMock.Setup(x => x
				.IsUserDocumentAuthor(inputModel.MessageId, inputModel.AuthorId))
				.ReturnsAsync(false);

			//Act & Assert
			Assert.That(async () => await service.AddReplyAsync(inputModel),
			Throws.TypeOf<ArgumentException>().With.Message
				.EqualTo("The User is not Authorized to make replies."));
		}

		[Test]
		public async Task RemoveAsync_ShouldRemoveMessage_WhenUserIsAuthor()
		{
			//Arrange
			Message message = new Message
			{
				Id = "Test Message Id",
				AuthorId = "Test Author Id",
				AuthorName = "Test Author Name",
				CreatedOn = DateTime.UtcNow,
				Subject = "Test Message Subject",
				Content = "This Message Content"
			};
			var fakeCollection = new List<Message> { message };

			repoMock.Setup(x => x
				.IsUserDocumentAuthor(message.Id, message.AuthorId))
				.ReturnsAsync(true);

			repoMock.Setup(x => x
				.DeleteOneAsync(message.Id))
				.ReturnsAsync(() =>
				{
					fakeCollection.Remove(message);
					return new DeleteResult.Acknowledged(1);
				});

			//Act
			await service.RemoveAsync(message.Id, message.AuthorId, isUserAdmin: false);

			//Assert
			Assert.That(fakeCollection.Any(), Is.False);
		}

		[Test]
		public async Task RemoveAsync_ShouldRemoveMessage_WhenUserIsAdmin()
		{
			//Arrange
			Message message = new Message
			{
				Id = "Test Message Id",
				AuthorId = "Test Author Id",
				AuthorName = "Test Author Name",
				CreatedOn = DateTime.UtcNow,
				Subject = "Test Message Subject",
				Content = "This Message Content"
			};
			var fakeCollection = new List<Message> { message };

			repoMock.Setup(x => x
				.IsUserDocumentAuthor(message.Id, message.AuthorId))
				.ReturnsAsync(false);

			repoMock.Setup(x => x
				.DeleteOneAsync(message.Id))
				.ReturnsAsync(() =>
				{
					fakeCollection.Remove(message);
					return new DeleteResult.Acknowledged(1);
				});

			//Act
			await service.RemoveAsync(message.Id, message.AuthorId, isUserAdmin: true);

			//Assert
			Assert.That(fakeCollection.Any(), Is.False);
		}

		[Test]
		public void RemoveAsync_ShouldThrowException_WhenUserIsNotAuthorized()
		{
			//Arrange
			Message message = new Message
			{
				Id = "Test Message Id",
				AuthorId = "Test Author Id",
				AuthorName = "Test Author Name",
				CreatedOn = DateTime.UtcNow,
				Subject = "Test Message Subject",
				Content = "This Message Content"
			};
			var fakeCollection = new List<Message> { message };

			repoMock.Setup(x => x
				.IsUserDocumentAuthor(message.Id, message.AuthorId))
				.ReturnsAsync(false);

			//Act & Assert
			Assert.That(async () => await service
				.RemoveAsync(message.Id, message.AuthorId, isUserAdmin: false),
			Throws.TypeOf<ArgumentException>().With.Message
				.EqualTo("The User is not Authorized to delete the message."));
		}
	}
}
