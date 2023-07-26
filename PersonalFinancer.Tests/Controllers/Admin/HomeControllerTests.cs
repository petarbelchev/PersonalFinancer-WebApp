namespace PersonalFinancer.Tests.Controllers.Admin
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Web.Areas.Admin.Controllers;
	using PersonalFinancer.Web.Areas.Admin.Models.Home;
	using static PersonalFinancer.Common.Constants.UrlPathConstants;

	[TestFixture]
	internal class HomeControllerTests : ControllersUnitTestsBase
	{
		private HomeController controller;

		[SetUp]
		public void SetUp()
		{
			this.controller = new HomeController(
				this.usersServiceMock.Object,
				this.accountsInfoServiceMock.Object)
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
		public async Task Index_ShouldReturnCorrectViewModel()
		{
			//Arrange
			var expectedViewModel = new AdminDashboardViewModel
			{
				RegisteredUsers = 10,
				CreatedAccounts = 20,
				AdminFullName = "Great Admin",
				AccountsCashFlowEndpoint = ApiAccountsCashFlowEndpoint
			};

			this.usersServiceMock
				.Setup(x => x.UsersCountAsync())
				.ReturnsAsync(expectedViewModel.RegisteredUsers);

			this.usersServiceMock
				.Setup(x => x.UserFullNameAsync(this.userId))
				.ReturnsAsync(expectedViewModel.AdminFullName);

			this.accountsInfoServiceMock
				.Setup(x => x.GetAccountsCountAsync())
				.ReturnsAsync(expectedViewModel.CreatedAccounts);

			//Act
			var result = (ViewResult)await this.controller.Index();

			//Assert
			var viewModel = result.Model as AdminDashboardViewModel;
			Assert.That(viewModel, Is.Not.Null);
			AssertSamePropertiesValuesAreEqual(viewModel, expectedViewModel);
		}
	}
}
