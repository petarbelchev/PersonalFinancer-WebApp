namespace PersonalFinancer.Tests.Controllers
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.SignalR;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Services.Messages;
	using PersonalFinancer.Services.Messages.Models;
	using PersonalFinancer.Web.Controllers;
	using PersonalFinancer.Web.Hubs;
	using PersonalFinancer.Web.Models.Message;
	using static PersonalFinancer.Common.Constants.RoleConstants;

	[TestFixture]
	internal class MessagesControllerTests : ControllersUnitTestsBase
	{
		private ICollection<Message> fakeCollection;
		private Mock<IMessagesService> messagesServiceMock;
		private Mock<IHubContext<AllMessagesHub>> allMessagesHubMock;
		private Mock<IHubContext<NotificationsHub>> notificationsHubMock;
		private MessagesController controller;

		[SetUp]
		public void SetUp()
		{
			this.fakeCollection = this.SeedFakeCollection();
			this.messagesServiceMock = new Mock<IMessagesService>();

			this.allMessagesHubMock = new Mock<IHubContext<AllMessagesHub>>();
			this.notificationsHubMock = new Mock<IHubContext<NotificationsHub>>();

			var clientProxyMock = new Mock<IClientProxy>();
			clientProxyMock.Setup(x => x
				.SendCoreAsync(
					It.IsAny<string>(),
					It.IsAny<object[]>(),
					It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			var iHubClientsMock = new Mock<IHubClients>();
			iHubClientsMock.Setup(x => x
				.Users(It.IsAny<IReadOnlyList<string>>()))
				.Returns(clientProxyMock.Object);

			this.notificationsHubMock.Setup(x => x.Clients).Returns(iHubClientsMock.Object);
			this.allMessagesHubMock.Setup(x => x.Clients).Returns(iHubClientsMock.Object);

			this.controller = new MessagesController(
				this.messagesServiceMock.Object,
				this.usersServiceMock.Object,
				this.mapper,
				this.allMessagesHubMock.Object,
				this.notificationsHubMock.Object)
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
		public async Task All_ShouldReturnView_WhenUserIsNotAdmin()
		{
			//Arrange
			//int page = 1;

			//var serviceReturnDto = new MessagesDTO
			//{
			//	Messages = this.fakeCollection
			//		.Where(m => m.AuthorId == this.userId.ToString() && !m.IsArchivedByAuthor)
			//		.Select(m => new MessageOutputDTO
			//		{
			//			Id = m.Id,
			//			CreatedOnUtc = m.CreatedOnUtc,
			//			Subject = m.Subject,
			//			IsSeen = m.IsSeenByAuthor
			//		})
			//		.Skip(MessagesPerPage * (page - 1))
			//		.Take(MessagesPerPage),
			//	TotalMessagesCount = this.fakeCollection
			//		.Count(m => m.AuthorId == this.userId.ToString() && !m.IsArchivedByAuthor)
			//};

			//this.userMock.Setup(x => x
			//	.IsInRole(AdminRoleName))
			//	.Returns(false);

			//this.messagesServiceMock.Setup(x => x
			//	.GetUserMessagesAsync(this.userId.ToString(), page))
			//	.ReturnsAsync(serviceReturnDto);

			//Act
			var viewResult = (ViewResult)this.controller.All();

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				//MessagesViewModel viewModel = viewResult.Model as MessagesViewModel ??
				//	throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				//Assert.That(viewModel.Pagination.TotalElements, 
				//	Is.EqualTo(serviceReturnDto.TotalMessagesCount));

				//Assert.That(viewModel.Pagination.Page, Is.EqualTo(page));

				//AssertSamePropertiesValuesAreEqual(viewModel, serviceReturnDto);
			});
		}

		[Test]
		public async Task All_ShouldReturnView_WhenUserIsAdmin()
		{
			//Arrange
			int page = 1;

			//var serviceReturnDto = new MessagesDTO
			//{
			//	Messages = this.fakeCollection
			//		.Where(m => !m.IsArchivedByAdmin)
			//		.Select(m => new MessageOutputDTO
			//		{
			//			Id = m.Id,
			//			CreatedOnUtc = m.CreatedOnUtc,
			//			Subject = m.Subject,
			//			IsSeen = m.IsSeenByAdmin
			//		})
			//		.Skip(MessagesPerPage * (page - 1))
			//		.Take(MessagesPerPage),
			//	TotalMessagesCount = this.fakeCollection
			//		.Count(m => !m.IsArchivedByAdmin)
			//};

			//this.userMock.Setup(x => x
			//	.IsInRole(AdminRoleName))
			//	.Returns(true);

			//this.messagesServiceMock.Setup(x => x
			//	.GetAllMessagesAsync(page))
			//	.ReturnsAsync(serviceReturnDto);

			//Act
			var viewResult = (ViewResult)this.controller.All();

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				//MessagesViewModel viewModel = viewResult.Model as MessagesViewModel ??
				//	throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				//Assert.That(viewModel.Pagination.TotalElements,
				//	Is.EqualTo(serviceReturnDto.TotalMessagesCount));

				//Assert.That(viewModel.Pagination.Page, Is.EqualTo(page));

				//AssertSamePropertiesValuesAreEqual(viewModel.Messages, serviceReturnDto);
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

			var expected = new MessageOutputDTO
			{
				Id = "new message id",
				Subject = inputModel.Subject,
				IsSeen = true,
			};

			this.usersServiceMock.Setup(x => x
				.UserFullNameAsync(this.userId))
				.ReturnsAsync(userFullName);

			this.messagesServiceMock.Setup(x => x
				.CreateAsync(It.Is<MessageInputDTO>(m =>
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
				Assert.That(result.ActionName, Is.EqualTo("All"));
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
		public async Task MessageDetails_OnGet_ShouldReturnBadRequest_WhenMessageDoesNotExistOrUserIsNowOwnerOrAdmin()
		{
			//Arrange
			string messageId = "id";

			this.messagesServiceMock.Setup(x => x
				.GetMessageAsync(messageId, this.userId.ToString(), false))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await this.controller.Details(messageId);

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
					CreatedOnUtc = DateTime.UtcNow,
					Subject = "First User First Message",
					AuthorId = this.userId.ToString(),
					AuthorName = "First User username",
					Content = "First User First Message Content",
					Replies = new List<Reply>
					{
						new Reply
						{
							AuthorId = "admin ID",
							AuthorName = "Admin Name",
							Content = "Admin First Message Reply Content",
							CreatedOnUtc = DateTime.UtcNow
						}
					}
				},
				new Message
				{
					Id = "2",
					CreatedOnUtc = DateTime.UtcNow,
					Subject = "First User Second Message",
					AuthorId = this.userId.ToString(),
					AuthorName = "First User username",
					Content = "First User Second Message Content",
				},
				new Message
				{
					Id = "3",
					CreatedOnUtc = DateTime.UtcNow,
					Subject = "Second User First Message",
					AuthorId = "Second User ID",
					AuthorName = "Second User Name",
					Content = "Second User First Message Content",
				}
			};
		}
	}
}
