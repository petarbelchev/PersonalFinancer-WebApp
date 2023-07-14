﻿namespace PersonalFinancer.Tests.Controllers
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.ViewFeatures;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Data.Models.Enums;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Services.User.Models;
	using PersonalFinancer.Web.Controllers;
	using PersonalFinancer.Web.Models.Transaction;
	using static PersonalFinancer.Common.Constants.RoleConstants;

	[TestFixture]
	internal class TransactionsControllerTests : ControllersUnitTestsBase
	{
		private static readonly AccountDropdownDTO[] expAccountsDropdown = new AccountDropdownDTO[]
		{
			new AccountDropdownDTO
			{
				Id = Guid.NewGuid(),
				Name = "Account name 1"
			},
			new AccountDropdownDTO
			{
				Id = Guid.NewGuid(),
				Name = "Account name 2"
			}
		};
		private static readonly CategoryDropdownDTO[] expCategoriesDropdown = new CategoryDropdownDTO[]
		{
			new CategoryDropdownDTO
			{
				Id = Guid.NewGuid(),
				Name = "Category name 1"
			},
			new CategoryDropdownDTO
			{
				Id = Guid.NewGuid(),
				Name = "Category name 2"
			}
		};
		private static readonly AccountTypeDropdownDTO[] expAccountTypesDropdown = new AccountTypeDropdownDTO[]
		{
			new AccountTypeDropdownDTO
			{
				Id = Guid.NewGuid(),
				Name = "AccType 1"
			},
			new AccountTypeDropdownDTO
			{
				Id = Guid.NewGuid(),
				Name = "AccType 2"
			}
		};
		private static readonly CurrencyDropdownDTO[] expCurrenciesDropdown = new CurrencyDropdownDTO[]
		{
			new CurrencyDropdownDTO
			{
				Id = Guid.NewGuid(),
				Name = "Currency 1"
			},
			new CurrencyDropdownDTO
			{
				Id = Guid.NewGuid(),
				Name = "Currency 2"
			}
		};
		private static readonly AccountsAndCategoriesDropdownDTO expAccountsAndCategoriesDropdowns = new() 
		{
			OwnerAccounts = expAccountsDropdown,
			OwnerCategories = expCategoriesDropdown
		};
		private static readonly AccountTypesAndCurrenciesDropdownDTO expAccountTypesAndCurrenciesDropdowns = new()
		{
			OwnerAccountTypes = expAccountTypesDropdown,
			OwnerCurrencies = expCurrenciesDropdown
		};
		private static readonly TransactionsDTO expTransactionsDto = new()
		{
			Transactions = new TransactionTableDTO[]
			{
				new TransactionTableDTO
				{
					Id = Guid.NewGuid(),
					Amount = 10,
					AccountCurrencyName = "Currency",
					CategoryName = "Category",
					CreatedOnLocalTime = DateTime.Now.AddDays(-1),
					Reference = "Reference",
					TransactionType = TransactionType.Expense.ToString()
				},
				new TransactionTableDTO
				{
					Id = Guid.NewGuid(),
					Amount = 15,
					AccountCurrencyName = "Currency2",
					CategoryName = "Category2",
					CreatedOnLocalTime = DateTime.Now,
					Reference = "Reference2",
					TransactionType = TransactionType.Expense.ToString()
				}
			},
			TotalTransactionsCount = 10
		};
		private static readonly UserUsedDropdownsDTO expUserDropdowns = new()
		{
			OwnerAccounts = expAccountsDropdown,
			OwnerAccountTypes = expAccountTypesDropdown,
			OwnerCategories = expCategoriesDropdown,
			OwnerCurrencies = expCurrenciesDropdown
		};
		private TransactionsController controller;

		[SetUp]
		public void SetUp()
		{
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
				.GetUserAccountsAndCategoriesDropdownsAsync(this.userId))
				.ReturnsAsync(expAccountsAndCategoriesDropdowns);

			this.usersServiceMock.Setup(x => x
				.GetUserAccountTypesAndCurrenciesDropdownsAsync(this.userId))
				.ReturnsAsync(expAccountTypesAndCurrenciesDropdowns);

			this.usersServiceMock.Setup(x => x
				.GetUserUsedDropdownsAsync(this.userId))
				.ReturnsAsync(expUserDropdowns);

			this.usersServiceMock.Setup(x => x
				.GetUserTransactionsAsync(It.Is<TransactionsFilterDTO>(x =>
					x.UserId == this.userId
					&& x.CurrencyId == null
					&& x.AccountId == null
					&& x.AccountTypeId == null
					&& x.CategoryId == null)))
				.ReturnsAsync(expTransactionsDto);
		}

		[Test]
		public async Task Index_OnGet_ShouldReturnViewModel()
		{
			//Arrange

			//Act
			var viewResult = (ViewResult)await this.controller.Index();

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				UserTransactionsViewModel viewModel = viewResult.Model as UserTransactionsViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");
				
				Assert.That(viewModel.UserId, Is.EqualTo(this.userId));
				AssertSamePropertiesValuesAreEqual(viewModel, expTransactionsDto);
				AssertSamePropertiesValuesAreEqual(viewModel, expUserDropdowns);
			});
		}

		[Test]
		public async Task Index_OnPost_ShouldReturnViewModel()
		{
			//Arrange
			var inputModel = new UserTransactionsInputModel
			{
				FromLocalTime = DateTime.Now.AddMonths(-1),
				ToLocalTime = DateTime.Now
			};

			TransactionsFilterDTO filter = this.mapper.Map<TransactionsFilterDTO>(inputModel);
			filter.UserId = this.userId;
			var expectedViewModel = new UserTransactionsViewModel(filter, expUserDropdowns, expTransactionsDto);

			//Act
			var viewResult = (ViewResult)await this.controller.Index(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				UserTransactionsViewModel actualViewModel = viewResult.Model as UserTransactionsViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertAreEqualAsJson(actualViewModel, expectedViewModel);
			});
		}

		[Test]
		public async Task Index_OnPost_ShouldReturnViewModelWithErrors_WhenModelIsInvalid()
		{
			//Arrange
			var inputModel = new UserTransactionsInputModel
			{
				FromLocalTime = DateTime.Now.AddMonths(-1),
				ToLocalTime = DateTime.Now
			};

			var expectedViewModel = new UserTransactionsViewModel(inputModel, expUserDropdowns, this.userId);
			this.controller.ModelState.AddModelError(string.Empty, "Model is invalid.");

			//Act
			var viewResult = (ViewResult)await this.controller.Index(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				AssertModelStateErrorIsEqual(viewResult.ViewData.ModelState, string.Empty, "Model is invalid.");

				UserTransactionsViewModel actualViewModel = viewResult.Model as UserTransactionsViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertAreEqualAsJson(actualViewModel, expectedViewModel);
			});
		}

		[Test]
		public async Task Create_OnGet_ShouldReturnViewModel()
		{
			//Arrange
			TransactionType[] transactionTypes = Enum.GetValues<TransactionType>();

			//Act
			var viewResult = (ViewResult)await this.controller.Create();

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				var model = viewResult.Model as CreateEditTransactionViewModel;
				Assert.That(model, Is.Not.Null);
				Assert.That(model!.Reference, Is.Null);
				Assert.That(model.OwnerId, Is.EqualTo(this.userId));
				Assert.That(model.AccountId, Is.Null);
				Assert.That(model.Amount, Is.EqualTo(0));
				Assert.That(model.CategoryId, Is.Null);

				AssertSamePropertiesValuesAreEqual(model, expAccountsAndCategoriesDropdowns);
			});
		}

		[Test]
		public async Task Create_OnPost_ShouldReturnViewModelWithErrors_WhenModelIsInvalid()
		{
			//Arrange
			var inputModel = new CreateEditTransactionViewModel
			{
				Amount = -10,
				CreatedOnLocalTime = DateTime.Now,
				AccountId = Guid.NewGuid(),
				CategoryId = Guid.NewGuid(),
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

				CreateEditTransactionViewModel model = viewResult.Model as CreateEditTransactionViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(model, inputModel);
				AssertSamePropertiesValuesAreEqual(model, expAccountsAndCategoriesDropdowns);

				AssertModelStateErrorIsEqual(
					viewResult.ViewData.ModelState,
					nameof(inputModel.Amount),
					"Amount is invalid.");
			});
		}

		[Test]
		public async Task Create_OnPost_ShouldReturnBadRequest_WhenUserIsNotOwnerInInputModel()
		{
			//Arrange
			var inputModel = new CreateEditTransactionViewModel
			{
				Amount = -10,
				CreatedOnLocalTime = DateTime.Now,
				AccountId = Guid.NewGuid(),
				CategoryId = Guid.NewGuid(),
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
			var inputModel = new CreateEditTransactionInputModel
			{
				Amount = 10,
				CreatedOnLocalTime = DateTime.Now,
				AccountId = Guid.NewGuid(),
				CategoryId = Guid.NewGuid(),
				OwnerId = this.userId,
				Reference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};
			var newTransactionId = Guid.Parse("e8befb1f-72a9-4bb7-831c-cbe678a11af8");

			this.accountsUpdateServiceMock.Setup(x => x
				.CreateTransactionAsync(It.Is<CreateEditTransactionInputDTO>(x =>
					x.OwnerId == inputModel.OwnerId
					&& x.TransactionType == inputModel.TransactionType
					&& x.CreatedOnLocalTime == inputModel.CreatedOnLocalTime
					&& x.Reference == inputModel.Reference
					&& x.AccountId == inputModel.AccountId
					&& x.Amount == inputModel.Amount
					&& x.CategoryId == inputModel.CategoryId)))
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
				Assert.That(result.ActionName, Is.EqualTo("Details"));
				Assert.That(result.RouteValues, Is.Not.Null);

				AssertRouteValueIsEqual(result.RouteValues!, "id", newTransactionId);
				AssertTempDataMessageIsEqual(this.controller.TempData, "You create a new transaction successfully!");
			});
		}

		[Test]
		public async Task Create_OnPost_ShouldReturnBadRequest_WhenAccountDoesNotExist()
		{
			//Arrange
			var inputModel = new CreateEditTransactionInputModel
			{
				Amount = 10,
				CreatedOnLocalTime = DateTime.Now,
				AccountId = Guid.NewGuid(),
				CategoryId = Guid.NewGuid(),
				OwnerId = this.userId,
				Reference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};

			this.accountsUpdateServiceMock.Setup(x => x
				.CreateTransactionAsync(It.Is<CreateEditTransactionInputDTO>(x =>
					x.OwnerId == inputModel.OwnerId
					&& x.TransactionType == inputModel.TransactionType
					&& x.CreatedOnLocalTime == inputModel.CreatedOnLocalTime
					&& x.Reference == inputModel.Reference
					&& x.AccountId == inputModel.AccountId
					&& x.Amount == inputModel.Amount
					&& x.CategoryId == inputModel.CategoryId)))
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
				.IsInRole(AdminRoleName))
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
				AssertTempDataMessageIsEqual(this.controller.TempData, "Your transaction was successfully deleted!");
			});
		}

		[Test]
		public async Task Delete_ShouldRedirectToHomeIndex_WhenTransactionWasDeletedAndUserIsAdmin()
		{
			//Arrange
			var transactionId = Guid.NewGuid();
			decimal newBalance = 100;
			string? returnUrl = null;

			this.userMock.Setup(x => x.IsInRole(AdminRoleName)).Returns(true);

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
				AssertTempDataMessageIsEqual(this.controller.TempData, "You successfully delete a user's transaction!");
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
				.IsInRole(AdminRoleName))
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
				AssertTempDataMessageIsEqual(this.controller.TempData, "Your transaction was successfully deleted!");
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
				.IsInRole(AdminRoleName))
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
				AssertTempDataMessageIsEqual(this.controller.TempData, "You successfully delete a user's transaction!");
			});
		}

		[Test]
		public async Task Delete_ShouldReturnUnauthorized_WhenUserIsNotOwnerOrAdmin()
		{
			//Arrange
			var transactionId = Guid.NewGuid();
			string? returnUrl = "return url";

			this.userMock.Setup(x => x
				.IsInRole(AdminRoleName))
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
				.IsInRole(AdminRoleName))
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
			var serviceReturnDto = new TransactionDetailsDTO
			{
				Id = transactionId,
				AccountCurrencyName = "Currency",
				AccountName = "Account name",
				Amount = 10,
				CategoryName = "Category",
				CreatedOnLocalTime = DateTime.Now.AddDays(-1),
				OwnerId = this.userId,
				Reference = "Reference",
				TransactionType = TransactionType.Income.ToString()
			};

			this.userMock.Setup(x => x
				.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsInfoServiceMock.Setup(x => x
				.GetTransactionDetailsAsync(transactionId, this.userId, false))
				.ReturnsAsync(serviceReturnDto);

			//Act
			var viewResult = (ViewResult)await this.controller.Details(transactionId);

			//Assert
			Assert.Multiple(() =>
			{
				TransactionDetailsDTO actual = viewResult.Model as TransactionDetailsDTO ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertAreEqualAsJson(actual, serviceReturnDto);
			});
		}

		[Test]
		public async Task TransactionDetails_ShouldReturnUnauthorized_WhenUserIsNowOwnerOrAdmin()
		{
			//Arrange
			var transactionId = Guid.NewGuid();

			this.userMock.Setup(x => x
				.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsInfoServiceMock.Setup(x => x
				.GetTransactionDetailsAsync(transactionId, this.userId, false))
				.Throws<ArgumentException>();

			//Act
			var result = (UnauthorizedResult)await this.controller.Details(transactionId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(401));
		}

		[Test]
		public async Task TransactionDetails_ShouldReturnBadRequest_WhenTransactionDoesNotExist()
		{
			//Arrange
			var transactionId = Guid.NewGuid();

			this.userMock.Setup(x => x
				.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsInfoServiceMock.Setup(x => x
				.GetTransactionDetailsAsync(transactionId, this.userId, false))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await this.controller.Details(transactionId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public async Task EditTransaction_OnGet_ShouldReturnTransactionFormModel_WhenTransactionIsNotInitial()
		{
			//Arrange
			var transactionId = Guid.NewGuid();
			var serviceReturnDto = new CreateEditTransactionOutputDTO
			{
				Amount = 10,
				CreatedOnLocalTime = DateTime.Now.AddDays(-1),
				OwnerId = this.userId,
				Reference = "Reference",
				AccountId = Guid.NewGuid(),
				CategoryId = Guid.NewGuid(),
				OwnerAccounts = expAccountsAndCategoriesDropdowns.OwnerAccounts,
				OwnerCategories = expAccountsAndCategoriesDropdowns.OwnerCategories,
				TransactionType = TransactionType.Income
			};

			this.userMock.Setup(x => x
				.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsInfoServiceMock.Setup(x => x
				.GetTransactionFormDataAsync(transactionId, this.userId, false))
				.ReturnsAsync(serviceReturnDto);

			//Act
			var viewResult = (ViewResult)await this.controller.Edit(transactionId);

			//Assert
			Assert.Multiple(() =>
			{
				CreateEditTransactionViewModel viewModel = viewResult.Model as CreateEditTransactionViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(viewModel, serviceReturnDto);
			});
		}

		[Test]
		public async Task EditTransaction_OnGet_ShouldReturnBadRequest_WhenTransactionDoesNotExistOrIsInitial()
		{
			//Arrange
			var transactionId = Guid.NewGuid();

			this.userMock.Setup(x => x
				.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsInfoServiceMock.Setup(x => x
				.GetTransactionFormDataAsync(transactionId, this.userId, false))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await this.controller.Edit(transactionId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public async Task EditTransaction_OnPost_ShouldReturnViewModelWithErrors_WhenModelIsInvalidAndTransactionIsNotInitial()
		{
			//Arrange
			var transactionId = Guid.NewGuid();
			var inputModel = new CreateEditTransactionViewModel
			{
				Amount = -10,
				CreatedOnLocalTime = DateTime.Now,
				AccountId = Guid.NewGuid(),
				CategoryId = Guid.NewGuid(),
				OwnerId = this.userId,
				Reference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};

			this.controller.ModelState.AddModelError(nameof(inputModel.Amount), "Amount is invalid.");

			//Act
			var viewResult = (ViewResult)await this.controller.Edit(transactionId, inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				AssertModelStateErrorIsEqual(viewResult.ViewData.ModelState, nameof(inputModel.Amount), "Amount is invalid.");

				CreateEditTransactionViewModel viewModel = viewResult.Model as CreateEditTransactionViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(viewModel, inputModel);
				AssertSamePropertiesValuesAreEqual(viewModel, expAccountsAndCategoriesDropdowns);
			});
		}

		[Test]
		public async Task EditTransaction_OnPost_ShouldThrowException_WhenTransactionIsInitial()
		{
			//Arrange
			var transactionId = Guid.NewGuid();
			var inputModel = new CreateEditTransactionInputModel
			{
				Amount = 10,
				CreatedOnLocalTime = DateTime.Now,
				AccountId = Guid.NewGuid(),
				CategoryId = Guid.NewGuid(),
				OwnerId = this.userId,
				Reference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};

			this.accountsUpdateServiceMock.Setup(x => x
				.EditTransactionAsync(transactionId, It.Is<CreateEditTransactionInputDTO>(x =>
					x.OwnerId == inputModel.OwnerId
					&& x.TransactionType == inputModel.TransactionType
					&& x.CreatedOnLocalTime == inputModel.CreatedOnLocalTime
					&& x.Reference == inputModel.Reference
					&& x.AccountId == inputModel.AccountId
					&& x.Amount == inputModel.Amount
					&& x.CategoryId == inputModel.CategoryId)))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await this.controller.Edit(transactionId, inputModel);

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
			var inputModel = new CreateEditTransactionViewModel
			{
				Amount = 10,
				CreatedOnLocalTime = DateTime.Now,
				AccountId = Guid.NewGuid(),
				CategoryId = Guid.NewGuid(),
				OwnerId = Guid.NewGuid(),
				Reference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};

			this.userMock.Setup(x => x
				.IsInRole(AdminRoleName))
				.Returns(false);

			//Act
			var result = (BadRequestResult)await this.controller.Edit(transactionId, inputModel);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public async Task EditTransaction_OnPost_ShouldReturnBadRequest_WhenTransactionDoesNotExist()
		{
			//Arrange
			var transactionId = Guid.NewGuid();
			var inputModel = new CreateEditTransactionInputModel
			{
				Amount = 10,
				CreatedOnLocalTime = DateTime.Now,
				AccountId = Guid.NewGuid(),
				CategoryId = Guid.NewGuid(),
				OwnerId = this.userId,
				Reference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};

			this.userMock.Setup(x => x.IsInRole(AdminRoleName)).Returns(false);

			this.accountsUpdateServiceMock.Setup(x => x
				.EditTransactionAsync(transactionId, It.Is<CreateEditTransactionInputDTO>(x =>
					x.OwnerId == inputModel.OwnerId
					&& x.TransactionType == inputModel.TransactionType
					&& x.CreatedOnLocalTime == inputModel.CreatedOnLocalTime
					&& x.Reference == inputModel.Reference
					&& x.AccountId == inputModel.AccountId
					&& x.Amount == inputModel.Amount
					&& x.CategoryId == inputModel.CategoryId)))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await this.controller.Edit(transactionId, inputModel);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public async Task EditTransaction_OnPost_ShouldRedirectToAction_WhenTransactionWasEditedAndUserIsOwner()
		{
			//Arrange
			var transactionId = Guid.NewGuid();
			var inputModel = new CreateEditTransactionInputModel
			{
				Amount = 10,
				CreatedOnLocalTime = DateTime.Now,
				AccountId = Guid.NewGuid(),
				CategoryId = Guid.NewGuid(),
				OwnerId = this.userId,
				Reference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};

			this.userMock.Setup(x => x.IsInRole(AdminRoleName)).Returns(false);

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (RedirectToActionResult)await this.controller.Edit(transactionId, inputModel);

			this.accountsUpdateServiceMock.Verify(x => x
				.EditTransactionAsync(transactionId, It.Is<CreateEditTransactionInputDTO>(x =>
					x.OwnerId == inputModel.OwnerId
					&& x.TransactionType == inputModel.TransactionType
					&& x.CreatedOnLocalTime == inputModel.CreatedOnLocalTime
					&& x.Reference == inputModel.Reference
					&& x.AccountId == inputModel.AccountId
					&& x.Amount == inputModel.Amount
					&& x.CategoryId == inputModel.CategoryId)),
				Times.Once);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.ActionName, Is.EqualTo("Details"));

				Assert.That(result.RouteValues, Is.Not.Null);
				AssertRouteValueIsEqual(result.RouteValues!, "id", transactionId);

				AssertTempDataMessageIsEqual(this.controller.TempData, "Your transaction was successfully edited!");
			});
		}

		[Test]
		public async Task EditTransaction_OnPost_ShouldRedirectToAction_WhenTransactionWasEditedAndUserIsAdmin()
		{
			//Arrange
			var transactionId = Guid.NewGuid();
			var ownerId = Guid.NewGuid();
			var accountId = Guid.NewGuid();
			var inputModel = new CreateEditTransactionInputModel
			{
				Amount = 10,
				CreatedOnLocalTime = DateTime.Now,
				AccountId = accountId,
				CategoryId = Guid.NewGuid(),
				OwnerId = ownerId,
				Reference = "Test Transaction",
				TransactionType = TransactionType.Expense
			};

			this.userMock.Setup(x => x
				.IsInRole(AdminRoleName))
				.Returns(true);

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (RedirectToActionResult)await this.controller.Edit(transactionId, inputModel);

			this.accountsUpdateServiceMock.Verify(x => x
				.EditTransactionAsync(transactionId, It.Is<CreateEditTransactionInputDTO>(x =>
					x.OwnerId == inputModel.OwnerId
					&& x.TransactionType == inputModel.TransactionType
					&& x.CreatedOnLocalTime == inputModel.CreatedOnLocalTime
					&& x.Reference == inputModel.Reference
					&& x.AccountId == inputModel.AccountId
					&& x.Amount == inputModel.Amount
					&& x.CategoryId == inputModel.CategoryId)),
				Times.Once);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.ActionName, Is.EqualTo("Details"));

				Assert.That(result.RouteValues, Is.Not.Null);
				AssertRouteValueIsEqual(result.RouteValues!, "id", transactionId);

				AssertTempDataMessageIsEqual(this.controller.TempData, ResponseMessages.AdminEditedUserTransaction);
			});
		}
	}
}
