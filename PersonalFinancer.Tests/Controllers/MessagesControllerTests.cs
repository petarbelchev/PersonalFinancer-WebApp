namespace PersonalFinancer.Tests.Controllers
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Services.Messages;
	using PersonalFinancer.Services.Messages.Models;
	using PersonalFinancer.Web.Controllers;
	using PersonalFinancer.Web.Models.Message;
	using static PersonalFinancer.Common.Constants.RoleConstants;

	[TestFixture]
    internal class MessagesControllerTests : ControllersUnitTestsBase
	{
        private string userName;
        private string secondUserId;
        private string secondUserName;
        private string adminId;
        private string adminName;
		private ICollection<Message> fakeCollection;
		private Mock<IMessagesService> messagesServiceMock;
        private MessagesController controller;

        [SetUp]
        public void SetUp()
        {
            this.userName = "CurrentUser";
            this.secondUserId = "secondUserId";
            this.secondUserName = "SecondUserName";
            this.adminId = "adminId";
            this.adminName = "AdminName";

            this.fakeCollection = this.SeedFakeCollection();

            this.messagesServiceMock = new Mock<IMessagesService>();
			
            this.controller = new MessagesController(
				this.messagesServiceMock.Object,
				this.usersServiceMock.Object,
				this.mapper)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = new DefaultHttpContext
					{
						User = this.userMock.Object,
					}
				}
			};
		}

        [Test]
        public async Task AllMessages_ShouldReturnViewModelWithUserMessages_WhenUserIsNotAdmin()
        {
			//Arrange
			IEnumerable<MessageOutputDTO> serviceReturnDto = this.fakeCollection
                .Where(m => m.AuthorId == this.userId.ToString())
                .Select(m => new MessageOutputDTO
                {
                    Id = m.Id,
                    CreatedOn = m.CreatedOn,
                    Subject = m.Subject
                });

            this.userMock.Setup(x => x
                .IsInRole(AdminRoleName))
                .Returns(false);

            this.messagesServiceMock.Setup(x => x
                .GetUserMessagesAsync(this.userId.ToString()))
                .ReturnsAsync(serviceReturnDto);

            //Act
            var viewResult = (ViewResult)await this.controller.AllMessages();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(viewResult, Is.Not.Null);

				var viewModel = viewResult.Model as IEnumerable<MessageOutputDTO> ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(viewModel, serviceReturnDto);
            });
        }

        [Test]
        public async Task AllMessages_ShouldReturnViewModelWithAllUsersMessages_WhenUserIsAdmin()
        {
			//Arrange
			IEnumerable<MessageOutputDTO> serviceReturnDto = this.fakeCollection
				.Select(m => new MessageOutputDTO
				{
					Id = m.Id,
					CreatedOn = m.CreatedOn,
					Subject = m.Subject
				});

			this.userMock.Setup(x => x
                .IsInRole(AdminRoleName))
                .Returns(true);

            this.messagesServiceMock.Setup(x => x
                .GetAllAsync())
                .ReturnsAsync(serviceReturnDto);

            //Act
            var viewResult = (ViewResult)await this.controller.AllMessages();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(viewResult, Is.Not.Null);

				var viewModel = viewResult.Model as IEnumerable<MessageOutputDTO> ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(viewModel, serviceReturnDto);
            });
        }

        [Test]
        public void Create_OnGet_ShouldReturnViewModel()
        {
            //Arrange
            var expected = new MessageInputModel();

            //Act
            var viewResult = (ViewResult)this.controller.Create();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(viewResult, Is.Not.Null);

				MessageInputModel viewModel = viewResult.Model as MessageInputModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

                AssertSamePropertiesValuesAreEqual(viewModel, expected);
            });
        }

        [Test]
        public async Task Create_OnPost_ShouldReturnViewModelWithErrors_WhenModelIsInvalid()
        {
            //Arrange
            var inputModel = new MessageInputModel
            {
                Subject = "Subject",
                Content = ""
            };

            this.controller.ModelState.AddModelError(nameof(inputModel.Content), "Invalid content.");

            //Act
            var viewResult = (ViewResult)await this.controller.Create(inputModel);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(viewResult, Is.Not.Null);

				MessageInputModel model = viewResult.Model as MessageInputModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

                AssertSamePropertiesValuesAreEqual(model, inputModel);

                AssertModelStateErrorIsEqual(
                    viewResult.ViewData.ModelState, 
                    nameof(inputModel.Content), 
                    "Invalid content.");
            });
        }

        [Test]
        public async Task Create_OnPost_ShouldRedirectToAction_WhenMessageWasCreated()
        {
            //Arrange
            var inputModel = new MessageInputModel
            {
                Subject = "Subject",
                Content = "Message Valid Content"
            };

            string userFullName = "User Full Name";
            string newMessageId = "new message id";

            this.usersServiceMock.Setup(x => x
                .UserFullNameAsync(this.userId))
                .ReturnsAsync(userFullName);

            this.messagesServiceMock.Setup(x =>
                x.CreateAsync(It.Is<MessageInputDTO>(m =>
                    m.Subject == inputModel.Subject
                    && m.Content == inputModel.Content
                    && m.AuthorId == this.userId.ToString()
                    && m.AuthorName == userFullName)))
                .ReturnsAsync(newMessageId);

            //Act
            var result = (RedirectToActionResult)await this.controller.Create(inputModel);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo("MessageDetails"));
                Assert.That(result.RouteValues, Is.Not.Null);
                AssertRouteValueIsEqual(result.RouteValues!, "id", newMessageId);
            });
        }

        [Test]
        public async Task Delete_ShouldRedirectToAction_WhenMessageWasDeleted()
        {
            //Arrange
            string messageId = "id";

            this.userMock.Setup(x => x
                .IsInRole(AdminRoleName))
                .Returns(false);

            //Act
            var result = (RedirectToActionResult)await this.controller.Delete(messageId);

            this.messagesServiceMock.Verify(x => x
                .RemoveAsync(messageId, this.userId.ToString(), false),
                Times.Once());

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo("AllMessages"));
                Assert.That(result.RouteValues, Is.Null);
            });
        }

        [Test]
        public async Task Delete_ShouldReturnUnauthorized_WhenUserIsNotOwnerOrAdmin()
        {
            //Arrange
            string messageId = "id";

            this.userMock.Setup(x => x
                .IsInRole(AdminRoleName))
                .Returns(false);

            this.messagesServiceMock.Setup(x => x
                .RemoveAsync(messageId, this.userId.ToString(), false))
                .Throws<ArgumentException>();

            //Act
            var result = (UnauthorizedResult)await this.controller.Delete(messageId);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo(401));
        }

        [Test]
        public async Task Delete_ShouldReturnBadRequest_WhenDeleteWasUnsuccessful()
        {
            //Arrange
            string messageId = "id";

            this.userMock.Setup(x => x
                .IsInRole(AdminRoleName))
                .Returns(false);

            this.messagesServiceMock.Setup(x => x
                .RemoveAsync(messageId, this.userId.ToString(), false))
                .Throws<InvalidOperationException>();

            //Act
            var result = (BadRequestResult)await this.controller.Delete(messageId);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task MessageDetails_OnGet_ShouldReturnViewModel()
        {
            //Arrange
            string messageId = "1";

			MessageDetailsDTO serviceReturnDto = this.fakeCollection
                .Where(m => m.AuthorId == this.userId.ToString())
                .Select(m => this.mapper.Map<MessageDetailsDTO>(m))
                .First();

            this.messagesServiceMock.Setup(x => x
                .GetMessageAsync(messageId, this.userId.ToString(), false))
                .ReturnsAsync(serviceReturnDto);

            //Act
            var viewResult = (ViewResult)await this.controller.MessageDetails(messageId);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(viewResult, Is.Not.Null);

				MessageDetailsViewModel viewModel = viewResult.Model as MessageDetailsViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

                AssertSamePropertiesValuesAreEqual(viewModel, serviceReturnDto);
            });
        }

        [Test]
        public async Task MessageDetails_OnGet_ShouldReturnBadRequest_WhenMessageDoesNotExistOrUserIsNowOwnerOrAdmin()
        {
            //Arrange
            string messageId = "id";

            this.messagesServiceMock.Setup(x => x
                .GetMessageAsync(messageId, this.userId.ToString(), false))
                .Throws<InvalidOperationException>();

            //Act
            var result = (BadRequestResult)await this.controller.MessageDetails(messageId);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task MessageDetails_OnPost_ShouldReturnViewModelWithErrors_WhenTryToAddInvalidReply()
        {
            //Arrange
            var inputModel = new ReplyInputModel
            {
                Id = "1",
                ReplyContent = ""
            };

			MessageDetailsDTO serviceReturnDto = this.fakeCollection
                .Where(m => m.Id == inputModel.Id)
                .Select(m => this.mapper.Map<MessageDetailsDTO>(m))
                .First();

            this.userMock.Setup(x => x
                .IsInRole(AdminRoleName))
                .Returns(false);

            this.messagesServiceMock.Setup(x => x
                .GetMessageAsync(inputModel.Id, this.userId.ToString(), false))
                .ReturnsAsync(serviceReturnDto);

            this.controller.ModelState.AddModelError(nameof(inputModel.ReplyContent), "Invalid content.");

            //Act
            var viewResult = (ViewResult)await this.controller.MessageDetails(inputModel);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(viewResult, Is.Not.Null);

				MessageDetailsViewModel viewModel = viewResult.Model as MessageDetailsViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(viewModel, serviceReturnDto);

				AssertModelStateErrorIsEqual(viewResult.ViewData.ModelState, nameof(inputModel.ReplyContent), "Invalid content.");
            });
        }

        [Test]
        public async Task MessageDetails_OnPost_ShouldReturnBadRequest_WhenModelIsInvalidAndExceptionWasThrowed()
        {
            //Arrange
            var inputModel = new ReplyInputModel
            {
                Id = "Message id",
                ReplyContent = ""
            };

            this.userMock.Setup(x => x
                .IsInRole(AdminRoleName))
                .Returns(false);

            this.controller.ModelState.AddModelError(nameof(inputModel.ReplyContent), "Invalid content.");
            
            this.messagesServiceMock.Setup(x => x
                .GetMessageAsync(inputModel.Id, this.userId.ToString(), false))
                .Throws<InvalidOperationException>();

            //Act
            var result = (BadRequestResult)await this.controller.MessageDetails(inputModel);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task MessageDetails_OnPost_ShouldRedirectToAction_WhenReplyWasAdded()
        {
            //Arrange
            var inputModel = new ReplyInputModel
            {
                Id = "Message id",
                ReplyContent = "Reply content"
            };

            string userName = "username";

            this.userMock.Setup(x => x
                .IsInRole(AdminRoleName))
                .Returns(false);

            this.usersServiceMock.Setup(x => x
                .UserFullNameAsync(this.userId))
                .ReturnsAsync(userName);

            //Act
            var result = (RedirectToActionResult)await this.controller.MessageDetails(inputModel);

            this.messagesServiceMock.Verify(x => x
                .AddReplyAsync(It.Is<ReplyInputDTO>(m =>
                    m.MessageId == inputModel.Id
                    && m.IsAuthorAdmin == false
                    && m.Content == inputModel.ReplyContent
                    && m.AuthorName == userName
                    && m.AuthorId == this.userId.ToString())),
                Times.Once);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo("MessageDetails"));
                Assert.That(result.RouteValues, Is.Not.Null);
                AssertRouteValueIsEqual(result.RouteValues!, "id", inputModel.Id);
            });
        }

        [Test]
        public async Task MessageDetails_OnPost_ShouldReturnUnauthorized_WhenUserIsNotOwnerOrAdmin()
        {
            //Arrange
            var inputModel = new ReplyInputModel
            {
                Id = "Message id",
                ReplyContent = "Reply content"
            };

            string userName = "username";

            this.userMock.Setup(x => x
                .IsInRole(AdminRoleName))
                .Returns(false);

            this.usersServiceMock.Setup(x => x
                .UserFullNameAsync(this.userId))
                .ReturnsAsync(userName);

            this.messagesServiceMock.Setup(x => x
                .AddReplyAsync(It.Is<ReplyInputDTO>(m =>
                    m.MessageId == inputModel.Id
                    && m.IsAuthorAdmin == false
                    && m.Content == inputModel.ReplyContent
                    && m.AuthorName == userName
                    && m.AuthorId == this.userId.ToString())))
                .Throws<ArgumentException>();

            //Act
            var result = (UnauthorizedResult)await this.controller.MessageDetails(inputModel);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo(401));
        }

        [Test]
        public async Task MessageDetails_OnPost_ShouldReturnBadRequest_WhenAddingReplyWasUnsuccessful()
        {
            //Arrange
            var inputModel = new ReplyInputModel
            {
                Id = "Message id",
                ReplyContent = "Reply content"
            };

            string userName = "user name";

            this.userMock.Setup(x => x
                .IsInRole(AdminRoleName))
                .Returns(false);

            this.usersServiceMock.Setup(x => x
                .UserFullNameAsync(this.userId))
                .ReturnsAsync(userName);

            this.messagesServiceMock.Setup(x => x
                .AddReplyAsync(It.Is<ReplyInputDTO>(m =>
                    m.MessageId == inputModel.Id
                    && m.IsAuthorAdmin == false
                    && m.Content == inputModel.ReplyContent
                    && m.AuthorName == userName
                    && m.AuthorId == this.userId.ToString())))
                .Throws<InvalidOperationException>();

            //Act
            var result = (BadRequestResult)await this.controller.MessageDetails(inputModel);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo(400));
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
					AuthorId = this.userId.ToString(),
					AuthorName = this.userName,
					Content = "First User First Message Content",
					Replies = new List<Reply>
					{
						new Reply
						{
							AuthorId = this.adminId,
							AuthorName = this.adminName,
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
					AuthorId = this.userId.ToString(),
					AuthorName = this.userName,
					Content = "First User Second Message Content",
				},
				new Message
				{
					Id = "3",
					CreatedOn = DateTime.UtcNow,
					Subject = "Second User First Message",
					AuthorId = this.secondUserId,
					AuthorName = this.secondUserName,
					Content = "Second User First Message Content",
				}
			};
		}
	}
}
