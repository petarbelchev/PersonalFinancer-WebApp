namespace PersonalFinancer.Tests.Services
{
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Services.EmailSender;

	[TestFixture]
	internal class EmailSenderTests
	{
		private Mock<ILogger<EmailSender>> loggerMock;

		[SetUp]
		public void SetUp() 
			=> this.loggerMock = new Mock<ILogger<EmailSender>>();

		[Test]
		public async Task SendEmailAsync_ShouldLogError_WhenTheEmailSendingWasUnsuccessful()
		{
			//Arrange
			var options = Options.Create(new AuthEmailSenderOptions
			{
				EmailSender = "invalid email sender",
				SendGridKey = "invalid send grid key"
			});

			var emailSender = new EmailSender(options, loggerMock.Object);

			string email = "emailreceiver@fakemail.com";
			string subject = "subject";
			string message = "message content";

			//Act
			await emailSender.SendEmailAsync(email, subject, message);

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

		// To run this test, you need to provide a configuration file with real email settings.
		[Test]
		public async Task SendEmailAsync_ShouldLogInformation_WhenTheEmailSendingWasSuccessful()
		{
			//Arrange
			IConfiguration configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build();

			var emailSettings = configuration.GetSection("SendGrid").Get<AuthEmailSenderOptions>();
			var options = Options.Create(emailSettings);
			var emailSender = new EmailSender(options, loggerMock.Object);

			string email = "emailreceiver@mail.com";
			string subject = "subject";
			string message = "message content";

			//Act
			await emailSender.SendEmailAsync(email, subject, message);

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
		public void SendEmailAsync_ShouldThrowException_WhenTheEmailSettingsContainsNullValues(string senderEmail, string sendGridKey)
		{
			//Arrange
			var options = Options.Create(new AuthEmailSenderOptions
			{
				EmailSender = senderEmail,
				SendGridKey = sendGridKey
			});

			var emailSender = new EmailSender(options, loggerMock.Object);

			string email = "emailreceiver@fakemail.com";
			string subject = "subject";
			string message = "message content";

			//Act & Assert
			Assert.That(async () => await emailSender.SendEmailAsync(email, subject, message), 
			Throws.TypeOf<Exception>().With.Message.EqualTo("Null Message Sender Options"));
		}
	}
}
