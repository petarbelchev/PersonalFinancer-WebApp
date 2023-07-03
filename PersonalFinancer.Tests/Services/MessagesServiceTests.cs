namespace PersonalFinancer.Tests.Services
{
	using AutoMapper;
	using MongoDB.Driver;
	using MongoDB.Driver.Linq;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Repositories;
	using PersonalFinancer.Services.Messages;
	using PersonalFinancer.Services.Messages.Models;
	using PersonalFinancer.Tests.Mocks;

	[TestFixture]
	internal class MessagesServiceTests : UnitTestsBase
	{
		private readonly string AdminId = Guid.NewGuid().ToString();
		private readonly string AdminName = "Admin Name";
		private readonly string FirstUserId = Guid.NewGuid().ToString();
		private readonly string FirstUserName = "First User Name";
		private readonly string SecondUserId = Guid.NewGuid().ToString();
		private readonly string SecondUserName = "Second User Name";
		private ICollection<Message> fakeCollection;

		private Mock<UpdateResult> updateResultMock;
		private Mock<IMongoRepository<Message>> repoMock;
		private readonly IMapper mapper = ServicesMapperMock.Instance;

		private MessagesService messagesService;

		[SetUp]
		public void SetUp()
		{
			this.fakeCollection = this.SeedFakeCollection();
			this.updateResultMock = new Mock<UpdateResult>();
			this.repoMock = new Mock<IMongoRepository<Message>>();
			this.messagesService = new MessagesService(this.repoMock.Object, this.mapper);
		}

		[Test]
		public async Task GelAllAsync_ShouldReturnCorrectData()
		{
			//Arrange
			MessageOutputDTO[] expected = this.fakeCollection
				.Select(m => new MessageOutputDTO
				{
					Id = m.Id,
					CreatedOn = m.CreatedOn,
					Subject = m.Subject
				})
				.ToArray();

			this.repoMock.Setup(x => x
				.FindAsync(m => new MessageOutputDTO
				{
					Id = m.Id,
					CreatedOn = m.CreatedOn,
					Subject = m.Subject
				}))
				.ReturnsAsync(expected);

			//Act
			IEnumerable<MessageOutputDTO> actual = await this.messagesService.GetAllAsync();

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual.Count(), Is.EqualTo(expected.Length));
				AssertAreEqualAsJson(actual, expected);
			});
		}

		[Test]
		public async Task GetUserMessagesAsync_ShouldReturnCorrectData()
		{
			//Arrange
			MessageOutputDTO[] expected = this.fakeCollection
				.Where(m => m.AuthorId == this.FirstUserId)
				.Select(m => new MessageOutputDTO
				{
					Id = m.Id,
					CreatedOn = m.CreatedOn,
					Subject = m.Subject
				})
				.ToArray();

			string userId = this.FirstUserId;

			this.repoMock.Setup(x => x.FindAsync(
					m => m.AuthorId == userId,
					m => new MessageOutputDTO
					{
						Id = m.Id,
						CreatedOn = m.CreatedOn,
						Subject = m.Subject
					}))
				.ReturnsAsync(expected);

			//Act
			IEnumerable<MessageOutputDTO> actual = await this.messagesService.GetUserMessagesAsync(userId);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual.Count(), Is.EqualTo(expected.Length));
				AssertAreEqualAsJson(actual, expected);
			});
		}

		[Test]
		public async Task GetMessageAsync_ShouldReturnCorrectData_WhenUserIsAuthor()
		{
			//Arrange
			string messageId = "1";
			bool isUserAdmin = false;
			string userId = this.FirstUserId;

			MessageDetailsDTO expect = this.fakeCollection
				.Where(m => m.Id == messageId && (isUserAdmin || m.AuthorId == userId))
				.Select(m => new MessageDetailsDTO
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
				})
				.First();

			this.repoMock.Setup(x => x
				.FindOneAsync(
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
					}))
				.ReturnsAsync(expect);

			//Act
			MessageDetailsDTO actual = await this.messagesService.GetMessageAsync(messageId, userId, isUserAdmin);

			//Assert
			AssertAreEqualAsJson(actual, expect);
		}

		[Test]
		public async Task GetMessageAsync_ShouldReturnCorrectData_WhenUserIsAdmin()
		{
			//Arrange
			string messageId = "1";
			bool isUserAdmin = true;
			string userId = this.AdminId;

			MessageDetailsDTO expect = this.fakeCollection
				.Where(m => m.Id == messageId && (isUserAdmin || m.AuthorId == userId))
				.Select(m => new MessageDetailsDTO
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
				})
				.First();

			this.repoMock.Setup(x => x
				.FindOneAsync(
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
					}))
				.ReturnsAsync(expect);

			//Act
			MessageDetailsDTO actual = await this.messagesService.GetMessageAsync(messageId, userId, isUserAdmin);

			//Assert
			AssertAreEqualAsJson(actual, expect);
		}

		[Test]
		public void GetMessageAsync_ShouldThrowException_WhenUserIsNotAuthorOrAdmin()
		{
			//Arrange
			string messageId = "1";
			bool isUserAdmin = false;
			string notAuthorId = Guid.NewGuid().ToString();

			this.repoMock.Setup(x => x
				.FindOneAsync(
					x => x.Id == messageId && (isUserAdmin || x.AuthorId == notAuthorId),
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
					}))
				.Throws<InvalidOperationException>();

			//Act & Assert
			Assert.That(async () => await this.messagesService
				  .GetMessageAsync(messageId, notAuthorId, isUserAdmin),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task CreateAsync_ShouldCreateNewMessage_WithValidInput()
		{
			//Arrange
			var inputModel = new MessageInputDTO
			{
				AuthorId = this.FirstUserId,
				AuthorName = this.FirstUserName,
				Subject = "New Message Subject",
				Content = "New Message Content"
			};

			string expectedNewMessageId = "New Message Id";

			this.repoMock.Setup(x => x
				.InsertOneAsync(It.Is<Message>(m => 
					m.AuthorId == inputModel.AuthorId
					&& m.AuthorName == inputModel.AuthorName
					&& m.Subject == inputModel.Subject
					&& m.Content == inputModel.Content)))
				.Callback((Message message) =>
				{
					message.Id = expectedNewMessageId;
					this.fakeCollection.Add(message);
				});

			int messagesCountBefore = this.fakeCollection.Count;

			//Act
			string actualNewMessageId = await this.messagesService.CreateAsync(inputModel);

			//Arrange
			Message actualNewMessage = this.fakeCollection.First(m => m.Id == actualNewMessageId);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actualNewMessageId, Is.EqualTo(expectedNewMessageId));
				Assert.That(this.fakeCollection, Has.Count.EqualTo(messagesCountBefore + 1));
				AssertSamePropertiesValuesAreEqual(actualNewMessage, inputModel);
			});
		}

		[Test]
		public async Task AddReplyAsync_ShouldAddNewReplyToMessage_WhenUserIsMessageAuthor()
		{
			//Arrange
			var inputModel = new ReplyInputDTO
			{
				MessageId = "1",
				AuthorId = this.FirstUserId,
				AuthorName = this.FirstUserName,
				Content = "First User Test Reply Content",
				IsAuthorAdmin = false
			};

			this.updateResultMock.Setup(x => x.IsAcknowledged).Returns(true);

			this.repoMock.Setup(x => x
				.IsUserDocumentAuthor(inputModel.MessageId, inputModel.AuthorId))
				.ReturnsAsync(true);

			this.repoMock.Setup(x => x
				.UpdateOneAsync(x => 
					x.Id == inputModel.MessageId,
					It.IsAny<UpdateDefinition<Message>>()))
				.ReturnsAsync(this.updateResultMock.Object);

			//Act
			UpdateResult result = await this.messagesService.AddReplyAsync(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.IsAcknowledged, Is.True);
			});
		}

		[Test]
		public async Task AddReplyAsync_ShouldAddNewReplyToMessage_WhenUserIsAdmin()
		{
			//Arrange
			var inputModel = new ReplyInputDTO
			{
				MessageId = "1",
				AuthorId = this.AdminId,
				AuthorName = this.AdminName,
				Content = "Admin Test Reply Content",
				IsAuthorAdmin = true
			};

			this.updateResultMock.Setup(x => x.IsAcknowledged).Returns(true);

			this.repoMock.Setup(x => x
				.IsUserDocumentAuthor(inputModel.MessageId, inputModel.AuthorId))
				.ReturnsAsync(false);

			this.repoMock.Setup(x => x
				.UpdateOneAsync(x =>
					x.Id == inputModel.MessageId,
					It.IsAny<UpdateDefinition<Message>>()))
				.ReturnsAsync(this.updateResultMock.Object);

			//Act
			UpdateResult result = await this.messagesService.AddReplyAsync(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.IsAcknowledged, Is.True);
			});
		}

		[Test]
		public void AddReplyAsync_ShouldThrowException_WhenUserIsNotAuthorized()
		{
			//Arrange
			var inputModel = new ReplyInputDTO
			{
				MessageId = "1",
				AuthorId = this.SecondUserId,
				AuthorName = this.SecondUserName,
				Content = "Second User Unauthorized Test Reply Content",
				IsAuthorAdmin = false
			};

			this.repoMock.Setup(x => x
				.IsUserDocumentAuthor(inputModel.MessageId, inputModel.AuthorId))
				.ReturnsAsync(false);

			//Act & Assert
			Assert.That(async () => await this.messagesService.AddReplyAsync(inputModel),
			Throws.TypeOf<ArgumentException>().With.Message
				  .EqualTo(ExceptionMessages.UnauthorizedUser));
		}

		[Test]
		public async Task RemoveAsync_ShouldRemoveMessage_WhenUserIsAuthor()
		{
			//Arrange
			Message message = this.fakeCollection.First(m => m.Id == "1");

			this.repoMock.Setup(x => x
				.IsUserDocumentAuthor(message.Id, message.AuthorId))
				.ReturnsAsync(true);

			this.repoMock.Setup(x => x
				.DeleteOneAsync(message.Id))
				.ReturnsAsync(() =>
				{
					this.fakeCollection.Remove(message);
					return new DeleteResult.Acknowledged(1);
				});

			int messagesBefore = this.fakeCollection.Count;

			//Act
			await this.messagesService.RemoveAsync(message.Id, message.AuthorId, isUserAdmin: false);

			//Assert
			Assert.That(this.fakeCollection, Has.Count.EqualTo(messagesBefore - 1));
			Assert.That(this.fakeCollection.FirstOrDefault(m => m.Id == "1"), Is.Null);
		}

		[Test]
		public async Task RemoveAsync_ShouldRemoveMessage_WhenUserIsAdmin()
		{
			//Arrange
			Message message = this.fakeCollection.First(m => m.Id == "1");

			string adminId = this.AdminId;

			this.repoMock.Setup(x => x
				.IsUserDocumentAuthor(message.Id, adminId))
				.ReturnsAsync(false);

			this.repoMock.Setup(x => x
				.DeleteOneAsync(message.Id))
				.ReturnsAsync(() =>
				{
					this.fakeCollection.Remove(message);
					return new DeleteResult.Acknowledged(1);
				});

			int messagesBefore = this.fakeCollection.Count;

			//Act
			await this.messagesService.RemoveAsync(message.Id, adminId, isUserAdmin: true);

			//Assert
			Assert.That(this.fakeCollection, Has.Count.EqualTo(messagesBefore - 1));
			Assert.That(this.fakeCollection.FirstOrDefault(m => m.Id == "1"), Is.Null);
		}

		[Test]
		public void RemoveAsync_ShouldThrowException_WhenUserIsNotAuthorized()
		{
			//Arrange
			Message message = this.fakeCollection.First(m => m.Id == "1");

			string notAuthorId = this.SecondUserId;

			this.repoMock.Setup(x => x
				.IsUserDocumentAuthor(message.Id, notAuthorId))
				.ReturnsAsync(false);

			//Act & Assert
			Assert.That(async () => await this.messagesService
				  .RemoveAsync(message.Id, notAuthorId, isUserAdmin: false),
			Throws.TypeOf<ArgumentException>().With.Message
				  .EqualTo(ExceptionMessages.UnauthorizedUser));
		}

		private ICollection<Message> SeedFakeCollection()
		{
			return new List<Message>
			{
				new Message
				{
					Id = "1",
					CreatedOn = DateTime.UtcNow,
					Subject = "First User First Message",
					AuthorId = this.FirstUserId,
					AuthorName = this.FirstUserName,
					Content = "First User First Message Content",
					Replies = new List<Reply>
					{
						new Reply
						{
							AuthorId = this.AdminId,
							AuthorName = this.AdminName,
							Content = "Admin First Message Reply Content",
							CreatedOn = DateTime.UtcNow
						}
					}
				},
				new Message
				{
					Id = "2",
					CreatedOn = DateTime.UtcNow,
					Subject = "First User Second Message",
					AuthorId = this.FirstUserId,
					AuthorName = this.FirstUserName,
					Content = "First User Second Message Content",
				}
			};
		}
	}
}
