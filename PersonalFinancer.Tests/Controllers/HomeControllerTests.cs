using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using PersonalFinancer.Data.Models.Enums;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Services.User.Models;
using PersonalFinancer.Web.Controllers;
using PersonalFinancer.Web.Models.Home;
using PersonalFinancer.Web.Models.Shared;
using static PersonalFinancer.Data.Constants;
using static PersonalFinancer.Web.Infrastructure.Constants;

namespace PersonalFinancer.Tests.Controllers
{
	[TestFixture]
	internal class HomeControllerTests : ControllersUnitTestsBase
	{
		private readonly UserDashboardServiceModel expUserDashboard = new ()
		{
			Accounts = new AccountCardServiceModel[]
			{
				new AccountCardServiceModel
				{
					Id = "Test ID",
					Balance = 100,
					CurrencyName = "Test Currency",
					Name = "Test Name",
					OwnerId = "Test Owner Id"
				}
			},
			CurrenciesCashFlow = new CurrencyCashFlowServiceModel[]
			{
				new CurrencyCashFlowServiceModel
				{
					Name = "Test Currency",
					Incomes = 200,
					Expenses = 100
				}
			},
			LastTransactions = new TransactionTableServiceModel[]
			{
				new TransactionTableServiceModel
				{
					Id = "Test Id",
					AccountCurrencyName = "Test Currency",
					Amount = 100,
					CategoryName = "Test Category",
					CreatedOn = DateTime.UtcNow,
					Refference = "Test Refference",
					TransactionType = TransactionType.Expense.ToString()
				}
			}
		};
		private readonly AccountCardServiceModel[] expAccountCard = new AccountCardServiceModel[]
		{
			new AccountCardServiceModel
			{
				Id = "Test Id",
				Balance = 150,
				CurrencyName = "Test Currency Name",
				Name = "Test Name",
				OwnerId = "Test Owner Id"
			}
		};
				
		private HomeController controller;

		[SetUp]
		public void SetUp()
		{
			this.usersServiceMock.Setup(x => x.GetUserDashboardData(
					userId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
				.ReturnsAsync(expUserDashboard);					

			this.usersServiceMock.Setup(x => x.GetUserAccounts(userId))
				.ReturnsAsync(expAccountCard);
			
			controller = new HomeController(this.usersServiceMock.Object);

			controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = userMock.Object
				}
			};
		}

		[Test]
		public async Task Index_ShouldReturnCorrectModel_WhenUserIsAuthenticatedUser()
		{
			//Arrange			
			userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);
			userMock.Setup(x => x.Identity!.IsAuthenticated).Returns(true);

			//Act
			ViewResult viewResult = (ViewResult)await controller.Index();
			var actual = viewResult.Model as UserDashboardViewModel;

			//Assert
			Assert.That(actual, Is.Not.Null);

			Assert.That(actual.Accounts.Count(),
				Is.EqualTo(expUserDashboard.Accounts.Count()));

			for (int i = 0; i < expUserDashboard.Accounts.Count(); i++)
			{
				Assert.That(actual.Accounts.ElementAt(i).Id,
					Is.EqualTo(expUserDashboard.Accounts.ElementAt(i).Id));
				Assert.That(actual.Accounts.ElementAt(i).Name,
					Is.EqualTo(expUserDashboard.Accounts.ElementAt(i).Name));
				Assert.That(actual.Accounts.ElementAt(i).CurrencyName,
					Is.EqualTo(expUserDashboard.Accounts.ElementAt(i).CurrencyName));
				Assert.That(actual.Accounts.ElementAt(i).Balance,
					Is.EqualTo(expUserDashboard.Accounts.ElementAt(i).Balance));
				Assert.That(actual.Accounts.ElementAt(i).OwnerId,
					Is.EqualTo(expUserDashboard.Accounts.ElementAt(i).OwnerId));
			}

			Assert.That(actual.CurrenciesCashFlow.Count(),
				Is.EqualTo(expUserDashboard.CurrenciesCashFlow.Count()));

			for (int i = 0; i < expUserDashboard.CurrenciesCashFlow.Count(); i++)
			{
				Assert.That(actual.CurrenciesCashFlow.ElementAt(i).Name,
					Is.EqualTo(expUserDashboard.CurrenciesCashFlow.ElementAt(i).Name));
				Assert.That(actual.CurrenciesCashFlow.ElementAt(i).Incomes,
					Is.EqualTo(expUserDashboard.CurrenciesCashFlow.ElementAt(i).Incomes));
				Assert.That(actual.CurrenciesCashFlow.ElementAt(i).Expenses,
					Is.EqualTo(expUserDashboard.CurrenciesCashFlow.ElementAt(i).Expenses));
			}

			Assert.That(actual.Transactions.Count(),
				Is.EqualTo(expUserDashboard.LastTransactions.Count()));

