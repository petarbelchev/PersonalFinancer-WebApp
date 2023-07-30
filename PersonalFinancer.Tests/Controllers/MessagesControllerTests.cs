namespace PersonalFinancer.Tests.Controllers
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.SignalR;
	using Microsoft.Extensions.Logging;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Services.Messages;
	using PersonalFinancer.Services.Messages.Models;
	using PersonalFinancer.Web.Controllers;
	using PersonalFinancer.Web.Hubs;
	using PersonalFinancer.Web.Models.Message;
	using System.Security.Claims;
	using static PersonalFinancer.Common.Constants.RoleConstants;

	[TestFixture]
	internal class MessagesControllerTests : ControllersUnitTestsBase
	{
		private Mock<IMessagesService> messagesServiceMock;
		private Mock<IHubContext<AllMessagesHub>> allMessagesHubMock;
		private Mock<IHubContext<NotificationsHub>> notificationsHubMock;
		private Mock<ILogger<MessagesController>> loggerMock;
		private MessagesController controller;

		[SetUp]
		public void SetUp()
		{
			this.messagesServiceMock = new Mock<IMessagesService>();
			this.allMessagesHubMock = new Mock<IHubContext<AllMessagesHub>>();
			this.notificationsHubMock = new Mock<IHubContext<NotificationsHub>>();
			this.loggerMock = new Mock<ILogger<MessagesController>>();

			var clientProxyMock = new Mock<IClientProxy>();
			clientProxyMock
				.Setup(x => x.SendCoreAsync(
					It.IsAny<string>(),
					It.IsAny<object[]>(),
					It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			var notificationsHubClientsMock = new Mock<IHubClients>();
			notificationsHubClientsMock
				.Setup(x => x.Users(It.IsAny<IReadOnlyList<string>>()))
				.Returns(clientProxyMock.Object);

			this.notificationsHubMock
				.Setup(x => x.Clients)
				.Returns(notificationsHubClientsMock.Object);

			var allMessagesHubClientsMock = new Mock<IHubClients>();
			allMessagesHubClientsMock
				.Setup(x => x.Users(It.IsAny<IReadOnlyList<string>>()))
				.Returns(clientProxyMock.Object);

			this.allMessagesHubMock
				.Setup(x => x.Clients)
				.Returns(allMessagesHubClientsMock.Object);

			this.controller = new MessagesController(
				this.messagesServiceMock.Object,
				this.usersServiceMock.Object,
				this.mapper,
				this.allMessagesHubMock.Object,
				this.notificationsHubMock.Object,
				this.loggerMock.Object)
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
		public async Task Archive_OnPost_ShouldRedirectToAction_WhenTheMessageWasArchived()
		{
			//Arrange
			string messageId = "1";

			//Act
			var result = (RedirectToActionResult)await this.controller.Archive(messageId);

			//Assert
			this.messagesServiceMock.Verify(
				x => x.ArchiveAsync(messageId, this.userId.ToString(), false),
				Times.Once);

			Assert.That(result.ActionName, Is.EqualTo("Index"));
		}

		[Test]
		public async Task Archive_OnPost_ShouldReturnUnauthorized_WhenTheUserIsUnauthorized()
		{
			//Arrange
			string messageId = "1";

			this.messagesServiceMock
				.Setup(x => x.ArchiveAsync(messageId, this.userId.ToString(), false))
				.Throws<UnauthorizedAccessException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.UnauthorizedArchiveMessage,
				this.userId,
				messageId);

			//Act
			var result = (UnauthorizedResult)await this.controller.Archive(messageId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(401));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task Archive_OnPost_ShouldReturnBadRequest_WhenArchivingTheMessageWasUnsuccessful()
		{
			//Arrange
			string messageId = "valid id";

			this.messagesServiceMock
				.Setup(x => x.ArchiveAsync(messageId, this.userId.ToString(), false))
				.Throws<InvalidOperationException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.UnsuccessfulMessageArchiving,
				this.userId,
				messageId);

			//Act
			var result = (BadRequestResult)await this.controller.Archive(messageId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task Archive_OnPost_ShouldReturnBadRequest_WhenTheModelStateIsInvalid()
		{
			//Arrange
			string messageId = "invalid id";

			this.controller.ModelState.AddModelError("id", "invalid id");

			string expectedLogMessage = string.Format(
				LoggerMessages.ArchiveMessageWithInvalidInputData,
				this.userId,
				messageId);

			//Act
			var result = (BadRequestResult)await this.controller.Archive(messageId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public void Archived_ShouldReturnView()
		{
			//Arrange

			//Act
			var viewResult = (ViewResult)this.controller.Archived();

			//Assert
			Assert.That(viewResult, Is.Not.Null);
		}

		[Test]
		public void Create_OnGet_ShouldReturnViewModel()
		{
			//Arrange
			var expected = new MessageModel();

			//Act
			var viewResult = (ViewResult)this.controller.Create();

			//Assert
			Assert.That(viewResult, Is.Not.Null);

			MessageModel viewModel = viewResult.Model as MessageModel ??
				throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

			AssertSamePropertiesValuesAreEqual(viewModel, expected);
		}

		[Test]
		public async Task Create_OnPost_ShouldReturnViewModelWithErrors_WhenTheModelIsInvalid()
		{
			//Arrange
			var inputModel = new MessageModel
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

				MessageModel model = viewResult.Model as MessageModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(model, inputModel);

				AssertModelStateErrorIsEqual(
					viewResult.ViewData.ModelState,
					nameof(inputModel.Content),
					"Invalid content.");
			});
		}

		[Test]
		public async Task Create_OnPost_ShouldRedirectToAction_WhenTheMessageWasCreated()
		{
			//Arrange
			var inputModel = new MessageModel
			{
				Subject = "Subject",
				Content = "Message Valid Content"
			};

			string userFullName = "User Full Name";

			var expected = new MessageOutputDTO
			{
				Id = "new message id",
				Subject = inputModel.Subject,
				IsSeen = true,
			};

			this.usersServiceMock
				.Setup(x => x.UserFullNameAsync(this.userId))
				.ReturnsAsync(userFullName);

			this.messagesServiceMock
				.Setup(x => x.CreateAsync(It.Is<MessageInputDTO>(m =>
					m.Subject == inputModel.Subject
					&& m.Content == inputModel.Content
					&& m.AuthorId == this.userId.ToString()
					&& m.AuthorName == userFullName)))
				.ReturnsAsync(expected);

			//Act
			var actual = (RedirectToActionResult)await this.controller.Create(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual.ActionName, Is.EqualTo("Details"));
				Assert.That(actual.RouteValues, Is.Not.Null);
				AssertRouteValueIsEqual(actual.RouteValues!, "id", expected.Id);
			});
		}

		[Test]
		[TestCase(false, false)]
		[TestCase(true, false)]
		[TestCase(false, true)]
		[TestCase(true, true)]
		public async Task Delete_ShouldRedirectToAction_WhenTheMessageWasDeleted(bool isUserAdmin, bool hasUnseenMessages)
		{
			//Arrange
			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(isUserAdmin);

			string authorId = this.userId.ToString();
			string adminId = "adminId";
			string currUserId = isUserAdmin ? adminId : authorId;

			this.userMock
				.Setup(x => x.FindFirst(ClaimTypes.NameIdentifier))
				.Returns(new Claim(ClaimTypes.NameIdentifier, currUserId));

			string[] adminsIds = new[] { adminId };

			this.usersServiceMock
				.Setup(x => x.GetAdminsIdsAsync())
				.ReturnsAsync(adminsIds);

			string messageId = "id";
			this.messagesServiceMock
				.Setup(x => x.GetMessageAuthorIdAsync(messageId))
				.ReturnsAsync(authorId);

			this.messagesServiceMock
				.Setup(x => x.HasUnseenMessagesByAdminAsync())
				.ReturnsAsync(hasUnseenMessages);

			this.messagesServiceMock
				.Setup(x => x.HasUnseenMessagesByUserAsync(authorId))
				.ReturnsAsync(hasUnseenMessages);

			//Act
			var result = (RedirectToActionResult)await this.controller.Delete(messageId);

			//Assert
			this.messagesServiceMock.Verify(
				x => x.RemoveAsync(messageId, currUserId, isUserAdmin),
				Times.Once());

			this.allMessagesHubMock.Verify(
				x => x.Clients.Users(adminsIds),
				isUserAdmin ? Times.Never : Times.Once);

			this.allMessagesHubMock.Verify(
				x => x.Clients.Users(It.Is<List<string>>(x => x.Count() == 1 && x.First() == authorId)),
				isUserAdmin ? Times.Once : Times.Never);

			this.notificationsHubMock.Verify(
				x => x.Clients.Users(adminsIds),
				!isUserAdmin && !hasUnseenMessages ? Times.Once : Times.Never);

			this.notificationsHubMock.Verify(
				x => x.Clients.Users(It.Is<List<string>>(x => x.Count() == 1 && x.First() == authorId)),
				isUserAdmin && !hasUnseenMessages ? Times.Once : Times.Never);

			Assert.Multiple(() =>
			{
				Assert.That(result.ActionName, Is.EqualTo("Index"));
				Assert.That(result.RouteValues, Is.Null);
			});
		}

		[Test]
		public async Task Delete_ShouldReturnUnauthorized_WhenTheUserIsUnauthorized()
		{
			//Arrange
			string messageId = "id";

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.messagesServiceMock
				.Setup(x => x.RemoveAsync(messageId, this.userId.ToString(), false))
				.Throws<UnauthorizedAccessException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.UnauthorizedMessageDeletion,
				this.userId,
				messageId);

			//Act
			var result = (UnauthorizedResult)await this.controller.Delete(messageId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(401));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task Delete_ShouldReturnBadRequest_WhenTheDeleteWasUnsuccessful()
		{
			//Arrange
			string messageId = "id";

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.messagesServiceMock
				.Setup(x => x.RemoveAsync(messageId, this.userId.ToString(), false))
				.Throws<InvalidOperationException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.UnsuccessfulMessageDeletion,
				this.userId,
				messageId);

			//Act
			var result = (BadRequestResult)await this.controller.Delete(messageId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task Delete_ShouldReturnBadRequest_WhenTheModelStateIsInvalid()
		{
			//Arrange
			string messageId = "id";

			this.controller.ModelState.AddModelError("id", "invalid id");

			string expectedLogMessage = string.Format(
				LoggerMessages.DeleteMessageWithInvalidInputData,
				this.userId,
				messageId);

			//Act
			var result = (BadRequestResult)await this.controller.Delete(messageId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public void Index_ShouldReturnView()
		{
			//Arrange

			//Act
			var viewResult = (ViewResult)this.controller.Index();

			//Assert
			Assert.That(viewResult, Is.Not.Null);
		}

		[Test]
		public async Task Details_ShouldReturnViewModel()
		{
			//Arrange
			string messageId = "1";

			var serviceReturnDto = new MessageDetailsDTO
			{
				Id = messageId,
				AuthorId = this.userId.ToString(),
				AuthorName = "Author Name",
				Subject = "Message Subject",
				Content = "Message Content",
				CreatedOnUtc = DateTime.UtcNow.AddDays(-1),
				Replies = new ReplyOutputDTO[]
				{
					new ReplyOutputDTO
					{
						AuthorName = "Admin Name",
						Content = "Admin First Message Reply Content",
						CreatedOnUtc = DateTime.UtcNow
					}
				}
			};

			this.messagesServiceMock
				.Setup(x => x.GetMessageAsync(messageId, this.userId.ToString(), false))
				.ReturnsAsync(serviceReturnDto);

			//Act
			var viewResult = (ViewResult)await this.controller.Details(messageId);

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
		public async Task Details_ShouldReturnBadRequest_WhenMessageDoesNotExist()
		{
			//Arrange
			string messageId = "id";

			this.messagesServiceMock
				.Setup(x => x.GetMessageAsync(messageId, this.userId.ToString(), false))
				.Throws<InvalidOperationException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.GetMessageDetailsWithInvalidInputData,
				this.userId,
				messageId);

			//Act
			var result = (BadRequestResult)await this.controller.Details(messageId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task Details_ShouldReturnBadRequest_WhenMarkingAsSeenIsUnsuccessful()
		{
			//Arrange
			string messageId = "id";

			this.messagesServiceMock
				.Setup(x => x.GetMessageAsync(messageId, this.userId.ToString(), false))
				.Throws<ArgumentException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.UnsuccessfulMarkMessageAsSeen,
				this.userId,
				messageId);

			//Act
			var result = (BadRequestResult)await this.controller.Details(messageId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task Details_ShouldReturnBadRequest_WhenTheModelStateIsInvalid()
		{
			//Arrange
			string messageId = "id";

			this.controller.ModelState.AddModelError("id", "invalid id");

			string expectedLogMessage = string.Format(
				LoggerMessages.GetMessageDetailsWithInvalidInputData,
				this.userId,
				messageId);

			//Act
			var result = (BadRequestResult)await this.controller.Details(messageId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task Details_ShouldReturnUnauthorized_WhenTheUserIsUnauthorized()
		{
			//Arrange
			string messageId = "id";

			this.messagesServiceMock
				.Setup(x => x.GetMessageAsync(messageId, this.userId.ToString(), false))
				.Throws<UnauthorizedAccessException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.UnauthorizedGetMessageDetails,
				this.userId,
				messageId);

			//Act
			var result = (UnauthorizedResult)await this.controller.Details(messageId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(401));
		}
	}
}
