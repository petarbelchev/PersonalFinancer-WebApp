namespace PersonalFinancer.Tests.Controllers.Admin
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.SignalR;
	using Microsoft.Extensions.Logging;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Services.Messages;
	using PersonalFinancer.Web.Areas.Admin.Controllers;
	using PersonalFinancer.Web.Hubs;

	[TestFixture]
	internal class MessagesControllerTests : ControllersUnitTestsBase
	{
		private Mock<IMessagesService> messagesServiceMock;
		private Mock<IHubContext<AllMessagesHub>> allMessagesHubMock;
		private Mock<IHubContext<NotificationsHub>> notificationsHubMock;
		private Mock<ILogger<MessagesController>> loggerMock;

		private MessagesController? controller;

		[SetUp]
		public void SetUp()
		{
			this.allMessagesHubMock = new Mock<IHubContext<AllMessagesHub>>();
			this.notificationsHubMock = new Mock<IHubContext<NotificationsHub>>();
			this.messagesServiceMock = new Mock<IMessagesService>();
			this.loggerMock = new Mock<ILogger<MessagesController>>();
		}

		[Test]
		public void Constructor_ShouldCallBaseConstructorAndCreateInstance()
		{
			//Arrange

			//Act
			this.controller = new MessagesController(
				this.messagesServiceMock.Object,
				this.usersServiceMock.Object,
				this.mapper,
				this.allMessagesHubMock.Object,
				this.notificationsHubMock.Object,
				this.loggerMock.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = new DefaultHttpContext
					{
						User = this.userMock.Object,
					}
				}
			};

			//Assert
			Assert.That(this.controller, Is.Not.Null);
		}
	}
}
