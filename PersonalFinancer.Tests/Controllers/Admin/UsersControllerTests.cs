namespace PersonalFinancer.Tests.Controllers.Admin
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Logging;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Services.Users.Models;
	using PersonalFinancer.Web.Areas.Admin.Controllers;

	[TestFixture]
	internal class UsersControllerTests : ControllersUnitTestsBase
	{
		private Mock<ILogger<UsersController>> loggerMock;
		private UsersController controller;

		[SetUp]
		public void SetUp()
		{
			this.loggerMock = new Mock<ILogger<UsersController>>();

			this.controller = new UsersController(
				this.usersServiceMock.Object,
				this.loggerMock.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = new DefaultHttpContext
					{
						User = this.userMock.Object
					}
				}
			};
		}

		[Test]
		public async Task Details_ShouldReturnCorrectViewModel()
		{
			//Arrange
			var ownerId = Guid.NewGuid();

			var expectedViewModel = new UserDetailsDTO
			{
				Id = ownerId,
				UserName = "username",
				FirstName = "User First Name",
				LastName = "User Last Name",
				Email = "user@email.com",
				IsAdmin = true,
				PhoneNumber = "1234567890",
				Accounts = new AccountCardDTO[]
				{
					new AccountCardDTO
					{
						Id = Guid.NewGuid(),
						Balance = 100,
						Name = "Account Name",
						CurrencyName = "BGN",
						OwnerId = ownerId
					}
				}
			};

			this.usersServiceMock
				.Setup(x => x.UserDetailsAsync(ownerId))
				.ReturnsAsync(expectedViewModel);

			//Act
			var result = (ViewResult)await this.controller.Details(ownerId);
			var viewModel = result.Model as UserDetailsDTO;

			//Assert
			Assert.That(viewModel, Is.Not.Null);
			AssertSamePropertiesValuesAreEqual(viewModel, expectedViewModel);
		}

		[Test]
		public async Task Details_ShouldReturnBadRequest_WhenTheUserDoesNotExist()
		{
			//Arrange
			var invalidUserId = Guid.NewGuid();

			this.usersServiceMock
				.Setup(x => x.UserDetailsAsync(invalidUserId))
				.Throws<InvalidOperationException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.GetUserDetailsWithInvalidInputData,
				this.userId);

			//Act
			var result = (BadRequestResult)await this.controller.Details(invalidUserId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task Details_ShouldReturnBadRequest_WhenTheModelStateIsInvalid()
		{
			//Arrange
			var invalidUserId = Guid.Empty;
			this.controller.ModelState.AddModelError("id", "invalid id");

			string expectedLogMessage = string.Format(
				LoggerMessages.GetUserDetailsWithInvalidInputData,
				this.userId);

			//Act
			var result = (BadRequestResult)await this.controller.Details(invalidUserId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public void Index_ShouldReturnView()
		{
			//Arrange

			//Act
			var result = this.controller.Index();

			//Assert
			Assert.That(result, Is.Not.Null);
		}
	}
}
