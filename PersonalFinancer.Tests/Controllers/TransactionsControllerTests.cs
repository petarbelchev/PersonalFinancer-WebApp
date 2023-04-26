using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;

using PersonalFinancer.Data.Enums;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Services.User.Models;
using PersonalFinancer.Web.Controllers;
using PersonalFinancer.Web.Models.Shared;
using PersonalFinancer.Web.Models.Transaction;
using static PersonalFinancer.Data.Constants;

namespace PersonalFinancer.Tests.Controllers
{
	[TestFixture]
	internal class TransactionsControllerTests : ControllersUnitTestsBase
	{
		private TransactionsController controller;

		private TransactionsServiceModel userTransactionsDto = new()
		{
			Transactions = new TransactionTableServiceModel[]
			{
				new TransactionTableServiceModel
				{
					Id = "1",
					Amount = 10,
					AccountCurrencyName = "Currency",
					CategoryName = "Category",
					CreatedOn = DateTime.UtcNow.AddDays(-1),
					Refference = "Refference",
					TransactionType = TransactionType.Expense.ToString()
				},
				new TransactionTableServiceModel
				{
					Id = "2",
					Amount = 15,
					AccountCurrencyName = "Currency2",
					CategoryName = "Category2",
					CreatedOn = DateTime.UtcNow,
					Refference = "Refference2",
					TransactionType = TransactionType.Expense.ToString()
				}
			},
			TotalTransactionsCount = 10
		};
		private UserAccountsAndCategoriesServiceModel userAccAndCatDto = new()
		{
			UserAccounts = new AccountServiceModel[]
			{
				new AccountServiceModel
				{
					Id = "1",
					Name = "Account name 1"
				},
				new AccountServiceModel
				{
					Id = "2",
					Name = "Account name 2"
				}
			},
			UserCategories = new CategoryServiceModel[]
			{
				new CategoryServiceModel
				{
					Id = "1",
					Name = "Category name 1"
				},
				new CategoryServiceModel
				{
					Id = "2",
					Name = "Category name 2"
				}
			}
		};
		private UserAccountsAndCategoriesServiceModel userAccAndCatDtoInitialTransaction = new()
		{
			UserAccounts = new AccountServiceModel[]
			{
				new AccountServiceModel
				{
					Id = "1",
					Name = "Account name 1"
				}
			},
			UserCategories = new CategoryServiceModel[]
			{
				new CategoryServiceModel
				{
					Id = CategoryConstants.InitialBalanceCategoryId,
					Name = CategoryConstants.CategoryInitialBalanceName
				}
			}
		};

