namespace PersonalFinancer.Tests.Controllers.Api
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Logging;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Services.Users.Models;
	using PersonalFinancer.Web.Areas.Admin.Controllers.Api;
	using PersonalFinancer.Web.Areas.Admin.Models.User;
	using PersonalFinancer.Web.Models.Api;
	using static PersonalFinancer.Common.Constants.PaginationConstants;

	[TestFixture]
	internal class UsersApiControllerTests : ControllersUnitTestsBase
	{
		private UsersApiController apiController;
		private Mock<ILogger<UsersApiController>> loggerMock;
		
		[SetUp]
		public void SetUp()
		{
			this.loggerMock = new Mock<ILogger<UsersApiController>>();

			this.apiController = new UsersApiController(this.usersServiceMock.Object, this.loggerMock.Object)
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
		[TestCaseSource(nameof(GetTestUsersInfoDTOs))]
		public async Task Get_ShouldReturnCorrectData(UsersInfoDTO expected)
		{
			//Arrange
			var inputModel = new SearchFilterInputModel
			{
				Page = 1,
				Search = null
			};

			this.usersServiceMock
				.Setup(x => x.GetUsersInfoAsync(inputModel.Page, inputModel.Search))
				.ReturnsAsync(expected);

			//Act
			var actual = (OkObjectResult)await this.apiController.Get(inputModel);
			var value = actual.Value as UsersViewModel;

			//Assert
			Assert.That(value, Is.Not.Null);
			Assert.Multiple(() =>
			{
				Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
				Assert.That(value.Users, Is.EquivalentTo(expected.Users));

				AssertPaginationModelIsEqual(
					value.Pagination, 
					"users", 
					UsersPerPage, 
					expected.TotalUsersCount, 
					inputModel.Page);
			});
		}

		[Test]
		public async Task Get_ShouldReturnBadRequest_WhenTheModelStateIsInvalid()
		{
			//Arrange
			var inputModel = new SearchFilterInputModel
			{
				Page = 0,
				Search = null
			};

			this.apiController.ModelState.AddModelError(nameof(inputModel.Page), "Invalid page.");
			string expectedLogMessage = string.Format(LoggerMessages.GetUsersInfoWithInvalidInputData, this.userId);

			//Act
			var actual = (BadRequestResult)await this.apiController.Get(inputModel);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		private static IEnumerable<UsersInfoDTO> GetTestUsersInfoDTOs()
		{
			yield return new UsersInfoDTO
			{
				Users = new UserInfoDTO[]
				{
					new UserInfoDTO
					{
						Id = Guid.NewGuid(),
						UserName = "username",
						FirstName = "First Name",
						LastName = "Last Name",
						Email = "email@mail.com",
						IsAdmin = true
					},
					new UserInfoDTO
					{
						Id = Guid.NewGuid(),
						UserName = "username2",
						FirstName = "First Name 2",
						LastName = "Last Name 2",
						Email = "email2@mail.com",
					},
				},
				TotalUsersCount = 4
			};

			yield return new UsersInfoDTO 
			{ 
				Users = new UserInfoDTO[0], 
				TotalUsersCount = 0 
			};
		}
	}
}
