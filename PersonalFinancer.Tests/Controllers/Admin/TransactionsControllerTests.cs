namespace PersonalFinancer.Tests.Controllers.Admin
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Logging;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Web.Areas.Admin.Controllers;

	[TestFixture]
	internal class TransactionsControllerTests : ControllersUnitTestsBase
	{
		private Mock<ILogger<TransactionsController>>? logger;
		private TransactionsController? controller;

		[Test]
		public void Constructor_ShouldCallBaseConstructorAndCreateInstance()
		{
			//Arrange
			this.logger = new Mock<ILogger<TransactionsController>>();

			//Act
			this.controller = new TransactionsController(
				this.accountsUpdateServiceMock.Object,
				this.accountsInfoServiceMock.Object,
				this.usersServiceMock.Object,
				this.mapper,
				this.logger.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = new DefaultHttpContext
					{
						User = this.userMock.Object
					}
				}
			};

			//Assert
			Assert.That(this.controller, Is.Not.Null);
		}
	}
}
