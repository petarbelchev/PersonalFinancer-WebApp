namespace PersonalFinancer.Tests.Controllers.Api
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Logging;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Services.Api;
	using PersonalFinancer.Services.Api.Models;
	using PersonalFinancer.Web.Controllers.Api;
	using PersonalFinancer.Web.Models.Api;
	using static PersonalFinancer.Common.Constants.RoleConstants;

	[TestFixture]
	internal class AccountTypesApiControllerTests : ControllersUnitTestsBase
	{
		private Mock<ILogger<BaseApiController<AccountType>>> loggerMock;
		private Mock<IApiService<AccountType>> apiServiceMock;
		private AccountTypesApiController apiController;

		[SetUp]
		public void SetUp()
		{
			this.loggerMock = new Mock<ILogger<BaseApiController<AccountType>>>();
			this.apiServiceMock = new Mock<IApiService<AccountType>>();

			this.apiController = new AccountTypesApiController(
				this.apiServiceMock.Object,
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
		public async Task CreateAccountType_ShouldReturnCorrectData()
		{
			//Arrange
			var ownerId = Guid.NewGuid();

			var inputModel = new AccountTypeInputModel
			{
				Name = "Account Type Name",
				OwnerId = ownerId
			};

			var expected = new ApiEntityDTO
			{
				Id = Guid.NewGuid(),
				Name = inputModel.Name,
			};

			this.apiServiceMock
				.Setup(x => x.CreateEntityAsync(inputModel.Name, ownerId))
				.ReturnsAsync(expected);

			//Act
			var actual = (CreatedResult)await this.apiController.CreateAccountType(inputModel);
			var value = actual.Value as ApiEntityDTO;

			//Assert
			Assert.That(value, Is.Not.Null);
			Assert.Multiple(() =>
			{
				Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
				AssertSamePropertiesValuesAreEqual(value, expected);
			});
		}

		[Test]
		public async Task CreateAccountType_ShouldReturnBadRequestWithMessage_WhenTheNameExists()
		{
			//Arrange
			var ownerId = Guid.NewGuid();

			var inputModel = new AccountTypeInputModel
			{
				Name = "Account Type Name",
				OwnerId = ownerId
			};

			this.apiServiceMock
				.Setup(x => x.CreateEntityAsync(inputModel.Name, ownerId))
				.Throws(new ArgumentException(ExceptionMessages.ExistingEntityName));

			//Act
			var actual = (BadRequestObjectResult)await this.apiController.CreateAccountType(inputModel);
				
			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
				
				Assert.That(actual.Value, Is.EqualTo(string.Format(
					ExceptionMessages.ExistingUserEntityName,
					"account type",
					inputModel.Name)));
			});
		}

		[Test]
		public async Task CreateAccountType_ShouldReturnBadRequestWithModelErrors_WhenTheInputModelStateIsInvalid()
		{
			//Arrange
			var inputModel = new AccountTypeInputModel
			{
				Name = "Account Type Name",
				OwnerId = Guid.Empty
			};

			this.apiController.ModelState.AddModelError("id", "invalid id");

			string expectedLogMessage = string.Format(
				LoggerMessages.CreateEntityWithInvalidInputData,
				this.userId,
				"account type");

			//Act
			var actual = (BadRequestObjectResult)await this.apiController.CreateAccountType(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
				Assert.That(actual.Value, Is.EqualTo("invalid id"));
			});

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public void CreateAccountType_ShouldThrowInvalidOperationException_WhenTheOwnerIdIsNull()
		{
			//Arrange
			var inputModel = new AccountTypeInputModel
			{
				Name = "Account Type Name",
				OwnerId = null
			};

			//Act & Assert
			Assert.That(async () => await this.apiController.CreateAccountType(inputModel),
			Throws.TypeOf<InvalidOperationException>().With.Message
				  .EqualTo(string.Format(ExceptionMessages.NotNullableProperty, inputModel.OwnerId)));
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public async Task DeleteAccountType_ShouldReturnNoContentResponseType_WhenTheAccountTypeWasDeleted(bool isUserAdmin)
		{
			//Arrange
			var id = Guid.NewGuid();

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(isUserAdmin);

			//Act
			var actual = (NoContentResult)await this.apiController.DeleteAccountType(id);

			//Assert
			this.apiServiceMock.Verify(
				x => x.DeleteEntityAsync(id, this.userId, isUserAdmin), 
				Times.Once);

			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
		}

		[Test]
		public async Task DeleteAccountType_ShouldReturnBadRequestWithModelErrors_WhenTheInputModelStateIsInvalid()
		{
			//Arrange
			var id = Guid.Empty;

			this.apiController.ModelState.AddModelError("id", "invalid id");

			//Act
			var actual = (BadRequestObjectResult)await this.apiController.DeleteAccountType(id);

			string expectedLogMessage = string.Format(
				LoggerMessages.DeleteEntityWithInvalidInputData,
				this.userId,
				"account type",
				id);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
				Assert.That(actual.Value, Is.EqualTo("invalid id"));
			});

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task DeleteAccountType_ShouldReturnUnauthorized_WhenTheUserIsUnauthorized()
		{
			//Arrange
			var id = Guid.Empty;

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.apiServiceMock
				.Setup(x => x.DeleteEntityAsync(id, this.userId, false))
				.Throws<UnauthorizedAccessException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.UnauthorizedEntityDeletion,
				this.userId,
				"account type",
				id);

			//Act
			var actual = (UnauthorizedResult)await this.apiController.DeleteAccountType(id);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task DeleteAccountType_ShouldReturnBadRequest_WhenTheAccountTypeDoesNotExist()
		{
			//Arrange
			var id = Guid.Empty;

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.apiServiceMock
				.Setup(x => x.DeleteEntityAsync(id, this.userId, false))
				.Throws<InvalidOperationException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.DeleteEntityWithInvalidInputData,
				this.userId,
				"account type",
				id);

			//Act
			var actual = (BadRequestResult)await this.apiController.DeleteAccountType(id);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}
	}
}
