using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using PersonalFinancer.Services.Messages;
using PersonalFinancer.Services.Messages.Models;
using PersonalFinancer.Services.User;
using PersonalFinancer.Tests.Mocks;
using PersonalFinancer.Web.Controllers;
using PersonalFinancer.Web.Models.Message;
using System.Security.Claims;
using static PersonalFinancer.Data.Constants;

namespace PersonalFinancer.Tests.Controllers
{
	[TestFixture]
	internal class MessagesControllerTests
	{
		private Mock<IMessagesService> messagesServiceMock;
		private Mock<IUsersService> usersServiceMock;
		protected Mock<ClaimsPrincipal> userMock;
		private readonly IMapper mapper = ControllersMapperMock.Instance;
		private MessagesController controller;
		
		protected string userId = "user Id";

		[SetUp]
		public void SetUp()
		{
			this.messagesServiceMock = new Mock<IMessagesService>();
			this.usersServiceMock = new Mock<IUsersService>();
			this.controller = new MessagesController(
				this.messagesServiceMock.Object, this.usersServiceMock.Object, this.mapper);

			userMock = new Mock<ClaimsPrincipal>();

			userMock.Setup(x => x.FindFirst(ClaimTypes.NameIdentifier))
				.Returns(new Claim(ClaimTypes.NameIdentifier, userId));

			this.controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = this.userMock.Object,
				}
			};
		}

		[Test]
		public async Task AllMessages_ShouldReturnViewModelWithUserMessages_WhenUserIsNotAdmin()
		{
			//Arrange
			var serviceReturnDto = new MessageOutputServiceModel[]
			{
				new MessageOutputServiceModel
				{
					Id = "1",
					Subject = "Subject",
					CreatedOn = DateTime.UtcNow.AddDays(-1)
				},
				new MessageOutputServiceModel
				{
					Id = "2",
					Subject = "Subject 2",
					CreatedOn = DateTime.UtcNow
				}
			};

			this.userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(false);

			this.messagesServiceMock.Setup(x => x
				.GetUserMessagesAsync(this.userId))
				.ReturnsAsync(serviceReturnDto);

			//Act
			ViewResult viewResult = (ViewResult)await controller.AllMessages();

			//Assert
			Assert.That(viewResult, Is.Not.Null);
			var model = viewResult.Model as MessageOutputServiceModel[];
			Assert.That(model, Is.Not.Null);
			Assert.That(model, Has.Length.EqualTo(serviceReturnDto.Length));

			for (int i = 0; i < serviceReturnDto.Length; i++)
			{
				Assert.That(model.ElementAt(i).Id,
					Is.EqualTo(serviceReturnDto[i].Id));
				Assert.That(model.ElementAt(i).CreatedOn,
					Is.EqualTo(serviceReturnDto[i].CreatedOn));
				Assert.That(model.ElementAt(i).Subject,
					Is.EqualTo(serviceReturnDto[i].Subject));
			}
		}
				
		[Test]
		public async Task AllMessages_ShouldReturnViewModelWithUserMessages_WhenUserIsAdmin()
		{
			//Arrange
			var serviceReturnDto = new MessageOutputServiceModel[]
			{
				new MessageOutputServiceModel
				{
					Id = "1",
					Subject = "Subject",
					CreatedOn = DateTime.UtcNow.AddDays(-1)
				},
				new MessageOutputServiceModel
				{
					Id = "2",
					Subject = "Subject 2",
					CreatedOn = DateTime.UtcNow
				}
			};

			this.userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(true);

			this.messagesServiceMock.Setup(x => x
				.GetAllAsync())
				.ReturnsAsync(serviceReturnDto);

			//Act
			ViewResult viewResult = (ViewResult)await controller.AllMessages();

			//Assert
			Assert.That(viewResult, Is.Not.Null);
			var model = viewResult.Model as MessageOutputServiceModel[];
			Assert.That(model, Is.Not.Null);
			Assert.That(model, Has.Length.EqualTo(serviceReturnDto.Length));

			for (int i = 0; i < serviceReturnDto.Length; i++)
			{
				Assert.That(model.ElementAt(i).Id,
					Is.EqualTo(serviceReturnDto[i].Id));
				Assert.That(model.ElementAt(i).CreatedOn,
					Is.EqualTo(serviceReturnDto[i].CreatedOn));
				Assert.That(model.ElementAt(i).Subject,
					Is.EqualTo(serviceReturnDto[i].Subject));
			}
		}

		[Test]
		public void Create_OnGet_ShouldReturnViewModel()
		{
			//Arrange
			var expected = new MessageInputModel();

			//Act
			ViewResult viewResult = (ViewResult)controller.Create();

			//Assert
			Assert.That(viewResult, Is.Not.Null);
			var model = viewResult.Model as MessageInputModel;
			Assert.That(model, Is.Not.Null);
			Assert.That(model.Subject, Is.EqualTo(expected.Subject));
			Assert.That(model.Content, Is.EqualTo(expected.Content));
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
			var viewResult = (ViewResult)await controller.Create(inputModel);

			//Assert
			Assert.That(viewResult, Is.Not.Null);
			var model = viewResult.Model as MessageInputModel;
			Assert.That(model, Is.Not.Null);
			Assert.That(model.Subject, Is.EqualTo(inputModel.Subject));
			Assert.That(model.Content, Is.EqualTo(inputModel.Content));
			CheckModelStateErrors(viewResult.ViewData.ModelState, nameof(inputModel.Content), "Invalid content.");
		}
				
		[Test]
		public async Task Create_OnPost_ShouldRedirectToAction_WhenMessageWasCreated()
		{
			//Arrange
			var inputModel = new MessageInputModel
			{
				Subject = "Subject",
				Content = ""
			};

			string fullName = "full name";
			string newMessage = "new message id";

			this.usersServiceMock.Setup(x => x
				.UserFullName(this.userId))
				.ReturnsAsync(fullName);

			this.messagesServiceMock.Setup(x => 
				x.CreateAsync(It.Is<MessageInputServiceModel>(m => 
					m.Subject == inputModel.Subject
					&& m.Content == inputModel.Content
					&& m.AuthorId == this.userId
					&& m.AuthorName == fullName)))
				.ReturnsAsync(newMessage);

			//Act
			var result = (RedirectToActionResult)await controller.Create(inputModel);

			//Assert
			Assert.That(result.ActionName, Is.EqualTo("MessageDetails"));
			Assert.That(result.RouteValues, Is.Not.Null);
			CheckRouteValues(result.RouteValues, "id", newMessage);
		}

		[Test]
		public async Task Delete_ShouldRedirectToAction_WhenMessageWasDeleted()
		{
			//Arrange
			string messageId = "id";

			this.userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(false);
			
			//Act
			var result = (RedirectToActionResult)await controller.Delete(messageId);

			this.messagesServiceMock.Verify(x => x
				.RemoveAsync(messageId, this.userId, false), 
				Times.Once());
						
			//Assert
			Assert.That(result.ActionName, Is.EqualTo("AllMessages"));
			Assert.That(result.RouteValues, Is.Null);
		}

		[Test]
		public async Task Delete_ShouldReturnUnauthorized_WhenUserIsNotOwnerOrAdmin()
		{
			//Arrange
			string messageId = "id";

			this.userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(false);

			this.messagesServiceMock.Setup(x => x
				.RemoveAsync(messageId, this.userId, false))
				.Throws<ArgumentException>();

			//Act
			var result = (UnauthorizedResult)await controller.Delete(messageId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(401));
		}
		
		[Test]
		public async Task Delete_ShouldReturnBadRequest_WhenDeleteWasUnsuccessful()
		{
			//Arrange
			string messageId = "id";

			this.userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(false);

			this.messagesServiceMock.Setup(x => x
				.RemoveAsync(messageId, this.userId, false))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await controller.Delete(messageId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public async Task MessageDetails_OnGet_ShouldReturnViewModel()
		{
			//Arrange
			string messageId = "id";

			var serviceReturnDto = new MessageDetailsServiceModel
			{
				Id = messageId,
				AuthorName = "Account name",
				Content = "Content",
				CreatedOn = DateTime.UtcNow.AddDays(-1),
				Replies = new ReplyOutputServiceModel[]
				{
					new ReplyOutputServiceModel
					{
						AuthorName = "Author name",
						Content = "Content",
						CreatedOn = DateTime.UtcNow
					}
				},
				Subject = "Subject"
			};

			this.messagesServiceMock.Setup(x => x
				.GetMessageAsync(messageId, this.userId, false))
				.ReturnsAsync(serviceReturnDto);

			//Act
			var viewResult = (ViewResult)await controller.MessageDetails(messageId);

			//Assert
			Assert.That(viewResult, Is.Not.Null);

			var model = viewResult.Model as MessageDetailsViewModel;
			Assert.That(model, Is.Not.Null);
			Assert.That(model.Id, Is.EqualTo(serviceReturnDto.Id));
			Assert.That(model.Content, Is.EqualTo(serviceReturnDto.Content));
			Assert.That(model.ReplyContent, Is.Null);
			Assert.That(model.AuthorName, Is.EqualTo(serviceReturnDto.AuthorName));
			Assert.That(model.CreatedOn, Is.EqualTo(serviceReturnDto.CreatedOn));
			Assert.That(model.Subject, Is.EqualTo(serviceReturnDto.Subject));
			Assert.That(model.Replies.Count(), Is.EqualTo(serviceReturnDto.Replies.Count()));

			for (int i = 0; i < serviceReturnDto.Replies.Count(); i++)
			{
				Assert.That(model.Replies.ElementAt(i).Content,
					Is.EqualTo(serviceReturnDto.Replies.ElementAt(i).Content));
				Assert.That(model.Replies.ElementAt(i).AuthorName,
					Is.EqualTo(serviceReturnDto.Replies.ElementAt(i).AuthorName));
				Assert.That(model.Replies.ElementAt(i).CreatedOn,
					Is.EqualTo(serviceReturnDto.Replies.ElementAt(i).CreatedOn));
			}
		}
		
		[Test]
		public async Task MessageDetails_OnGet_ShouldReturnBadRequest_WhenMessageDoesNotExistOrUserIsNowOwnerOrAdmin()
		{
			//Arrange
			string messageId = "id";

			this.messagesServiceMock.Setup(x => x
				.GetMessageAsync(messageId, this.userId, false))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await controller.MessageDetails(messageId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}
		
		[Test]
		public async Task MessageDetails_OnPost_ShouldReturnViewModelWithErrors_WhenModelIsInvalid()
		{
			//Arrange
			var inputModel = new ReplyInputModel
			{
				Id = "Message id",
				ReplyContent = ""
			};
			
			var serviceReturnDto = new MessageDetailsServiceModel
			{
				Id = inputModel.Id,
				AuthorName = "Account name",
				Content = "Content",
				CreatedOn = DateTime.UtcNow.AddDays(-1),
				Replies = new ReplyOutputServiceModel[]
				{
					new ReplyOutputServiceModel
					{
						AuthorName = "Author name",
						Content = "Content",
						CreatedOn = DateTime.UtcNow
					}
				},
				Subject = "Subject"
			};
			
			this.userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(false);

			this.messagesServiceMock.Setup(x => x
				.GetMessageAsync(inputModel.Id, this.userId, false))
				.ReturnsAsync(serviceReturnDto);

			this.controller.ModelState.AddModelError(nameof(inputModel.ReplyContent), "Invalid content.");

			//Act
			var viewResult = (ViewResult)await controller.MessageDetails(inputModel);

			//Assert
			Assert.That(viewResult, Is.Not.Null);
			var model = viewResult.Model as MessageDetailsViewModel;
			Assert.That(model, Is.Not.Null);
			Assert.That(model.Subject, Is.EqualTo(serviceReturnDto.Subject));
			Assert.That(model.Content, Is.EqualTo(serviceReturnDto.Content));
			Assert.That(model.Id, Is.EqualTo(serviceReturnDto.Id));
			Assert.That(model.AuthorName, Is.EqualTo(serviceReturnDto.AuthorName));
			Assert.That(model.ReplyContent, Is.EqualTo(inputModel.ReplyContent));
			Assert.That(model.CreatedOn, Is.EqualTo(serviceReturnDto.CreatedOn));
			Assert.That(model.Replies.Count(), Is.EqualTo(serviceReturnDto.Replies.Count()));

			for (int i = 0; i < serviceReturnDto.Replies.Count(); i++)
			{
				Assert.That(model.Replies.ElementAt(i).Content,
					Is.EqualTo(serviceReturnDto.Replies.ElementAt(i).Content));
				Assert.That(model.Replies.ElementAt(i).AuthorName,
					Is.EqualTo(serviceReturnDto.Replies.ElementAt(i).AuthorName));
				Assert.That(model.Replies.ElementAt(i).CreatedOn,
					Is.EqualTo(serviceReturnDto.Replies.ElementAt(i).CreatedOn));
			}

			CheckModelStateErrors(viewResult.ViewData.ModelState, nameof(inputModel.ReplyContent), "Invalid content.");
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
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(false);

			this.messagesServiceMock.Setup(x => x
				.GetMessageAsync(inputModel.Id, this.userId, false))
				.Throws<InvalidOperationException>();

			this.controller.ModelState.AddModelError(nameof(inputModel.ReplyContent), "Invalid content.");

			//Act
			var result = (BadRequestResult)await controller.MessageDetails(inputModel);

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

			string userName = "user name";
			
			this.userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(false);
			
			this.usersServiceMock.Setup(x => x
				.UserFullName(this.userId))
				.ReturnsAsync(userName);

			//Act
			var result = (RedirectToActionResult)await controller.MessageDetails(inputModel);

			this.messagesServiceMock.Verify(x => x
				.AddReplyAsync(It.Is<ReplyInputServiceModel>(m => 
					m.MessageId == inputModel.Id
					&& m.IsAuthorAdmin == false
					&& m.Content == inputModel.ReplyContent
					&& m.AuthorName == userName
					&& m.AuthorId == this.userId)), 
				Times.Once);

			//Assert
			Assert.That(result.ActionName, Is.EqualTo("MessageDetails"));
			Assert.That(result.RouteValues, Is.Not.Null);
			CheckRouteValues(result.RouteValues, "id", inputModel.Id);
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

			string userName = "user name";
			
			this.userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(false);
			
			this.usersServiceMock.Setup(x => x
				.UserFullName(this.userId))
				.ReturnsAsync(userName);

			this.messagesServiceMock.Setup(x => x
				.AddReplyAsync(It.Is<ReplyInputServiceModel>(m => 
					m.MessageId == inputModel.Id
					&& m.IsAuthorAdmin == false
					&& m.Content == inputModel.ReplyContent
					&& m.AuthorName == userName
					&& m.AuthorId == this.userId)))
				.Throws<ArgumentException>();

			//Act
			var result = (UnauthorizedResult)await controller.MessageDetails(inputModel);

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
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(false);
			
			this.usersServiceMock.Setup(x => x
				.UserFullName(this.userId))
				.ReturnsAsync(userName);

			this.messagesServiceMock.Setup(x => x
				.AddReplyAsync(It.Is<ReplyInputServiceModel>(m => 
					m.MessageId == inputModel.Id
					&& m.IsAuthorAdmin == false
					&& m.Content == inputModel.ReplyContent
					&& m.AuthorName == userName
					&& m.AuthorId == this.userId)))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await controller.MessageDetails(inputModel);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}
			
		protected static void CheckModelStateErrors(
			ModelStateDictionary modelState, string key, string errorMessage)
		{
			Assert.That(modelState.Keys.Count(), Is.EqualTo(1));
			Assert.That(modelState.Keys.First(), Is.EqualTo(key));
			Assert.That(modelState.Values.Count(), Is.EqualTo(1));
			Assert.That(modelState.Values.First().Errors, Has.Count.EqualTo(1));

			Assert.That(modelState.Values.First().Errors.First().ErrorMessage,
				Is.EqualTo(errorMessage));
		}
				
		protected static void CheckRouteValues(RouteValueDictionary routeValues, string key, string value)
		{			
			Assert.That(routeValues, Is.Not.Null);
			Assert.That(routeValues.Keys, Has.Count.EqualTo(1));
			Assert.That(routeValues.ContainsKey(key), Is.True);
			Assert.That(routeValues.Values, Has.Count.EqualTo(1));
			Assert.That(routeValues.Values.First(), Is.EqualTo(value));
		}
	}
}
