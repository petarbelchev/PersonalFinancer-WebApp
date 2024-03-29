﻿namespace PersonalFinancer.Tests.Controllers.Api
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
	internal class CurrenciesApiControllerTests : ControllersUnitTestsBase
	{
		private Mock<ILogger<BaseApiController<Currency>>> loggerMock;
		private Mock<IApiService<Currency>> apiServiceMock;
		private CurrenciesApiController apiController;

		[SetUp]
		public void SetUp()
		{
			this.loggerMock = new Mock<ILogger<BaseApiController<Currency>>>();
			this.apiServiceMock = new Mock<IApiService<Currency>>();

			this.apiController = new CurrenciesApiController(
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
		public async Task CreateCurrency_ShouldReturnCorrectData()
		{
			//Arrange
			var ownerId = Guid.NewGuid();

			var inputModel = new CurrencyInputModel
			{
				Name = "Currency Name",
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
			var actual = (CreatedResult)await this.apiController.CreateCurrency(inputModel);
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
		public async Task CreateCurrency_ShouldReturnBadRequestWithMessage_WhenTheNameExists()
		{
			//Arrange
			var ownerId = Guid.NewGuid();

			var inputModel = new CurrencyInputModel
			{
				Name = "Currency Name",
				OwnerId = ownerId
			};

			this.apiServiceMock
				.Setup(x => x.CreateEntityAsync(inputModel.Name, ownerId))
				.Throws(new ArgumentException(ExceptionMessages.ExistingEntityName));

			//Act
			var actual = (BadRequestObjectResult)await this.apiController.CreateCurrency(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));

				Assert.That(actual.Value, Is.EqualTo(string.Format(
					ExceptionMessages.ExistingUserEntityName,
					"currency",
					inputModel.Name)));
			});
		}

		[Test]
		public async Task CreateCurrency_ShouldReturnBadRequestWithModelErrors_WhenTheInputModelStateIsInvalid()
		{
			//Arrange
			var inputModel = new CurrencyInputModel
			{
				Name = "Currency Name",
				OwnerId = Guid.Empty
			};

			this.apiController.ModelState.AddModelError("id", "invalid id");

			string expectedLogMessage = string.Format(
				LoggerMessages.CreateEntityWithInvalidInputData,
				this.userId,
				"currency");

			//Act
			var actual = (BadRequestObjectResult)await this.apiController.CreateCurrency(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
				Assert.That(actual.Value, Is.EqualTo("invalid id"));
			});

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public void CreateCurrency_ShouldThrowInvalidOperationException_WhenTheOwnerIdIsNull()
		{
			//Arrange
			var inputModel = new CurrencyInputModel
			{
				Name = "Currency Name",
				OwnerId = null
			};

			//Act & Assert
			Assert.That(async () => await this.apiController.CreateCurrency(inputModel),
			Throws.TypeOf<InvalidOperationException>().With.Message
				  .EqualTo(string.Format(ExceptionMessages.NotNullableProperty, inputModel.OwnerId)));
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public async Task DeleteCurrency_ShouldReturnNoContentResponseType_WhenTheCurrencyWasDeleted(bool isUserAdmin)
		{
			//Arrange
			var id = Guid.NewGuid();

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(isUserAdmin);

			//Act
			var actual = (NoContentResult)await this.apiController.DeleteCurrency(id);

			//Assert
			this.apiServiceMock.Verify(
				x => x.DeleteEntityAsync(id, this.userId, isUserAdmin),
				Times.Once);

			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
		}

		[Test]
		public async Task DeleteCurrency_ShouldReturnBadRequestWithModelErrors_WhenTheInputModelStateIsInvalid()
		{
			//Arrange
			var id = Guid.Empty;

			this.apiController.ModelState.AddModelError("id", "invalid id");

			string expectedLogMessage = string.Format(
				LoggerMessages.DeleteEntityWithInvalidInputData,
				this.userId,
				"currency");

			//Act
			var actual = (BadRequestObjectResult)await this.apiController.DeleteCurrency(id);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
				Assert.That(actual.Value, Is.EqualTo("invalid id"));
			});

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task DeleteCurrency_ShouldReturnUnauthorized_WhenTheUserIsUnauthorized()
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
				"currency",
				id);

			//Act
			var actual = (UnauthorizedResult)await this.apiController.DeleteCurrency(id);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task DeleteCurrency_ShouldReturnBadRequest_WhenTheCurrencyDoesNotExist()
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
				"currency");

			//Act
			var actual = (BadRequestResult)await this.apiController.DeleteCurrency(id);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}
	}
}
