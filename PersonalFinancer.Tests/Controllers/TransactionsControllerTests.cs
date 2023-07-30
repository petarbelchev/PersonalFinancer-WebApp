namespace PersonalFinancer.Tests.Controllers
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.ViewFeatures;
	using Microsoft.Extensions.Logging;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Data.Models.Enums;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Services.Users.Models;
	using PersonalFinancer.Web.Controllers;
	using PersonalFinancer.Web.Models.Transaction;
	using System.Security.Claims;
	using static PersonalFinancer.Common.Constants.RoleConstants;

	[TestFixture]
	internal class TransactionsControllerTests : ControllersUnitTestsBase
	{
		private static readonly DropdownDTO[] expAccountsDropdown = new DropdownDTO[]
		{
			new DropdownDTO
			{
				Id = Guid.NewGuid(),
				Name = "Account name 1"
			},
			new DropdownDTO
			{
				Id = Guid.NewGuid(),
				Name = "Account name 2"
			}
		};
		private static readonly DropdownDTO[] expCategoriesDropdown = new DropdownDTO[]
		{
			new DropdownDTO
			{
				Id = Guid.NewGuid(),
				Name = "Category name 1"
			},
			new DropdownDTO
			{
				Id = Guid.NewGuid(),
				Name = "Category name 2"
			}
		};
		private static readonly AccountsAndCategoriesDropdownDTO expAccountsAndCategoriesDropdowns = new() 
		{
			OwnerAccounts = expAccountsDropdown,
			OwnerCategories = expCategoriesDropdown
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
			OwnerCategories = expCategoriesDropdown,
			OwnerAccountTypes = new DropdownDTO[]
			{
				new DropdownDTO
				{
					Id = Guid.NewGuid(),
					Name = "AccType 1"
				},
				new DropdownDTO
				{
					Id = Guid.NewGuid(),
					Name = "AccType 2"
				}
			},
			OwnerCurrencies = new DropdownDTO[]
			{
				new DropdownDTO
				{
					Id = Guid.NewGuid(),
					Name = "Currency 1"
				},
				new DropdownDTO
				{
					Id = Guid.NewGuid(),
					Name = "Currency 2"
				}
			}
		};

		private Mock<ILogger<TransactionsController>> logger;
		private TransactionsController controller;

		[SetUp]
		public void SetUp()
		{
			this.logger = new Mock<ILogger<TransactionsController>>();

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

			this.usersServiceMock
				.Setup(x => x.GetUserAccountsAndCategoriesDropdownsAsync(this.userId))
				.ReturnsAsync(expAccountsAndCategoriesDropdowns);

			this.usersServiceMock
				.Setup(x => x.GetUserUsedDropdownsAsync(this.userId))
				.ReturnsAsync(expUserDropdowns);

			this.usersServiceMock
				.Setup(x => x.GetUserTransactionsAsync(It.Is<TransactionsFilterDTO>(x =>
					x.UserId == this.userId
					&& x.CurrencyId == null
					&& x.AccountId == null
					&& x.AccountTypeId == null
					&& x.CategoryId == null)))
				.ReturnsAsync(expTransactionsDto);
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
		public async Task Create_OnPost_ShouldReturnUnauthorized_WhenUserIsUnauthorized()
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

			string expectedLogMessage = string.Format(
				LoggerMessages.CreateTransactionWithAnotherUserId,
				this.userId,
				inputModel.OwnerId);

			//Act
			var result = (UnauthorizedResult)await this.controller.Create(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(401));
			});

			VerifyLoggerLogWarning(this.logger, expectedLogMessage);
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

			this.accountsUpdateServiceMock
				.Setup(x => x.CreateTransactionAsync(It.Is<CreateEditTransactionInputDTO>(x => ValidateObjectsAreEqual(x, inputModel))))
				.ReturnsAsync(newTransactionId);

			this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

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
				AssertTempDataMessageIsEqual(this.controller.TempData, ResponseMessages.CreatedTransaction);
			});
		}

		[Test]
		public async Task Create_OnPost_ShouldReturnBadRequest_WhenTheAccountDoesNotExist()
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

			this.accountsUpdateServiceMock
				.Setup(x => x.CreateTransactionAsync(It.Is<CreateEditTransactionInputDTO>(x => ValidateObjectsAreEqual(x, inputModel))))
				.Throws<InvalidOperationException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.CreateTransactionWithInvalidInputData,
				this.userId);

			//Act
			var result = (BadRequestResult)await this.controller.Create(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});

			VerifyLoggerLogWarning(this.logger, expectedLogMessage);
		}

		[Test]
		[TestCase(false, null)]
		[TestCase(false, "return url")]
		[TestCase(true, null)]
		[TestCase(true, "return url")]
		public async Task Delete_ShouldRedirect_WhenTransactionWasDeleted(bool isUserAdmin, string? returnUrl)
		{
			//Arrange
			var transactionId = Guid.NewGuid();
			decimal newBalance = 100;

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(isUserAdmin);

			this.accountsUpdateServiceMock
				.Setup(x => x.DeleteTransactionAsync(transactionId, this.userId, isUserAdmin))
				.ReturnsAsync(newBalance);

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			string expectedTempDataMessage = isUserAdmin
				? ResponseMessages.AdminDeletedUserTransaction
				: ResponseMessages.DeletedTransaction;

			//Act
			var result = await this.controller.Delete(transactionId, returnUrl);

			//Assert
			Assert.Multiple(() =>
			{
				if (returnUrl == null)
				{
					var redirectToActionResult = result as RedirectToActionResult;
					Assert.That(redirectToActionResult!.ControllerName, Is.EqualTo("Home"));
					Assert.That(redirectToActionResult.ActionName, Is.EqualTo("Index"));
				}
				else
				{
					var localRedirectResult = result as LocalRedirectResult;
					Assert.That(localRedirectResult!.Url, Is.EqualTo(returnUrl));
				}

				AssertTempDataMessageIsEqual(this.controller.TempData, expectedTempDataMessage);
			});
		}

		[Test]
		public async Task Delete_ShouldReturnUnauthorized_WhenUserIsUnauthorized()
		{
			//Arrange
			var transactionId = Guid.NewGuid();
			string? returnUrl = "return url";

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
			var result = (UnauthorizedResult)await this.controller.Delete(transactionId, returnUrl);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(401));

			VerifyLoggerLogWarning(this.logger, expectedLogMessage);
		}

		[Test]
		public async Task Delete_ShouldReturnBadRequest_WhenTransactionDoesNotExist()
		{
			//Arrange
			var transactionId = Guid.NewGuid();
			string? returnUrl = "return url";

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
			var result = (BadRequestResult)await this.controller.Delete(transactionId, returnUrl);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));

			VerifyLoggerLogWarning(this.logger, expectedLogMessage);
		}

		[Test]
		public async Task Delete_ShouldReturnBadRequest_WhenTheModelIsInvalid()
		{
			//Arrange
			this.controller.ModelState.AddModelError("id", "invalid id");

			string expectedLogMessage = string.Format(
				LoggerMessages.DeleteTransactionWithInvalidInputData,
				this.userId);

			//Act
			var result = (BadRequestResult)await this.controller.Delete(Guid.Empty);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));

			VerifyLoggerLogWarning(this.logger, expectedLogMessage);
		}

		[Test]
		public async Task Details_ShouldReturnTransactionDetailsModel()
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

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsInfoServiceMock
				.Setup(x => x.GetTransactionDetailsAsync(transactionId, this.userId, false))
				.ReturnsAsync(serviceReturnDto);

			//Act
			var viewResult = (ViewResult)await this.controller.Details(transactionId);

			//Assert
			Assert.Multiple(() =>
			{
				TransactionDetailsDTO actual = viewResult.Model as TransactionDetailsDTO ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(actual, serviceReturnDto);
			});
		}

		[Test]
		public async Task Details_ShouldReturnUnauthorized_WhenUserIsUnauthorized()
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
			var result = (UnauthorizedResult)await this.controller.Details(transactionId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(401));

			VerifyLoggerLogWarning(this.logger, expectedLogMessage);
		}

		[Test]
		public async Task Details_ShouldReturnBadRequest_WhenTransactionDoesNotExist()
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
			var result = (BadRequestResult)await this.controller.Details(transactionId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));

			VerifyLoggerLogWarning(this.logger, expectedLogMessage);
		}

		[Test]
		public async Task Details_ShouldReturnBadRequest_WhenTheModelIsInvalid()
		{
			//Arrange
			this.controller.ModelState.AddModelError("id", "invalid id");

			string expectedLogMessage = string.Format(
				LoggerMessages.GetTransactionDetailsWithInvalidInputData,
				this.userId);

			//Act
			var result = (BadRequestResult)await this.controller.Details(Guid.Empty);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));

			VerifyLoggerLogWarning(this.logger, expectedLogMessage);
		}

		[Test]
		public async Task Edit_OnGet_ShouldReturnTransactionFormModel_WhenTransactionIsNotInitial()
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

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsInfoServiceMock
				.Setup(x => x.GetTransactionFormDataAsync(transactionId, this.userId, false))
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
		public async Task Edit_OnGet_ShouldReturnBadRequest_WhenTransactionDoesNotExistOrIsInitial()
		{
			//Arrange
			var transactionId = Guid.NewGuid();

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsInfoServiceMock
				.Setup(x => x.GetTransactionFormDataAsync(transactionId, this.userId, false))
				.Throws<InvalidOperationException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.EditTransactionWithInvalidInputData,
				this.userId);

			//Act
			var result = (BadRequestResult)await this.controller.Edit(transactionId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));

			VerifyLoggerLogWarning(this.logger, expectedLogMessage);
		}

		[Test]
		public async Task Edit_OnGet_ShouldReturnBadRequest_WhenTheModelIsInvalid()
		{
			//Arrange
			this.controller.ModelState.AddModelError("id", "invalid id");

			string expectedLogMessage = string.Format(
				LoggerMessages.EditTransactionWithInvalidInputData,
				this.userId);

			//Act
			var result = (BadRequestResult)await this.controller.Edit(Guid.Empty);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));

			VerifyLoggerLogWarning(this.logger, expectedLogMessage);
		}

		[Test]
		public async Task Edit_OnGet_ShouldReturnUnauthorized_WhenTheUserIsUnauthorized()
		{
			//Arrange
			var transactionId = Guid.NewGuid();

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsInfoServiceMock
				.Setup(x => x.GetTransactionFormDataAsync(transactionId, this.userId, false))
				.Throws<UnauthorizedAccessException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.UnauthorizedTransactionEdit,
				this.userId,
				transactionId);

			//Act
			var result = (UnauthorizedResult)await this.controller.Edit(transactionId);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(401));

			VerifyLoggerLogWarning(this.logger, expectedLogMessage);
		}

		[Test]
		public async Task Edit_OnPost_ShouldReturnViewModelWithErrors_WhenModelIsInvalidAndTransactionIsNotInitial()
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
		public async Task Edit_OnPost_ShouldReturnUnauthorized_WhenTheUserIsUnauthorized()
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

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			string expectedLogMessage = string.Format(
				LoggerMessages.UnauthorizedTransactionEdit,
				this.userId,
				transactionId);

			//Act
			var result = (UnauthorizedResult)await this.controller.Edit(transactionId, inputModel);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(401));

			VerifyLoggerLogWarning(this.logger, expectedLogMessage);
		}

		[Test]
		public async Task Edit_OnPost_ShouldReturnBadRequest_WhenTransactionDoesNotExistOrIsInitial()
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

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsUpdateServiceMock
				.Setup(x => x.EditTransactionAsync(
					transactionId, 
					It.Is<CreateEditTransactionInputDTO>(x => ValidateObjectsAreEqual(x, inputModel))))
				.Throws<InvalidOperationException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.EditTransactionWithInvalidInputData,
				this.userId);

			//Act
			var result = (BadRequestResult)await this.controller.Edit(transactionId, inputModel);

			//Assert
			Assert.That(result.StatusCode, Is.EqualTo(400));

			VerifyLoggerLogWarning(this.logger, expectedLogMessage);
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public async Task Edit_OnPost_ShouldRedirectToAction_WhenTransactionWasEdited(bool isUserAdmin)
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

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(isUserAdmin);

			string currentUserId = isUserAdmin ? "adminId" : this.userId.ToString();

			this.userMock
				.Setup(x => x.FindFirst(ClaimTypes.NameIdentifier))
				.Returns(new Claim(ClaimTypes.NameIdentifier, currentUserId));

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			string expectedTempDataMessage = isUserAdmin
				? ResponseMessages.AdminEditedUserTransaction
				: ResponseMessages.EditedTransaction;

			//Act
			var result = (RedirectToActionResult)await this.controller.Edit(transactionId, inputModel);

			this.accountsUpdateServiceMock.Verify(
				x => x.EditTransactionAsync(
					transactionId, 
					It.Is<CreateEditTransactionInputDTO>(x => ValidateObjectsAreEqual(x, inputModel))),
				Times.Once);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.ActionName, Is.EqualTo("Details"));

				Assert.That(result.RouteValues, Is.Not.Null);
				AssertRouteValueIsEqual(result.RouteValues!, "id", transactionId);

				AssertTempDataMessageIsEqual(this.controller.TempData, expectedTempDataMessage);
			});
		}

		[Test]
		public async Task Filter_ShouldReturnViewModel()
		{
			//Arrange
			var inputModel = new UserTransactionsInputModel
			{
				FromLocalTime = DateTime.Now.AddMonths(-1),
				ToLocalTime = DateTime.Now
			};

			var expectedViewModel = new UserTransactionsViewModel(inputModel, expUserDropdowns, this.userId);

			//Act
			var viewResult = (ViewResult)await this.controller.Filter(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				UserTransactionsViewModel actualViewModel = viewResult.Model as UserTransactionsViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(actualViewModel, expectedViewModel);
			});
		}

		[Test]
		public async Task Filter_ShouldReturnViewModelWithErrors_WhenModelIsInvalid()
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
			var viewResult = (ViewResult)await this.controller.Filter(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				AssertModelStateErrorIsEqual(viewResult.ViewData.ModelState, string.Empty, "Model is invalid.");

				UserTransactionsViewModel actualViewModel = viewResult.Model as UserTransactionsViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(actualViewModel, expectedViewModel);
			});
		}

		[Test]
		public async Task Index_ShouldReturnViewModel()
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
	}
}
