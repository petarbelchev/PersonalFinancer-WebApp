namespace PersonalFinancer.Tests.Controllers.Admin
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using NUnit.Framework;
	using PersonalFinancer.Web.Areas.Admin.Controllers;

	[TestFixture]
	internal class AccountsControllerTests : ControllersUnitTestsBase
	{
		private AccountsController controller;

		[SetUp]
		public void SetUp()
		{
			this.controller = new AccountsController(
				this.accountsUpdateServiceMock.Object,
				this.accountsInfoServiceMock.Object,
				this.usersServiceMock.Object,
				this.mapper)
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
		public void Index_ShouldReturnView()
		{
			//Arrange

			//Act
			var result = this.controller.Index();

			//Assert
			Assert.That(result, Is.Not.Null );
		}
	}
}
