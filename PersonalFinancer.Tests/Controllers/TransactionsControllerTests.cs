﻿namespace PersonalFinancer.Tests.Controllers
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.ViewFeatures;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Data.Models.Enums;
	using PersonalFinancer.Services.Accounts;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Web.Controllers;
	using PersonalFinancer.Web.Models.Transaction;
	using static PersonalFinancer.Data.Constants;

	[TestFixture]
	internal class TransactionsControllerTests : ControllersUnitTestsBase
	{
		private TransactionsController controller;

		private readonly TransactionsServiceModel userTransactionsDto = new()
		{
			Transactions = new TransactionTableServiceModel[]
			{
				new TransactionTableServiceModel
				{
					Id = Guid.NewGuid(),
					Amount = 10,
					AccountCurrencyName = "Currency",
					CategoryName = "Category",
					CreatedOn = DateTime.UtcNow.AddDays(-1),
					Reference = "Reference",
					TransactionType = TransactionType.Expense.ToString()
				},
				new TransactionTableServiceModel
				{
					Id = Guid.NewGuid(),
					Amount = 15,
					AccountCurrencyName = "Currency2",
					CategoryName = "Category2",
					CreatedOn = DateTime.UtcNow,
					Reference = "Reference2",
					TransactionType = TransactionType.Expense.ToString()
				}
			},
			TotalTransactionsCount = 10
		};

		private readonly AccountServiceModel[] expectedAccounts = new AccountServiceModel[]
		{
			new AccountServiceModel
			{
				Id = Guid.NewGuid(),
				Name = "Account name 1"
			},
			new AccountServiceModel
			{
				Id = Guid.NewGuid(),
				Name = "Account name 2"
			}
		};

		private readonly CategoryServiceModel[] expectedCategories = new CategoryServiceModel[]
		{
			new CategoryServiceModel
			{
				Id = Guid.NewGuid(),
				Name = "Category name 1"
			},
			new CategoryServiceModel
			{
				Id = Guid.NewGuid(),
				Name = "Category name 2"
			}
		};

		private readonly AccountTypeServiceModel[] expectedAccountTypes = new AccountTypeServiceModel[]
		{
			new AccountTypeServiceModel
			{
				Id = Guid.NewGuid(),
				Name = "AccType 1"
			},
			new AccountTypeServiceModel
			{
				Id = Guid.NewGuid(),
				Name = "AccType 2"
			}
		};

		private readonly CurrencyServiceModel[] expectedCurrencies = new CurrencyServiceModel[]
		{
			new CurrencyServiceModel
			{
				Id = Guid.NewGuid(),
				Name = "Currency 1"
			},
			new CurrencyServiceModel
			{
				Id = Guid.NewGuid(),
				Name = "Currency 2"
			}
		};

		[SetUp]
		public void SetUp()
		{
			this.accountsInfoServiceMock = new Mock<IAccountsInfoService>();

			this.controller = new TransactionsController(
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

			this.usersServiceMock.Setup(x => x
				.GetUserAccountsDropdownData(this.userId, true))
				.ReturnsAsync(this.expectedAccounts);

			this.usersServiceMock.Setup(x => x
				.GetUserAccountsDropdownData(this.userId, false))
				.ReturnsAsync(this.expectedAccounts);

			this.usersServiceMock.Setup(x => x
				.GetUserCategoriesDropdownData(this.userId, true))
				.ReturnsAsync(this.expectedCategories);

			this.usersServiceMock.Setup(x => x
				.GetUserCategoriesDropdownData(this.userId, false))
				.ReturnsAsync(this.expectedCategories);

			this.usersServiceMock.Setup(x => x
				.GetUserAccountTypesDropdownData(this.userId, true))
				.ReturnsAsync(this.expectedAccountTypes);

			this.usersServiceMock.Setup(x => x
				.GetUserAccountTypesDropdownData(this.userId, false))
				.ReturnsAsync(this.expectedAccountTypes);

			this.usersServiceMock.Setup(x => x
				.GetUserCurrenciesDropdownData(this.userId, true))
				.ReturnsAsync(this.expectedCurrencies);

			this.usersServiceMock.Setup(x => x
				.GetUserCurrenciesDropdownData(this.userId, false))
				.ReturnsAsync(this.expectedCurrencies);
		}

		[Test]
		public async Task All_OnGet_ShouldReturnViewModel()
		{
			//Arrange
			this.accountsInfoServiceMock.Setup(x => x
				.GetUserTransactionsAsync(this.userId, It.IsAny<UserTransactionsInputModel>(), 1))
				.ReturnsAsync(this.userTransactionsDto);

			//Act
			var viewResult = (ViewResult)await this.controller.All();

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				var model = viewResult.Model as UserTransactionsViewModel;
				Assert.That(model, Is.Not.Null);
				this.CheckUserTransactionsViewModel(model!);
			});
		}

		[Test]
		public async Task All_OnPost_ShouldReturnViewModel()
		{
			//Arrange
			var inputModel = new UserTransactionsInputModel
			{
				StartDate = DateTime.UtcNow.AddMonths(-1),
				EndDate = DateTime.UtcNow
			};

			this.accountsInfoServiceMock.Setup(x => x
				.GetUserTransactionsAsync(this.userId, inputModel, 1))
				.ReturnsAsync(this.userTransactionsDto);

			//Act
			var viewResult = (ViewResult)await this.controller.All(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				var model = viewResult.Model as UserTransactionsViewModel;
				Assert.That(model, Is.Not.Null);
				Assert.That(model!.StartDate, Is.EqualTo(inputModel.StartDate));
				Assert.That(model.EndDate, Is.EqualTo(inputModel.EndDate));
				this.CheckUserTransactionsViewModel(model);
			});
		}

		[Test]
		public async Task All_OnPost_ShouldReturnViewModelWithErrors_WhenModelIsInvalid()
		{
			//Arrange
			var inputModel = new UserTransactionsInputModel
			{
				StartDate = DateTime.UtcNow.AddMonths(-1),
				EndDate = DateTime.UtcNow
			};

			this.controller.ModelState.AddModelError(string.Empty, "Model is invalid.");

			//Act
			var viewResult = (ViewResult)await this.controller.All(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);
				CheckModelStateErrors(viewResult.ViewData.ModelState, string.Empty, "Model is invalid.");

				var model = viewResult.Model as UserTransactionsViewModel;
				Assert.That(model, Is.Not.Null);
				Assert.That(model!.StartDate, Is.EqualTo(inputModel.StartDate));
				Assert.That(model.EndDate, Is.EqualTo(inputModel.EndDate));
				Assert.That(model.Transactions.Any(), Is.False);
			});
		}

		[Test]
		public async Task Create_OnGet_ShouldReturnViewModel()
		{
			//Arrange

			//Act
			var viewResult = (ViewResult)await this.controller.Create();

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				var model = viewResult.Model as TransactionFormModel;
				Assert.That(model, Is.Not.Null);
				Assert.That(model!.IsInitialBalance, Is.False);
				Assert.That(model.Reference, Is.Null);
				Assert.That(model.OwnerId, Is.EqualTo(this.userId));
				Assert.That(model.AccountId, Is.Null);
				Assert.That(model.Amount, Is.EqualTo(0));
				Assert.That(model.CategoryId, Is.Null);

				this.CheckUserAccountsAndCategories(model);

				Assert.That(model.TransactionTypes, Has.Length.EqualTo(2));
				Assert.That(model.TransactionTypes[0], Is.EqualTo(TransactionType.Income));
				Assert.That(model.TransactionTypes[1], Is.EqualTo(TransactionType.Expense));
			});
		}

		[Test]
		public async Task Create_OnPost_ShouldReturnViewModelWithErrors_WhenModelIsInvalid()
		{
			//Arrange
			var inputModel = new TransactionFormModel
			{
				Amount = -10,
				CreatedOn = DateTime.UtcNow,
				AccountId = Guid.NewGuid(),
				CategoryId = Guid.NewGuid(),
				IsInitialBalance = false,
				OwnerId = this.userId,
				Reference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};

			this.controller.ModelState.AddModelError(nameof(inputModel.Amount), "Amount is invalid.");

			//Act
			var viewResult = (ViewResult)await this.controller.Create(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				var model = viewResult.Model as TransactionFormModel;
				Assert.That(model, Is.Not.Null);

				CheckTransactionFormModel(model!, inputModel);

				this.CheckUserAccountsAndCategories(model!);

				Assert.That(model!.TransactionTypes, Has.Length.EqualTo(2));
				Assert.That(model.TransactionTypes[0], Is.EqualTo(TransactionType.Income));
				Assert.That(model.TransactionTypes[1], Is.EqualTo(TransactionType.Expense));

				CheckModelStateErrors(viewResult.ViewData.ModelState,
					nameof(inputModel.Amount), "Amount is invalid.");
			});
		}

		[Test]
		public async Task Create_OnPost_ShouldReturnBadRequest_WhenUserIsNotOwnerInInputModel()
		{
			//Arrange
			var inputModel = new TransactionFormModel
			{
				Amount = -10,
				CreatedOn = DateTime.UtcNow,
				AccountId = Guid.NewGuid(),
				CategoryId = Guid.NewGuid(),
				IsInitialBalance = false,
				OwnerId = Guid.NewGuid(),
				Reference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};

			//Act
			var result = (BadRequestResult)await this.controller.Create(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});
		}

		[Test]
		public async Task Create_OnPost_ShouldRedirectToAction_WhenTransactionWasCreated()
		{
			//Arrange
			var inputModel = new TransactionFormModel
			{
				Amount = 10,
				CreatedOn = DateTime.UtcNow,
				AccountId = Guid.NewGuid(),
				CategoryId = Guid.NewGuid(),
				IsInitialBalance = false,
				OwnerId = this.userId,
				Reference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};
			var newTransactionId = Guid.NewGuid();

			this.accountsUpdateServiceMock.Setup(x => x
				.CreateTransactionAsync(inputModel))
				.ReturnsAsync(newTransactionId);

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (RedirectToActionResult)await this.controller.Create(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.ControllerName, Is.Null);
				Assert.That(result.ActionName, Is.EqualTo("TransactionDetails"));
				Assert.That(result.RouteValues, Is.Not.Null);
				CheckRouteValues(result.RouteValues!, "id", newTransactionId);
				CheckTempDataMessage(this.controller.TempData, "You create a new transaction successfully!");
			});
		}

		[Test]
		public async Task Create_OnPost_ShouldReturnBadRequest_WhenAccountDoesNotExist()
		{
			//Arrange
			var inputModel = new TransactionFormModel
			{
				Amount = 10,
				CreatedOn = DateTime.UtcNow,
				AccountId = Guid.NewGuid(),
				CategoryId = Guid.NewGuid(),
				IsInitialBalance = false,
				OwnerId = this.userId,
				Reference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};

			this.accountsUpdateServiceMock.Setup(x => x
				.CreateTransactionAsync(inputModel))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await this.controller.Create(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});
		}

		[Test]
		public async Task Delete_ShouldRedirectToHomeIndex_WhenTransactionWasDeletedAndUserIsOwner()
		{
			//Arrange
			var transactionId = Guid.NewGuid();
			decimal newBalance = 100;
			string? returnUrl = null;

			this.userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(false);

			this.accountsUpdateServiceMock.Setup(x => x
				.DeleteTransactionAsync(transactionId, this.userId, false))
				.ReturnsAsync(newBalance);

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (RedirectToActionResult)await this.controller.Delete(transactionId, returnUrl);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result.ControllerName, Is.EqualTo("Home"));
				Assert.That(result.ActionName, Is.EqualTo("Index"));
				CheckTempDataMessage(this.controller.TempData, "Your transaction was successfully deleted!");
			});
		}

		[Test]
		public async Task Delete_ShouldRedirectToHomeIndex_WhenTransactionWasDeletedAndUserIsAdmin()
		{
			//Arrange
			var transactionId = Guid.NewGuid();
			decimal newBalance = 100;
			string? returnUrl = null;

			this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(true);

			this.accountsUpdateServiceMock.Setup(x => x
				.DeleteTransactionAsync(transactionId, this.userId, true))
				.ReturnsAsync(newBalance);

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (RedirectToActionResult)await this.controller.Delete(transactionId, returnUrl);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result.ControllerName, Is.EqualTo("Home"));
				Assert.That(result.ActionName, Is.EqualTo("Index"));
				CheckTempDataMessage(this.controller.TempData, "You successfully delete a user's transaction!");
			});
		}

		[Test]
		public async Task Delete_ShouldRedirectToReturnUrl_WhenTransactionWasDeletedAndUserIsOwner()
		{
			//Arrange
			var transactionId = Guid.NewGuid();
			decimal newBalance = 100;
			string? returnUrl = "return url";

			this.userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(false);

			this.accountsUpdateServiceMock.Setup(x => x
				.DeleteTransactionAsync(transactionId, this.userId, false))
				.ReturnsAsync(newBalance);

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (LocalRedirectResult)await this.controller.Delete(transactionId, returnUrl);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result.Url, Is.EqualTo(returnUrl));
				CheckTempDataMessage(this.controller.TempData, "Your transaction was successfully deleted!");
			});
		}

		[Test]
		public async Task Delete_ShouldRedirectToReturnUrl_WhenTransactionWasDeletedAndUserIsAdmin()
		{
			//Arrange
			var transactionId = Guid.NewGuid();
			decimal newBalance = 100;
			string? returnUrl = "return url";

			this.userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(true);

			this.accountsUpdateServiceMock.Setup(x => x
				.DeleteTransactionAsync(transactionId, this.userId, true))
				.ReturnsAsync(newBalance);

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (LocalRedirectResult)await this.controller.Delete(transactionId, returnUrl);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result.Url, Is.EqualTo(returnUrl));
				CheckTempDataMessage(this.controller.TempData, "You successfully delete a user's transaction!");
			});
		}

		[Test]
		public async Task Delete_ShouldReturnUnauthorized_WhenUserIsNotOwnerOrAdmin()
		{
			//Arrange
			var transactionId = Guid.NewGuid();
			string? returnUrl = "return url";

			this.userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(false);

			this.accountsUpdateServiceMock.Setup(x => x
				.DeleteTransactionAsync(transactionId, this.userId, false))
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
			var transactionId = Guid.NewGuid();
			string? returnUrl = "return url";

			this.userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(false);

			this.accountsUpdateServiceMock.Setup(x => x
				.DeleteTransactionAsync(transactionId, this.userId, false))
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
			var transactionId = Guid.NewGuid();
			var serviceReturnDto = new TransactionDetailsServiceModel
			{
				Id = transactionId,
				AccountCurrencyName = "Currency",
				AccountName = "Account name",
				Amount = 10,
				CategoryName = "Category",
				CreatedOn = DateTime.UtcNow.AddDays(-1),
				OwnerId = this.userId,
				Reference = "Reference",
				TransactionType = TransactionType.Income.ToString()
			};

			this.userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(false);

			this.accountsInfoServiceMock.Setup(x => x
				.GetTransactionDetailsAsync(transactionId, this.userId, false))
				.ReturnsAsync(serviceReturnDto);

			//Act
			var result = (ViewResult)await this.controller.TransactionDetails(transactionId);
			var model = result.Model as TransactionDetailsServiceModel;

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(model, Is.Not.Null);
				Assert.That(model!.Id, Is.EqualTo(serviceReturnDto.Id));
				Assert.That(model.Reference, Is.EqualTo(serviceReturnDto.Reference));
				Assert.That(model.TransactionType, Is.EqualTo(serviceReturnDto.TransactionType));
				Assert.That(model.AccountCurrencyName, Is.EqualTo(serviceReturnDto.AccountCurrencyName));
				Assert.That(model.OwnerId, Is.EqualTo(serviceReturnDto.OwnerId));
				Assert.That(model.AccountName, Is.EqualTo(serviceReturnDto.AccountName));
				Assert.That(model.Amount, Is.EqualTo(serviceReturnDto.Amount));
				Assert.That(model.CategoryName, Is.EqualTo(serviceReturnDto.CategoryName));
				Assert.That(model.CreatedOn, Is.EqualTo(serviceReturnDto.CreatedOn));
			});
		}

		[Test]
		public async Task TransactionDetails_ShouldReturnUnauthorized_WhenUserIsNowOwnerOrAdmin()
		{
			//Arrange
			var transactionId = Guid.NewGuid();

			this.userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(false);

			this.accountsInfoServiceMock.Setup(x => x
				.GetTransactionDetailsAsync(transactionId, this.userId, false))
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
			var transactionId = Guid.NewGuid();

			this.userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(false);

			this.accountsInfoServiceMock.Setup(x => x
				.GetTransactionDetailsAsync(transactionId, this.userId, false))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await this.controller.TransactionDetails(transactionId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public async Task EditTransaction_OnGet_ShouldReturnTransactionFormModel_WhenTransactionIsNotInitial()
		{
			//Arrange
			var transactionId = Guid.NewGuid();
			var serviceReturnDto = new TransactionFormModel
			{
				Amount = 10,
				CreatedOn = DateTime.UtcNow.AddDays(-1),
				OwnerId = this.userId,
				Reference = "Reference",
				AccountId = Guid.NewGuid(),
				CategoryId = Guid.NewGuid(),
				IsInitialBalance = false,
				UserAccounts = this.expectedAccounts,
				UserCategories = this.expectedCategories,
				TransactionType = TransactionType.Income
			};

			this.userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(false);

			this.accountsInfoServiceMock.Setup(x => x
				.GetTransactionFormDataAsync(transactionId, this.userId))
				.ReturnsAsync(serviceReturnDto);

			//Act
			var result = (ViewResult)await this.controller.EditTransaction(transactionId);
			var model = result.Model as TransactionFormModel;

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(model, Is.Not.Null);
				CheckTransactionFormModel(model!, serviceReturnDto);

				this.CheckUserAccountsAndCategories(model!);

				Assert.That(model!.TransactionTypes, Has.Length.EqualTo(2));
				Assert.That(model.TransactionTypes[0], Is.EqualTo(TransactionType.Income));
				Assert.That(model.TransactionTypes[1], Is.EqualTo(TransactionType.Expense));
			});
		}

		[Test]
		public async Task EditTransaction_OnGet_ShouldReturnBadRequest_WhenTransactionDoesNotExistOrIsInitial()
		{
			//Arrange
			var transactionId = Guid.NewGuid();

			this.userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(false);

			this.accountsInfoServiceMock.Setup(x => x
				.GetTransactionFormDataAsync(transactionId, this.userId))
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
			var transactionId = Guid.NewGuid();
			var inputModel = new TransactionFormModel
			{
				Amount = -10,
				CreatedOn = DateTime.UtcNow,
				AccountId = Guid.NewGuid(),
				CategoryId = Guid.NewGuid(),
				IsInitialBalance = false,
				OwnerId = this.userId,
				Reference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};

			this.controller.ModelState.AddModelError(nameof(inputModel.Amount), "Amount is invalid.");

			//Act
			var result = (ViewResult)await this.controller.EditTransaction(transactionId, inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);

				CheckModelStateErrors(result.ViewData.ModelState, nameof(inputModel.Amount), "Amount is invalid.");

				var model = result.Model as TransactionFormModel;
				Assert.That(model, Is.Not.Null);
				CheckTransactionFormModel(model!, inputModel);
				this.CheckUserAccountsAndCategories(model!);
			});
		}

		[Test]
		public async Task EditTransaction_OnPost_ShouldThrowException_WhenTransactionIsInitial()
		{
			//Arrange
			var transactionId = Guid.NewGuid();
			var inputModel = new TransactionFormModel
			{
				Amount = 10,
				CreatedOn = DateTime.UtcNow,
				AccountId = Guid.NewGuid(),
				CategoryId = Guid.NewGuid(),
				IsInitialBalance = true,
				OwnerId = this.userId,
				Reference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};
			
			this.accountsUpdateServiceMock.Setup(x => x
				.EditTransactionAsync(transactionId, inputModel))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await this.controller.EditTransaction(transactionId, inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});
		}

		[Test]
		public async Task EditTransaction_OnPost_ShouldReturnBadRequest_WhenUserIsNotOwnerOrAdmin()
		{
			//Arrange
			var transactionId = Guid.NewGuid();
			var inputModel = new TransactionFormModel
			{
				Amount = 10,
				CreatedOn = DateTime.UtcNow,
				AccountId = Guid.NewGuid(),
				CategoryId = Guid.NewGuid(),
				IsInitialBalance = false,
				OwnerId = Guid.NewGuid(),
				Reference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};

			this.userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(false);

			//Act
			var result = (BadRequestResult)await this.controller.EditTransaction(transactionId, inputModel);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public async Task EditTransaction_OnPost_ShouldReturnBadRequest_WhenTransactionDoesNotExist()
		{
			//Arrange
			var transactionId = Guid.NewGuid();
			var inputModel = new TransactionFormModel
			{
				Amount = 10,
				CreatedOn = DateTime.UtcNow,
				AccountId = Guid.NewGuid(),
				CategoryId = Guid.NewGuid(),
				IsInitialBalance = false,
				OwnerId = this.userId,
				Reference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};

			this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

			this.accountsUpdateServiceMock.Setup(x => x
				.EditTransactionAsync(transactionId, inputModel))
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
			var transactionId = Guid.NewGuid();
			var inputModel = new TransactionFormModel
			{
				Amount = 10,
				CreatedOn = DateTime.UtcNow,
				AccountId = Guid.NewGuid(),
				CategoryId = Guid.NewGuid(),
				IsInitialBalance = false,
				OwnerId = this.userId,
				Reference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};

			this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (RedirectToActionResult)await this.controller.EditTransaction(transactionId, inputModel);

			this.accountsUpdateServiceMock.Verify(x => x
				.EditTransactionAsync(transactionId, inputModel),
				Times.Once);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.ActionName, Is.EqualTo("TransactionDetails"));

				Assert.That(result.RouteValues, Is.Not.Null);
				CheckRouteValues(result.RouteValues!, "id", transactionId);

				CheckTempDataMessage(this.controller.TempData, "Your transaction was successfully edited!");
			});
		}

		[Test]
		public async Task EditTransaction_OnPost_ShouldRedirectToAction_WhenTransactionWasEditedAndUserIsAdmin()
		{
			//Arrange
			var transactionId = Guid.NewGuid();
			var ownerId = Guid.NewGuid();
			var accountId = Guid.NewGuid();
			var inputModel = new TransactionFormModel
			{
				Amount = 10,
				CreatedOn = DateTime.UtcNow,
				AccountId = accountId,
				CategoryId = Guid.NewGuid(),
				IsInitialBalance = false,
				OwnerId = ownerId,
				Reference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};

			this.userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(true);

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			this.accountsInfoServiceMock.Setup(x => x
				.GetAccountOwnerIdAsync(accountId))
				.ReturnsAsync(ownerId);

			//Act
			var result = (RedirectToActionResult)await this.controller.EditTransaction(transactionId, inputModel);

			this.accountsUpdateServiceMock.Verify(x => x
				.EditTransactionAsync(transactionId, inputModel),
				Times.Once);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.ActionName, Is.EqualTo("TransactionDetails"));

				Assert.That(result.RouteValues, Is.Not.Null);
				CheckRouteValues(result.RouteValues!, "id", transactionId);
			});

			CheckTempDataMessage(this.controller.TempData, "You successfully edit User's transaction!");
		}

		private void CheckUserTransactionsViewModel(UserTransactionsViewModel model)
		{
			Assert.That(model.Id, Is.EqualTo(this.userId));

			Assert.That(model.Transactions.Count(),
				Is.EqualTo(this.userTransactionsDto.Transactions.Count()));

			Assert.That(model.Pagination.TotalElements,
				Is.EqualTo(this.userTransactionsDto.TotalTransactionsCount));

			for (int i = 0; i < this.userTransactionsDto.Transactions.Count(); i++)
			{
				Assert.That(model.Transactions.ElementAt(i).Amount,
					Is.EqualTo(this.userTransactionsDto.Transactions.ElementAt(i).Amount));
				Assert.That(model.Transactions.ElementAt(i).Reference,
					Is.EqualTo(this.userTransactionsDto.Transactions.ElementAt(i).Reference));
				Assert.That(model.Transactions.ElementAt(i).TransactionType,
					Is.EqualTo(this.userTransactionsDto.Transactions.ElementAt(i).TransactionType));
				Assert.That(model.Transactions.ElementAt(i).AccountCurrencyName,
					Is.EqualTo(this.userTransactionsDto.Transactions.ElementAt(i).AccountCurrencyName));
				Assert.That(model.Transactions.ElementAt(i).Id,
					Is.EqualTo(this.userTransactionsDto.Transactions.ElementAt(i).Id));
				Assert.That(model.Transactions.ElementAt(i).CategoryName,
					Is.EqualTo(this.userTransactionsDto.Transactions.ElementAt(i).CategoryName));
				Assert.That(model.Transactions.ElementAt(i).CreatedOn,
					Is.EqualTo(this.userTransactionsDto.Transactions.ElementAt(i).CreatedOn));
			}
		}

		private void CheckUserAccountsAndCategories(TransactionFormModel model)
		{
			Assert.That(model.UserCategories.Count(),
				Is.EqualTo(this.expectedCategories.Length));

			for (int i = 0; i < this.expectedCategories.Length; i++)
			{
				Assert.That(model.UserCategories.ElementAt(i).Id,
					Is.EqualTo(this.expectedCategories[i].Id));
				Assert.That(model.UserCategories.ElementAt(i).Name,
					Is.EqualTo(this.expectedCategories[i].Name));
			}

			Assert.That(model.UserAccounts.Count(),
				Is.EqualTo(this.expectedAccounts.Length));

			for (int i = 0; i < this.expectedAccounts.Length; i++)
			{
				Assert.That(model.UserAccounts.ElementAt(i).Id,
					Is.EqualTo(this.expectedAccounts[i].Id));
				Assert.That(model.UserAccounts.ElementAt(i).Name,
					Is.EqualTo(this.expectedAccounts[i].Name));
			}
		}

		private static void CheckTransactionFormModel(TransactionFormModel model, TransactionFormModel inputModel)
		{
			Assert.That(model.Amount, Is.EqualTo(inputModel.Amount));
			Assert.That(model.TransactionType, Is.EqualTo(inputModel.TransactionType));
			Assert.That(model.Reference, Is.EqualTo(inputModel.Reference));
			Assert.That(model.OwnerId, Is.EqualTo(inputModel.OwnerId));
			Assert.That(model.CreatedOn, Is.EqualTo(inputModel.CreatedOn));
			Assert.That(model.AccountId, Is.EqualTo(inputModel.AccountId));
			Assert.That(model.CategoryId, Is.EqualTo(inputModel.CategoryId));
			Assert.That(model.IsInitialBalance, Is.EqualTo(inputModel.IsInitialBalance));
		}
	}
}