			for (int i = 0; i < expUserDashboard.LastTransactions.Count(); i++)
			{
				Assert.That(actual.Transactions.ElementAt(i).Amount,
					Is.EqualTo(expUserDashboard.LastTransactions.ElementAt(i).Amount));
				Assert.That(actual.Transactions.ElementAt(i).Refference,
					Is.EqualTo(expUserDashboard.LastTransactions.ElementAt(i).Refference));
				Assert.That(actual.Transactions.ElementAt(i).TransactionType,
					Is.EqualTo(expUserDashboard.LastTransactions.ElementAt(i).TransactionType));
				Assert.That(actual.Transactions.ElementAt(i).AccountCurrencyName,
					Is.EqualTo(expUserDashboard.LastTransactions.ElementAt(i).AccountCurrencyName));
				Assert.That(actual.Transactions.ElementAt(i).CategoryName,
					Is.EqualTo(expUserDashboard.LastTransactions.ElementAt(i).CategoryName));
				Assert.That(actual.Transactions.ElementAt(i).CreatedOn,
					Is.EqualTo(expUserDashboard.LastTransactions.ElementAt(i).CreatedOn));
				Assert.That(actual.Transactions.ElementAt(i).Id,
					Is.EqualTo(expUserDashboard.LastTransactions.ElementAt(i).Id));
			}
		}

		[Test]
		public async Task Index_ShouldReturnEmptyResult_WhenUserIsNotAuthenticated()
		{
			//Arrange
			userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);
			userMock.Setup(x => x.Identity!.IsAuthenticated).Returns(false);

			//Act
			ViewResult viewResult = (ViewResult)await controller.Index();

