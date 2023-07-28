namespace PersonalFinancer.Tests.Controllers.Api
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Services.Users.Models;
	using PersonalFinancer.Web.Controllers.Api;
	using PersonalFinancer.Web.Models.User;
	using static PersonalFinancer.Common.Constants.PaginationConstants;

	[TestFixture]
	internal class UsersApiControllerTests : ControllersUnitTestsBase
	{
		private UsersApiController apiController;
		
		[SetUp]
		public void SetUp()
		{
			this.apiController = new UsersApiController(this.usersServiceMock.Object)
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
		public async Task AllUsers_ShouldReturnCorrectData(UsersInfoDTO expected)
		{
			//Arrange
			int page = 1;

			this.usersServiceMock
				.Setup(x => x.GetUsersInfoAsync(page))
				.ReturnsAsync(expected);

			//Act
			var actual = (OkObjectResult)await this.apiController.AllUsers(page);
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
					page);
			});
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