		[SetUp]
		public void SetUp()
		{
			this.controller = new TransactionsController(
				this.accountsServiceMock.Object, this.usersServiceMock.Object, this.mapper);

			controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = userMock.Object
				}
			};
		}

		[Test]
		public async Task All_OnGet_ShouldReturnViewModel()
		{
			//Arrange
			usersServiceMock.Setup(x => x
				.GetUserTransactions(this.userId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), 1))
				.ReturnsAsync(this.userTransactionsDto);

			//Act
			ViewResult viewResult = (ViewResult)await controller.All();

			//Assert
			Assert.That(viewResult, Is.Not.Null);

			var model = viewResult.Model as UserTransactionsViewModel;
			Assert.That(model, Is.Not.Null);
			CheckUserTransactionsViewModel(model);
		}

		[Test]
		public async Task All_OnPost_ShouldReturnViewModel()
		{
			//Arrange
			var inputModel = new DateFilterModel
			{
				StartDate = DateTime.UtcNow.AddMonths(-1),
				EndDate = DateTime.UtcNow
			};
			this.userTransactionsDto.StartDate = inputModel.StartDate;
			this.userTransactionsDto.EndDate = inputModel.EndDate;

			usersServiceMock.Setup(x => x
				.GetUserTransactions(this.userId, inputModel.StartDate, inputModel.EndDate, 1))
				.ReturnsAsync(this.userTransactionsDto);

			//Act
			var viewResult = (ViewResult)await controller.All(inputModel);

			//Assert
			Assert.That(viewResult, Is.Not.Null);

			var model = viewResult.Model as UserTransactionsViewModel;
			Assert.That(model, Is.Not.Null);
			Assert.That(model.StartDate, Is.EqualTo(inputModel.StartDate));
			Assert.That(model.EndDate, Is.EqualTo(inputModel.EndDate));
			CheckUserTransactionsViewModel(model);
		}

		[Test]
		public async Task All_OnPost_ShouldReturnViewModelWithErrors_WhenModelIsInvalid()
		{
			//Arrange
			var inputModel = new DateFilterModel
			{
				StartDate = DateTime.UtcNow.AddMonths(-1),
				EndDate = DateTime.UtcNow
			};

			controller.ModelState.AddModelError(string.Empty, "Model is invalid.");

			//Act
			var viewResult = (ViewResult)await controller.All(inputModel);

			//Assert
			Assert.That(viewResult, Is.Not.Null);
			CheckModelStateErrors(viewResult.ViewData.ModelState, string.Empty, "Model is invalid.");

			var model = viewResult.Model as UserTransactionsViewModel;
			Assert.That(model, Is.Not.Null);
			Assert.That(model.StartDate, Is.EqualTo(inputModel.StartDate));
			Assert.That(model.EndDate, Is.EqualTo(inputModel.EndDate));
			Assert.That(model.Transactions.Any(), Is.False);
		}

		[Test]
		public async Task Create_OnGet_ShouldReturnViewModel()
		{
			//Arrange
			this.userAccAndCatDto.OwnerId = this.userId;

			usersServiceMock.Setup(x => x
				.GetUserAccountsAndCategories(this.userId))
				.ReturnsAsync(this.userAccAndCatDto);

			//Act
			ViewResult viewResult = (ViewResult)await controller.Create();

			//Assert
			Assert.That(viewResult, Is.Not.Null);

			var model = viewResult.Model as TransactionFormModel;
			Assert.That(model, Is.Not.Null);
			Assert.That(model.IsInitialBalance, Is.False);
			Assert.That(model.Refference, Is.Null);
			Assert.That(model.OwnerId, Is.EqualTo(userAccAndCatDto.OwnerId));
			Assert.That(model.AccountId, Is.Null);
			Assert.That(model.Amount, Is.EqualTo(0));
			Assert.That(model.CategoryId, Is.Null);

			CheckUserAccountsAndCategories(model, this.userAccAndCatDto);

			Assert.That(model.TransactionTypes.Count(), Is.EqualTo(2));
			Assert.That(model.TransactionTypes[0], Is.EqualTo(TransactionType.Income));
			Assert.That(model.TransactionTypes[1], Is.EqualTo(TransactionType.Expense));
		}

		[Test]
		public async Task Create_OnPost_ShouldReturnViewModelWithErrors_WhenModelIsInvalid()
		{
			//Arrange
			var inputModel = new TransactionFormModel
			{
				Amount = -10,
				CreatedOn = DateTime.UtcNow,
				AccountId = "1",
				CategoryId = "1",
				IsInitialBalance = false,
				OwnerId = this.userId,
				Refference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};

			this.userAccAndCatDto.OwnerId = this.userId;

			usersServiceMock.Setup(x => x
				.GetUserAccountsAndCategories(this.userId))
				.ReturnsAsync(this.userAccAndCatDto);

			controller.ModelState.AddModelError(nameof(inputModel.Amount), "Amount is invalid.");

			//Act
			ViewResult viewResult = (ViewResult)await controller.Create(inputModel);

			//Assert
			Assert.That(viewResult, Is.Not.Null);

			var model = viewResult.Model as TransactionFormModel;
			Assert.That(model, Is.Not.Null);

			CheckTransactionFormModel(model, inputModel);
			
			CheckUserAccountsAndCategories(model, this.userAccAndCatDto);

			Assert.That(model.TransactionTypes.Count(), Is.EqualTo(2));
			Assert.That(model.TransactionTypes[0], Is.EqualTo(TransactionType.Income));
			Assert.That(model.TransactionTypes[1], Is.EqualTo(TransactionType.Expense));

			CheckModelStateErrors(viewResult.ViewData.ModelState,
				nameof(inputModel.Amount), "Amount is invalid.");
		}

		[Test]
		public async Task Create_OnPost_ShouldReturnBadRequest_WhenUserIsNotOwnerInInputModel()
		{
			//Arrange
			var inputModel = new TransactionFormModel
			{
				Amount = -10,
				CreatedOn = DateTime.UtcNow,
				AccountId = "1",
				CategoryId = "1",
				IsInitialBalance = false,
				OwnerId = "owner id",
				Refference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};

			//Act
			var result = (BadRequestResult)await controller.Create(inputModel);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public async Task Create_OnPost_ShouldRedirectToAction_WhenTransactionWasCreated()
		{
			//Arrange
			var inputModel = new TransactionFormModel
			{
				Amount = 10,
				CreatedOn = DateTime.UtcNow,
				AccountId = "1",
				CategoryId = "1",
				IsInitialBalance = false,
				OwnerId = this.userId,
				Refference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};
			string newTransactionId = "new id";

			this.accountsServiceMock.Setup(x => x
				.CreateTransaction(It.Is<TransactionFormShortServiceModel>(m =>
					m.IsInitialBalance == inputModel.IsInitialBalance
					&& m.Amount == inputModel.Amount
					&& m.TransactionType == inputModel.TransactionType
					&& m.OwnerId == inputModel.OwnerId
					&& m.Refference == inputModel.Refference
					&& m.AccountId == inputModel.AccountId
					&& m.CategoryId == inputModel.CategoryId
					&& m.CreatedOn == inputModel.CreatedOn)))
				.ReturnsAsync(newTransactionId);

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (RedirectToActionResult)await this.controller.Create(inputModel);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.ControllerName, Is.Null);
			Assert.That(result.ActionName, Is.EqualTo("TransactionDetails"));
			Assert.That(result.RouteValues, Is.Not.Null);
			CheckRouteValues(result.RouteValues, "id", newTransactionId);
			CheckTempDataMessage(this.controller.TempData, "You create a new transaction successfully!");
		}

		[Test]
		public async Task Create_OnPost_ShouldReturnBadRequest_WhenAccountDoesNotExist()
		{
			//Arrange
			var inputModel = new TransactionFormModel
			{
				Amount = 10,
				CreatedOn = DateTime.UtcNow,
				AccountId = "1",
				CategoryId = "1",
				IsInitialBalance = false,
				OwnerId = this.userId,
				Refference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};

			this.accountsServiceMock.Setup(x => x
				.CreateTransaction(It.Is<TransactionFormShortServiceModel>(m =>
					m.IsInitialBalance == inputModel.IsInitialBalance
					&& m.Amount == inputModel.Amount
					&& m.TransactionType == inputModel.TransactionType
					&& m.OwnerId == inputModel.OwnerId
					&& m.Refference == inputModel.Refference
					&& m.AccountId == inputModel.AccountId
					&& m.CategoryId == inputModel.CategoryId
					&& m.CreatedOn == inputModel.CreatedOn)))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await this.controller.Create(inputModel);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public async Task Delete_ShouldRedirectToHomeIndex_WhenTransactionWasDeletedAndUserIsOwner()
		{
			//Arrange
			string transactionId = "id";
			decimal newBalance = 100;
			string? returnUrl = null;

			this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

			this.accountsServiceMock.Setup(x => x
				.DeleteTransaction(transactionId, this.userId, false))
				.ReturnsAsync(newBalance);

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (RedirectToActionResult)await this.controller.Delete(transactionId, returnUrl);

			//Assert
			Assert.That(result.ControllerName, Is.EqualTo("Home"));
			Assert.That(result.ActionName, Is.EqualTo("Index"));
			CheckTempDataMessage(this.controller.TempData, "Your transaction was successfully deleted!");
		}

		[Test]
		public async Task Delete_ShouldRedirectToHomeIndex_WhenTransactionWasDeletedAndUserIsAdmin()
		{
			//Arrange
			string transactionId = "id";
			decimal newBalance = 100;
			string? returnUrl = null;

			this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(true);

			this.accountsServiceMock.Setup(x => x
				.DeleteTransaction(transactionId, this.userId, true))
				.ReturnsAsync(newBalance);

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (RedirectToActionResult)await this.controller.Delete(transactionId, returnUrl);

			//Assert
			Assert.That(result.ControllerName, Is.EqualTo("Home"));
			Assert.That(result.ActionName, Is.EqualTo("Index"));
			CheckTempDataMessage(this.controller.TempData, "You successfully delete a user's transaction!");
		}

		[Test]
		public async Task Delete_ShouldRedirectToReturnUrl_WhenTransactionWasDeletedAndUserIsOwner()
		{
			//Arrange
			string transactionId = "id";
			decimal newBalance = 100;
			string? returnUrl = "return url";

			this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

			this.accountsServiceMock.Setup(x => x
				.DeleteTransaction(transactionId, this.userId, false))
				.ReturnsAsync(newBalance);

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (LocalRedirectResult)await this.controller.Delete(transactionId, returnUrl);

			//Assert
			Assert.That(result.Url, Is.EqualTo(returnUrl));
			CheckTempDataMessage(this.controller.TempData, "Your transaction was successfully deleted!");
		}

		[Test]
		public async Task Delete_ShouldRedirectToReturnUrl_WhenTransactionWasDeletedAndUserIsAdmin()
		{
			//Arrange
			string transactionId = "id";
			decimal newBalance = 100;
			string? returnUrl = "return url";

			this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(true);

			this.accountsServiceMock.Setup(x => x
				.DeleteTransaction(transactionId, this.userId, true))
				.ReturnsAsync(newBalance);

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (LocalRedirectResult)await this.controller.Delete(transactionId, returnUrl);

			//Assert
			Assert.That(result.Url, Is.EqualTo(returnUrl));
			CheckTempDataMessage(this.controller.TempData, "You successfully delete a user's transaction!");
		}

		[Test]
		public async Task Delete_ShouldReturnUnauthorized_WhenUserIsNotOwnerOrAdmin()
		{
			//Arrange
			string transactionId = "id";
			string? returnUrl = "return url";

			this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

			this.accountsServiceMock.Setup(x => x
				.DeleteTransaction(transactionId, this.userId, false))
				.Throws<ArgumentException>();

			//Act
			var result = (UnauthorizedResult)await this.controller.Delete(transactionId, returnUrl);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(401));
		}

		[Test]
		public async Task Delete_ShouldReturnBadRequest_WhenTransactionDoesNotExist()
		{
			//Arrange
			string transactionId = "id";
			string? returnUrl = "return url";

			this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

			this.accountsServiceMock.Setup(x => x
				.DeleteTransaction(transactionId, this.userId, false))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await this.controller.Delete(transactionId, returnUrl);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public async Task TransactionDetails_ShouldReturnTransactionDetailsModel()
		{
			//Arrange
			string transactionId = "id";
			var serviceReturnDto = new TransactionDetailsServiceModel
			{
				Id = transactionId,
				AccountCurrencyName = "Currency",
				AccountName = "Account name",
				Amount = 10,
				CategoryName = "Category",
				CreatedOn = DateTime.UtcNow.AddDays(-1),
				OwnerId = this.userId,
				Refference = "Refference",
				TransactionType = TransactionType.Income.ToString()
			};

			this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

			this.accountsServiceMock.Setup(x => x
				.GetTransactionDetails(transactionId, this.userId, false))
				.ReturnsAsync(serviceReturnDto);

			//Act
			var result = (ViewResult)await this.controller.TransactionDetails(transactionId);
			var model = result.Model as TransactionDetailsServiceModel;

			//Assert
			Assert.That(model, Is.Not.Null);
			Assert.That(model.Id, Is.EqualTo(serviceReturnDto.Id));
			Assert.That(model.Refference, Is.EqualTo(serviceReturnDto.Refference));
			Assert.That(model.TransactionType, Is.EqualTo(serviceReturnDto.TransactionType));
			Assert.That(model.AccountCurrencyName, Is.EqualTo(serviceReturnDto.AccountCurrencyName));
			Assert.That(model.OwnerId, Is.EqualTo(serviceReturnDto.OwnerId));
			Assert.That(model.AccountName, Is.EqualTo(serviceReturnDto.AccountName));
			Assert.That(model.Amount, Is.EqualTo(serviceReturnDto.Amount));
			Assert.That(model.CategoryName, Is.EqualTo(serviceReturnDto.CategoryName));
			Assert.That(model.CreatedOn, Is.EqualTo(serviceReturnDto.CreatedOn));
		}

		[Test]
		public async Task TransactionDetails_ShouldReturnUnauthorized_WhenUserIsNowOwnerOrAdmin()
		{
			//Arrange
			string transactionId = "id";

			this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

			this.accountsServiceMock.Setup(x => x
				.GetTransactionDetails(transactionId, this.userId, false))
				.Throws<ArgumentException>();

			//Act
			var result = (UnauthorizedResult)await this.controller.TransactionDetails(transactionId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(401));
		}

		[Test]
		public async Task TransactionDetails_ShouldReturnBadRequest_WhenTransactionDoesNotExist()
		{
			//Arrange
			string transactionId = "id";

			this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

			this.accountsServiceMock.Setup(x => x
				.GetTransactionDetails(transactionId, this.userId, false))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await this.controller.TransactionDetails(transactionId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public async Task EditTransaction_OnGet_ShouldReturnTransactionFormModelAndTransactionIsNotInitial()
		{
			//Arrange
			string transactionId = "id";
			var serviceReturnDto = new TransactionFormServiceModel
			{
				Amount = 10,
				CreatedOn = DateTime.UtcNow.AddDays(-1),
				OwnerId = this.userId,
				Refference = "Refference",
				AccountId = "1",
				CategoryId = "1",
				IsInitialBalance = false,
				UserAccounts = this.userAccAndCatDto.UserAccounts,
				UserCategories = this.userAccAndCatDto.UserCategories,
				TransactionType = TransactionType.Income
			};

			this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

			this.accountsServiceMock.Setup(x => x
				.GetTransactionFormData(transactionId))
				.ReturnsAsync(serviceReturnDto);

			//Act
			var result = (ViewResult)await this.controller.EditTransaction(transactionId);
			var model = result.Model as TransactionFormModel;

			//Assert
			Assert.That(model, Is.Not.Null);
			CheckTransactionFormModel(model, serviceReturnDto);

			CheckUserAccountsAndCategories(model, this.userAccAndCatDto);

			Assert.That(model.TransactionTypes.Count(), Is.EqualTo(2));
			Assert.That(model.TransactionTypes[0], Is.EqualTo(TransactionType.Income));
			Assert.That(model.TransactionTypes[1], Is.EqualTo(TransactionType.Expense));
		}

		[Test]
		public async Task EditTransaction_OnGet_ShouldReturnTransactionFormModelAndTransactionIsInitial()
		{
			//Arrange
			string transactionId = "id";
			var serviceReturnDto = new TransactionFormServiceModel
			{
				Amount = 10,
				CreatedOn = DateTime.UtcNow.AddDays(-1),
				OwnerId = this.userId,
				Refference = CategoryConstants.CategoryInitialBalanceName,
				AccountId = this.userAccAndCatDtoInitialTransaction.UserAccounts.First().Id,
				CategoryId = this.userAccAndCatDtoInitialTransaction.UserCategories.First().Id,
				IsInitialBalance = true,
				UserAccounts = this.userAccAndCatDtoInitialTransaction.UserAccounts,
				UserCategories = this.userAccAndCatDtoInitialTransaction.UserCategories,
				TransactionType = TransactionType.Income
			};

			this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

			this.accountsServiceMock.Setup(x => x
				.GetTransactionFormData(transactionId))
				.ReturnsAsync(serviceReturnDto);

			//Act
			var result = (ViewResult)await this.controller.EditTransaction(transactionId);
			var model = result.Model as TransactionFormModel;

			//Assert
			Assert.That(model, Is.Not.Null);

			CheckTransactionFormModel(model, serviceReturnDto);

			CheckUserAccountsAndCategories(model, this.userAccAndCatDtoInitialTransaction);

			Assert.That(model.TransactionTypes.Count(), Is.EqualTo(1));
			Assert.That(model.TransactionTypes[0], Is.EqualTo(serviceReturnDto.TransactionType));
		}
		
		[Test]
		public async Task EditTransaction_OnGet_ShouldReturnUnauthorized_WhenUserIsNowOwnerOrAdmin()
		{
			//Arrange
			string transactionId = "id";
			var serviceReturnDto = new TransactionFormServiceModel
			{
				Amount = 10,
				CreatedOn = DateTime.UtcNow.AddDays(-1),
				OwnerId = "owner id",
				Refference = CategoryConstants.CategoryInitialBalanceName,
				AccountId = "1",
				CategoryId = CategoryConstants.InitialBalanceCategoryId,
				IsInitialBalance = true,
				UserAccounts = new AccountServiceModel[]
				{
					this.userAccAndCatDto.UserAccounts.First()
				},
				UserCategories = new CategoryServiceModel[]
				{
					new CategoryServiceModel
					{
						Id = CategoryConstants.InitialBalanceCategoryId,
						Name = CategoryConstants.CategoryInitialBalanceName
					}
				},
				TransactionType = TransactionType.Income
			};

			this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

			this.accountsServiceMock.Setup(x => x
				.GetTransactionFormData(transactionId))
				.ReturnsAsync(serviceReturnDto);

			//Act
			var result = (UnauthorizedResult)await this.controller.EditTransaction(transactionId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(401));
		}
		
		[Test]
		public async Task EditTransaction_OnGet_ShouldReturnBadRequest_WhenTransactionDoesNotExist()
		{
			//Arrange
			string transactionId = "id";

			this.accountsServiceMock.Setup(x => x
				.GetTransactionFormData(transactionId))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await this.controller.EditTransaction(transactionId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public async Task EditTransaction_OnPost_ShouldReturnViewModelWithErrors_WhenModelIsInvalidAndTransactionIsNotInitial()
		{			
			//Arrange
			string transactionId = "id";
			var inputModel = new TransactionFormModel
			{
				Amount = -10,
				CreatedOn = DateTime.UtcNow,
				AccountId = "1",
				CategoryId = "1",
				IsInitialBalance = false,
				OwnerId = this.userId,
				Refference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};

			this.controller.ModelState.AddModelError(nameof(inputModel.Amount), "Amount is invalid.");
			this.usersServiceMock.Setup(x => x
				.GetUserAccountsAndCategories(this.userId))
				.ReturnsAsync(this.userAccAndCatDto);

			//Act
			var result = (ViewResult)await this.controller.EditTransaction(transactionId, inputModel);

			//Assert
			Assert.That(result, Is.Not.Null);

			CheckModelStateErrors(result.ViewData.ModelState, nameof(inputModel.Amount), "Amount is invalid.");
			
			var model = result.Model as TransactionFormModel;
			Assert.That(model, Is.Not.Null);
			CheckTransactionFormModel(model, inputModel);
			CheckUserAccountsAndCategories(model, this.userAccAndCatDto);
		}
		
		[Test]
		public async Task EditTransaction_OnPost_ShouldReturnViewModelWithErrors_WhenModelIsInvalidAndTransactionIsInitial()
		{			
			//Arrange
			string transactionId = "id";
			var inputModel = new TransactionFormModel
			{
				Amount = -10,
				CreatedOn = DateTime.UtcNow,
				AccountId = "1",
				CategoryId = "1",
				IsInitialBalance = false,
				OwnerId = this.userId,
				Refference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};

			this.controller.ModelState.AddModelError(nameof(inputModel.Amount), "Amount is invalid.");
			this.usersServiceMock.Setup(x => x
				.GetUserAccountsAndCategories(this.userId))
				.ReturnsAsync(this.userAccAndCatDtoInitialTransaction);

			//Act
			var result = (ViewResult)await this.controller.EditTransaction(transactionId, inputModel);

			//Assert
			Assert.That(result, Is.Not.Null);

			CheckModelStateErrors(result.ViewData.ModelState, nameof(inputModel.Amount), "Amount is invalid.");
			
			var model = result.Model as TransactionFormModel;
			Assert.That(model, Is.Not.Null);
			CheckTransactionFormModel(model, inputModel);
			CheckUserAccountsAndCategories(model, this.userAccAndCatDtoInitialTransaction);
		}
		
		[Test]
		public async Task EditTransaction_OnPost_ShouldReturnUnauthorized_WhenUserIsNotOwnerOrAdmin()
		{			
			//Arrange
			string transactionId = "id";
			var inputModel = new TransactionFormModel
			{
				Amount = 10,
				CreatedOn = DateTime.UtcNow,
				AccountId = "1",
				CategoryId = "1",
				IsInitialBalance = false,
				OwnerId = "owner id",
				Refference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};

			this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

			//Act
			var result = (UnauthorizedResult)await this.controller.EditTransaction(transactionId, inputModel);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(401));
		}
		
		[Test]
		public async Task EditTransaction_OnPost_ShouldReturnBadRequest_WhenTransactionDoesNotExist()
		{		
			//Arrange
			string transactionId = "id";
			var inputModel = new TransactionFormModel
			{
				Amount = 10,
				CreatedOn = DateTime.UtcNow,
				AccountId = "1",
				CategoryId = "1",
				IsInitialBalance = false,
				OwnerId = this.userId,
				Refference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};
			
			this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

			this.accountsServiceMock.Setup(x => 
				x.EditTransaction(transactionId, It.Is<TransactionFormShortServiceModel>(m => 
					m.IsInitialBalance == inputModel.IsInitialBalance
					&& m.TransactionType == inputModel.TransactionType
					&& m.CreatedOn == inputModel.CreatedOn
					&& m.Refference == inputModel.Refference
					&& m.OwnerId == inputModel.OwnerId
					&& m.AccountId == inputModel.AccountId
					&& m.CategoryId == inputModel.CategoryId
					&& m.Amount == inputModel.Amount)))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await this.controller.EditTransaction(transactionId, inputModel);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}
		
		[Test]
		public async Task EditTransaction_OnPost_ShouldRedirectToAction_WhenTransactionWasEditedAndUserIsOwner()
		{			
			//Arrange
			string transactionId = "id";
			var inputModel = new TransactionFormModel
			{
				Amount = 10,
				CreatedOn = DateTime.UtcNow,
				AccountId = "1",
				CategoryId = "1",
				IsInitialBalance = false,
				OwnerId = this.userId,
				Refference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};
			
			this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (RedirectToActionResult)await this.controller.EditTransaction(transactionId, inputModel);

			this.accountsServiceMock.Verify(x => 
				x.EditTransaction(transactionId, It.Is<TransactionFormShortServiceModel>(m => 
					m.IsInitialBalance == inputModel.IsInitialBalance
					&& m.TransactionType == inputModel.TransactionType
					&& m.CreatedOn == inputModel.CreatedOn
					&& m.Refference == inputModel.Refference
					&& m.OwnerId == inputModel.OwnerId
					&& m.AccountId == inputModel.AccountId
					&& m.CategoryId == inputModel.CategoryId
					&& m.Amount == inputModel.Amount)), 
				Times.Once);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.ActionName, Is.EqualTo("TransactionDetails"));

			Assert.That(result.RouteValues, Is.Not.Null);
			CheckRouteValues(result.RouteValues, "id", transactionId);

			CheckTempDataMessage(this.controller.TempData, "Your transaction was successfully edited!");
		}
		
		[Test]
		public async Task EditTransaction_OnPost_ShouldRedirectToAction_WhenTransactionWasEditedAndUserIsAdmin()
		{			
			//Arrange
			string transactionId = "id";
			string ownerId = "owner id";
			var inputModel = new TransactionFormModel
			{
				Amount = 10,
				CreatedOn = DateTime.UtcNow,
				AccountId = "1",
				CategoryId = "1",
				IsInitialBalance = false,
				OwnerId = ownerId,
				Refference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};
			
			this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(true);

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			this.accountsServiceMock.Setup(x => x
				.GetOwnerId(inputModel.AccountId))
				.ReturnsAsync(ownerId);

			//Act
			var result = (RedirectToActionResult)await this.controller.EditTransaction(transactionId, inputModel);

			this.accountsServiceMock.Verify(x => 
				x.EditTransaction(transactionId, It.Is<TransactionFormShortServiceModel>(m => 
					m.IsInitialBalance == inputModel.IsInitialBalance
					&& m.TransactionType == inputModel.TransactionType
					&& m.CreatedOn == inputModel.CreatedOn
					&& m.Refference == inputModel.Refference
					&& m.OwnerId == inputModel.OwnerId
					&& m.AccountId == inputModel.AccountId
					&& m.CategoryId == inputModel.CategoryId
					&& m.Amount == inputModel.Amount)), 
				Times.Once);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.ControllerName, Is.EqualTo("Accounts"));
			Assert.That(result.ActionName, Is.EqualTo("AccountDetails"));

			Assert.That(result.RouteValues, Is.Not.Null);
			CheckRouteValues(result.RouteValues, "id", inputModel.AccountId);

			CheckTempDataMessage(this.controller.TempData, "You successfully edit User's transaction!");
		}
		
		private void CheckUserTransactionsViewModel(UserTransactionsViewModel model)
		{
			Assert.That(model.Id, Is.EqualTo(this.userId));

			Assert.That(model.Transactions.Count(),
				Is.EqualTo(userTransactionsDto.Transactions.Count()));

			Assert.That(model.Pagination.TotalElements,
				Is.EqualTo(userTransactionsDto.TotalTransactionsCount));

			for (int i = 0; i < userTransactionsDto.Transactions.Count(); i++)
			{
				Assert.That(model.Transactions.ElementAt(i).Amount,
					Is.EqualTo(userTransactionsDto.Transactions.ElementAt(i).Amount));
				Assert.That(model.Transactions.ElementAt(i).Refference,
					Is.EqualTo(userTransactionsDto.Transactions.ElementAt(i).Refference));
				Assert.That(model.Transactions.ElementAt(i).TransactionType,
					Is.EqualTo(userTransactionsDto.Transactions.ElementAt(i).TransactionType));
				Assert.That(model.Transactions.ElementAt(i).AccountCurrencyName,
					Is.EqualTo(userTransactionsDto.Transactions.ElementAt(i).AccountCurrencyName));
				Assert.That(model.Transactions.ElementAt(i).Id,
					Is.EqualTo(userTransactionsDto.Transactions.ElementAt(i).Id));
				Assert.That(model.Transactions.ElementAt(i).CategoryName,
					Is.EqualTo(userTransactionsDto.Transactions.ElementAt(i).CategoryName));
				Assert.That(model.Transactions.ElementAt(i).CreatedOn,
					Is.EqualTo(userTransactionsDto.Transactions.ElementAt(i).CreatedOn));
			}
		}

		private void CheckUserAccountsAndCategories(
			TransactionFormModel model, 
			UserAccountsAndCategoriesServiceModel serviceModel)
		{
			Assert.That(model.UserCategories.Count(),
				Is.EqualTo(serviceModel.UserCategories.Count()));

			for (int i = 0; i < serviceModel.UserCategories.Count(); i++)
			{
				Assert.That(model.UserCategories.ElementAt(i).Id,
					Is.EqualTo(serviceModel.UserCategories.ElementAt(i).Id));
				Assert.That(model.UserCategories.ElementAt(i).Name,
					Is.EqualTo(serviceModel.UserCategories.ElementAt(i).Name));
			}

			Assert.That(model.UserAccounts.Count(),
				Is.EqualTo(serviceModel.UserAccounts.Count()));

			for (int i = 0; i < serviceModel.UserAccounts.Count(); i++)
			{
				Assert.That(model.UserAccounts.ElementAt(i).Id,
					Is.EqualTo(serviceModel.UserAccounts.ElementAt(i).Id));
				Assert.That(model.UserAccounts.ElementAt(i).Name,
					Is.EqualTo(serviceModel.UserAccounts.ElementAt(i).Name));
			}
		}

		private void CheckTransactionFormModel(TransactionFormModel model, TransactionFormServiceModel serviceModel)
		{
			Assert.That(model.Amount, Is.EqualTo(serviceModel.Amount));
			Assert.That(model.TransactionType, Is.EqualTo(serviceModel.TransactionType));
			Assert.That(model.Refference, Is.EqualTo(serviceModel.Refference));
			Assert.That(model.OwnerId, Is.EqualTo(serviceModel.OwnerId));
			Assert.That(model.CreatedOn, Is.EqualTo(serviceModel.CreatedOn));
			Assert.That(model.AccountId, Is.EqualTo(serviceModel.AccountId));
			Assert.That(model.CategoryId, Is.EqualTo(serviceModel.CategoryId));
			Assert.That(model.IsInitialBalance, Is.EqualTo(serviceModel.IsInitialBalance));
		}
				
		private void CheckTransactionFormModel(TransactionFormModel model, TransactionFormModel inputModel)
		{
			Assert.That(model.Amount, Is.EqualTo(inputModel.Amount));
			Assert.That(model.TransactionType, Is.EqualTo(inputModel.TransactionType));
			Assert.That(model.Refference, Is.EqualTo(inputModel.Refference));
			Assert.That(model.OwnerId, Is.EqualTo(inputModel.OwnerId));
			Assert.That(model.CreatedOn, Is.EqualTo(inputModel.CreatedOn));
			Assert.That(model.AccountId, Is.EqualTo(inputModel.AccountId));
			Assert.That(model.CategoryId, Is.EqualTo(inputModel.CategoryId));
			Assert.That(model.IsInitialBalance, Is.EqualTo(inputModel.IsInitialBalance));
		}
	}
}
