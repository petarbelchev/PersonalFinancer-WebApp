namespace PersonalFinancer.Tests.Controllers.Api
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Logging;
	using MongoDB.Bson;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Data.Models.Enums;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Web.Controllers.Api;
	using PersonalFinancer.Web.Models.Api;
	using PersonalFinancer.Web.Models.Shared;
	using static PersonalFinancer.Common.Constants.PaginationConstants;
	using static PersonalFinancer.Common.Constants.RoleConstants;

	[TestFixture]
	internal class TransactionsApiControllerTests : ControllersUnitTestsBase
	{
		private Mock<ILogger<TransactionsApiController>> loggerMock;
		private TransactionsApiController apiController;

		[SetUp]
		public void SetUp()
		{
			this.loggerMock = new Mock<ILogger<TransactionsApiController>>();

			this.apiController = new TransactionsApiController(
				this.usersServiceMock.Object,
				this.accountsInfoServiceMock.Object,
				this.accountsUpdateServiceMock.Object,
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
		[TestCase(false)]
		[TestCase(true)]
		public async Task DeleteTransaction_ShouldReturnCorrectData(bool isUserAdmin)
		{
			//Arrange
			var transactionId = Guid.NewGuid();

			decimal balance = 100;
			string message = isUserAdmin
				? ResponseMessages.AdminDeletedUserTransaction
				: ResponseMessages.DeletedTransaction;

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(isUserAdmin);

			this.accountsUpdateServiceMock
				.Setup(x => x.DeleteTransactionAsync(transactionId, this.userId, isUserAdmin))
				.ReturnsAsync(balance);

			//Act
			var actual = (OkObjectResult)await this.apiController.DeleteTransaction(transactionId);
			var value = actual.Value as DeleteTransactionOutputModel;

			//Arrange
			Assert.That(value, Is.Not.Null);
			Assert.Multiple(() =>
			{
				Assert.That(value.Balance, Is.EqualTo(balance));
				Assert.That(value.Message, Is.EqualTo(message));
			});
		}

		[Test]
		public async Task DeleteTransaction_ShouldReturnUnauthorized_WhenTheUserIsUnauthorized()
		{
			//Arrange
			var transactionId = Guid.NewGuid();

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsUpdateServiceMock
				.Setup(x => x.DeleteTransactionAsync(transactionId, this.userId, false))
				.Throws<UnauthorizedAccessException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.UnauthorizedTransactionDeletion,
				this.userId,
				transactionId);

			//Act
			var actual = (UnauthorizedResult)await this.apiController.DeleteTransaction(transactionId);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task DeleteTransaction_ShouldReturnBadRequest_WhenTheTransactionDoesNotExist()
		{
			//Arrange
			var transactionId = Guid.NewGuid();

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsUpdateServiceMock
				.Setup(x => x.DeleteTransactionAsync(transactionId, this.userId, false))
				.Throws<InvalidOperationException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.DeleteTransactionWithInvalidInputData,
				this.userId);

			//Act
			var actual = (BadRequestResult)await this.apiController.DeleteTransaction(transactionId);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task DeleteTransaction_ShouldReturnBadRequest_WhenTheModelStateIsInvalid()
		{
			//Arrange
			var transactionId = Guid.Empty;
			this.apiController.ModelState.AddModelError("id", "invalid id");

			string expectedLogMessage = string.Format(
				LoggerMessages.DeleteTransactionWithInvalidInputData,
				this.userId);

			//Act
			var actual = (BadRequestResult)await this.apiController.DeleteTransaction(transactionId);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public async Task GetTransactionDetails_ShouldReturnCorrectData(bool isUserAdmin)
		{
			//Arrange
			var transactionId = Guid.NewGuid();
			var expected = new TransactionDetailsDTO
			{
				Id = transactionId,
				AccountCurrencyName = "BGN",
				AccountName = "Account Name",
				Amount = 100,
				CategoryName = "Category Name",
				CreatedOnLocalTime = DateTime.Now,
				OwnerId = isUserAdmin ? Guid.NewGuid() : this.userId,
				Reference = "Reference",
				TransactionType = TransactionType.Expense.ToString()
			};

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(isUserAdmin);

			this.accountsInfoServiceMock
				.Setup(x => x.GetTransactionDetailsAsync(transactionId, this.userId, isUserAdmin))
				.ReturnsAsync(expected);

			//Act
			var actual = (OkObjectResult)await this.apiController.GetTransactionDetails(transactionId);
			var value = actual.Value as TransactionDetailsDTO;

			//Assert
			Assert.That(value, Is.Not.Null);
			Assert.Multiple(() =>
			{
				Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
				Assert.That(value.ToJson(), Is.EqualTo(expected.ToJson()));
			});
		}

		[Test]
		public async Task GetTransactionDetails_ShouldReturnUnauthorized_WhenTheUserIsUnauthorized()
		{
			//Arrange
			var transactionId = Guid.NewGuid();

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsInfoServiceMock
				.Setup(x => x.GetTransactionDetailsAsync(transactionId, this.userId, false))
				.Throws<UnauthorizedAccessException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.UnauthorizedGetTransactionDetails,
				this.userId,
				transactionId);

			//Act
			var actual = (UnauthorizedResult)await this.apiController.GetTransactionDetails(transactionId);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task GetTransactionDetails_ShouldReturnBadRequest_WhenTheTransactionDoesNotExist()
		{
			//Arrange
			var transactionId = Guid.NewGuid();

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsInfoServiceMock
				.Setup(x => x.GetTransactionDetailsAsync(transactionId, this.userId, false))
				.Throws<InvalidOperationException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.GetTransactionDetailsWithInvalidInputData,
				this.userId);

			//Act
			var actual = (BadRequestResult)await this.apiController.GetTransactionDetails(transactionId);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task GetTransactionDetails_ShouldReturnBadRequest_WhenTheModelStateIsInvalid()
		{
			//Arrange
			var transactionId = Guid.Empty;
			this.apiController.ModelState.AddModelError("id", "invalid id");

			string expectedLogMessage = string.Format(
				LoggerMessages.GetTransactionDetailsWithInvalidInputData,
				this.userId);

			//Act
			var actual = (BadRequestResult)await this.apiController.GetTransactionDetails(transactionId);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task GetUserTransactions_ShouldReturnCorrectData()
		{
			//Arrange
			var inputModel = new UserTransactionsApiInputModel
			{
				Id = this.userId,
				FromLocalTime = DateTime.Now,
				ToLocalTime = DateTime.Now,
				Page = 1
			};

			var filterDTO = this.mapper.Map<TransactionsFilterDTO>(inputModel);

			var transactionsDTO = new TransactionsDTO
			{
				Transactions = new TransactionTableDTO[]
				{
					new TransactionTableDTO
					{
						Id = Guid.NewGuid(),
						Amount = 100,
						CreatedOnLocalTime = DateTime.Now,
						AccountCurrencyName = "BGN",
						CategoryName = "Food",
						Reference = "Lunch",
						TransactionType = TransactionType.Expense.ToString()
					},
					new TransactionTableDTO
					{
						Id = Guid.NewGuid(),
						Amount = 500,
						CreatedOnLocalTime = DateTime.Now,
						AccountCurrencyName = "EUR",
						CategoryName = "Salary",
						Reference = "Salary",
						TransactionType = TransactionType.Income.ToString()
					}
				},
				TotalTransactionsCount = 4
			};

			this.usersServiceMock
				.Setup(x => x.GetUserTransactionsAsync(
					It.Is<TransactionsFilterDTO>(x => ValidateObjectsAreEqual(x, filterDTO))))
				.ReturnsAsync(transactionsDTO);

			//Act
			var actual = (OkObjectResult)await this.apiController.GetUserTransactions(inputModel);
			var value = actual.Value as TransactionsViewModel;

			//Assert
			Assert.That(value, Is.Not.Null);
			Assert.Multiple(() =>
			{
				Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
				Assert.That(value.Transactions, Is.EquivalentTo(transactionsDTO.Transactions));

				AssertPaginationModelIsEqual(
					value.Pagination, 
					"transactions", 
					TransactionsPerPage, 
					transactionsDTO.TotalTransactionsCount, 
					filterDTO.Page);
			});
		}

		[Test]
		public async Task GetUserTransactions_ShouldReturnBadRequest_WhenTheModelStateIsInvalid()
		{
			//Arrange
			var inputModel = new UserTransactionsApiInputModel
			{
				Id = null,
				FromLocalTime = DateTime.Now,
				ToLocalTime = DateTime.Now,
				Page = 1
			};

			this.apiController.ModelState.AddModelError("id", "invalid id");

			string expectedLogMessage = string.Format(
				LoggerMessages.GetUserTransactionsWithInvalidInputData,
				this.userId);

			//Act
			var actual = (BadRequestResult)await this.apiController.GetUserTransactions(inputModel);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task GetUserTransactions_ShouldReturnUnauthorized_WhenTheUserIsUnauthorized()
		{
			//Arrange
			var inputModel = new UserTransactionsApiInputModel
			{
				Id = Guid.NewGuid(),
				FromLocalTime = DateTime.Now,
				ToLocalTime = DateTime.Now,
				Page = 1
			};

			string expectedLogMessage = string.Format(
				LoggerMessages.UnauthorizedGetUserTransactions,
				this.userId,
				inputModel.Id);

			//Act
			var actual = (UnauthorizedResult)await this.apiController.GetUserTransactions(inputModel);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}
	}
}
