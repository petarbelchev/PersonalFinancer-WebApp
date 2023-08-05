namespace PersonalFinancer.Tests.Services
{
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Services.EmailSender;
	using SendGrid;
	using SendGrid.Helpers.Mail;
	using System.Net;

	[TestFixture]
	internal class SendGridEmailSenderTests
	{
		private string emailReceiver = "email.receiver@fake.mail.com";
		private string emailSubject = "subject";
		private string emailContent = "message content";

		private readonly AuthEmailSenderOptions fakeEmailSenderOptions = new()
		{
			EmailSender = "fake email sender",
			SendGridKey = "fake send grid key"
		};

		private Mock<ISendGridClient> sendGridClientMock;
		private IOptions<AuthEmailSenderOptions> options;
		private Mock<ILogger<SendGridEmailSender>> loggerMock;
		private SendGridEmailSender emailSender;

		[SetUp]
		public void SetUp()
		{
			this.sendGridClientMock = new Mock<ISendGridClient>();
			this.options = Options.Create(this.fakeEmailSenderOptions);
			this.loggerMock = new Mock<ILogger<SendGridEmailSender>>();

			this.emailSender = new SendGridEmailSender(
				this.sendGridClientMock.Object,
				this.options,
				this.loggerMock.Object);
		}

		[Test]
		public async Task SendEmailAsync_ShouldLogError_WhenTheEmailSendingWasUnsuccessful()
		{
			//Arrange
			var responseMock = new Mock<Response>(HttpStatusCode.BadRequest, null, null);

			this.sendGridClientMock
				.Setup(x => x.SendEmailAsync(
					It.Is<SendGridMessage>(msg =>
						msg.From.Email == fakeEmailSenderOptions.EmailSender &&
						msg.Subject == emailSubject &&
						msg.HtmlContent == emailContent),
					It.IsAny<CancellationToken>()))
				.ReturnsAsync(responseMock.Object);

			//Act
			await this.emailSender.SendEmailAsync(emailReceiver, emailSubject, emailContent);

			//Assert
			loggerMock.Verify(
				x => x.Log(
					It.Is<LogLevel>(l => l == LogLevel.Information),
					It.IsAny<EventId>(),
					It.IsAny<It.IsAnyType>(),
					It.IsAny<Exception>(),
					(Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
				Times.Never);

			loggerMock.Verify(
				x => x.Log(
					It.Is<LogLevel>(l => l == LogLevel.Error),
					It.IsAny<EventId>(),
					It.IsAny<It.IsAnyType>(),
					It.IsAny<Exception>(),
					(Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
				Times.Once);
		}

		[Test]
		public async Task SendEmailAsync_ShouldLogInformation_WhenTheEmailSendingWasSuccessful()
		{
			//Arrange
			var responseMock = new Mock<Response>(HttpStatusCode.OK, null, null);

			this.sendGridClientMock
				.Setup(x => x.SendEmailAsync(
					It.Is<SendGridMessage>(msg =>
						msg.From.Email == fakeEmailSenderOptions.EmailSender &&
						msg.Subject == emailSubject &&
						msg.HtmlContent == emailContent),
					It.IsAny<CancellationToken>()))
				.ReturnsAsync(responseMock.Object);

			//Act
			await this.emailSender.SendEmailAsync(emailReceiver, emailSubject, emailContent);

			//Assert
			loggerMock.Verify(
				x => x.Log(
					It.Is<LogLevel>(l => l == LogLevel.Information),
					It.IsAny<EventId>(),
					It.IsAny<It.IsAnyType>(),
					It.IsAny<Exception>(),
					(Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
				Times.Once);

			loggerMock.Verify(
				x => x.Log(
					It.Is<LogLevel>(l => l == LogLevel.Error),
					It.IsAny<EventId>(),
					It.IsAny<It.IsAnyType>(),
					It.IsAny<Exception>(),
					(Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
				Times.Never);
		}

		[Test]
		[TestCase("invalid but not null", null)]
		[TestCase(null, "invalid but not null")]
		public void SendEmailAsync_ShouldThrowException_WhenTheEmailSettingsContainsNullValues(string emailSender, string sendGridKey)
		{
			//Arrange
			this.fakeEmailSenderOptions.EmailSender = emailSender;
			this.fakeEmailSenderOptions.SendGridKey = sendGridKey;

			//Act & Assert
			Assert.That(async () => await this.emailSender.SendEmailAsync(emailReceiver, emailSubject, emailContent),
			Throws.TypeOf<Exception>().With.Message.EqualTo("Null Message Sender Options"));
		}
	}
}
