namespace PersonalFinancer.Tests.Controllers
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.ViewFeatures;
	using Microsoft.Extensions.Logging;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Services.Users.Models;
	using PersonalFinancer.Web.Controllers;
	using PersonalFinancer.Web.Models.Account;
	using System.Security.Claims;
	using static PersonalFinancer.Common.Constants.RoleConstants;

	[TestFixture]
	internal class AccountsControllerTests : ControllersUnitTestsBase
	{
		private static readonly AccountTypesAndCurrenciesDropdownDTO expAccountTypesAndCurrencies = new()
		{
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
		private static AccountDetailsDTO expectedAccountDetailsDto;

		private Mock<ILogger<AccountsController>> loggerMock;
		private AccountsController controller;

		[SetUp]
		public void SetUp()
		{
			expectedAccountDetailsDto = new()
			{
				Id = Guid.NewGuid(),
				Name = "Account Name",
				Balance = 100,
				CurrencyName = "Currency Name",
				AccountTypeName = "Account TypeName",
				OwnerId = this.userId,
			};

			this.usersServiceMock
				.Setup(x => x
				.GetUserAccountTypesAndCurrenciesDropdownsAsync(this.userId))
				.ReturnsAsync(expAccountTypesAndCurrencies);

			this.loggerMock = new Mock<ILogger<AccountsController>>();

			this.controller = new AccountsController(
				this.accountsUpdateServiceMock.Object,
				this.accountsInfoServiceMock.Object,
				this.usersServiceMock.Object,
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
		public async Task Create_OnGet_ShouldReturnCorrectAccountFormViewModel()
		{
			//Arrange

			//Act
			var viewResult = (ViewResult)await this.controller.Create();

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				var viewModel = viewResult.Model as CreateEditAccountViewModel ??
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
		public async Task Create_OnPost_ShouldReturnViewResultWithModelErrors_WhenModelIsInvalid()
		{
			//Arrange
			var inputModel = new CreateEditAccountInputModel
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

				CreateEditAccountViewModel viewModel = viewResult.Model as CreateEditAccountViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(viewModel, inputModel);
				AssertSamePropertiesValuesAreEqual(viewModel, expAccountTypesAndCurrencies);
			});
		}

		[Test]
		public async Task Create_OnPost_ShouldReturnBadRequest_WhenOwnerIdIsDifferentFromUserId()
		{
			//Arrange
			var inputModel = new CreateEditAccountInputModel
			{
				Name = "Test",
				Balance = -100,
				CurrencyId = Guid.NewGuid(),
				AccountTypeId = Guid.NewGuid(),
				OwnerId = Guid.NewGuid()
			};

			string expectedLogMessage = string.Format(
				LoggerMessages.CreateAccountWithAnotherUserId,
				this.userId,
				inputModel.OwnerId);

			//Act
			var result = (BadRequestResult)await this.controller.Create(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});
			
			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task Create_OnPost_ShouldPassDtoToServiceAndRedirectToNewAccountDetailsPage()
		{
			//Arrange
			var inputModel = new CreateEditAccountInputModel
			{
				Name = "Test Account",
				Balance = 100,
				CurrencyId = Guid.NewGuid(),
				AccountTypeId = Guid.NewGuid(),
				OwnerId = this.userId
			};

			var newAccId = Guid.NewGuid();

			this.accountsUpdateServiceMock
				.Setup(x => x.CreateAccountAsync(It.Is<CreateEditAccountInputDTO>(a => ValidateObjectsAreEqual(a, inputModel))))
				.ReturnsAsync(newAccId);

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (RedirectToActionResult)await this.controller.Create(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.ActionName, Is.EqualTo("Details"));
				AssertTempDataMessageIsEqual(this.controller.TempData, ResponseMessages.CreatedAccount);

				Assert.That(result.RouteValues, Is.Not.Null);
				AssertRouteValueIsEqual(result.RouteValues!, "id", newAccId);
			});
		}

		[Test]
		public async Task Create_OnPost_ShouldCatchExceptionAndReturnViewResultWithModelErrors_WhenTryToCreateAccountWithExistingName()
		{
			//Arrange
			var inputModel = new CreateEditAccountInputModel
			{
				Name = "Test Account",
				Balance = 100,
				CurrencyId = Guid.NewGuid(),
				AccountTypeId = Guid.NewGuid(),
				OwnerId = this.userId
			};

			string errorMessage = string.Format(ExceptionMessages.ExistingUserEntityName, "account", inputModel.Name);

			this.accountsUpdateServiceMock
				.Setup(x => x.CreateAccountAsync(It.Is<CreateEditAccountInputDTO>(x => ValidateObjectsAreEqual(x , inputModel))))
				.Throws(new ArgumentException(errorMessage));

			//Act
			var viewResult = (ViewResult)await this.controller.Create(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				AssertModelStateErrorIsEqual(viewResult.ViewData.ModelState, nameof(inputModel.Name), errorMessage);

				CreateEditAccountViewModel viewModel = viewResult.Model as CreateEditAccountViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(viewModel, inputModel);
				AssertSamePropertiesValuesAreEqual(viewModel, expAccountTypesAndCurrencies);
			});
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public async Task Details_ShouldReturnViewModel(bool isUserAdmin)
		{
			//Arrange
			Guid currentUserId = isUserAdmin ? Guid.NewGuid() : this.userId;
			string? returnUrl = isUserAdmin ? "return url" : null;

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(isUserAdmin);

			this.userMock
				.Setup(x => x.FindFirst(ClaimTypes.NameIdentifier))
				.Returns(new Claim(ClaimTypes.NameIdentifier, currentUserId.ToString()));

			this.accountsInfoServiceMock.Setup(x => x
				.GetAccountDetailsAsync(expectedAccountDetailsDto.Id, currentUserId, isUserAdmin))
				.ReturnsAsync(expectedAccountDetailsDto);

			//Act
			var viewResult = (ViewResult)await this.controller.Details(expectedAccountDetailsDto.Id, returnUrl);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				AccountDetailsViewModel viewModel = viewResult.Model as AccountDetailsViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				Assert.That(viewModel.ReturnUrl, Is.EqualTo(returnUrl));

				AssertSamePropertiesValuesAreEqual(viewModel, expectedAccountDetailsDto);
			});
		}

		[Test]
		public async Task Details_ShouldReturnBadRequest_WhenTheAccountDoesNotExist()
		{
			//Arrange
			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsInfoServiceMock
				.Setup(x => x.GetAccountDetailsAsync(It.IsAny<Guid>(), this.userId, false))
				.Throws<InvalidOperationException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.GetAccountDetailsWithInvalidInputData,
				this.userId);

			//Act
			var result = (BadRequestResult)await this.controller.Details(Guid.NewGuid(), null);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task Details_ShouldReturnUnauthorized_WhenTheUserIsUnauthorized()
		{
			//Arrange
			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			var accountId = Guid.NewGuid();

			this.accountsInfoServiceMock
				.Setup(x => x.GetAccountDetailsAsync(accountId, this.userId, false))
				.Throws<UnauthorizedAccessException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.UnauthorizedGetAccountDetails,
				this.userId,
				accountId);

			//Act
			var result = (UnauthorizedResult)await this.controller.Details(accountId, null);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(401));
			});

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task Details_ShouldReturnBadRequest_WhenTheModelStateIsInvalid()
		{
			//Arrange
			this.controller.ModelState.AddModelError("id", "invalid id");

			string expectedLogMessage = string.Format(
				LoggerMessages.GetAccountDetailsWithInvalidInputData,
				this.userId);

			//Act
			var result = (BadRequestResult)await this.controller.Details(Guid.NewGuid(), null);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task Delete_OnGet_ShouldReturnCorrectViewModel_WhenInputIsValid()
		{
			//Arrange
			var accountId = Guid.NewGuid();
			string accountName = "name";

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsInfoServiceMock
				.Setup(x => x.GetAccountNameAsync(accountId, this.userId, false))
				.ReturnsAsync(accountName);

			//Act
			var viewResult = (ViewResult)await this.controller.Delete(accountId, null);

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
		public async Task Delete_OnGet_ShouldReturnBadRequest_WhenInputIsInvalid()
		{
			//Arrange
			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			var accountId = Guid.NewGuid();

			this.accountsInfoServiceMock
				.Setup(x => x.GetAccountNameAsync(accountId, this.userId, false))
				.Throws<InvalidOperationException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.DeleteAccountWithInvalidInputData,
				this.userId);

			//Act
			var result = (BadRequestResult)await this.controller.Delete(accountId, null);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task Delete_OnGet_ShouldReturnUnauthorized_WhenTheUserIsUnauthorized()
		{
			//Arrange
			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			var accountId = Guid.NewGuid();

			this.accountsInfoServiceMock
				.Setup(x => x.GetAccountNameAsync(accountId, this.userId, false))
				.Throws<UnauthorizedAccessException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.UnauthorizedAccountDeletion,
				this.userId,
				accountId);

			//Act
			var result = (UnauthorizedResult)await this.controller.Delete(accountId, null);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(401));
			});

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task Delete_OnGet_ShouldReturnViewModelWithModelErrors_WhenModelIsInvalid()
		{
			//Arrange
			this.controller.ModelState.AddModelError(string.Empty, "Model is invalid");

			string expectedLogMessage = string.Format(
					LoggerMessages.DeleteAccountWithInvalidInputData,
					this.userId);

			//Act
			var result = (BadRequestResult)await this.controller.Delete(Guid.NewGuid(), null);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public async Task Delete_OnPost_ShouldRedirectWhenAccountIsDeleted(bool isUserAdmin)
		{
			//Arrange
			var accountId = Guid.NewGuid();
			Guid currentUserId = isUserAdmin ? Guid.NewGuid() : this.userId;

			var inputModel = new DeleteAccountInputModel
			{
				Id = accountId,
				ConfirmButton = "accept",
				ShouldDeleteTransactions = false,
				ReturnUrl = isUserAdmin ? "/Admin/Users/Details/" + this.userId : null
			};

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(isUserAdmin);

			this.userMock
				.Setup(x => x.FindFirst(ClaimTypes.NameIdentifier))
				.Returns(new Claim(ClaimTypes.NameIdentifier, currentUserId.ToString()));

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), 
				Mock.Of<ITempDataProvider>());

			string? expectedReturnUrl = isUserAdmin ? inputModel.ReturnUrl : "/";

			string expectedTempDataMessage = isUserAdmin
				? ResponseMessages.AdminDeletedUserAccount
				: ResponseMessages.DeletedAccount;

			//Act
			var result = (LocalRedirectResult)await this.controller.Delete(inputModel);

			//Assert
			this.accountsUpdateServiceMock.Verify(
				x => x.DeleteAccountAsync(accountId, currentUserId, isUserAdmin, false),
				Times.Once);

			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Url, Is.EqualTo(expectedReturnUrl));

				AssertTempDataMessageIsEqual(this.controller.TempData, expectedTempDataMessage);
			});
		}

		[Test]
		public async Task Delete_OnPost_ShouldCatchExceptionAndReturnUnauthorized_WhenUserIsUnauthorized()
		{
			//Arrange
			var accountId = Guid.NewGuid();
			var inputModel = new DeleteAccountInputModel
			{
				Id = accountId,
				ConfirmButton = "accept",
				ShouldDeleteTransactions = false
			};

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsUpdateServiceMock
				.Setup(x =>x.DeleteAccountAsync(accountId, this.userId, false, false))
				.Throws<UnauthorizedAccessException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.UnauthorizedAccountDeletion,
				this.userId,
				accountId);

			//Act
			var result = (UnauthorizedResult)await this.controller.Delete(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(401));
			});

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task Delete_OnPost_ShouldCatchExceptionAndReturnBadRequest_WhenDeleteWasUnsuccessful()
		{
			//Arrange
			var accountId = Guid.NewGuid();
			var inputModel = new DeleteAccountInputModel
			{
				Id = accountId,
				ConfirmButton = "accept",
				ShouldDeleteTransactions = false
			};

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsUpdateServiceMock
				.Setup(x => x.DeleteAccountAsync(accountId, this.userId, false, false))
				.Throws<InvalidOperationException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.DeleteAccountWithInvalidInputData,
				this.userId);

			//Act
			var result = (BadRequestResult)await this.controller.Delete(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task Delete_OnPost_ShouldCatchExceptionAndReturnBadRequest_WhenTheAccountIdIsNull()
		{
			//Arrange
			var accountId = Guid.NewGuid();
			var inputModel = new DeleteAccountInputModel
			{
				Id = null,
				ConfirmButton = "accept",
				ShouldDeleteTransactions = false
			};

			string expectedLogMessage = string.Format(
				LoggerMessages.DeleteAccountWithInvalidInputData,
				this.userId);

			//Act
			var result = (BadRequestResult)await this.controller.Delete(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task Delete_OnPost_ShouldLocalRedirect_WhenRejectButtonIsClicked()
		{
			//Arrange
			var inputModel = new DeleteAccountInputModel
			{
				Id = Guid.NewGuid(),
				ConfirmButton = "reject",
				ShouldDeleteTransactions = false,
				ReturnUrl = "returnUrl",
			};

			//Act
			var result = (RedirectToActionResult)await this.controller.Delete(inputModel);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.ActionName, Is.EqualTo("Details"));

			AssertRouteValueIsEqual(
				result.RouteValues!,
				nameof(inputModel.Id),
				inputModel.Id,
				totalRouteValues: 2);

			AssertRouteValueIsEqual(
				result.RouteValues!,
				nameof(inputModel.ReturnUrl),
				inputModel.ReturnUrl,
				totalRouteValues: 2);
		}

		[Test]
		public async Task Delete_OnPost_ShouldReturnBadRequest_WhenModelIsInvalid()
		{
			//Arrange
			var inputModel = new DeleteAccountInputModel
			{
				Id = Guid.NewGuid(),
				ConfirmButton = "accept",
				ShouldDeleteTransactions = false
			};

			this.controller.ModelState.AddModelError(string.Empty, "Model is invalid.");

			string expectedLogMessage = string.Format(
				LoggerMessages.DeleteAccountWithInvalidInputData,
				this.userId);

			//Act
			var result = (BadRequestResult)await this.controller.Delete(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public async Task Edit_OnGet_ShouldReturnAccountFormViewModel_WhenUserIsOwner(bool isUserAdmin)
		{
			//Arrange
			var accId = Guid.NewGuid();
			Guid currentUserId = isUserAdmin ? Guid.NewGuid() : this.userId;

			var expServiceDto = new CreateEditAccountOutputDTO
			{
				Name = "name",
				AccountTypeId = Guid.NewGuid(),
				CurrencyId = Guid.NewGuid(),
				Balance = 100,
				OwnerId = this.userId,
				OwnerAccountTypes = expAccountTypesAndCurrencies.OwnerAccountTypes,
				OwnerCurrencies = expAccountTypesAndCurrencies.OwnerCurrencies
			};

			this.userMock
				.Setup(x => x.FindFirst(ClaimTypes.NameIdentifier))
				.Returns(new Claim(ClaimTypes.NameIdentifier, currentUserId.ToString()));

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(isUserAdmin);

			this.accountsInfoServiceMock
				.Setup(x => x.GetAccountFormDataAsync(accId, currentUserId, isUserAdmin))
				.ReturnsAsync(expServiceDto);

			//Act
			var viewResult = (ViewResult)await this.controller.Edit(accId);

			//Assert
			Assert.Multiple(() =>
			{
				CreateEditAccountViewModel viewModel = viewResult.Model as CreateEditAccountViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(viewModel, expServiceDto);
			});
		}

		[Test]
		public async Task Edit_OnGet_ShouldReturnBadRequest_WhenTheAccountDoesNotExist()
		{
			//Arrange
			this.accountsInfoServiceMock
				.Setup(x => x.GetAccountFormDataAsync(It.IsAny<Guid>(), this.userId, false))
				.Throws<InvalidOperationException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.EditAccountWithInvalidInputData,
				this.userId);

			//Act
			var result = (BadRequestResult)await this.controller.Edit(Guid.NewGuid());

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task Edit_OnGet_ShouldReturnUnauthorized_WhenTheUserIsUnauthorized()
		{
			//Arrange
			var accountId = Guid.NewGuid();

			this.accountsInfoServiceMock
				.Setup(x => x.GetAccountFormDataAsync(accountId, this.userId, false))
				.Throws<UnauthorizedAccessException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.UnauthorizedAccountEdit,
				this.userId,
				accountId);

			//Act
			var result = (UnauthorizedResult)await this.controller.Edit(accountId);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(401));
			});

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task Edit_OnGet_ShouldReturnBadRequest_WhenTheModelStateIsInvalid()
		{
			//Arrange
			this.controller.ModelState.AddModelError("id", "invalid id");

			string expectedLogMessage = string.Format(
				LoggerMessages.EditAccountWithInvalidInputData,
				this.userId);

			//Act
			var result = (BadRequestResult)await this.controller.Edit(Guid.NewGuid());

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task Edit_OnPost_ShouldReturnViewModelWithErrors_WhenModelIsInvalid()
		{
			//Arrange
			var accId = Guid.NewGuid();
			var inputModel = new CreateEditAccountInputModel
			{
				Name = "a",
				Balance = 100,
				OwnerId = this.userId,
				CurrencyId = Guid.NewGuid(),
				AccountTypeId = Guid.NewGuid(),
			};

			this.controller.ModelState.AddModelError(nameof(inputModel.Name), "Name is invalid.");

			//Act
			var viewResult = (ViewResult)await this.controller.Edit(accId, inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				AssertModelStateErrorIsEqual(viewResult.ViewData.ModelState, nameof(inputModel.Name), "Name is invalid.");

				CreateEditAccountViewModel viewModel = viewResult.Model as CreateEditAccountViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(viewModel, inputModel);
				AssertSamePropertiesValuesAreEqual(viewModel, expAccountTypesAndCurrencies);
			});
		}

		[Test]
		public async Task Edit_OnPost_ShouldReturnUnauthorized_WhenTheUserIsUnauthorized()
		{
			//Arrange
			var accId = Guid.NewGuid();
			var inputFormModel = new CreateEditAccountInputModel
			{
				Name = "Account name",
				Balance = 100,
				OwnerId = Guid.NewGuid(),
				CurrencyId = Guid.NewGuid(),
				AccountTypeId = Guid.NewGuid()
			};

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			string expectedLogMessage = string.Format(
				LoggerMessages.UnauthorizedAccountEdit,
				this.userId,
				accId);

			//Act
			var viewResult = (UnauthorizedResult)await this.controller.Edit(accId, inputFormModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult.StatusCode, Is.EqualTo(401));
			});

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public async Task Edit_OnPost_ShouldPassToServiceModelAndRedirectToReturnUrl(bool isUserAdmin)
		{
			//Arrange
			var accId = Guid.NewGuid();
			Guid currentUserId = isUserAdmin ? Guid.NewGuid() : this.userId;

			var inputFormModel = new CreateEditAccountInputModel
			{
				Name = "Account name",
				Balance = 100,
				OwnerId = this.userId,
				CurrencyId = Guid.NewGuid(),
				AccountTypeId = Guid.NewGuid(),
			};

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(isUserAdmin);

			this.userMock
				.Setup(x => x.FindFirst(ClaimTypes.NameIdentifier))
				.Returns(new Claim(ClaimTypes.NameIdentifier, currentUserId.ToString()));

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), 
				Mock.Of<ITempDataProvider>());

			string expectedTempDataMessage = isUserAdmin
				? ResponseMessages.AdminEditedUserAccount
				: ResponseMessages.EditedAccount;

			//Act
			var result = (RedirectToActionResult)await this.controller.Edit(accId, inputFormModel);

			this.accountsUpdateServiceMock.Verify(
				x => x.EditAccountAsync(
					accId, 
					It.Is<CreateEditAccountInputDTO>(m => ValidateObjectsAreEqual(m, inputFormModel))),
				Times.Once);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.ActionName, Is.EqualTo("Details"));
				AssertRouteValueIsEqual(result.RouteValues!, "Id", accId);
				AssertTempDataMessageIsEqual(this.controller.TempData, expectedTempDataMessage);
			});
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public async Task Edit_OnPost_ShouldCatchServiceArgumentExceptionAndReturnModelWithErrors(bool isUserAdmin)
		{
			//Arrange
			var accId = Guid.NewGuid();
			Guid currentUserId = isUserAdmin ? Guid.NewGuid() : this.userId;

			var inputModel = new CreateEditAccountInputModel
			{
				Name = "Account name",
				Balance = 100,
				OwnerId = this.userId,
				CurrencyId = Guid.NewGuid(),
				AccountTypeId = Guid.NewGuid(),
			};

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(isUserAdmin);

			this.userMock
				.Setup(x => x.FindFirst(ClaimTypes.NameIdentifier))
				.Returns(new Claim(ClaimTypes.NameIdentifier, currentUserId.ToString()));

			string errorMessage = string.Format(
				isUserAdmin
					? ExceptionMessages.AdminExistingUserEntityName
					: ExceptionMessages.ExistingUserEntityName, 
				"account", 
				inputModel.Name);

			this.accountsUpdateServiceMock
				.Setup(x => x.EditAccountAsync(
					accId, 
					It.Is<CreateEditAccountInputDTO>(m => ValidateObjectsAreEqual(m, inputModel))))
				.Throws(new ArgumentException(errorMessage));

			this.usersServiceMock
				.Setup(x => x.GetUserAccountTypesAndCurrenciesDropdownsAsync(currentUserId))
				.ReturnsAsync(expAccountTypesAndCurrencies);

			//Act
			var viewResult = (ViewResult)await this.controller.Edit(accId, inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				AssertModelStateErrorIsEqual(viewResult.ViewData.ModelState, nameof(inputModel.Name), errorMessage);

				CreateEditAccountViewModel viewModel = viewResult.Model as CreateEditAccountViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(viewModel, inputModel);
				AssertSamePropertiesValuesAreEqual(viewModel, expAccountTypesAndCurrencies);
			});
		}

		[Test]
		public async Task Edit_OnPost_ShouldCatchServiceInvalidOperationExceptionAndReturnBadRequest_WhenAccountDoesNotExist()
		{
			//Arrange
			var accId = Guid.NewGuid();
			var inputFormModel = new CreateEditAccountInputModel
			{
				Name = "Account name",
				Balance = 100,
				OwnerId = this.userId,
				CurrencyId = Guid.NewGuid(),
				AccountTypeId = Guid.NewGuid(),
			};

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsUpdateServiceMock
				.Setup(x => x.EditAccountAsync(
					accId,
					It.Is<CreateEditAccountInputDTO>(m => ValidateObjectsAreEqual(m, inputFormModel))))
				.Throws<InvalidOperationException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.EditAccountWithInvalidInputData,
				this.userId);

			//Act
			var result = (BadRequestResult)await this.controller.Edit(accId, inputFormModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public async Task Filtered_ShouldReturnViewModel(bool isUserAdmin)
		{
			//Arrange
			Guid currentUserId = isUserAdmin ? Guid.NewGuid() : this.userId;

			var inputModel = new AccountDetailsInputModel
			{
				Id = expectedAccountDetailsDto.Id,
				FromLocalTime = DateTime.Now.AddMonths(-1),
				ToLocalTime = DateTime.Now
			};

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(isUserAdmin);

			this.userMock
				.Setup(x => x.FindFirst(ClaimTypes.NameIdentifier))
				.Returns(new Claim(ClaimTypes.NameIdentifier, currentUserId.ToString()));

			this.accountsInfoServiceMock
				.Setup(x => x.GetAccountDetailsAsync(expectedAccountDetailsDto.Id, currentUserId, isUserAdmin))
				.ReturnsAsync(expectedAccountDetailsDto);

			//Act
			var viewResult = (ViewResult)await this.controller.Filtered(inputModel);

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
		public async Task Filtered_ShouldReturnBadRequest_WhenTheAccountDoesNotExist()
		{
			//Arrange
			var inputModel = new AccountDetailsInputModel
			{
				Id = expectedAccountDetailsDto.Id,
				FromLocalTime = DateTime.Now.AddMonths(-1),
				ToLocalTime = DateTime.Now
			};

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsInfoServiceMock
				.Setup(x => x.GetAccountDetailsAsync(expectedAccountDetailsDto.Id, this.userId, false))
				.Throws<InvalidOperationException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.GetAccountDetailsWithInvalidInputData,
				this.userId);

			//Act
			var result = (BadRequestResult)await this.controller.Filtered(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task Filtered_ShouldReturnUnauthorized_WhenTheUserIsUnauthorized()
		{
			//Arrange
			var inputModel = new AccountDetailsInputModel
			{
				Id = expectedAccountDetailsDto.Id,
				FromLocalTime = DateTime.Now.AddMonths(-1),
				ToLocalTime = DateTime.Now
			};

			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsInfoServiceMock
				.Setup(x => x.GetAccountDetailsAsync(expectedAccountDetailsDto.Id, this.userId, false))
				.Throws<UnauthorizedAccessException>();

			string expectedLogMessage = string.Format(
				LoggerMessages.UnauthorizedGetAccountDetails,
				this.userId,
				expectedAccountDetailsDto.Id);

			//Act
			var result = (UnauthorizedResult)await this.controller.Filtered(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(401));
			});

			VerifyLoggerLogWarning(this.loggerMock, expectedLogMessage);
		}

		[Test]
		public async Task Filtered_ShouldReturnBadRequest_WhenTheAccountIdIsInvalid()
		{
			//Arrange
			var inputModel = new AccountDetailsInputModel
			{
				Id = null,
				FromLocalTime = DateTime.Now.AddMonths(-1),
				ToLocalTime = DateTime.Now
			};

			//Act
			var result = (BadRequestResult)await this.controller.Filtered(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});
		}

		[Test]
		public async Task Filtered_ShouldReturnViewModelWithErrors_WhenDatesAreInvalidButAccountExists()
		{
			//Arrange
			var inputModel = new AccountDetailsInputModel
			{
				Id = expectedAccountDetailsDto.Id,
				ToLocalTime = DateTime.Now
			};

			this.controller.ModelState.AddModelError(nameof(inputModel.FromLocalTime), "Start Date is invalid");

			this.accountsInfoServiceMock
				.Setup(x => x.GetAccountDetailsAsync(expectedAccountDetailsDto.Id, this.userId, false))
				.ReturnsAsync(expectedAccountDetailsDto);

			//Act
			var viewResult = (ViewResult)await this.controller.Filtered(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);
				AssertModelStateErrorIsEqual(
					viewResult.ViewData.ModelState,
					nameof(inputModel.FromLocalTime),
					"Start Date is invalid");

				AccountDetailsViewModel viewModel = viewResult.Model as AccountDetailsViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(viewModel, expectedAccountDetailsDto);
			});
		}

		[Test]
		public async Task Filtered_ShouldReturnBadRequest_WhenModelIsInvalidAndAccountDoesNotExist()
		{
			//Arrange
			var accountId = Guid.NewGuid();
			var inputModel = new AccountDetailsInputModel
			{
				Id = accountId,
				ToLocalTime = DateTime.Now
			};

			this.controller.ModelState.AddModelError(string.Empty, "Model is invalid");

			this.accountsInfoServiceMock
				.Setup(x => x.GetAccountDetailsAsync(accountId, this.userId, false))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await this.controller.Filtered(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});
		}
	}
}
