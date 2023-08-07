namespace PersonalFinancer.Tests.Controllers.Admin
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Logging;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	internal class AccountsControllerTests : ControllersUnitTestsBase
	{
		private Mock<ILogger<Web.Controllers.AccountsController>> loggerMock;
		private Web.Areas.Admin.Controllers.AccountsController controller;

		[SetUp]
		public void SetUp()
		{
			this.loggerMock = new Mock<ILogger<Web.Controllers.AccountsController>>();

			this.controller = new Web.Areas.Admin.Controllers.AccountsController(
				this.accountsUpdateServiceMock.Object,
				this.accountsInfoServiceMock.Object,
				this.usersServiceMock.Object,
				this.mapper,
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
		[TestCase(null)]
		[TestCase("test")]
		public void Index_ShouldReturnView(string? search)
		{
			//Arrange

			//Act
			var result = (ViewResult)this.controller.Index(search);

			//Assert
			Assert.That(result, Is.Not.Null );
			Assert.Multiple(() =>
			{
				Assert.That(result.ViewData.ContainsKey("Search"), Is.True);
				Assert.That(result.ViewData["Search"], Is.EqualTo(search));
			});
		}
	}
}
