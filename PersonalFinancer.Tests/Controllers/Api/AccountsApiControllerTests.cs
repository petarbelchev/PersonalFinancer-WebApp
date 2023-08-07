namespace PersonalFinancer.Tests.Controllers.Api
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Logging;
	using MongoDB.Bson;
	using MongoDB.Driver.Linq;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Data.Models.Enums;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Web.Controllers.Api;
	using PersonalFinancer.Web.Models.Account;
	using PersonalFinancer.Web.Models.Api;
	using PersonalFinancer.Web.Models.Shared;
	using static PersonalFinancer.Common.Constants.PaginationConstants;
	using static PersonalFinancer.Common.Constants.RoleConstants;

	[TestFixture]
	internal class AccountsApiControllerTests : ControllersUnitTestsBase
	{
		private Mock<ILogger<AccountsApiController>> loggerMock;
		private AccountsApiController apiController;

		[SetUp]
		public void SetUp()
		{
			this.loggerMock = new Mock<ILogger<AccountsApiController>>();

			this.apiController = new AccountsApiController(
				this.accountsInfoServiceMock.Object,
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
		[TestCaseSource(nameof(GetTestAccountsCardsDTOs))]
		public async Task GetAccounts_ShouldReturnCorrectData(AccountsCardsDTO expected)
		{
			//Arrange
			var inputModel = new SearchFilterInputModel
			{
				Page = 1,
				Search = null
			};

			this.accountsInfoServiceMock
				.Setup(x => x.GetAccountsCardsDataAsync(inputModel.Page, inputModel.Search))
				.ReturnsAsync(expected);

			//Act
			var actual = (OkObjectResult)await this.apiController.GetAccounts(inputModel);
			var value = actual.Value as UsersAccountsCardsViewModel;

			//Assert
			Assert.That(value, Is.Not.Null);
			Assert.Multiple(() =>
			{
				Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
				Assert.That(value.AccountsCards, Is.EquivalentTo(expected.Accounts));
				AssertPaginationModelIsEqual(value.Pagination, "accounts", AccountsPerPage, expected.TotalAccountsCount, inputModel.Page);
			});
		}

		[Test]
		public async Task GetAccounts_ShouldReturnBadRequest_WhenTheModelStateIsInvalid()
		{
			//Arrange
			var inputModel = new SearchFilterInputModel
			{
				Page = 0,
				Search = null
			};

			this.apiController.ModelState.AddModelError(nameof(inputModel.Page), "Invalid page.");
			string expectedLogMessage = string.Format(LoggerMessages.GetAccountsInfoWithInvalidInputData, this.userId);

			//Act
			var actual = (BadRequestResult)await this.apiController.GetAccounts(inputModel);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		[TestCaseSource(nameof(GetTestCurrencyCashFlowDTOs))]
		public async Task GetAccountsCashFlow_ShouldReturnCorrectData(IEnumerable<CurrencyCashFlowDTO> expected)
		{
			//Arrange
			this.accountsInfoServiceMock
				.Setup(x => x.GetCashFlowByCurrenciesAsync())
				.ReturnsAsync(expected);

			//Act
			var actual = (OkObjectResult)await this.apiController.GetAccountsCashFlow();
			var value = actual.Value as IEnumerable<CurrencyCashFlowDTO>;

			//Assert
			Assert.That(value, Is.Not.Null);
			Assert.Multiple(() =>
			{
				Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status200OK));

				for (int i = 0; i < expected.Count(); i++)
				{
					Assert.That(value!.ElementAt(i).ToJson(), Is.EqualTo(expected.ElementAt(i).ToJson()));
				}
			});
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public async Task GetAccountTransactions_ShouldReturnCorrectData(bool isUserAdmin)
		{
			//Arrange
			Guid ownerId = isUserAdmin ? Guid.NewGuid() : this.userId;

			var inputModel = new AccountTransactionsInputModel
			{
				Id = Guid.NewGuid(),
				FromLocalTime = DateTime.Now.AddMonths(-1),
				ToLocalTime = DateTime.Now,
				OwnerId = ownerId,
				Page = 1
			};

			var filterDTO = this.mapper.Map<AccountTransactionsFilterDTO>(inputModel);

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

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(isUserAdmin);

			this.accountsInfoServiceMock
				.Setup(x => x.GetAccountTransactionsAsync(
					It.Is<AccountTransactionsFilterDTO>(x => ValidateObjectsAreEqual(x, filterDTO))))
				.ReturnsAsync(transactionsDTO);

			//Act
			var actual = (OkObjectResult)await this.apiController.GetAccountTransactions(inputModel);
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
		public async Task GetAccountTransactions_ShouldReturnBadRequest_WhenTheModelStateIsInvalid()
		{
			//Arrange
			var inputModel = new AccountTransactionsInputModel
			{
				Id = null,
				FromLocalTime = DateTime.Now.AddMonths(-1),
				ToLocalTime = DateTime.Now,
				OwnerId = this.userId,
				Page = 1
			};

			this.apiController.ModelState.AddModelError("id", "invalid id");

			string expectedLogMessage = string.Format(
				LoggerMessages.GetAccountTransactionsWithInvalidInputData,
				this.userId);

			//Act
			var actual = (BadRequestResult)await this.apiController.GetAccountTransactions(inputModel);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task GetAccountTransactions_ShouldReturnUnauthorized_WhenTheUserIsUnauthorized()
		{
			//Arrange
			var inputModel = new AccountTransactionsInputModel
			{
				Id = Guid.NewGuid(),
				FromLocalTime = DateTime.Now.AddMonths(-1),
				ToLocalTime = DateTime.Now,
				OwnerId = Guid.NewGuid(),
				Page = 1
			};

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			string expectedLogMessage = string.Format(
				LoggerMessages.UnauthorizedGetAccountTransactions,
				this.userId,
				inputModel.Id);

			//Act
			var actual = (UnauthorizedResult)await this.apiController.GetAccountTransactions(inputModel);

			//Assert
			Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		private static IEnumerable<AccountsCardsDTO> GetTestAccountsCardsDTOs()
		{
			yield return new AccountsCardsDTO
			{
				Accounts = new AccountCardDTO[]
				{
					new AccountCardDTO
					{
						Id = Guid.NewGuid(),
						Balance = 100,
						CurrencyName = "BGN",
						Name = "Account Name",
						OwnerId = Guid.NewGuid(),
					}
				},
				TotalAccountsCount = 1
			};

			yield return new AccountsCardsDTO
			{
				Accounts = new AccountCardDTO[0],
				TotalAccountsCount = 0
			};
		}

		private static IEnumerable<IEnumerable<CurrencyCashFlowDTO>> GetTestCurrencyCashFlowDTOs()
		{
			yield return new CurrencyCashFlowDTO[]
			{
				new CurrencyCashFlowDTO
				{
					Name = "BGN",
					Incomes = 1000,
					Expenses = 500
				},
				new CurrencyCashFlowDTO
				{
					Name = "EUR",
					Incomes = 1200,
					Expenses = 600
				}
			};

			yield return new CurrencyCashFlowDTO[0];
		}
	}
}
