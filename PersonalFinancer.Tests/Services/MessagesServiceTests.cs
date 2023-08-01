namespace PersonalFinancer.Tests.Services
{
	using AutoMapper;
	using Microsoft.AspNetCore.Http;
	using MongoDB.Bson;
	using MongoDB.Bson.Serialization;
	using MongoDB.Driver;
	using MongoDB.Driver.Linq;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Repositories;
	using PersonalFinancer.Services;
	using PersonalFinancer.Services.Messages;
	using PersonalFinancer.Services.Messages.Models;
	using System.Linq.Expressions;
	using System.Text;
	using static PersonalFinancer.Common.Constants.PaginationConstants;

	[TestFixture]
	internal class MessagesServiceTests : UnitTestsBase
	{
		private readonly string AdminId = Guid.NewGuid().ToString();
		private readonly string AdminName = "Admin Name";
		private readonly string FirstUserId = Guid.NewGuid().ToString();
		private readonly string FirstUserName = "First User Name";
		private readonly string SecondUserId = Guid.NewGuid().ToString();
		private readonly string SecondUserName = "Second User Name";

		private static readonly IBsonSerializer<Message> documentSerializer 
			= BsonSerializer.SerializerRegistry.GetSerializer<Message>();

		private static readonly IBsonSerializerRegistry serializerRegistry 
			= BsonSerializer.SerializerRegistry;

		private readonly IMapper mapper = new MapperConfiguration(cfg => cfg
			.AddProfile<ServiceMappingProfile>())
			.CreateMapper();

		private ICollection<Message> fakeCollection;
		private Mock<UpdateResult> updateResultMock;
		private Mock<DeleteResult> deleteResultMock;
		private Mock<IMongoRepository<Message>> repoMock;
		private MessagesService messagesService;

		[SetUp]
		public void SetUp()
		{
			this.fakeCollection = this.GetFakeCollection();
			this.updateResultMock = new Mock<UpdateResult>();
			this.deleteResultMock = new Mock<DeleteResult>();
			this.repoMock = new Mock<IMongoRepository<Message>>();
			this.messagesService = new MessagesService(this.repoMock.Object, this.mapper);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public async Task AddReplyAsync_ShouldAddNewReplyToMessage(bool isUserAdmin)
		{
			//Arrange
			var inputModel = new ReplyInputDTO
			{
				MessageId = this.fakeCollection
					.Where(m => m.AuthorId == this.FirstUserId)
					.Select(m => m.Id)
					.First(),
				AuthorId = isUserAdmin ? this.AdminId : this.FirstUserId,
				AuthorName = isUserAdmin ? this.AdminName : this.FirstUserName,
				Content = "Test Reply Content",
				IsAuthorAdmin = isUserAdmin
			};

			this.repoMock
				.Setup(x => x.IsUserDocumentAuthor(inputModel.MessageId, inputModel.AuthorId))
				.ReturnsAsync(isUserAdmin ? false : true);

			this.updateResultMock
				.Setup(x => x.IsAcknowledged)
				.Returns(true);

			UpdateDefinition<Message>? actualDefinition = null;

			this.repoMock
				.Setup(x => x.UpdateOneAsync(
					x => x.Id == inputModel.MessageId,
					It.IsAny<UpdateDefinition<Message>>()))
				.Callback((Expression<Func<Message, bool>> filterExpression, UpdateDefinition<Message> updateDefinition)
					=> actualDefinition = updateDefinition)
				.ReturnsAsync(this.updateResultMock.Object);

			//Act
			ReplyOutputDTO? actualReply = await this.messagesService.AddReplyAsync(inputModel);

			//Arrange
			Reply expectedReply = this.mapper.Map<Reply>(inputModel);
			expectedReply.CreatedOnUtc = actualReply.CreatedOnUtc;

			var expectedDefinition = Builders<Message>.Update
				.Push(x => x.Replies, expectedReply)
				.Set(x => inputModel.IsAuthorAdmin ? x.IsSeenByAuthor : x.IsSeenByAdmin, false)
				.Set(x => inputModel.IsAuthorAdmin ? x.IsArchivedByAuthor : x.IsArchivedByAdmin, false);

			//Assert
			Assert.That(actualDefinition, Is.Not.Null);
			Assert.Multiple(() =>
			{
				Assert.That(ValidateUpdateDefinition(actualDefinition, expectedDefinition),
					Is.True);

				AssertSamePropertiesValuesAreEqual(actualReply, expectedReply);
			});
		}

		[Test]
		public void AddReplyAsync_ShouldThrowUnauthorizedAccessException_WhenTheUserIsUnauthorized()
		{
			//Arrange
			var inputModel = new ReplyInputDTO
			{
				MessageId = this.fakeCollection
					.Where(m => m.AuthorId == this.FirstUserId)
					.Select(m => m.Id)
					.First(),
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
			Throws.TypeOf<UnauthorizedAccessException>().With.Message.EqualTo(ExceptionMessages.UnauthorizedUser));
		}

		[Test]
		public void AddReplyAsync_ShouldThrowInvalidOperationException_WhenTheUpdateWasUnsuccessful()
		{
			//Arrange
			var inputModel = new ReplyInputDTO
			{
				MessageId = this.fakeCollection
					.Where(m => m.AuthorId == this.FirstUserId)
					.Select(m => m.Id)
					.First(),
				AuthorId = this.FirstUserId,
				AuthorName = this.FirstUserId,
				Content = "First User Unsuccessful Reply Content",
				IsAuthorAdmin = false
			};

			this.repoMock
				.Setup(x => x.IsUserDocumentAuthor(inputModel.MessageId, inputModel.AuthorId))
				.ReturnsAsync(true);

			this.updateResultMock
				.Setup(x => x.IsAcknowledged)
				.Returns(false);

			this.repoMock
				.Setup(x => x.UpdateOneAsync(
					x => x.Id == inputModel.MessageId,
					It.IsAny<UpdateDefinition<Message>>()))
				.ReturnsAsync(this.updateResultMock.Object);

			//Act & Assert
			Assert.That(async () => await this.messagesService.AddReplyAsync(inputModel),
			Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo(ExceptionMessages.UnsuccessfulUpdate));
		}

		[Test]
		public void ArchiveAsync_ShouldArchiveTheMessageForTheAuthor()
		{
			//Arrange
			bool isUserAdmin = false;
			string userId = this.FirstUserId;
			string messageId = this.fakeCollection
				.Where(m => m.AuthorId == userId && !m.IsArchivedByAuthor)
				.Select(m => m.Id)
				.First();

			this.repoMock
				.Setup(x => x.IsUserDocumentAuthor(messageId, userId))
				.ReturnsAsync(true);

			this.updateResultMock
				.Setup(x => x.IsAcknowledged)
				.Returns(true);

			UpdateDefinition<Message> expectedUpdate = Builders<Message>.Update
				.Set(x => isUserAdmin ? x.IsArchivedByAdmin : x.IsArchivedByAuthor, true);

			//Act & Assert
			this.repoMock
				.Setup(x => x.UpdateOneAsync(
					x => x.Id == messageId && (isUserAdmin || x.AuthorId == userId),
					It.Is<UpdateDefinition<Message>>(actualUpdate => ValidateUpdateDefinition(actualUpdate, expectedUpdate))))
				.ReturnsAsync(this.updateResultMock.Object);

			Assert.That(async () => await this.messagesService.ArchiveAsync(messageId, userId, isUserAdmin),
			Throws.Nothing);
		}

		[Test]
		public void ArchiveAsync_ShouldThrowInvalidOperationException_WhenTheUpdateWasUnsuccessful()
		{
			//Arrange
			bool isUserAdmin = false;
			string userId = this.FirstUserId;
			string messageId = this.fakeCollection
				.Where(m => m.AuthorId == userId && !m.IsArchivedByAuthor)
				.Select(m => m.Id)
				.First();

			this.repoMock
				.Setup(x => x.IsUserDocumentAuthor(messageId, userId))
				.ReturnsAsync(true);

			this.updateResultMock
				.Setup(x => x.IsAcknowledged)
				.Returns(false);

			UpdateDefinition<Message> expectedUpdate = Builders<Message>.Update
				.Set(x => isUserAdmin ? x.IsArchivedByAdmin : x.IsArchivedByAuthor, true);

			//Act & Assert
			this.repoMock
			.Setup(x => x.UpdateOneAsync(
					x => x.Id == messageId && (isUserAdmin || x.AuthorId == userId),
					It.Is<UpdateDefinition<Message>>(actualUpdate => ValidateUpdateDefinition(actualUpdate, expectedUpdate))))
				.ReturnsAsync(this.updateResultMock.Object);

			Assert.That(async () => await this.messagesService.ArchiveAsync(messageId, userId, isUserAdmin),
			Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo(ExceptionMessages.UnsuccessfulUpdate));
		}

		[Test]
		[TestCase("image/jpeg", 0 * 1024)]
		[TestCase("image/png", 100 * 1024)]
		[TestCase("image/jpeg", 199 * 1024)]
		[TestCase("image/png", 200 * 1024)]
		public async Task CreateAsync_ShouldCreateNewMessageWithImage_WhenTheConstraintsAreMet(string contentType, int byteArrLength)
		{
			//Arrange
			string fakeImageFile = "This is a fake image file.";
			byte[] fakeByteArr = Encoding.UTF8.GetBytes(fakeImageFile);
			var formFileMock = new Mock<IFormFile>();

			formFileMock
				.Setup(x => x.ContentType)
				.Returns(contentType);

			formFileMock
				.Setup(x => x.Length)
				.Returns(byteArrLength);

			formFileMock
				.Setup(x => x.CopyToAsync(It.IsAny<Stream>(), CancellationToken.None))
				.Callback((Stream memoryStream, CancellationToken token) =>
				{
					using var writer = new StreamWriter(memoryStream);
					writer.Write(fakeImageFile);
				});

			var inputModel = new MessageInputDTO
			{
				AuthorId = this.FirstUserId,
				AuthorName = this.FirstUserName,
				Subject = "New Message Subject",
				Content = "New Message Content",
				Image = formFileMock.Object
			};

			string expectedNewMessageId = Guid.NewGuid().ToString();

			this.repoMock
				.Setup(x => x.InsertOneAsync(It.Is<Message>(m => 
					m.AuthorId == inputModel.AuthorId 
					&& m.AuthorName == inputModel.AuthorName
					&& m.Subject == inputModel.Subject
					&& m.Content == inputModel.Content
					&& m.Image!.SequenceEqual(fakeByteArr))))
				.Callback((Message message) => message.Id = expectedNewMessageId);

			//Act
			string newMessageId = await this.messagesService.CreateAsync(inputModel);

			//Assert
			Assert.That(newMessageId, Is.EqualTo(expectedNewMessageId));
		}

		[Test]
		public async Task CreateAsync_ShouldCreateNewMessageWithoutImage_WhenTheImageIsNotProvided()
		{
			//Arrange
			var inputModel = new MessageInputDTO
			{
				AuthorId = this.FirstUserId,
				AuthorName = this.FirstUserName,
				Subject = "New Message Subject",
				Content = "New Message Content",
				Image = null
			};

			string expectedNewMessageId = Guid.NewGuid().ToString();

			this.repoMock
				.Setup(x => x.InsertOneAsync(It.Is<Message>(m =>
					m.AuthorId == inputModel.AuthorId
					&& m.AuthorName == inputModel.AuthorName
					&& m.Subject == inputModel.Subject
					&& m.Content == inputModel.Content
					&& m.Image == null)))
				.Callback((Message message) => message.Id = expectedNewMessageId);

			//Act
			string newMessageId = await this.messagesService.CreateAsync(inputModel);

			//Assert
			Assert.That(newMessageId, Is.EqualTo(expectedNewMessageId));
		}

		[Test]
		public void CreateAsync_ShouldThrowArgumentException_WhenTheImageFileTypeIsInvalid()
		{
			//Arrange
			string fakeImageFile = "This is a fake image file.";
			byte[] fakeByteArr = Encoding.UTF8.GetBytes(fakeImageFile);
			var formFileMock = new Mock<IFormFile>();

			formFileMock
				.Setup(x => x.ContentType)
				.Returns("invalid content type");

			var inputModel = new MessageInputDTO
			{
				AuthorId = this.FirstUserId,
				AuthorName = this.FirstUserName,
				Subject = "New Message Subject",
				Content = "New Message Content",
				Image = formFileMock.Object
			};

			//Act & Assert
			Assert.That(async () => await this.messagesService.CreateAsync(inputModel), 
			Throws.TypeOf<ArgumentException>().With.Message.EqualTo(ValidationMessages.InvalidImageFileType));
		}

		[Test]
		[TestCase(201 * 1024)]
		[TestCase(1000 * 1024)]
		public void CreateAsync_ShouldThrowArgumentException_WhenTheImageSizeIsInvalid(int byteArrLength)
		{
			//Arrange
			string fakeImageFile = "This is a fake image file.";
			byte[] fakeByteArr = Encoding.UTF8.GetBytes(fakeImageFile);
			var formFileMock = new Mock<IFormFile>();

			formFileMock
				.Setup(x => x.ContentType)
				.Returns("image/jpeg");

			formFileMock
				.Setup(x => x.Length)
				.Returns(byteArrLength);

			var inputModel = new MessageInputDTO
			{
				AuthorId = this.FirstUserId,
				AuthorName = this.FirstUserName,
				Subject = "New Message Subject",
				Content = "New Message Content",
				Image = formFileMock.Object
			};

			//Act & Assert
			Assert.That(async () => await this.messagesService.CreateAsync(inputModel),
			Throws.TypeOf<ArgumentException>().With.Message.EqualTo(ValidationMessages.InvalidImageSize));
		}

		[Test]
		public async Task GetAllArchivedMessagesAsync_ShouldReturnCorrectData()
		{
			//Arrange
			bool isUserAdmin = true;
			int page = 1;

			IEnumerable<MessageOutputDTO> expectedMessages = this.fakeCollection
				.Where(m => m.IsArchivedByAdmin)
				.Select(m => new MessageOutputDTO
				{
					Id = m.Id,
					CreatedOnUtc = m.CreatedOnUtc,
					Subject = m.Subject,
					IsSeen = m.IsSeenByAdmin
				})
				.Take(MessagesPerPage);

			int expectedTotalMessages = this.fakeCollection.Count(m => m.IsArchivedByAdmin);

			SortDefinition<Message> expectedSort = Builders<Message>.Sort.Descending("CreatedOnUtc");

			this.repoMock
				.Setup(x => x.FindAsync(
					m => m.IsArchivedByAdmin,
					It.Is<SortDefinition<Message>>(actualSort => ValidateSortDefinition(actualSort, expectedSort)),
					m => new MessageOutputDTO
					{
						Id = m.Id,
						CreatedOnUtc = m.CreatedOnUtc,
						Subject = m.Subject,
						IsSeen = isUserAdmin ? m.IsSeenByAdmin : m.IsSeenByAuthor
					},
					page))
				.ReturnsAsync(expectedMessages);

			this.repoMock
				.Setup(x => x.CountAsync(m => m.IsArchivedByAdmin))
				.ReturnsAsync(expectedTotalMessages);

			//Act
			MessagesDTO actual = await this.messagesService.GetAllArchivedMessagesAsync(page: 1);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual.TotalMessagesCount, Is.EqualTo(expectedTotalMessages));
				Assert.That(actual.Messages.ToJson(), Is.EqualTo(expectedMessages.ToJson()));
			});
		}

		[Test]
		public async Task GetAllMessagesAsync_ShouldReturnCorrectData()
		{
			//Arrange
			int page = 1;
			bool isUserAdmin = true;

			IEnumerable<MessageOutputDTO> expectedMessages = this.fakeCollection
				.Where(m => !m.IsArchivedByAdmin)
				.Select(m => new MessageOutputDTO
				{
					Id = m.Id,
					CreatedOnUtc = m.CreatedOnUtc,
					Subject = m.Subject,
					IsSeen = m.IsSeenByAdmin
				})
				.OrderByDescending(m => m.CreatedOnUtc)
				.Skip(MessagesPerPage * (page - 1))
				.Take(MessagesPerPage);

			int expectedTotalMessages = this.fakeCollection.Count(m => !m.IsArchivedByAdmin);

			SortDefinition<Message> expectedSort = Builders<Message>.Sort.Descending("CreatedOnUtc");

			this.repoMock
				.Setup(x => x.FindAsync(
					m => !m.IsArchivedByAdmin,
					It.Is<SortDefinition<Message>>(actualSort => ValidateSortDefinition(actualSort, expectedSort)),
					m => new MessageOutputDTO
					{
						Id = m.Id,
						CreatedOnUtc = m.CreatedOnUtc,
						Subject = m.Subject,
						IsSeen = isUserAdmin ? m.IsSeenByAdmin : m.IsSeenByAuthor
					},
					page))
				.ReturnsAsync(expectedMessages);

			this.repoMock.Setup(x => x
				.CountAsync(m => !m.IsArchivedByAdmin))
				.ReturnsAsync(expectedTotalMessages);

			//Act
			MessagesDTO actual = await this.messagesService.GetAllMessagesAsync();

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual.TotalMessagesCount, Is.EqualTo(expectedTotalMessages));
				Assert.That(actual.Messages.ToJson(), Is.EqualTo(expectedMessages.ToJson()));
			});
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public async Task GetMessageAsync_ShouldReturnCorrectData(bool isUserAdmin)
		{
			//Arrange
			string messageId = this.fakeCollection.Select(m => m.Id).First();
			string currentUserId = isUserAdmin ? this.AdminId : this.FirstUserId;

			MessageDetailsDTO expectedMessage = this.fakeCollection
				.Where(m => m.Id == messageId && (isUserAdmin || m.AuthorId == currentUserId))
				.Select(m => this.mapper.Map<MessageDetailsDTO>(m))
				.First();

			this.repoMock
				.Setup(x => x.FindOneAsync(
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
					}))
				.ReturnsAsync(expectedMessage);

			this.updateResultMock
				.Setup(x => x.IsAcknowledged)
				.Returns(true);

			UpdateDefinition<Message> expectedUpdate = Builders<Message>.Update
				.Set(x => isUserAdmin ? x.IsSeenByAdmin : x.IsSeenByAuthor, true);

			this.repoMock
				.Setup(x => x.UpdateOneAsync(
					x => x.Id == messageId && (isUserAdmin || x.AuthorId == currentUserId),
					It.Is<UpdateDefinition<Message>>(actualUpdate => ValidateUpdateDefinition(actualUpdate, expectedUpdate))))
				.ReturnsAsync(this.updateResultMock.Object);

			//Act
			MessageDetailsDTO actual = await this.messagesService.GetMessageAsync(messageId, currentUserId, isUserAdmin);

			//Assert
			Assert.That(actual.ToJson(), Is.EqualTo(expectedMessage.ToJson()));
		}

		[Test]
		public void GetMessageAsync_ShouldThrowUnauthorizedAccessException_WhenTheUserIsUnauthorized()
		{
			//Arrange
			MessageDetailsDTO message = this.fakeCollection
				.Where(m => m.AuthorId == this.FirstUserId)
				.Select(m => this.mapper.Map<MessageDetailsDTO>(m))
				.First();

			string messageId = message.Id;
			bool isUserAdmin = false;
			string notAuthorId = this.SecondUserId;

			this.repoMock
				.Setup(x => x.FindOneAsync(
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
					}))
				.ReturnsAsync(message);

			//Act & Assert
			Assert.That(async () => await this.messagesService
				  .GetMessageAsync(messageId, notAuthorId, isUserAdmin),
			Throws.TypeOf<UnauthorizedAccessException>().With.Message.EqualTo(ExceptionMessages.UnauthorizedUser));
		}

		[Test]
		public async Task GetMessageAuthorIdAsync_ShouldReturnCorrectData()
		{
			//Arrange
			Message message = this.fakeCollection.First();
			string messageId = message.Id;
			string authorId = message.AuthorId;

			this.repoMock
				.Setup(x => x.FindOneAsync(
					m => m.Id == messageId,
					m => m.AuthorId))
				.ReturnsAsync(authorId);

			//Act
			string actual = await this.messagesService.GetMessageAuthorIdAsync(messageId);

			//Assert
			Assert.That(actual, Is.EqualTo(authorId));
		}

		[Test]
		public void GetMessageAuthorIdAsync_ShouldThrowInvalidOperationException_WhenTheMessageDoesNotExist()
		{
			//Arrange
			string messageId = Guid.NewGuid().ToString();

			this.repoMock
				.Setup(x => x.FindOneAsync(
					m => m.Id == messageId,
					m => m.AuthorId))
				.Throws<InvalidOperationException>();

			//Act & Assert
			Assert.That(async () => await this.messagesService.GetMessageAuthorIdAsync(messageId),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetUserArchivedMessagesAsync_ShouldReturnCorrectData()
		{
			//Arrange
			string userId = this.FirstUserId;
			bool isUserAdmin = false;
			int page = 1;

			IEnumerable<MessageOutputDTO> expectedMessages = this.fakeCollection
				.Where(m => m.AuthorId == userId && m.IsArchivedByAuthor)
				.Select(m => new MessageOutputDTO
				{
					Id = m.Id,
					CreatedOnUtc = m.CreatedOnUtc,
					Subject = m.Subject,
					IsSeen = m.IsSeenByAuthor
				})
				.OrderByDescending(m => m.CreatedOnUtc)
				.Skip(MessagesPerPage * (page - 1))
				.Take(MessagesPerPage);

			int expectedTotalMessages = this.fakeCollection
				.Count(m => m.AuthorId == userId && m.IsArchivedByAuthor);

			SortDefinition<Message> expectedSorting = Builders<Message>.Sort.Descending("CreatedOnUtc");

			this.repoMock
				.Setup(x => x.FindAsync(
					m => m.AuthorId == userId && m.IsArchivedByAuthor,
					It.Is<SortDefinition<Message>>(actualSorting => ValidateSortDefinition(actualSorting, expectedSorting)),
					m => new MessageOutputDTO
					{
						Id = m.Id,
						CreatedOnUtc = m.CreatedOnUtc,
						Subject = m.Subject,
						IsSeen = isUserAdmin ? m.IsSeenByAdmin : m.IsSeenByAuthor
					},
					page))
				.ReturnsAsync(expectedMessages);

			this.repoMock
				.Setup(x => x.CountAsync(m => m.AuthorId == userId && m.IsArchivedByAuthor))
				.ReturnsAsync(expectedTotalMessages);

			//Act
			MessagesDTO actual = await this.messagesService.GetUserArchivedMessagesAsync(this.FirstUserId, page);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual.TotalMessagesCount, Is.EqualTo(expectedTotalMessages));
				Assert.That(actual.Messages.ToJson(), Is.EqualTo(expectedMessages.ToJson()));
			});
		}

		[Test]
		public async Task GetUserMessagesAsync_ShouldReturnCorrectData()
		{
			//Arrange
			int page = 1;
			bool isUserAdmin = false;
			string userId = this.FirstUserId;

			IEnumerable<MessageOutputDTO> expectedMessages = this.fakeCollection
				.Where(m => m.AuthorId == userId && !m.IsArchivedByAuthor)
				.Select(m => new MessageOutputDTO
				{
					Id = m.Id,
					CreatedOnUtc = m.CreatedOnUtc,
					Subject = m.Subject,
					IsSeen = m.IsSeenByAuthor
				})
				.OrderByDescending(m => m.CreatedOnUtc)
				.Skip(MessagesPerPage * (page - 1))
				.Take(MessagesPerPage);

			int expectedTotalMessages = this.fakeCollection
				.Count(m => m.AuthorId == userId && !m.IsArchivedByAuthor);

			SortDefinition<Message> expectedSorting = Builders<Message>.Sort.Descending("CreatedOnUtc");

			this.repoMock
				.Setup(x => x.FindAsync(
					m => m.AuthorId == userId && !m.IsArchivedByAuthor,
					It.Is<SortDefinition<Message>>(actualSorting => ValidateSortDefinition(actualSorting, expectedSorting)),
					m => new MessageOutputDTO
					{
						Id = m.Id,
						CreatedOnUtc = m.CreatedOnUtc,
						Subject = m.Subject,
						IsSeen = isUserAdmin ? m.IsSeenByAdmin : m.IsSeenByAuthor
					},
					page))
				.ReturnsAsync(expectedMessages);

			this.repoMock
				.Setup(x => x.CountAsync(m => m.AuthorId == userId && !m.IsArchivedByAuthor))
				.ReturnsAsync(expectedTotalMessages);

			//Act
			MessagesDTO actualMessages = await this.messagesService.GetUserMessagesAsync(userId);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actualMessages.TotalMessagesCount, Is.EqualTo(expectedTotalMessages));
				Assert.That(actualMessages.Messages.ToJson(), Is.EqualTo(expectedMessages.ToJson()));
			});
		}

		[Test]
		public async Task HasUnseenMessagesByAdminAsync_ShouldReturnCorrectData()
		{
			//Arrange
			bool expected = this.fakeCollection.Any(m => !m.IsSeenByAdmin);

			this.repoMock
				.Setup(x => x.AnyAsync(m => !m.IsSeenByAdmin))
				.ReturnsAsync(expected);

			//Act
			bool actual = await this.messagesService.HasUnseenMessagesByAdminAsync();

			//Assert
			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		public async Task HasUnseenMessagesByUserAsync_ShouldReturnCorrectData()
		{
			//Arrange
			string userId = this.FirstUserId;

			bool expected = this.fakeCollection
				.Any(m => m.AuthorId == userId && !m.IsSeenByAuthor);

			this.repoMock
				.Setup(x => x.AnyAsync(m => m.AuthorId == userId && !m.IsSeenByAuthor))
				.ReturnsAsync(expected);

			//Act
			bool actual = await this.messagesService.HasUnseenMessagesByUserAsync(userId);

			//Assert
			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public void MarkMessageAsSeenAsync_ShouldWorkCorrectly(bool isUserAdmin)
		{
			//Arrange
			string currentUserId = isUserAdmin ? this.AdminId : this.FirstUserId;

			string messageId = this.fakeCollection
				.Where(m => m.AuthorId == this.FirstUserId && !m.IsSeenByAuthor)
				.Select(m => m.Id)
				.First();

			UpdateDefinition<Message> expectedDefinition = Builders<Message>.Update
				.Set(x => isUserAdmin ? x.IsSeenByAdmin : x.IsSeenByAuthor, true);

			this.updateResultMock
				.Setup(x => x.IsAcknowledged)
				.Returns(true);

			this.repoMock
				.Setup(x => x.UpdateOneAsync(
					m => m.Id == messageId && (isUserAdmin || m.AuthorId == currentUserId),
					It.Is<UpdateDefinition<Message>>(actualDefinition => ValidateUpdateDefinition(actualDefinition, expectedDefinition))))
				.ReturnsAsync(this.updateResultMock.Object);

			//Act & Assert
			Assert.That(async () => await this.messagesService.MarkMessageAsSeenAsync(messageId, currentUserId, isUserAdmin),
			Throws.Nothing);
		}

		[Test]
		public void MarkMessageAsSeenAsync_ShouldThrowArgumentException_WhenTheUpdateWasUnsuccessful()
		{
			//Arrange
			bool isUserAdmin = false;
			string userId = this.FirstUserId;
			string messageId = this.fakeCollection
				.Where(m => m.AuthorId == this.FirstUserId && !m.IsSeenByAdmin)
				.Select(m => m.Id)
				.First();

			UpdateDefinition<Message> expectedDefinition = Builders<Message>.Update
				.Set(x => isUserAdmin ? x.IsSeenByAdmin : x.IsSeenByAuthor, true);

			this.updateResultMock
				.Setup(x => x.IsAcknowledged)
				.Returns(false);

			this.repoMock
				.Setup(x => x.UpdateOneAsync(
					m => m.Id == messageId && (isUserAdmin || m.AuthorId == userId),
					It.Is<UpdateDefinition<Message>>(actualDefinition => 
						ValidateUpdateDefinition(actualDefinition, expectedDefinition))))
				.ReturnsAsync(this.updateResultMock.Object);

			//Act & Assert
			Assert.That(async () => await this.messagesService
				  .MarkMessageAsSeenAsync(messageId, userId, isUserAdmin),
			Throws.TypeOf<ArgumentException>().With.Message
				  .EqualTo(ExceptionMessages.UnsuccessfulUpdate));
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public void RemoveAsync_ShouldRemoveMessage(bool isUserAdmin)
		{
			//Arrange
			Message message = this.fakeCollection.First(m => m.AuthorId == this.FirstUserId);
			string currentUserId = isUserAdmin ? this.AdminId : this.FirstUserId;

			this.repoMock
				.Setup(x => x.IsUserDocumentAuthor(message.Id, currentUserId))
				.ReturnsAsync(!isUserAdmin);

			this.deleteResultMock
				.Setup(x => x.IsAcknowledged)
				.Returns(true);

			this.repoMock
				.Setup(x => x.DeleteOneAsync(message.Id))
				.ReturnsAsync(this.deleteResultMock.Object);

			//Act & Assert
			Assert.That(async () => await this.messagesService
				  .RemoveAsync(message.Id, currentUserId, isUserAdmin),
			Throws.Nothing);
		}

		[Test]
		public void RemoveAsync_ShouldThrowUnauthorizedAccessException_WhenTheUserIsUnauthorized()
		{
			//Arrange
			string messageId = this.fakeCollection
				.Where(m => m.AuthorId == this.FirstUserId)
				.Select(m => m.Id)
				.First();

			string notAuthorId = this.SecondUserId;

			this.repoMock
				.Setup(x => x.IsUserDocumentAuthor(messageId, notAuthorId))
				.ReturnsAsync(false);

			//Act & Assert
			Assert.That(async () => await this.messagesService
				  .RemoveAsync(messageId, notAuthorId, isUserAdmin: false),
			Throws.TypeOf<UnauthorizedAccessException>().With.Message
				  .EqualTo(ExceptionMessages.UnauthorizedUser));
		}

		[Test]
		public void RemoveAsync_ShouldThrowInvalidOperationException_WhenTheUpdateWasUnsuccessful()
		{
			//Arrange
			Message message = this.fakeCollection.First();

			this.repoMock
				.Setup(x => x.IsUserDocumentAuthor(message.Id, message.AuthorId))
				.ReturnsAsync(true);

			this.deleteResultMock
				.Setup(x => x.IsAcknowledged)
				.Returns(false);

			this.repoMock
				.Setup(x => x.DeleteOneAsync(message.Id))
				.ReturnsAsync(this.deleteResultMock.Object);

			//Act & Assert
			Assert.That(async () => await this.messagesService.RemoveAsync(message.Id, message.AuthorId, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo(ExceptionMessages.UnsuccessfulDelete));
		}

		private ICollection<Message> GetFakeCollection()
		{
			return new List<Message>
			{
				new Message
				{
					Id = Guid.NewGuid().ToString(),
					CreatedOnUtc = DateTime.UtcNow,
					Subject = "First User First Message",
					AuthorId = this.FirstUserId,
					AuthorName = this.FirstUserName,
					Content = "First User First Message Content",
					IsSeenByAuthor = false,
					IsSeenByAdmin = true,
					Replies = new List<Reply>
					{
						new Reply
						{
							AuthorId = this.AdminId,
							AuthorName = this.AdminName,
							Content = "Admin First Message Reply Content",
							CreatedOnUtc = DateTime.UtcNow
						}
					}
				},
				new Message
				{
					Id = Guid.NewGuid().ToString(),
					CreatedOnUtc = DateTime.UtcNow,
					Subject = "First User Second Message",
					AuthorId = this.FirstUserId,
					AuthorName = this.FirstUserName,
					Content = "First User Second Message Content",
					IsSeenByAuthor = true
				},
				new Message
				{
					Id = Guid.NewGuid().ToString(),
					CreatedOnUtc = DateTime.UtcNow,
					Subject = "First User Third Message",
					AuthorId = this.FirstUserId,
					AuthorName = this.FirstUserName,
					Content = "First User Third Message Content",
					IsSeenByAuthor = true,
					IsSeenByAdmin = true,
					IsArchivedByAdmin = true
				},
				new Message
				{
					Id = Guid.NewGuid().ToString(),
					CreatedOnUtc = DateTime.UtcNow,
					Subject = "First User Fourth Message",
					AuthorId = this.FirstUserId,
					AuthorName = this.FirstUserName,
					Content = "First User Fourth Message Content",
					IsSeenByAuthor = true,
					IsArchivedByAuthor = true
				},
				new Message
				{
					Id = Guid.NewGuid().ToString(),
					CreatedOnUtc = DateTime.UtcNow,
					Subject = "Second User First Message",
					AuthorId = this.SecondUserId,
					AuthorName = this.SecondUserName,
					Content = "Second User First Message Content",
					IsSeenByAuthor = true
				},
				new Message
				{
					Id = Guid.NewGuid().ToString(),
					CreatedOnUtc = DateTime.UtcNow,
					Subject = "Second User Second Message",
					AuthorId = this.SecondUserId,
					AuthorName = this.SecondUserName,
					Content = "Second User Second Message Content",
					IsSeenByAdmin = true,
					Replies = new List<Reply>
					{
						new Reply
						{
							AuthorId = this.AdminId,
							AuthorName = this.AdminName,
							Content = "Admin Second Message Reply Content",
							CreatedOnUtc = DateTime.UtcNow
						}
					}
				}
			};
		}

		private static bool ValidateUpdateDefinition(UpdateDefinition<Message> actualUpdate, UpdateDefinition<Message> expectedUpdate)
		{
			var actualRendered = actualUpdate.Render(documentSerializer, serializerRegistry);
			var expectedRendered = expectedUpdate.Render(documentSerializer, serializerRegistry);

			return actualRendered.ToJson().Equals(expectedRendered.ToJson());
		}

		private static bool ValidateSortDefinition(SortDefinition<Message> actualSort, SortDefinition<Message> expectedSort)
		{
			var actualRendered = actualSort.Render(documentSerializer, serializerRegistry);
			var expectedRendered = expectedSort.Render(documentSerializer, serializerRegistry);

			return actualRendered.ToJson().Equals(expectedRendered.ToJson());
		}
	}
}
