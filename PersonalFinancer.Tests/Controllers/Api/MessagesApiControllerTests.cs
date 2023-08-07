namespace PersonalFinancer.Tests.Controllers.Api
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Logging;
	using MongoDB.Bson;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Services.Messages;
	using PersonalFinancer.Services.Messages.Models;
	using PersonalFinancer.Web.Controllers.Api;
	using PersonalFinancer.Web.Models.Api;
	using PersonalFinancer.Web.Models.Message;
	using static PersonalFinancer.Common.Constants.PaginationConstants;
	using static PersonalFinancer.Common.Constants.RoleConstants;

	[TestFixture]
	internal class MessagesApiControllerTests : ControllersUnitTestsBase
	{
		private Mock<ILogger<MessagesApiController>> loggerMock;
		private Mock<IMessagesService> messagesServiceMock;
		private MessagesApiController apiController;

		[SetUp]
		public void SetUp()
		{
			this.loggerMock = new Mock<ILogger<MessagesApiController>>();
			this.messagesServiceMock = new Mock<IMessagesService>();

			this.apiController = new MessagesApiController(
				this.messagesServiceMock.Object,
				this.usersServiceMock.Object,
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
		public async Task AddReply_ShouldReturnCorrectData()
		{
			//Arrange
			var inputModel = new ReplyInputModel
			{
				MessageId = "Message id",
				ReplyContent = "Reply content"
			};

			string userFullName = "User Full Name";

			this.usersServiceMock
				.Setup(x => x.UserFullNameAsync(this.userId))
				.ReturnsAsync(userFullName);

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			var expected = new ReplyOutputDTO
			{
				AuthorName = userFullName,
				Content = inputModel.ReplyContent,
				CreatedOnUtc = DateTime.UtcNow
			};

			this.messagesServiceMock
				.Setup(x => x.AddReplyAsync(It.Is<ReplyInputDTO>(y =>
					y.AuthorId == this.userId.ToString()
					&& y.Content == inputModel.ReplyContent
					&& y.IsAuthorAdmin == false
					&& y.MessageId == inputModel.MessageId
					&& y.AuthorName == userFullName)))
				.ReturnsAsync(expected);

			//Act
			var actual = (OkObjectResult)await this.apiController.AddReply(inputModel);
			var value = actual.Value as ReplyOutputDTO;

			//Assert
			Assert.That(value, Is.Not.Null);
			Assert.Multiple(() =>
			{
				Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
				Assert.That(value.ToJson(), Is.EqualTo(expected.ToJson()));
			});
		}

		[Test]
		public async Task AddReply_ShouldReturnUnauthorized_WhenTheUserIsUnauthorized()
		{
			//Arrange
			var inputModel = new ReplyInputModel
			{
				MessageId = "Message id",
				ReplyContent = "Reply content"
			};

			string userFullName = "User Full Name";

			this.usersServiceMock
				.Setup(x => x.UserFullNameAsync(this.userId))
				.ReturnsAsync(userFullName);

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.messagesServiceMock
				.Setup(x => x.AddReplyAsync(It.Is<ReplyInputDTO>(y =>
					y.AuthorId == this.userId.ToString()
					&& y.Content == inputModel.ReplyContent
					&& y.IsAuthorAdmin == false
					&& y.MessageId == inputModel.MessageId
					&& y.AuthorName == userFullName)))
				.Throws<UnauthorizedAccessException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.UnauthorizedReplyAddition,
				this.userId,
				inputModel.MessageId);

			//Act
			var actual = (UnauthorizedResult)await this.apiController.AddReply(inputModel);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task AddReply_ShouldReturnBadRequest_WhenAddingTheReplyWasUnsuccessful()
		{
			//Arrange
			var inputModel = new ReplyInputModel
			{
				MessageId = "Message id",
				ReplyContent = "Reply content"
			};

			string userFullName = "User Full Name";

			this.usersServiceMock
				.Setup(x => x.UserFullNameAsync(this.userId))
				.ReturnsAsync(userFullName);

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.messagesServiceMock
				.Setup(x => x.AddReplyAsync(It.Is<ReplyInputDTO>(y =>
					y.AuthorId == this.userId.ToString()
					&& y.Content == inputModel.ReplyContent
					&& y.IsAuthorAdmin == false
					&& y.MessageId == inputModel.MessageId
					&& y.AuthorName == userFullName)))
				.Throws<InvalidOperationException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.UnsuccessfulReplyAddition,
				this.userId,
				inputModel.MessageId);

			//Act
			var actual = (BadRequestResult)await this.apiController.AddReply(inputModel);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task AddReply_ShouldReturnBadRequest_WhenTheModelStateIsInvalid()
		{
			//Arrange
			var inputModel = new ReplyInputModel
			{
				MessageId = "Message id",
				ReplyContent = "Reply content"
			};

			this.apiController.ModelState.AddModelError("messageId", "invalid id");

			string expectedLogMessage = string.Format(
				LoggerMessages.AddReplyWithInvalidInputData,
				this.userId);

			//Act
			var actual = (BadRequestResult)await this.apiController.AddReply(inputModel);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public async Task All_ShouldReturnCorrectData(bool isUserAdmin)
		{
			//Arrange
			var inputModel = new SearchFilterInputModel
			{
				Page = 1,
				Search = null
			};

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(isUserAdmin);

			var messagesDTO = new MessagesDTO
			{
				Messages = new MessageOutputDTO[]
				{
					new MessageOutputDTO
					{
						Id = "id 1",
						Subject = "Subject 1",
						CreatedOnUtc = DateTime.UtcNow,
						IsSeen = false
					},
					new MessageOutputDTO
					{
						Id = "id 2",
						Subject = "Subject 2",
						CreatedOnUtc = DateTime.UtcNow,
						IsSeen = true
					}
				},
				TotalMessagesCount = 4
			};

			this.messagesServiceMock
				.Setup(x => x.GetAllMessagesAsync(inputModel.Page, null))
				.ReturnsAsync(messagesDTO);

			this.messagesServiceMock
				.Setup(x => x.GetUserMessagesAsync(this.userId.ToString(), inputModel.Page, null))
				.ReturnsAsync(messagesDTO);

			//Act
			var actual = (OkObjectResult)await this.apiController.All(inputModel);
			var value = actual.Value as MessagesViewModel;

			//Assert
			this.messagesServiceMock.Verify(
				x => x.GetAllMessagesAsync(inputModel.Page, inputModel.Search),
				isUserAdmin ? Times.Once : Times.Never);

			this.messagesServiceMock.Verify(
				x => x.GetUserMessagesAsync(this.userId.ToString(), inputModel.Page, inputModel.Search),
				isUserAdmin ? Times.Never : Times.Once);

			Assert.That(value, Is.Not.Null);
			Assert.Multiple(() =>
			{
				Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
				Assert.That(value.Messages.ToJson(), Is.EqualTo(messagesDTO.Messages.ToJson()));

				AssertPaginationModelIsEqual(
					value.Pagination,
					"messages",
					MessagesPerPage,
					messagesDTO.TotalMessagesCount,
					inputModel.Page);
			});
		}

		[Test]
		public async Task All_ShouldReturnBadRequest_WhenTheModelStateIsInvalid()
		{
			//Arrange
			var inputModel = new SearchFilterInputModel
			{
				Page = 0,
				Search = null
			};

			this.apiController.ModelState.AddModelError(nameof(inputModel.Page), "Invalid page.");
			string expectedLogMessage = string.Format(LoggerMessages.GetMessagesInfoWithInvalidInputData, this.userId);

			//Act
			var actual = (BadRequestResult)await this.apiController.All(inputModel);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public async Task Archived_ShouldReturnCorrectData(bool isUserAdmin)
		{
			//Arrange
			var inputModel = new SearchFilterInputModel
			{
				Page = 1,
				Search = null
			};

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(isUserAdmin);

			var messagesDTO = new MessagesDTO
			{
				Messages = new MessageOutputDTO[]
				{
					new MessageOutputDTO
					{
						Id = "id 1",
						Subject = "Subject 1",
						CreatedOnUtc = DateTime.UtcNow,
						IsSeen = false
					},
					new MessageOutputDTO
					{
						Id = "id 2",
						Subject = "Subject 2",
						CreatedOnUtc = DateTime.UtcNow,
						IsSeen = true
					}
				},
				TotalMessagesCount = 4
			};

			this.messagesServiceMock
				.Setup(x => x.GetAllArchivedMessagesAsync(inputModel.Page, inputModel.Search))
				.ReturnsAsync(messagesDTO);

			this.messagesServiceMock
				.Setup(x => x.GetUserArchivedMessagesAsync(this.userId.ToString(), inputModel.Page, inputModel.Search))
				.ReturnsAsync(messagesDTO);

			//Act
			var actual = (OkObjectResult)await this.apiController.Archived(inputModel);
			var value = actual.Value as MessagesViewModel;

			//Assert
			this.messagesServiceMock.Verify(
				x => x.GetAllArchivedMessagesAsync(inputModel.Page, inputModel.Search),
				isUserAdmin ? Times.Once : Times.Never);

			this.messagesServiceMock.Verify(
				x => x.GetUserArchivedMessagesAsync(this.userId.ToString(), inputModel.Page, inputModel.Search),
				isUserAdmin ? Times.Never : Times.Once);

			Assert.That(value, Is.Not.Null);
			Assert.Multiple(() =>
			{
				Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
				Assert.That(value.Messages.ToJson(), Is.EqualTo(messagesDTO.Messages.ToJson()));

				AssertPaginationModelIsEqual(
					value.Pagination,
					"messages",
					MessagesPerPage,
					messagesDTO.TotalMessagesCount,
					inputModel.Page);
			});
		}

		[Test]
		public async Task Archived_ShouldReturnBadRequest_WhenTheModelStateIsInvalid()
		{
			//Arrange
			var inputModel = new SearchFilterInputModel
			{
				Page = 0,
				Search = null
			};

			this.apiController.ModelState.AddModelError(nameof(inputModel.Page), "Invalid page.");
			string expectedLogMessage = string.Format(LoggerMessages.GetMessagesInfoWithInvalidInputData, this.userId);

			//Act
			var actual = (BadRequestResult)await this.apiController.Archived(inputModel);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public async Task MarkAsSeen_ShouldReturnOk_WhenTheMessageWasMarkAsSeen(bool isUserAdmin)
		{
			//Arrange
			string messageId = "message id";

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(isUserAdmin);

			//Act
			var actual = (OkResult)await this.apiController.MarkAsSeen(messageId);

			//Assert
			this.messagesServiceMock.Verify(
				x => x.MarkMessageAsSeenAsync(messageId, this.userId.ToString(), isUserAdmin),
				Times.Once);

			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
		}

		[Test]
		public async Task MarkAsSeen_ShouldReturnBadRequest_WhenTheUpdateWasUnsuccessful()
		{
			//Arrange
			string messageId = "message id";

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.messagesServiceMock
				.Setup(x => x.MarkMessageAsSeenAsync(messageId, this.userId.ToString(), false))
				.Throws<ArgumentException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.UnsuccessfulMarkMessageAsSeen,
				this.userId,
				messageId);

			//Act
			var actual = (BadRequestResult)await this.apiController.MarkAsSeen(messageId);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task MarkAsSeen_ShouldReturnBadRequest_WhenTheModelStateIsInvalid()
		{
			//Arrange
			string messageId = "invalid id";

			this.apiController.ModelState.AddModelError("id", "invalid id");

			string expectedLogMessage = string.Format(
				LoggerMessages.MarkAsSeenWithInvalidInputData,
				this.userId);

			//Act
			var actual = (BadRequestResult)await this.apiController.MarkAsSeen(messageId);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}
	}
}