			//Assert
			Assert.That(viewResult, Is.Not.Null);
			Assert.That(viewResult.Model, Is.Null);
		}

		[Test]
		public async Task Index_ShouldRedirectToAdminArea_WhenUserIsAdmin()
		{
			//Arrange
			userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(true);

			//Act
			LocalRedirectResult redirectResult =
				(LocalRedirectResult)await controller.Index();

			//Assert
			Assert.That(redirectResult, Is.Not.Null);
			Assert.That(redirectResult.Url, Is.EqualTo("/Admin"));
		}

		[Test]
		public async Task Index_ShouldReturnCorrectViewModel_WhenInputIsValid()
		{
			//Arrange
			var inputModel = new DateFilterModel
			{
				StartDate = DateTime.UtcNow.AddDays(-1),
				EndDate = DateTime.UtcNow
			};

			//Act
			ViewResult viewResult = (ViewResult)await controller.Index(inputModel);

			//Assert
			Assert.That(viewResult, Is.Not.Null);

			var actual = viewResult.Model as UserDashboardViewModel;
			Assert.That(actual, Is.Not.Null);

			Assert.That(actual.Accounts.Count(),
				Is.EqualTo(expUserDashboard.Accounts.Count()));

			for (int i = 0; i < expUserDashboard.Accounts.Count(); i++)
			{
				Assert.That(actual.Accounts.ElementAt(i).Id,
					Is.EqualTo(expUserDashboard.Accounts.ElementAt(i).Id));
				Assert.That(actual.Accounts.ElementAt(i).Name,
					Is.EqualTo(expUserDashboard.Accounts.ElementAt(i).Name));
				Assert.That(actual.Accounts.ElementAt(i).CurrencyName,
					Is.EqualTo(expUserDashboard.Accounts.ElementAt(i).CurrencyName));
				Assert.That(actual.Accounts.ElementAt(i).Balance,
					Is.EqualTo(expUserDashboard.Accounts.ElementAt(i).Balance));
				Assert.That(actual.Accounts.ElementAt(i).OwnerId,
					Is.EqualTo(expUserDashboard.Accounts.ElementAt(i).OwnerId));
			}

			Assert.That(actual.CurrenciesCashFlow.Count(),
				Is.EqualTo(expUserDashboard.CurrenciesCashFlow.Count()));

			for (int i = 0; i < expUserDashboard.CurrenciesCashFlow.Count(); i++)
			{
				Assert.That(actual.CurrenciesCashFlow.ElementAt(i).Name,
					Is.EqualTo(expUserDashboard.CurrenciesCashFlow.ElementAt(i).Name));
				Assert.That(actual.CurrenciesCashFlow.ElementAt(i).Incomes,
					Is.EqualTo(expUserDashboard.CurrenciesCashFlow.ElementAt(i).Incomes));
				Assert.That(actual.CurrenciesCashFlow.ElementAt(i).Expenses,
					Is.EqualTo(expUserDashboard.CurrenciesCashFlow.ElementAt(i).Expenses));
			}

			Assert.That(actual.Transactions.Count(),
				Is.EqualTo(expUserDashboard.LastTransactions.Count()));

			for (int i = 0; i < expUserDashboard.LastTransactions.Count(); i++)
			{
				Assert.That(actual.Transactions.ElementAt(i).Amount,
					Is.EqualTo(expUserDashboard.LastTransactions.ElementAt(i).Amount));
				Assert.That(actual.Transactions.ElementAt(i).Refference,
					Is.EqualTo(expUserDashboard.LastTransactions.ElementAt(i).Refference));
				Assert.That(actual.Transactions.ElementAt(i).TransactionType,
					Is.EqualTo(expUserDashboard.LastTransactions.ElementAt(i).TransactionType));
				Assert.That(actual.Transactions.ElementAt(i).AccountCurrencyName,
					Is.EqualTo(expUserDashboard.LastTransactions.ElementAt(i).AccountCurrencyName));
				Assert.That(actual.Transactions.ElementAt(i).CategoryName,
					Is.EqualTo(expUserDashboard.LastTransactions.ElementAt(i).CategoryName));
				Assert.That(actual.Transactions.ElementAt(i).CreatedOn,
					Is.EqualTo(expUserDashboard.LastTransactions.ElementAt(i).CreatedOn));
				Assert.That(actual.Transactions.ElementAt(i).Id,
					Is.EqualTo(expUserDashboard.LastTransactions.ElementAt(i).Id));
			}
		}

		[Test]
		public async Task Index_ShouldReturnCorrectViewModel_WhenInputIsInvalid()
		{
			//Arrange
			var inputModel = new DateFilterModel
			{
				StartDate = DateTime.UtcNow,
				EndDate = DateTime.UtcNow.AddDays(-1)
			};
			string errorMessage = "error message";

			//Act
			controller.ModelState.AddModelError("error", errorMessage);
			ViewResult viewResult = (ViewResult)await controller.Index(inputModel);
			var actual = viewResult.Model as UserDashboardViewModel;
			var modelStateErrors = viewResult.ViewData.ModelState.Values.First().Errors;
			
			//Assert
			Assert.That(modelStateErrors, Has.Count.EqualTo(1));
			Assert.That(modelStateErrors.First().ErrorMessage, Is.EqualTo(errorMessage));

			Assert.That(actual, Is.Not.Null);
			Assert.That(actual.CurrenciesCashFlow.Count, Is.EqualTo(0));
			Assert.That(actual.Transactions.Count, Is.EqualTo(0));
			Assert.That(actual.StartDate, Is.EqualTo(inputModel.StartDate));
			Assert.That(actual.EndDate, Is.EqualTo(inputModel.EndDate));
			Assert.That(actual.Accounts.Count, Is.EqualTo(expAccountCard.Length));

			for (int i = 0; i < expAccountCard.Length; i++)
			{
				Assert.That(actual.Accounts.ElementAt(i).Id, 
					Is.EqualTo(expAccountCard[i].Id));
				Assert.That(actual.Accounts.ElementAt(i).Name, 
					Is.EqualTo(expAccountCard[i].Name));
				Assert.That(actual.Accounts.ElementAt(i).Balance, 
					Is.EqualTo(expAccountCard[i].Balance));
				Assert.That(actual.Accounts.ElementAt(i).CurrencyName, 
					Is.EqualTo(expAccountCard[i].CurrencyName));
				Assert.That(actual.Accounts.ElementAt(i).OwnerId, 
					Is.EqualTo(expAccountCard[i].OwnerId));
			}
		}

		[Test]
		public void Error_ShouldReturnCorrectErrorImgUrl_WhenStatusCodeIs400()
		{
			//Act
			ViewResult viewResult = (ViewResult)controller.Error(400);
			var viewBagKeys = viewResult.ViewData.Keys;
			var viewBagValues = viewResult.ViewData.Values;

			//Assert
			Assert.That(viewResult.ViewData, Has.Count.EqualTo(1));
			Assert.That(viewBagKeys, Has.Count.EqualTo(1));
			Assert.That(viewBagValues, Has.Count.EqualTo(1)); 
			Assert.That(viewBagValues.First(), 
				Is.EqualTo(HostConstants.BadRequestImgUrl));
		}
		
		[Test]
		public void Error_ShouldReturnCorrectErrorImgUrl_WhenStatusCodeIsNot400()
		{
			//Act
			IActionResult result = controller.Error(500);
			ViewResult viewResult = (ViewResult)result;
			var viewBagKeys = viewResult.ViewData.Keys;
			var viewBagValues = viewResult.ViewData.Values;

			//Assert
			Assert.That(viewResult.ViewData, Has.Count.EqualTo(1));
			Assert.That(viewBagKeys, Has.Count.EqualTo(1));
			Assert.That(viewBagValues, Has.Count.EqualTo(1)); 
			Assert.That(viewBagValues.First(), 
				Is.EqualTo(HostConstants.InternalServerErrorImgUrl));
		}
	}
}
