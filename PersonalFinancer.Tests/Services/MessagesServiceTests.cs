namespace PersonalFinancer.Tests.Services
{
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

    [TestFixture]
    internal class MessagesServiceTests
    {
        private Mock<IMongoRepository<Message>> repoMock;
        private MessagesService service;
        private readonly IMapper mapper = ServicesMapperMock.Instance;

        [SetUp]
        public void SetUp()
        {
            this.repoMock = new Mock<IMongoRepository<Message>>();
            this.service = new MessagesService(this.repoMock.Object, this.mapper);
        }

        [Test]
        public async Task GelAllAsync_ShouldReturnCorrectData()
        {
            //Arrange
            var expect = new List<MessageOutputDTO>
            {
                new MessageOutputDTO
                {
                    Id = "1",
                    CreatedOn = DateTime.UtcNow,
                    Subject = "Test Subject 1"
                },
                new MessageOutputDTO
                {
                    Id = "2",
                    CreatedOn = DateTime.UtcNow,
                    Subject = "Test Subject 2"
                }
            };

            this.repoMock.Setup(x => x.FindAsync(
                m => new MessageOutputDTO
                {
                    Id = m.Id,
                    CreatedOn = m.CreatedOn,
                    Subject = m.Subject
                })).ReturnsAsync(expect);

            //Act
            IEnumerable<MessageOutputDTO> actual = await this.service.GetAllAsync();

            //Assert
            Assert.Multiple(() =>
            {
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
            });
        }

        [Test]
        public async Task GetUserMessagesAsync_ShouldReturnCorrectData()
        {
            //Arrange
            var expect = new List<MessageOutputDTO>
            {
                new MessageOutputDTO
                {
                    Id = "1",
                    CreatedOn = DateTime.UtcNow,
                    Subject = "Test Subject 1"
                }
            };

            string userId = Guid.NewGuid().ToString();

            this.repoMock.Setup(x => x.FindAsync(
                m => m.AuthorId == userId,
                m => new MessageOutputDTO
                {
                    Id = m.Id,
                    CreatedOn = m.CreatedOn,
                    Subject = m.Subject
                })).ReturnsAsync(expect);

            //Act
            IEnumerable<MessageOutputDTO> actual = await this.service.GetUserMessagesAsync(userId);

            //Assert
            Assert.Multiple(() =>
            {
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
            });
        }

        [Test]
        public async Task GetMessageAsync_ShouldReturnCorrectData_WhenUserIsAuthor()
        {
            //Arrange
            var expect = new MessageDetailsDTO
            {
                Id = "1",
                AuthorName = "Test Author Name",
                Content = "Test Content",
                Subject = "Test Subject",
                CreatedOn = DateTime.UtcNow,
                Replies = new List<ReplyOutputDTO>
                {
                    new ReplyOutputDTO
                    {
                        AuthorName = "Test Reply Author",
                        CreatedOn = DateTime.UtcNow,
                        Content = "Test Reply Content"
                    }
                }
            };

            string messageId = Guid.NewGuid().ToString();
            bool isUserAdmin = false;
            string userId = Guid.NewGuid().ToString();

            this.repoMock.Setup(x => x.FindOneAsync(
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
                })).ReturnsAsync(expect);

            //Act
            MessageDetailsDTO actual = await this.service.GetMessageAsync(messageId, userId, isUserAdmin);

            //Assert
            Assert.Multiple(() =>
            {
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
            });
        }

        [Test]
        public async Task GetMessageAsync_ShouldReturnCorrectData_WhenUserIsAdmin()
        {
            //Arrange
            var expect = new MessageDetailsDTO
            {
                Id = "1",
                AuthorName = "Test Author Name",
                Content = "Test Content",
                Subject = "Test Subject",
                CreatedOn = DateTime.UtcNow,
                Replies = new List<ReplyOutputDTO>
                {
                    new ReplyOutputDTO
                    {
                        AuthorName = "Test Reply Author",
                        CreatedOn = DateTime.UtcNow,
                        Content = "Test Reply Content"
                    }
                }
            };

            string messageId = Guid.NewGuid().ToString();
            bool isUserAdmin = true;
            string userId = Guid.NewGuid().ToString();

            this.repoMock.Setup(x => x.FindOneAsync(
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
                })).ReturnsAsync(expect);

            //Act
            MessageDetailsDTO actual = await this.service.GetMessageAsync(messageId, userId, isUserAdmin);

            //Assert
            Assert.Multiple(() =>
            {
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
            });
        }

        [Test]
        public void GetMessageAsync_ShouldThrowException_WhenUserIsNotAuthorOrAdmin()
        {
            //Arrange
            string messageId = Guid.NewGuid().ToString();
            bool isUserAdmin = false;
            string notAuthorId = Guid.NewGuid().ToString();

            this.repoMock.Setup(x => x.FindOneAsync(
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
                })).Throws<InvalidOperationException>();

            //Act & Assert
            Assert.That(async () => await this.service
                  .GetMessageAsync(messageId, notAuthorId, isUserAdmin),
            Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public async Task CreateAsync_ShouldCreateNewMessage_WithValidInput()
        {
            //Arrange
            var fakeCollection = new List<Message>();

            var inputModel = new MessageInputDTO
            {
                AuthorId = Guid.NewGuid().ToString(),
                AuthorName = "Test Author Name",
                Subject = "Test Subject",
                Content = "Test Content"
            };

            string newMessageId = "New Message Id";

            this.repoMock.Setup(x => x.InsertOneAsync(It.IsAny<Message>()))
                .Callback((Message message) =>
                {
                    message.Id = newMessageId;
                    fakeCollection.Add(message);
                });

            //Act
            string returnedId = await this.service.CreateAsync(inputModel);
            Message msgInCollection = fakeCollection.First();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(returnedId, Is.Not.Null);
                Assert.That(returnedId, Is.EqualTo(newMessageId));
                Assert.That(fakeCollection, Has.Count.EqualTo(1));
                Assert.That(msgInCollection.AuthorId, Is.EqualTo(inputModel.AuthorId));
                Assert.That(msgInCollection.AuthorName, Is.EqualTo(inputModel.AuthorName));
                Assert.That(msgInCollection.Subject, Is.EqualTo(inputModel.Subject));
                Assert.That(msgInCollection.Content, Is.EqualTo(inputModel.Content));
            });
        }

        [Test]
        public async Task AddReplyAsync_ShouldAddNewReplyToMessage_WhenUserIsMessageAuthor()
        {
            //Arrange
            var inputModel = new ReplyInputDTO
            {
                AuthorId = Guid.NewGuid().ToString(),
                AuthorName = "Test Author Name",
                MessageId = "Test Message Id",
                Content = "Test Content",
                IsAuthorAdmin = false
            };

            var updateResultMock = new Mock<UpdateResult>();
            updateResultMock.Setup(x => x.IsAcknowledged).Returns(true);

            this.repoMock.Setup(x => x
                .IsUserDocumentAuthor(inputModel.MessageId, inputModel.AuthorId))
                .ReturnsAsync(true);

            this.repoMock.Setup(x => x
                .UpdateOneAsync(
                    x => x.Id == inputModel.MessageId,
                    It.IsAny<UpdateDefinition<Message>>()))
                .ReturnsAsync(updateResultMock.Object);

            //Act
            UpdateResult result = await this.service.AddReplyAsync(inputModel);

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
                AuthorId = Guid.NewGuid().ToString(),
                AuthorName = "Test Author Name",
                MessageId = "Test Message Id",
                Content = "Test Content",
                IsAuthorAdmin = true
            };

            var updateResultMock = new Mock<UpdateResult>();
            updateResultMock.Setup(x => x.IsAcknowledged).Returns(true);

            this.repoMock.Setup(x => x
                .IsUserDocumentAuthor(inputModel.MessageId, inputModel.AuthorId))
                .ReturnsAsync(false);

            this.repoMock.Setup(x => x
                .UpdateOneAsync(
                    x => x.Id == inputModel.MessageId,
                    It.IsAny<UpdateDefinition<Message>>()))
                .ReturnsAsync(updateResultMock.Object);

            //Act
            UpdateResult result = await this.service.AddReplyAsync(inputModel);

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
                AuthorId = Guid.NewGuid().ToString(),
                AuthorName = "Test Author Name",
                MessageId = "Test Message Id",
                Content = "Test Content",
                IsAuthorAdmin = false
            };

            this.repoMock.Setup(x => x
                .IsUserDocumentAuthor(inputModel.MessageId, inputModel.AuthorId))
                .ReturnsAsync(false);

            //Act & Assert
            Assert.That(async () => await this.service.AddReplyAsync(inputModel),
            Throws.TypeOf<ArgumentException>().With.Message
                  .EqualTo("The User is not Authorized to make replies."));
        }

        [Test]
        public async Task RemoveAsync_ShouldRemoveMessage_WhenUserIsAuthor()
        {
            //Arrange
            var message = new Message
            {
                Id = "Test Message Id",
                AuthorId = Guid.NewGuid().ToString(),
                AuthorName = "Test Author Name",
                CreatedOn = DateTime.UtcNow,
                Subject = "Test Message Subject",
                Content = "This Message Content"
            };
            var fakeCollection = new List<Message> { message };

            this.repoMock.Setup(x => x
                .IsUserDocumentAuthor(message.Id, message.AuthorId))
                .ReturnsAsync(true);

            this.repoMock.Setup(x => x
                .DeleteOneAsync(message.Id))
                .ReturnsAsync(() =>
                {
                    fakeCollection.Remove(message);
                    return new DeleteResult.Acknowledged(1);
                });

            //Act
            await this.service.RemoveAsync(message.Id, message.AuthorId, isUserAdmin: false);

            //Assert
            Assert.That(fakeCollection.Any(), Is.False);
        }

        [Test]
        public async Task RemoveAsync_ShouldRemoveMessage_WhenUserIsAdmin()
        {
            //Arrange
            var message = new Message
            {
                Id = "Test Message Id",
                AuthorId = Guid.NewGuid().ToString(),
                AuthorName = "Test Author Name",
                CreatedOn = DateTime.UtcNow,
                Subject = "Test Message Subject",
                Content = "This Message Content"
            };
            var fakeCollection = new List<Message> { message };

            this.repoMock.Setup(x => x
                .IsUserDocumentAuthor(message.Id, message.AuthorId))
                .ReturnsAsync(false);

            this.repoMock.Setup(x => x
                .DeleteOneAsync(message.Id))
                .ReturnsAsync(() =>
                {
                    fakeCollection.Remove(message);
                    return new DeleteResult.Acknowledged(1);
                });

            //Act
            await this.service.RemoveAsync(message.Id, message.AuthorId, isUserAdmin: true);

            //Assert
            Assert.That(fakeCollection.Any(), Is.False);
        }

        [Test]
        public void RemoveAsync_ShouldThrowException_WhenUserIsNotAuthorized()
        {
            //Arrange
            var message = new Message
            {
                Id = "Test Message Id",
                AuthorId = Guid.NewGuid().ToString(),
                AuthorName = "Test Author Name",
                CreatedOn = DateTime.UtcNow,
                Subject = "Test Message Subject",
                Content = "This Message Content"
            };
            var fakeCollection = new List<Message> { message };

            this.repoMock.Setup(x => x
                .IsUserDocumentAuthor(message.Id, message.AuthorId))
                .ReturnsAsync(false);

            //Act & Assert
            Assert.That(async () => await this.service
                  .RemoveAsync(message.Id, message.AuthorId, isUserAdmin: false),
            Throws.TypeOf<ArgumentException>().With.Message
                  .EqualTo("The User is not Authorized to delete the message."));
        }
    }
}
