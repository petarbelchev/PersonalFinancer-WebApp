namespace PersonalFinancer.Tests.Controllers
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
	using PersonalFinancer.Web.Models.Account;
	using static PersonalFinancer.Common.Constants.RoleConstants;

	[TestFixture]
	internal class AccountsControllerTests : ControllersUnitTestsBase
	{
		private AccountsController controller;

		private static readonly AccountTypesAndCurrenciesDropdownDTO expAccountTypesAndCurrencies = new()
		{
			OwnerAccountTypes = new AccountTypeDropdownDTO[]
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
			},
			OwnerCurrencies = new CurrencyDropdownDTO[]
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
			}
		};

		private static readonly AccountDetailsLongDTO expectedAccountDetailsDto = new()
		{
			Id = Guid.NewGuid(),
			Balance = 100,
			CurrencyName = "Currency",
			Name = "Account Name",
			Transactions = new TransactionTableDTO[]
				{
					new TransactionTableDTO
					{
						Id = Guid.NewGuid(),
						AccountCurrencyName = "Currency",
						Amount = 50,
						CategoryName = "Category",
						CreatedOn = DateTime.UtcNow,
						Reference = "Test transaction",
						TransactionType = TransactionType.Expense.ToString()
					}
				},
			TotalAccountTransactions = 10
		};

		[SetUp]
		public void SetUp()
		{
			this.usersServiceMock
				.Setup(x => x
				.GetUserAccountTypesAndCurrenciesDropdownDataAsync(this.userId))
				.ReturnsAsync(expAccountTypesAndCurrencies);

			this.controller = new AccountsController(
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
		}

		[Test]
		public async Task Create_ShouldReturnCorrectAccountFormViewModel()
		{
			//Arrange

			//Act
			var viewResult = (ViewResult)await this.controller.Create();

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				AccountFormViewModel viewModel = viewResult.Model as AccountFormViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				Assert.That(viewModel.OwnerId, Is.EqualTo(this.userId));
				Assert.That(viewModel.Name, Is.Null);
				Assert.That(viewModel.Balance, Is.EqualTo(0));
				Assert.That(viewModel.CurrencyId, Is.Null);
				Assert.That(viewModel.AccountTypeId, Is.Null);

				AssertSamePropertiesValuesAreEqual(viewModel!, expAccountTypesAndCurrencies);
			});
		}

		[Test]
		public async Task Create_ShouldReturnViewResultWithModelErrors_WhenModelIsInvalid()
		{
			//Arrange
			var inputModel = new AccountFormViewModel
			{
				Name = "Test",
				Balance = -100,
				CurrencyId = Guid.NewGuid(),
				AccountTypeId = Guid.NewGuid(),
				OwnerId = this.userId
			};

			this.controller.ModelState.AddModelError(nameof(inputModel.Balance), "Balance is invalid.");

			//Act
			var viewResult = (ViewResult)await this.controller.Create(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				AssertModelStateErrorIsEqual(viewResult.ViewData.ModelState, nameof(inputModel.Balance), "Balance is invalid.");

				AccountFormViewModel viewModel = viewResult.Model as AccountFormViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(viewModel, inputModel);
				AssertSamePropertiesValuesAreEqual(viewModel, expAccountTypesAndCurrencies);
			});
		}

		[Test]
		public async Task Create_ShouldReturnBadRequest_WhenOwnerIdIsDifferentFromUserId()
		{
			//Arrange
			var inputModel = new AccountFormViewModel
			{
				Name = "Test",
				Balance = -100,
				CurrencyId = Guid.NewGuid(),
				AccountTypeId = Guid.NewGuid(),
				OwnerId = Guid.NewGuid()
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
		public async Task Create_ShouldPassDtoToServiceAndRedirectToNewAccountDetailsPage()
		{
			//Arrange
			var inputModel = new AccountFormViewModel
			{
				Name = "Test Account",
				Balance = 100,
				CurrencyId = Guid.NewGuid(),
				AccountTypeId = Guid.NewGuid(),
				OwnerId = this.userId
			};

			var newAccId = Guid.NewGuid();

			this.accountsUpdateServiceMock.Setup(x => x
				.CreateAccountAsync(
					It.Is<CreateEditAccountDTO>(a =>
						a.Balance == inputModel.Balance
						&& a.CurrencyId == inputModel.CurrencyId
						&& a.Name == inputModel.Name
						&& a.OwnerId == inputModel.OwnerId
						&& a.AccountTypeId == inputModel.AccountTypeId)))
				.ReturnsAsync(newAccId);

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (RedirectToActionResult)await this.controller.Create(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.ActionName, Is.EqualTo("AccountDetails"));
				AssertTempDataMessageIsEqual(this.controller.TempData, "You create a new account successfully!");

				Assert.That(result.RouteValues, Is.Not.Null);
				AssertRouteValueIsEqual(result.RouteValues!, "id", newAccId);
			});
		}

		[Test]
		public async Task Create_ShouldCatchExceptionAndReturnViewResultWithModelErrors_WhenTryToCreateAccountWithExistingName()
		{
			//Arrange
			var inputModel = new AccountFormViewModel
			{
				Name = "Test Account",
				Balance = 100,
				CurrencyId = Guid.NewGuid(),
				AccountTypeId = Guid.NewGuid(),
				OwnerId = this.userId
			};

			string errorMessage = string.Format(ExceptionMessages.ExistingUserEntityName, "account", inputModel.Name);

			this.accountsUpdateServiceMock.Setup(x => x
				.CreateAccountAsync(It.Is<CreateEditAccountDTO>(
					x => x.CurrencyId == inputModel.CurrencyId
					&& x.Balance == inputModel.Balance
					&& x.AccountTypeId == inputModel.AccountTypeId
					&& x.OwnerId == inputModel.OwnerId
					&& x.Name == inputModel.Name)))
				.Throws(new ArgumentException(errorMessage));

			//Act
			var viewResult = (ViewResult)await this.controller.Create(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				AssertModelStateErrorIsEqual(viewResult.ViewData.ModelState, nameof(inputModel.Name), errorMessage);

				AccountFormViewModel viewModel = viewResult.Model as AccountFormViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(viewModel, inputModel);
				AssertSamePropertiesValuesAreEqual(viewModel, expAccountTypesAndCurrencies);
			});
		}

		[Test]
		public async Task AccountDetailsOnGet_ShouldReturnViewModel_WhenUserIsOwner()
		{
			//Arrange
			expectedAccountDetailsDto.OwnerId = this.userId;

			this.userMock.Setup(x => x.IsInRole(AdminRoleName)).Returns(false);

			this.accountsInfoServiceMock.Setup(x => x
				.GetAccountDetailsAsync(
					expectedAccountDetailsDto.Id,
					It.IsAny<DateTime>(),
					It.IsAny<DateTime>(),
					this.userId,
					false))
				.ReturnsAsync(expectedAccountDetailsDto);

			//Act
			var viewResult = (ViewResult)await this.controller.AccountDetails(expectedAccountDetailsDto.Id);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				AccountDetailsViewModel viewModel = viewResult.Model as AccountDetailsViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(viewModel, expectedAccountDetailsDto);
			});
		}

		[Test]
		public async Task AccountDetailsOnGet_ShouldReturnViewModel_WhenUserIsNotOwnerButIsAdmin()
		{
			//Arrange
			expectedAccountDetailsDto.OwnerId = Guid.NewGuid();

			this.userMock.Setup(x => x.IsInRole(AdminRoleName)).Returns(true);

			this.accountsInfoServiceMock.Setup(x => x
				.GetAccountDetailsAsync(expectedAccountDetailsDto.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>(), this.userId, true))
				.ReturnsAsync(expectedAccountDetailsDto);

			//Act
			var viewResult = (ViewResult)await this.controller.AccountDetails(expectedAccountDetailsDto.Id);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				AccountDetailsViewModel viewModel = viewResult.Model as AccountDetailsViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(viewModel, expectedAccountDetailsDto);
			});
		}

		[Test]
		public async Task AccountDetailsOnGet_ShouldReturnBadRequest_WhenUserNotOwnerOrAdminOrAccountDoesNotExist()
		{
			//Arrange
			this.userMock.Setup(x => x
				.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsInfoServiceMock.Setup(x => x
				.GetAccountDetailsAsync(
					It.IsAny<Guid>(),
					It.IsAny<DateTime>(),
					It.IsAny<DateTime>(),
					this.userId,
					false))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await this.controller.AccountDetails(Guid.NewGuid());

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});
		}

		[Test]
		public async Task AccountDetailsOnPost_ShouldReturnViewModel_WhenUserIsNotOwnerButIsAdmin()
		{
			//Arrange
			var inputModel = new AccountDetailsInputModel
			{
				Id = expectedAccountDetailsDto.Id,
				StartDate = DateTime.UtcNow.AddMonths(-1),
				EndDate = DateTime.UtcNow
			};

			expectedAccountDetailsDto.OwnerId = Guid.NewGuid();
			expectedAccountDetailsDto.StartDate = inputModel.StartDate ?? throw new InvalidOperationException();
			expectedAccountDetailsDto.EndDate = inputModel.EndDate ?? throw new InvalidOperationException();

			this.userMock.Setup(x => x
				.IsInRole(AdminRoleName))
				.Returns(true);

			this.accountsInfoServiceMock.Setup(x => x
				.GetAccountDetailsAsync(
					expectedAccountDetailsDto.Id,
					expectedAccountDetailsDto.StartDate,
					expectedAccountDetailsDto.EndDate,
					this.userId,
					true))
				.ReturnsAsync(expectedAccountDetailsDto);

			//Act
			var viewResult = (ViewResult)await this.controller.AccountDetails(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				AccountDetailsViewModel viewModel = viewResult.Model as AccountDetailsViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(viewModel, expectedAccountDetailsDto);
			});
		}

		[Test]
		public async Task AccountDetailsOnPost_ShouldReturnViewModel_WhenUserIsOwner()
		{
			//Arrange
			var inputModel = new AccountDetailsInputModel
			{
				Id = expectedAccountDetailsDto.Id,
				StartDate = DateTime.UtcNow.AddMonths(-1),
				EndDate = DateTime.UtcNow
			};

			expectedAccountDetailsDto.OwnerId = this.userId;
			expectedAccountDetailsDto.StartDate = inputModel.StartDate ?? throw new InvalidOperationException();
			expectedAccountDetailsDto.EndDate = inputModel.EndDate ?? throw new InvalidOperationException();

			this.userMock.Setup(x => x.IsInRole(AdminRoleName)).Returns(false);

			this.accountsInfoServiceMock.Setup(x => x
				.GetAccountDetailsAsync(
					expectedAccountDetailsDto.Id,
					expectedAccountDetailsDto.StartDate,
					expectedAccountDetailsDto.EndDate,
					this.userId,
					false))
				.ReturnsAsync(expectedAccountDetailsDto);

			//Act
			var viewResult = (ViewResult)await this.controller.AccountDetails(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				AccountDetailsViewModel viewModel = viewResult.Model as AccountDetailsViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(viewModel, expectedAccountDetailsDto);
			});
		}

		[Test]
		public async Task AccountDetailsOnPost_ShouldReturnBadRequest_WhenUserNotOwnerOrAdminOrAccountDoesNotExist()
		{
			//Arrange
			DateTime startDate = DateTime.UtcNow.AddMonths(-1);
			DateTime endDate = DateTime.UtcNow;

			var inputModel = new AccountDetailsInputModel
			{
				Id = expectedAccountDetailsDto.Id,
				StartDate = startDate,
				EndDate = endDate
			};

			this.userMock.Setup(x => x.IsInRole(AdminRoleName)).Returns(false);

			this.accountsInfoServiceMock.Setup(x => x
				.GetAccountDetailsAsync(expectedAccountDetailsDto.Id, startDate, endDate, this.userId, false))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await this.controller.AccountDetails(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});
		}

		[Test]
		public async Task AccountDetailsOnPost_ShouldReturnViewModelWithErrors_WhenModelIsInvalidButAccountExists()
		{
			//Arrange
			var inputModel = new AccountDetailsInputModel
			{
				Id = expectedAccountDetailsDto.Id,
				EndDate = DateTime.UtcNow
			};

			var serviceModel = new AccountDetailsShortDTO
			{
				Name = expectedAccountDetailsDto.Name,
				Balance = expectedAccountDetailsDto.Balance,
				AccountTypeName = expectedAccountDetailsDto.AccountTypeName,
				CurrencyName = expectedAccountDetailsDto.CurrencyName
			};

			this.controller.ModelState.AddModelError(nameof(inputModel.StartDate), "Start Date is invalid");

			this.accountsInfoServiceMock
				.Setup(x => x.GetAccountShortDetailsAsync(expectedAccountDetailsDto.Id))
				.ReturnsAsync(serviceModel);

			//Act
			var viewResult = (ViewResult)await this.controller.AccountDetails(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);
				AssertModelStateErrorIsEqual(
					viewResult.ViewData.ModelState,
					nameof(inputModel.StartDate),
					"Start Date is invalid");

				AccountDetailsViewModel viewModel = viewResult.Model as AccountDetailsViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(viewModel, serviceModel);
			});
		}

		[Test]
		public async Task AccountDetailsOnPost_ShouldReturnBadRequest_WhenModelIsInvalidAndAccountDoesNotExist()
		{
			//Arrange
			var accountId = Guid.NewGuid();
			var inputModel = new AccountDetailsInputModel
			{
				Id = accountId,
				EndDate = DateTime.UtcNow
			};

			this.controller.ModelState.AddModelError(string.Empty, "Model is invalid");

			this.accountsInfoServiceMock
				.Setup(x => x.GetAccountShortDetailsAsync(accountId))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await this.controller.AccountDetails(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});
		}

		[Test]
		public async Task DeleteOnGet_ShouldReturnCorrectViewModel_WhenInputIsValid()
		{
			//Arrange
			var accountId = Guid.NewGuid();
			string accountName = "name";

			this.userMock.Setup(x => x
				.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsInfoServiceMock.Setup(x => x
				.GetAccountNameAsync(accountId, this.userId, false))
				.ReturnsAsync(accountName);

			//Act
			var viewResult = (ViewResult)await this.controller.Delete(accountId);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				var model = viewResult.Model as DeleteAccountViewModel;
				Assert.That(model, Is.Not.Null);
				Assert.That(model!.Name, Is.EqualTo(accountName));
				Assert.That(model.ShouldDeleteTransactions, Is.False);
			});
		}

		[Test]
		public async Task DeleteOnGet_ShouldReturnBadRequest_WhenInputIsInvalid()
		{
			//Arrange
			this.userMock.Setup(x => x
				.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsInfoServiceMock.Setup(x => x
				.GetAccountNameAsync(It.IsAny<Guid>(), this.userId, false))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await this.controller.Delete(Guid.NewGuid());

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});
		}

		[Test]
		public async Task DeleteOnGet_ShouldReturnViewModelWithModelErrors_WhenModelIsInvalid()
		{
			//Arrange
			this.controller.ModelState.AddModelError(string.Empty, "Model is invalid");

			//Act
			var result = (BadRequestResult)await this.controller.Delete(Guid.NewGuid());

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});
		}

		[Test]
		public async Task DeleteOnPost_ShouldRedirectWhenAccountIsDeletedAndUserIsOwner()
		{
			//Arrange
			var accountId = Guid.NewGuid();
			var inputModel = new DeleteAccountInputModel
			{
				Id = accountId,
				ConfirmButton = "accept",
				ReturnUrl = "returnUrl",
				ShouldDeleteTransactions = false
			};

			this.userMock.Setup(x => x
				.IsInRole(AdminRoleName))
				.Returns(false);

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (RedirectToActionResult)await this.controller.Delete(inputModel);

			this.accountsUpdateServiceMock.Verify(x => x
				.DeleteAccountAsync(accountId, this.userId, false, false),
				Times.Once);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.ControllerName, Is.EqualTo("Home"));
				Assert.That(result.ActionName, Is.EqualTo("Index"));

				AssertTempDataMessageIsEqual(this.controller.TempData, "Your account was successfully deleted!");
			});
		}

		[Test]
		public async Task DeleteOnPost_ShouldRedirectWhenAccountIsDeletedAndUserIsAdmin()
		{
			//Arrange
			var accountId = Guid.NewGuid();
			var inputModel = new DeleteAccountInputModel
			{
				Id = accountId,
				ConfirmButton = "button",
				ReturnUrl = "returnUrl",
				ShouldDeleteTransactions = false
			};

			var ownerId = Guid.NewGuid();

			this.userMock.Setup(x => x
				.IsInRole(AdminRoleName))
				.Returns(true);

			this.accountsInfoServiceMock.Setup(x => x
				.GetAccountOwnerIdAsync(accountId))
				.ReturnsAsync(ownerId);

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (LocalRedirectResult)await this.controller.Delete(inputModel);

			this.accountsUpdateServiceMock.Verify(x => x
				.DeleteAccountAsync(accountId, this.userId, true, false),
				Times.Once);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Url, Is.EqualTo("/Admin/Users/Details/" + ownerId));

				AssertTempDataMessageIsEqual(this.controller.TempData, "You successfully delete user's account!");
			});
		}

		[Test]
		public async Task DeleteOnPost_ShouldCatchExceptionAndReturnUnauthorized_WhenUserIsUnauthorized()
		{
			//Arrange
			var accountId = Guid.NewGuid();
			var inputModel = new DeleteAccountInputModel
			{
				Id = accountId,
				ConfirmButton = "accept",
				ReturnUrl = "returnUrl",
				ShouldDeleteTransactions = false
			};

			this.userMock.Setup(x => x
				.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsUpdateServiceMock.Setup(x =>
				x.DeleteAccountAsync(accountId, this.userId, false, false))
				.Throws<ArgumentException>();

			//Act
			var result = (UnauthorizedResult)await this.controller.Delete(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(401));
			});
		}

		[Test]
		public async Task DeleteOnPost_ShouldCatchExceptionAndReturnBadRequest_WhenDeleteWasUnsuccessful()
		{
			//Arrange
			var accountId = Guid.NewGuid();
			var inputModel = new DeleteAccountInputModel
			{
				Id = accountId,
				ConfirmButton = "accept",
				ReturnUrl = "returnUrl",
				ShouldDeleteTransactions = false
			};

			this.userMock.Setup(x => x
				.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsUpdateServiceMock.Setup(x => x
				.DeleteAccountAsync(accountId, this.userId, false, false))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await this.controller.Delete(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});
		}

		[Test]
		public async Task DeleteOnPost_ShouldLocalRedirect_WhenRejectButtonIsClicked()
		{
			//Arrange
			var inputModel = new DeleteAccountInputModel
			{
				Id = Guid.NewGuid(),
				ConfirmButton = "reject",
				ReturnUrl = "returnUrl",
				ShouldDeleteTransactions = false
			};

			//Act
			var result = (LocalRedirectResult)await this.controller.Delete(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Url, Is.EqualTo(inputModel.ReturnUrl));
			});
		}

		[Test]
		public async Task DeleteOnPost_ShouldReturnBadRequest_WhenModelIsInvalid()
		{
			//Arrange
			var inputModel = new DeleteAccountInputModel
			{
				Id = Guid.NewGuid(),
				ConfirmButton = "accept",
				ReturnUrl = "returnUrl",
				ShouldDeleteTransactions = false
			};

			this.controller.ModelState.AddModelError(string.Empty, "Model is invalid.");

			//Act
			var result = (BadRequestResult)await this.controller.Delete(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});
		}

		[Test]
		public async Task EditAccountOnGet_ShouldReturnAccountFormViewModel_WhenUserIsOwner()
		{
			//Arrange
			var accId = Guid.NewGuid();
			var expServiceDto = new CreateEditAccountDTO
			{
				Name = "name",
				AccountTypeId = Guid.NewGuid(),
				CurrencyId = Guid.NewGuid(),
				Balance = 100,
				OwnerId = this.userId,
				OwnerAccountTypes = expAccountTypesAndCurrencies.OwnerAccountTypes,
				OwnerCurrencies = expAccountTypesAndCurrencies.OwnerCurrencies
			};

			this.accountsInfoServiceMock.Setup(x => x
				.GetAccountFormDataAsync(accId, this.userId, false))
				.ReturnsAsync(expServiceDto);

			//Act
			var viewResult = (ViewResult)await this.controller.EditAccount(accId);

			//Assert
			Assert.Multiple(() =>
			{
				AccountFormViewModel viewModel = viewResult.Model as AccountFormViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(viewModel, expServiceDto);
			});
		}

		[Test]
		public async Task EditAccountOnGet_ShouldReturnBadRequest_WhenServiceThrowException()
		{
			//Arrange
			this.accountsInfoServiceMock.Setup(x => x
				.GetAccountFormDataAsync(It.IsAny<Guid>(), this.userId, false))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await this.controller.EditAccount(Guid.NewGuid());

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});
		}

		[Test]
		public async Task EditAccountOnPost_ShouldReturnViewModelWithErrors_WhenModelIsInvalid()
		{
			//Arrange
			var accId = Guid.NewGuid();
			string returnUrl = "returnUrl";
			var inputModel = new AccountFormViewModel
			{
				Name = "a",
				Balance = 100,
				OwnerId = this.userId,
				CurrencyId = Guid.NewGuid(),
				AccountTypeId = Guid.NewGuid(),
			};

			this.controller.ModelState.AddModelError(nameof(inputModel.Name), "Name is invalid.");

			//Act
			var viewResult = (ViewResult)await this.controller.EditAccount(accId, inputModel, returnUrl);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				AssertModelStateErrorIsEqual(viewResult.ViewData.ModelState, nameof(inputModel.Name), "Name is invalid.");

				AccountFormViewModel viewModel = viewResult.Model as AccountFormViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(viewModel, inputModel);
				AssertSamePropertiesValuesAreEqual(viewModel, expAccountTypesAndCurrencies);
			});
		}

		[Test]
		public async Task EditAccountOnPost_ShouldReturnBadRequest_WhenUserIsNotOwnerOrAdmin()
		{
			//Arrange
			var accId = Guid.NewGuid();
			string returnUrl = "returnUrl";
			var inputFormModel = new AccountFormViewModel
			{
				Name = "Account name",
				Balance = 100,
				OwnerId = Guid.NewGuid(),
				CurrencyId = Guid.NewGuid(),
				AccountTypeId = Guid.NewGuid()
			};

			this.userMock.Setup(x => x.IsInRole(AdminRoleName)).Returns(false);

			//Act
			var viewResult = (BadRequestResult)await this.controller.EditAccount(accId, inputFormModel, returnUrl);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult.StatusCode, Is.EqualTo(400));
			});
		}

		[Test]
		public async Task EditAccountOnPost_ShouldPassToServiceModelAndRedirectToReturnUrl_WhenUserIsOwnerAndAccountIsEdited()
		{
			//Arrange
			var accId = Guid.NewGuid();
			string returnUrl = "returnUrl";
			var inputFormModel = new AccountFormViewModel
			{
				Name = "Account name",
				Balance = 100,
				OwnerId = this.userId,
				CurrencyId = Guid.NewGuid(),
				AccountTypeId = Guid.NewGuid(),
			};

			this.userMock.Setup(x => x.IsInRole(AdminRoleName)).Returns(false);

			this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (LocalRedirectResult)await this.controller.EditAccount(accId, inputFormModel, returnUrl);

			this.accountsUpdateServiceMock.Verify(x => x.EditAccountAsync(accId,
				It.Is<CreateEditAccountDTO>(m =>
					m.CurrencyId == inputFormModel.CurrencyId
					&& m.Balance == inputFormModel.Balance
					&& m.OwnerId == inputFormModel.OwnerId
					&& m.AccountTypeId == inputFormModel.AccountTypeId
					&& m.Name == inputFormModel.Name)),
				Times.Once);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Url, Is.EqualTo(returnUrl));
				AssertTempDataMessageIsEqual(this.controller.TempData, "Your account was successfully edited!");
			});
		}

		[Test]
		public async Task EditAccountOnPost_ShouldPassToServiceModelAndRedirectToReturnUrl_WhenUserIsAdminAndAccountIsEdited()
		{
			//Arrange
			var accId = Guid.NewGuid();
			string returnUrl = "returnUrl";
			var ownerId = Guid.NewGuid();
			var inputFormModel = new AccountFormViewModel
			{
				Name = "Account name",
				Balance = 100,
				OwnerId = ownerId,
				CurrencyId = Guid.NewGuid(),
				AccountTypeId = Guid.NewGuid(),
			};

			this.userMock.Setup(x => x.IsInRole(AdminRoleName)).Returns(true);
			this.accountsInfoServiceMock.Setup(x => x.GetAccountOwnerIdAsync(accId)).ReturnsAsync(ownerId);
			this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (LocalRedirectResult)await this.controller.EditAccount(accId, inputFormModel, returnUrl);

			this.accountsUpdateServiceMock.Verify(x => x.EditAccountAsync(accId,
				It.Is<CreateEditAccountDTO>(m =>
					m.CurrencyId == inputFormModel.CurrencyId
					&& m.Balance == inputFormModel.Balance
					&& m.OwnerId == inputFormModel.OwnerId
					&& m.AccountTypeId == inputFormModel.AccountTypeId
					&& m.Name == inputFormModel.Name)),
				Times.Once);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Url, Is.EqualTo(returnUrl));
				AssertTempDataMessageIsEqual(this.controller.TempData, "You successfully edited user's account!");
			});
		}

		[Test]
		public async Task EditAccountOnPost_ShouldCatchServiceArgumentExceptionAndReturnModelWithErrors_WhenUserIsAdminAndAccountNameExist()
		{
			//Arrange
			var accId = Guid.NewGuid();
			string returnUrl = "returnUrl";
			var ownerId = Guid.NewGuid();
			var inputModel = new AccountFormViewModel
			{
				Name = "Account name",
				Balance = 100,
				OwnerId = ownerId,
				CurrencyId = Guid.NewGuid(),
				AccountTypeId = Guid.NewGuid(),
			};

			this.userMock.Setup(x => x
				.IsInRole(AdminRoleName))
				.Returns(true);

			this.accountsInfoServiceMock.Setup(x => x
				.GetAccountOwnerIdAsync(accId))
				.ReturnsAsync(ownerId);

			this.accountsUpdateServiceMock.Setup(x => x
				.EditAccountAsync(accId, It.Is<CreateEditAccountDTO>(m =>
					m.CurrencyId == inputModel.CurrencyId
					&& m.Balance == inputModel.Balance
					&& m.OwnerId == inputModel.OwnerId
					&& m.AccountTypeId == inputModel.AccountTypeId
					&& m.Name == inputModel.Name)))
				.Throws<ArgumentException>();

			this.usersServiceMock.Setup(x => x
				.GetUserAccountTypesAndCurrenciesDropdownDataAsync(ownerId))
				.ReturnsAsync(expAccountTypesAndCurrencies);

			//Act
			var viewResult = (ViewResult)await this.controller.EditAccount(accId, inputModel, returnUrl);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				AssertModelStateErrorIsEqual(viewResult.ViewData.ModelState, nameof(inputModel.Name),
					string.Format(ExceptionMessages.AdminExistingUserEntityName, "account", inputModel.Name));

				AccountFormViewModel viewModel = viewResult.Model as AccountFormViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(viewModel, inputModel);
				AssertSamePropertiesValuesAreEqual(viewModel, expAccountTypesAndCurrencies);
			});
		}

		[Test]
		public async Task EditAccountOnPost_ShouldCatchServiceArgumentExceptionAndReturnModelWithErrors_WhenUserIsOwnerAndAccountNameExist()
		{
			//Arrange
			var accId = Guid.NewGuid();
			string returnUrl = "returnUrl";
			var inputModel = new AccountFormViewModel
			{
				Name = "Account name",
				Balance = 100,
				OwnerId = this.userId,
				CurrencyId = Guid.NewGuid(),
				AccountTypeId = Guid.NewGuid(),
			};

			this.userMock.Setup(x => x.IsInRole(AdminRoleName)).Returns(false);

			string errorMessage = string.Format(ExceptionMessages.ExistingUserEntityName, "account", inputModel.Name);

			this.accountsUpdateServiceMock.Setup(x => x.EditAccountAsync(accId,
				It.Is<CreateEditAccountDTO>(m =>
					m.CurrencyId == inputModel.CurrencyId
					&& m.Balance == inputModel.Balance
					&& m.OwnerId == inputModel.OwnerId
					&& m.AccountTypeId == inputModel.AccountTypeId
					&& m.Name == inputModel.Name)))
				.Throws(new ArgumentException(errorMessage));

			Guid userId = inputModel.OwnerId ?? throw new InvalidOperationException();

			//Act
			var viewResult = (ViewResult)await this.controller.EditAccount(accId, inputModel, returnUrl);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				AssertModelStateErrorIsEqual(viewResult.ViewData.ModelState, nameof(inputModel.Name), errorMessage);

				AccountFormViewModel viewModel = viewResult.Model as AccountFormViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(viewModel, inputModel);
				AssertSamePropertiesValuesAreEqual(viewModel, expAccountTypesAndCurrencies);
			});
		}

		[Test]
		public async Task EditAccountOnPost_ShouldCatchServiceInvalidOperationExceptionAndReturnBadRequest_WhenAccountDoesNotExist()
		{
			//Arrange
			var accId = Guid.NewGuid();
			string returnUrl = "returnUrl";
			var inputFormModel = new AccountFormViewModel
			{
				Name = "Account name",
				Balance = 100,
				OwnerId = this.userId,
				CurrencyId = Guid.NewGuid(),
				AccountTypeId = Guid.NewGuid(),
			};

			this.userMock.Setup(x => x.IsInRole(AdminRoleName)).Returns(false);

			this.accountsUpdateServiceMock.Setup(x => x.EditAccountAsync(accId,
				It.Is<CreateEditAccountDTO>(m =>
					m.CurrencyId == inputFormModel.CurrencyId
					&& m.Balance == inputFormModel.Balance
					&& m.OwnerId == inputFormModel.OwnerId
					&& m.AccountTypeId == inputFormModel.AccountTypeId
					&& m.Name == inputFormModel.Name)))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await this.controller.EditAccount(accId, inputFormModel, returnUrl);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});
		}
	}
}
