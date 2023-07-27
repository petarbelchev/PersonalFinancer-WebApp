namespace PersonalFinancer.Tests.Controllers.Api
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
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
	internal class CategoriesApiControllerTests : ControllersUnitTestsBase
	{
		private Mock<IApiService<Category>> apiServiceMock;
		private CategoriesApiController apiController;

		[SetUp]
		public void SetUp()
		{
			this.apiServiceMock = new Mock<IApiService<Category>>();

			this.apiController = new CategoriesApiController(this.apiServiceMock.Object)
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
		public async Task CreateCategory_ShouldReturnCorrectData()
		{
			//Arrange
			var ownerId = Guid.NewGuid();

			var inputModel = new CategoryInputModel
			{
				Name = "Category Name",
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
			var actual = (CreatedResult)await this.apiController.CreateCategory(inputModel);
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
		public async Task CreateCategory_ShouldReturnBadRequestWithMessage_WhenTheNameExists()
		{
			//Arrange
			var ownerId = Guid.NewGuid();

			var inputModel = new CategoryInputModel
			{
				Name = "Category Name",
				OwnerId = ownerId
			};

			this.apiServiceMock
				.Setup(x => x.CreateEntityAsync(inputModel.Name, ownerId))
				.Throws(new ArgumentException(ExceptionMessages.ExistingEntityName));

			//Act
			var actual = (BadRequestObjectResult)await this.apiController.CreateCategory(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));

				Assert.That(actual.Value, Is.EqualTo(string.Format(
					ExceptionMessages.ExistingUserEntityName,
					"category",
					inputModel.Name)));
			});
		}

		[Test]
		public async Task CreateCategory_ShouldReturnBadRequestWithModelErrors_WhenTheInputModelStateIsInvalid()
		{
			//Arrange
			var inputModel = new CategoryInputModel
			{
				Name = "Category Name",
				OwnerId = Guid.Empty
			};

			this.apiController.ModelState.AddModelError("id", "invalid id");

			//Act
			var actual = (BadRequestObjectResult)await this.apiController.CreateCategory(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
				Assert.That(actual.Value, Is.EqualTo("invalid id"));
			});
		}

		[Test]
		public void CreateCategory_ShouldThrowInvalidOperationException_WhenTheOwnerIdIsNull()
		{
			//Arrange
			var inputModel = new CategoryInputModel
			{
				Name = "Category Name",
				OwnerId = null
			};

			//Act & Assert
			Assert.That(async () => await this.apiController.CreateCategory(inputModel),
			Throws.TypeOf<InvalidOperationException>().With.Message
				  .EqualTo(string.Format(ExceptionMessages.NotNullableProperty, inputModel.OwnerId)));
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public async Task DeleteCategory_ShouldReturnNoContentResponseType_WhenTheCategoryWasDeleted(bool isUserAdmin)
		{
			//Arrange
			var id = Guid.NewGuid();

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(isUserAdmin);

			//Act
			var actual = (NoContentResult)await this.apiController.DeleteCategory(id);

			//Assert
			this.apiServiceMock.Verify(
				x => x.DeleteEntityAsync(id, this.userId, isUserAdmin),
				Times.Once);

			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
		}

		[Test]
		public async Task DeleteCategory_ShouldReturnBadRequestWithModelErrors_WhenTheInputModelStateIsInvalid()
		{
			//Arrange
			var id = Guid.Empty;

			this.apiController.ModelState.AddModelError("id", "invalid id");

			//Act
			var actual = (BadRequestObjectResult)await this.apiController.DeleteCategory(id);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
				Assert.That(actual.Value, Is.EqualTo("invalid id"));
			});
		}

		[Test]
		public async Task DeleteCategory_ShouldReturnUnauthorized_WhenTheUserIsUnauthorized()
		{
			//Arrange
			var id = Guid.Empty;

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.apiServiceMock
				.Setup(x => x.DeleteEntityAsync(id, this.userId, false))
				.Throws<ArgumentException>();

			//Act
			var actual = (UnauthorizedResult)await this.apiController.DeleteCategory(id);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));
		}

		[Test]
		public async Task DeleteCategory_ShouldReturnBadRequest_WhenTheCategoryDoesNotExist()
		{
			//Arrange
			var id = Guid.Empty;

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.apiServiceMock
				.Setup(x => x.DeleteEntityAsync(id, this.userId, false))
				.Throws<InvalidOperationException>();

			//Act
			var actual = (BadRequestResult)await this.apiController.DeleteCategory(id);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
		}
	}
}
