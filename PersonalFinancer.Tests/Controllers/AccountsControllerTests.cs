using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using PersonalFinancer.Data.Models.Enums;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Services.User.Models;
using PersonalFinancer.Web.Controllers;
using PersonalFinancer.Web.Models.Account;
using static PersonalFinancer.Services.Infrastructure.Constants;
using static PersonalFinancer.Web.Infrastructure.Constants;

namespace PersonalFinancer.Tests.Controllers
{
	[TestFixture]
	internal class AccountsControllerTests : ControllersUnitTestsBase
	{
		private AccountsController controller;

		private readonly UserAccountTypesAndCurrenciesServiceModel expAccTypesAndCurrencies = new ()
			{
				AccountTypes = new AccountTypeServiceModel[]
				{
					new AccountTypeServiceModel
					{
						Id = "1",
						Name = "AccType 1"
					},
					new AccountTypeServiceModel
					{
						Id = "2",
						Name = "AccType 2"
					}
				},
				Currencies = new CurrencyServiceModel[]
				{
					new CurrencyServiceModel
					{
						Id = "1",
						Name = "Currency 1"
					},
					new CurrencyServiceModel
					{
						Id = "2",
						Name = "Currency 2"
					}
				}
			};
		private readonly AccountDetailsServiceModel expAccDetailsDto = new ()
		{
			Id = "account id",
			Balance = 100,
			CurrencyName = "Currency",
			Name = "Account Name",
			Transactions = new TransactionTableServiceModel[]
				{
					new TransactionTableServiceModel
					{
						Id = "1",
						AccountCurrencyName = "Currency",
						Amount = 50,
						CategoryName = "Category",
						CreatedOn = DateTime.UtcNow,
						Refference = "Test transaction",
						TransactionType = TransactionType.Expense.ToString()
					}
				},
			TotalAccountTransactions = 10
		};

		[SetUp]
		public void SetUp()
		{
			usersServiceMock
				.Setup(x => x.GetUserAccountTypesAndCurrencies(this.userId))
				.ReturnsAsync(expAccTypesAndCurrencies);

			this.controller = new AccountsController(
				this.accountsServiceMock.Object,
				this.mapper,
				this.usersServiceMock.Object);

			controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = userMock.Object
				}
			};
		}

		[Test]
		public async Task Create_ShouldReturnCorrectAccountFormViewModel()
		{
			//Arrange

			//Act
			ViewResult viewResult = (ViewResult)await controller.Create();

			//Assert
			Assert.That(viewResult, Is.Not.Null);
			CheckAccountFormViewModel(viewResult.Model as AccountFormViewModel);
		}

		[Test]
		public async Task Create_ShouldReturnViewResultWithModelErrors_WhenModelIsInvalid()
		{
			//Arrange
			var inputModel = new AccountFormViewModel
			{
				Name = "Test",
				Balance = -100,
				CurrencyId = "1",
				AccountTypeId = "1",
				OwnerId = this.userId
			};

			controller.ModelState.AddModelError(nameof(inputModel.Balance), "Balance is invalid.");

			//Act
			ViewResult viewResult = (ViewResult)await controller.Create(inputModel);

			//Assert
			Assert.That(viewResult, Is.Not.Null);
			CheckModelStateErrors(viewResult.ViewData.ModelState, nameof(inputModel.Balance), "Balance is invalid.");
			CheckAccountFormViewModel(viewResult.Model as AccountFormViewModel, inputModel);
		}

		[Test]
		public async Task Create_ShouldReturnBadRequest_WhenOwnerIdIsDifferentFromUserId()
		{
			//Arrange
			var inputModel = new AccountFormViewModel
			{
				Name = "Test",
				Balance = -100,
				CurrencyId = "1",
				AccountTypeId = "1",
				OwnerId = "invalid id"
			};

			//Act
			BadRequestResult result = (BadRequestResult)await controller.Create(inputModel);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public async Task Create_ShouldPassDtoToServiceAndRedirectToNewAccountDetailsPage()
		{
			//Arrange
			var inputModel = new AccountFormViewModel
			{
				Name = "Test Account",
				Balance = 100,
				CurrencyId = "1",
				AccountTypeId = "1",
				OwnerId = this.userId
			};

			string newAccId = "new acc id";

			accountsServiceMock.Setup(x =>
				x.CreateAccount(
					It.Is<AccountFormShortServiceModel>(a =>
						a.Balance == inputModel.Balance
						&& a.CurrencyId == inputModel.CurrencyId
						&& a.Name == inputModel.Name
						&& a.OwnerId == inputModel.OwnerId
						&& a.AccountTypeId == inputModel.AccountTypeId)))
				.ReturnsAsync(newAccId);

			controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (RedirectToActionResult)await controller.Create(inputModel);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.ActionName, Is.EqualTo("AccountDetails"));

			Assert.That(result.RouteValues, Is.Not.Null);			
			CheckRouteValues(result.RouteValues, "id", newAccId);

			CheckTempDataMessage(controller.TempData, "You create a new account successfully!");
		}

		[Test]
		public async Task Create_ShouldCatchExceptionAndReturnViewResultWithModelErrors_WhenTryToCreateAccountWithExistingName()
		{
			//Arrange
			var inputModel = new AccountFormViewModel
			{
				Name = "Test Account",
				Balance = 100,
				CurrencyId = "1",
				AccountTypeId = "1",
				OwnerId = this.userId
			};

			accountsServiceMock
				.Setup(x => x.CreateAccount(It.IsAny<AccountFormShortServiceModel>()))
				.Throws<ArgumentException>();

			//Act
			ViewResult viewResult = (ViewResult)await controller.Create(inputModel);

			//Assert
			Assert.That(viewResult, Is.Not.Null);
			CheckModelStateErrors(viewResult.ViewData.ModelState, nameof(inputModel.Name),
				"You already have Account with that name.");
			CheckAccountFormViewModel(viewResult.Model as AccountFormViewModel, inputModel);
		}

		[Test]
		public async Task AccountDetailsOnGet_ShouldReturnViewModel_WhenUserIsOwner()
		{
			//Arrange
			expAccDetailsDto.OwnerId = this.userId;

			userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

			accountsServiceMock.Setup(x => x
				.GetAccountDetails(expAccDetailsDto.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>(), this.userId, false))
				.ReturnsAsync(expAccDetailsDto);

			//Act
			ViewResult viewResult = (ViewResult)await controller.AccountDetails(expAccDetailsDto.Id);

			//Assert
			Assert.That(viewResult, Is.Not.Null);
			CheckAccountDetailsViewModel(viewResult.Model as AccountDetailsViewModel, expAccDetailsDto, isUserAdmin: false);
		}

		[Test]
		public async Task AccountDetailsOnGet_ShouldReturnViewModel_WhenUserIsNotOwnerButIsAdmin()
		{
			//Arrange
			expAccDetailsDto.OwnerId = "owner id";

			userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(true);

			accountsServiceMock.Setup(x => x
				.GetAccountDetails(expAccDetailsDto.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>(), this.userId, true))
				.ReturnsAsync(expAccDetailsDto);

			//Act
			ViewResult viewResult = (ViewResult)await controller.AccountDetails(expAccDetailsDto.Id);

			//Assert
			Assert.That(viewResult, Is.Not.Null);
			CheckAccountDetailsViewModel(viewResult.Model as AccountDetailsViewModel, expAccDetailsDto, isUserAdmin: true);
		}

		[Test]
		public async Task AccountDetailsOnGet_ShouldReturnBadRequest_WhenUserNotOwnerOrAdminOrAccountDoesNotExist()
		{
			//Arrange
			userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

			accountsServiceMock.Setup(x => x
				.GetAccountDetails(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), this.userId, false))
				.Throws<InvalidOperationException>();

			//Act
			BadRequestResult result = (BadRequestResult)await controller.AccountDetails("account id");

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public async Task AccountDetailsOnPost_ShouldReturnViewModel_WhenUserIsNotOwnerButIsAdmin()
		{
			//Arrange
			var inputModel = new AccountDetailsInputModel
			{
				Id = expAccDetailsDto.Id,
				StartDate = DateTime.UtcNow.AddMonths(-1),
				EndDate = DateTime.UtcNow
			};
			expAccDetailsDto.OwnerId = "not owner id";
			expAccDetailsDto.StartDate = inputModel.StartDate;
			expAccDetailsDto.EndDate = inputModel.EndDate;

			userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(true);

			accountsServiceMock.Setup(x => x
				.GetAccountDetails(inputModel.Id, inputModel.StartDate, inputModel.EndDate, this.userId, true))
				.ReturnsAsync(expAccDetailsDto);

			//Act
			ViewResult viewResult = (ViewResult)await controller.AccountDetails(inputModel);

			//Assert
			Assert.That(viewResult, Is.Not.Null);
			CheckAccountDetailsViewModel(viewResult.Model as AccountDetailsViewModel,
				expAccDetailsDto, isUserAdmin: true, isPostRequest: true);
		}

		[Test]
		public async Task AccountDetailsOnPost_ShouldReturnViewModel_WhenUserIsOwner()
		{
			//Arrange
			var inputModel = new AccountDetailsInputModel
			{
				Id = expAccDetailsDto.Id,
				StartDate = DateTime.UtcNow.AddMonths(-1),
				EndDate = DateTime.UtcNow
			};
			expAccDetailsDto.OwnerId = this.userId;
			expAccDetailsDto.StartDate = inputModel.StartDate;
			expAccDetailsDto.EndDate = inputModel.EndDate;

			userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

			accountsServiceMock.Setup(x => x
				.GetAccountDetails(inputModel.Id, inputModel.StartDate, inputModel.EndDate, this.userId, false))
				.ReturnsAsync(expAccDetailsDto);

			//Act
			ViewResult viewResult = (ViewResult)await controller.AccountDetails(inputModel);

			//Assert
			Assert.That(viewResult, Is.Not.Null);
			CheckAccountDetailsViewModel(viewResult.Model as AccountDetailsViewModel,
				expAccDetailsDto, isUserAdmin: false, isPostRequest: true);
		}

		[Test]
		public async Task AccountDetailsOnPost_ShouldReturnBadRequest_WhenUserNotOwnerOrAdminOrAccountDoesNotExist()
		{
			//Arrange
			var inputModel = new AccountDetailsInputModel
			{
				Id = expAccDetailsDto.Id,
				StartDate = DateTime.UtcNow.AddMonths(-1),
				EndDate = DateTime.UtcNow
			};

			userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

			accountsServiceMock.Setup(x => x
				.GetAccountDetails(expAccDetailsDto.Id, inputModel.StartDate, inputModel.EndDate, this.userId, false))
				.Throws<InvalidOperationException>();

			//Act
			BadRequestResult result = (BadRequestResult)await controller.AccountDetails(inputModel);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public async Task AccountDetailsOnPost_ShouldReturnViewModelWithErrors_WhenModelIsInvalidButAccountExists()
		{
			//Arrange
			var inputModel = new AccountDetailsInputModel
			{
				Id = expAccDetailsDto.Id,
				EndDate = DateTime.UtcNow
			};

			var serviceReturnModel = new AccountDetailsShortServiceModel
			{
				Name = expAccDetailsDto.Name,
				Balance = expAccDetailsDto.Balance,
				CurrencyName = expAccDetailsDto.CurrencyName
			};

			controller.ModelState.AddModelError(nameof(inputModel.StartDate), "Start Date is invalid");

			accountsServiceMock
				.Setup(x => x.GetAccountShortDetails(inputModel.Id))
				.ReturnsAsync(serviceReturnModel);

			//Act
			ViewResult viewResult = (ViewResult)await controller.AccountDetails(inputModel);

			//Assert
			Assert.That(viewResult, Is.Not.Null);
			CheckModelStateErrors(viewResult.ViewData.ModelState, nameof(inputModel.StartDate), "Start Date is invalid");

			var model = viewResult.Model as AccountDetailsViewModel;
			Assert.That(model, Is.Not.Null);
			Assert.That(model.Name, Is.EqualTo(serviceReturnModel.Name));
			Assert.That(model.Balance, Is.EqualTo(serviceReturnModel.Balance));
			Assert.That(model.CurrencyName, Is.EqualTo(serviceReturnModel.CurrencyName));
		}

		[Test]
		public async Task AccountDetailsOnPost_ShouldReturnBadRequest_WhenModelIsInvalidAndAccountDoesNotExist()
		{
			//Arrange
			var inputModel = new AccountDetailsInputModel
			{
				Id = "invalid id",
				EndDate = DateTime.UtcNow
			};

			controller.ModelState.AddModelError(string.Empty, "Model is invalid");

			accountsServiceMock
				.Setup(x => x.GetAccountShortDetails(inputModel.Id))
				.Throws<InvalidOperationException>();

			//Act
			BadRequestResult result = (BadRequestResult)await controller.AccountDetails(inputModel);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public async Task DeleteOnGet_ShouldReturnCorrectViewModel_WhenInputIsValid()
		{
			//Arrange
			string accountId = "id";
			string accountName = "name";

			userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(false);

			accountsServiceMock.Setup(x => x
				.GetAccountName(accountId, this.userId, false))
				.ReturnsAsync(accountName);

			//Act
			ViewResult viewResult = (ViewResult)await controller.Delete(accountId);

			//Assert
			Assert.That(viewResult, Is.Not.Null);

			var model = viewResult.Model as DeleteAccountViewModel;
			Assert.That(model, Is.Not.Null);
			Assert.That(model.Name, Is.EqualTo(accountName));
			Assert.That(model.ShouldDeleteTransactions, Is.False);
		}

		[Test]
		public async Task DeleteOnGet_ShouldReturnBadRequest_WhenInputIsInvalid()
		{
			//Arrange
			userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(false);

			accountsServiceMock.Setup(x => x
				.GetAccountName(It.IsAny<string>(), this.userId, false))
				.Throws<InvalidOperationException>();

			//Act
			BadRequestResult result = (BadRequestResult)await controller.Delete("account id");

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public async Task DeleteOnGet_ShouldReturnViewModelWithModelErrors_WhenModelIsInvalid()
		{
			//Arrange
			controller.ModelState.AddModelError(string.Empty, "Model is invalid");

			//Act
			BadRequestResult result = (BadRequestResult)await controller.Delete("account id");

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public async Task DeleteOnPost_ShouldRedirectWhenAccountIsDeletedAndUserIsOwner()
		{
			//Arrange
			var inputModel = new DeleteAccountInputModel
			{
				Id = "account id",
				ConfirmButton = "accept",
				ReturnUrl = "returnUrl",
				ShouldDeleteTransactions = false
			};

			userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(false);
			controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			RedirectToActionResult result =
				(RedirectToActionResult)await controller.Delete(inputModel);

			accountsServiceMock.Verify(x =>
				x.DeleteAccount(inputModel.Id, this.userId, false, false),
				Times.Once);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.ControllerName, Is.EqualTo("Home"));
			Assert.That(result.ActionName, Is.EqualTo("Index"));

			CheckTempDataMessage(controller.TempData, "Your account was successfully deleted!");
		}

		[Test]
		public async Task DeleteOnPost_ShouldRedirectWhenAccountIsDeletedAndUserIsAdmin()
		{
			//Arrange
			var inputModel = new DeleteAccountInputModel
			{
				Id = "account id",
				ConfirmButton = "button",
				ReturnUrl = "returnUrl",
				ShouldDeleteTransactions = false
			};

			string ownerId = "owner id";

			userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(true);

			accountsServiceMock.Setup(x => x.GetOwnerId(inputModel.Id)).ReturnsAsync(ownerId);

			controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			LocalRedirectResult result =
				(LocalRedirectResult)await controller.Delete(inputModel);

			accountsServiceMock.Verify(x =>
				x.DeleteAccount(inputModel.Id, this.userId, true, false),
				Times.Once);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Url, Is.EqualTo("/Admin/Users/Details/" + ownerId));

			CheckTempDataMessage(controller.TempData, "You successfully delete user's account!");
		}

		[Test]
		public async Task DeleteOnPost_ShouldCatchExcepctionAndReturnUnauthorized_WhenUserIsUnauthorized()
		{
			//Arrange
			var inputModel = new DeleteAccountInputModel
			{
				Id = "account id",
				ConfirmButton = "accept",
				ReturnUrl = "returnUrl",
				ShouldDeleteTransactions = false
			};

			userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(false);

			accountsServiceMock.Setup(x =>
				x.DeleteAccount(inputModel.Id, this.userId, false, false))
				.Throws<ArgumentException>();

			//Act
			UnauthorizedResult result =
				(UnauthorizedResult)await controller.Delete(inputModel);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.StatusCode, Is.EqualTo(401));
		}

		[Test]
		public async Task DeleteOnPost_ShouldCatchExcepctionAndReturnBadRequest_WhenDeleteWasUnsuccessful()
		{
			//Arrange
			var inputModel = new DeleteAccountInputModel
			{
				Id = "account id",
				ConfirmButton = "accept",
				ReturnUrl = "returnUrl",
				ShouldDeleteTransactions = false
			};

			userMock.Setup(x => x
				.IsInRole(RoleConstants.AdminRoleName))
				.Returns(false);

			accountsServiceMock.Setup(x =>
				x.DeleteAccount(inputModel.Id, this.userId, false, false))
				.Throws<InvalidOperationException>();

			//Act
			BadRequestResult result =
				(BadRequestResult)await controller.Delete(inputModel);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public async Task DeleteOnPost_ShouldLocalRedirect_WhenRejectButtonIsClicked()
		{
			//Arrange
			var inputModel = new DeleteAccountInputModel
			{
				Id = "account id",
				ConfirmButton = "reject",
				ReturnUrl = "returnUrl",
				ShouldDeleteTransactions = false
			};

			//Act
			LocalRedirectResult result =
				(LocalRedirectResult)await controller.Delete(inputModel);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Url, Is.EqualTo(inputModel.ReturnUrl));
		}

		[Test]
		public async Task DeleteOnPost_ShouldReturnBadRequest_WhenModelIsInvalid()
		{
			//Arrange
			var inputModel = new DeleteAccountInputModel
			{
				Id = "account id",
				ConfirmButton = "accept",
				ReturnUrl = "returnUrl",
				ShouldDeleteTransactions = false
			};

			controller.ModelState.AddModelError(string.Empty, "Model is invalid.");

			//Act
			BadRequestResult result =
				(BadRequestResult)await controller.Delete(inputModel);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public async Task EditAccountOnGet_ShouldReturnAccountFormViewModel_WhenUserIsOwner()
		{
			//Arrange
			string accId = "acc id";
			var expServiceDto = new AccountFormServiceModel
			{
				Name = "name",
				AccountTypeId = "1",
				CurrencyId = "1",
				Balance = 100,
				OwnerId = this.userId,
				AccountTypes = new AccountTypeServiceModel[]
				{
					new AccountTypeServiceModel
					{
						Id = "1",
						Name = "AccType 1"
					},
					new AccountTypeServiceModel
					{
						Id = "2",
						Name = "AccType 2"
					}
				},
				Currencies = new CurrencyServiceModel[]
				{
					new CurrencyServiceModel
					{
						Id = "1",
						Name = "Currency 1"
					},
					new CurrencyServiceModel
					{
						Id = "2",
						Name = "Currency 2"
					}
				}
			};

			accountsServiceMock.Setup(x => x
				.GetAccountFormData(accId, this.userId, false))
				.ReturnsAsync(expServiceDto);

			//Act
			ViewResult viewResult = (ViewResult)await controller.EditAccount(accId);

			//Assert
			Assert.That(viewResult, Is.Not.Null);
			var model = viewResult.Model as AccountFormViewModel;
			Assert.That(model, Is.Not.Null);
			Assert.That(model.Balance, Is.EqualTo(expServiceDto.Balance));
			Assert.That(model.CurrencyId, Is.EqualTo(expServiceDto.CurrencyId));
			Assert.That(model.OwnerId, Is.EqualTo(expServiceDto.OwnerId));
			Assert.That(model.AccountTypeId, Is.EqualTo(expServiceDto.AccountTypeId));
			CheckAccountTypesAndCurrencies(
				model.AccountTypes, model.Currencies,
				expServiceDto.AccountTypes, expServiceDto.Currencies);
		}

		[Test]
		public async Task EditAccountOnGet_ShouldReturnBadRequest_WhenServiceThrowException()
		{
			//Arrange
			accountsServiceMock.Setup(x => x
				.GetAccountFormData(It.IsAny<string>(), this.userId, false))
				.Throws<InvalidOperationException>();

			//Act
			BadRequestResult result = (BadRequestResult)await controller.EditAccount("account id");

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public async Task EditAccountOnPost_ShouldReturnViewModelWithErrors_WhenModelIsInvalid()
		{
			//Arrange
			string accId = "accId";
			string returnUrl = "returnUrl";
			var inputFormModel = new AccountFormViewModel
			{
				//Name = null,
				Balance = 100,
				OwnerId = this.userId,
				CurrencyId = "1",
				AccountTypeId = "1",
			};

			controller.ModelState.AddModelError(nameof(inputFormModel.Name), "Name is invalid.");

			//Act
			ViewResult viewResult = (ViewResult)await controller.EditAccount(accId, inputFormModel, returnUrl);

			//Assert
			Assert.That(viewResult, Is.Not.Null);
			CheckModelStateErrors(viewResult.ViewData.ModelState, nameof(inputFormModel.Name), "Name is invalid.");
			CheckAccountFormViewModel(viewResult.Model as AccountFormViewModel, inputFormModel);
		}

		[Test]
		public async Task EditAccountOnPost_ShouldReturnBadRequest_WhenUserIsNotOwnerOrAdmin()
		{
			//Arrange
			string accId = "accId";
			string returnUrl = "returnUrl";
			var inputFormModel = new AccountFormViewModel
			{
				Name = "Account name",
				Balance = 100,
				OwnerId = "owner Id",
				CurrencyId = "1",
				AccountTypeId = "1",
			};

			userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

			//Act
			var viewResult = (BadRequestResult)await controller.EditAccount(accId, inputFormModel, returnUrl);

			//Assert
			Assert.That(viewResult, Is.Not.Null);
			Assert.That(viewResult.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public async Task EditAccountOnPost_ShouldPassToServiceModelAndRedirectToReturnUrl_WhenUserIsOwnerAndAccountIsEdited()
		{
			//Arrange
			string accId = "accId";
			string returnUrl = "returnUrl";
			var inputFormModel = new AccountFormViewModel
			{
				Name = "Account name",
				Balance = 100,
				OwnerId = this.userId,
				CurrencyId = "1",
				AccountTypeId = "1",
			};

			userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

			controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (LocalRedirectResult)await controller.EditAccount(accId, inputFormModel, returnUrl);

			accountsServiceMock.Verify(x => x.EditAccount(accId,
				It.Is<AccountFormShortServiceModel>(m =>
					m.CurrencyId == inputFormModel.CurrencyId
					&& m.Balance == inputFormModel.Balance
					&& m.OwnerId == inputFormModel.OwnerId
					&& m.AccountTypeId == inputFormModel.AccountTypeId
					&& m.Name == inputFormModel.Name)),
				Times.Once);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Url, Is.EqualTo(returnUrl));
			CheckTempDataMessage(controller.TempData, "Your account was successfully edited!");
		}

		[Test]
		public async Task EditAccountOnPost_ShouldPassToServiceModelAndRedirectToReturnUrl_WhenUserIsAdminAndAccountIsEdited()
		{
			//Arrange
			string accId = "accId";
			string returnUrl = "returnUrl";
			string ownerId = "owner id";
			var inputFormModel = new AccountFormViewModel
			{
				Name = "Account name",
				Balance = 100,
				OwnerId = ownerId,
				CurrencyId = "1",
				AccountTypeId = "1",
			};

			userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(true);
			accountsServiceMock.Setup(x => x.GetOwnerId(accId)).ReturnsAsync(ownerId);
			controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (LocalRedirectResult)await controller.EditAccount(accId, inputFormModel, returnUrl);

			accountsServiceMock.Verify(x => x.EditAccount(accId,
				It.Is<AccountFormShortServiceModel>(m =>
					m.CurrencyId == inputFormModel.CurrencyId
					&& m.Balance == inputFormModel.Balance
					&& m.OwnerId == inputFormModel.OwnerId
					&& m.AccountTypeId == inputFormModel.AccountTypeId
					&& m.Name == inputFormModel.Name)),
				Times.Once);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Url, Is.EqualTo(returnUrl));
			CheckTempDataMessage(controller.TempData, "You successfully edited user's account!");
		}

		[Test]
		public async Task EditAccountOnPost_ShouldCatchServiceArgumentExceptionAndReturnModelWithErrors_WhenUserIsAdminAndAccountNameExist()
		{
			//Arrange
			string accId = "accId";
			string returnUrl = "returnUrl";
			string ownerId = "owner id";
			var inputFormModel = new AccountFormViewModel
			{
				Name = "Account name",
				Balance = 100,
				OwnerId = ownerId,
				CurrencyId = "1",
				AccountTypeId = "1",
			};

			userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(true);
			accountsServiceMock.Setup(x => x.GetOwnerId(accId)).ReturnsAsync(ownerId);

			accountsServiceMock.Setup(x => x.EditAccount(accId,
				It.Is<AccountFormShortServiceModel>(m =>
					m.CurrencyId == inputFormModel.CurrencyId
					&& m.Balance == inputFormModel.Balance
					&& m.OwnerId == inputFormModel.OwnerId
					&& m.AccountTypeId == inputFormModel.AccountTypeId
					&& m.Name == inputFormModel.Name)))
				.Throws<ArgumentException>();

			usersServiceMock.Setup(x => x
				.GetUserAccountTypesAndCurrencies(inputFormModel.OwnerId))
				.ReturnsAsync(expAccTypesAndCurrencies);

			//Act
			var result = (ViewResult)await controller.EditAccount(accId, inputFormModel, returnUrl);

			//Assert
			Assert.That(result, Is.Not.Null);

			CheckModelStateErrors(result.ViewData.ModelState, nameof(inputFormModel.Name),
				$"The user already have Account with \"{inputFormModel.Name}\" name.");

			CheckAccountFormViewModel(result.Model as AccountFormViewModel, inputFormModel);
		}
		
		[Test]
		public async Task EditAccountOnPost_ShouldCatchServiceArgumentExceptionAndReturnModelWithErrors_WhenUserIsOwnerAndAccountNameExist()
		{
			//Arrange
			string accId = "accId";
			string returnUrl = "returnUrl";
			var inputFormModel = new AccountFormViewModel
			{
				Name = "Account name",
				Balance = 100,
				OwnerId = this.userId,
				CurrencyId = "1",
				AccountTypeId = "1",
			};

			userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

			accountsServiceMock.Setup(x => x.EditAccount(accId,
				It.Is<AccountFormShortServiceModel>(m =>
					m.CurrencyId == inputFormModel.CurrencyId
					&& m.Balance == inputFormModel.Balance
					&& m.OwnerId == inputFormModel.OwnerId
					&& m.AccountTypeId == inputFormModel.AccountTypeId
					&& m.Name == inputFormModel.Name)))
				.Throws<ArgumentException>();

			usersServiceMock.Setup(x => x
				.GetUserAccountTypesAndCurrencies(inputFormModel.OwnerId))
				.ReturnsAsync(expAccTypesAndCurrencies);

			//Act
			var result = (ViewResult)await controller.EditAccount(accId, inputFormModel, returnUrl);

			//Assert
			Assert.That(result, Is.Not.Null);

			CheckModelStateErrors(result.ViewData.ModelState, nameof(inputFormModel.Name),
				$"You already have Account with \"{inputFormModel.Name}\" name.");

			CheckAccountFormViewModel(result.Model as AccountFormViewModel, inputFormModel);
		}

		[Test]
		public async Task EditAccountOnPost_ShouldCatchServiceInvalidOperationExceptionAndReturnBadRequest_WhenAccountDoesNotExist()
		{
			//Arrange
			string accId = "accId";
			string returnUrl = "returnUrl";
			var inputFormModel = new AccountFormViewModel
			{
				Name = "Account name",
				Balance = 100,
				OwnerId = this.userId,
				CurrencyId = "1",
				AccountTypeId = "1",
			};

			userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

			accountsServiceMock.Setup(x => x.EditAccount(accId,
				It.Is<AccountFormShortServiceModel>(m =>
					m.CurrencyId == inputFormModel.CurrencyId
					&& m.Balance == inputFormModel.Balance
					&& m.OwnerId == inputFormModel.OwnerId
					&& m.AccountTypeId == inputFormModel.AccountTypeId
					&& m.Name == inputFormModel.Name)))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await controller.EditAccount(accId, inputFormModel, returnUrl);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.StatusCode, Is.EqualTo(400));
		}

		private void CheckAccountFormViewModel(
			AccountFormViewModel? viewModel,
			AccountFormViewModel? inputModel = null)
		{
			Assert.That(viewModel, Is.Not.Null);

			if (inputModel == null)
			{
				Assert.That(viewModel.OwnerId, Is.EqualTo(this.userId));
				Assert.That(viewModel.Name, Is.Null);
				Assert.That(viewModel.Balance, Is.EqualTo(0));
				Assert.That(viewModel.CurrencyId, Is.Null);
				Assert.That(viewModel.AccountTypeId, Is.Null);
			}
			else
			{
				Assert.That(viewModel.OwnerId, Is.EqualTo(inputModel.OwnerId));
				Assert.That(viewModel.Name, Is.EqualTo(inputModel.Name));
				Assert.That(viewModel.Balance, Is.EqualTo(inputModel.Balance));
				Assert.That(viewModel.CurrencyId, Is.EqualTo(inputModel.CurrencyId));
				Assert.That(viewModel.AccountTypeId, Is.EqualTo(inputModel.AccountTypeId));
			}

			if (inputModel != null)
			{
				CheckAccountTypesAndCurrencies(
					viewModel.AccountTypes, viewModel.Currencies,
					inputModel.AccountTypes, inputModel.Currencies);
			}
		}

		private void CheckAccountTypesAndCurrencies(
			IEnumerable<AccountTypeServiceModel> actualAccountTypes,
			IEnumerable<CurrencyServiceModel> actualCurrencies,
			IEnumerable<AccountTypeServiceModel> expAccountTypes,
			IEnumerable<CurrencyServiceModel> expCurrencies)
		{
			Assert.That(actualAccountTypes.Count(),
				Is.EqualTo(expAccountTypes.Count()));

			for (int i = 0; i < expAccTypesAndCurrencies.AccountTypes.Count(); i++)
			{
				Assert.That(actualAccountTypes.ElementAt(i).Id,
					Is.EqualTo(expAccountTypes.ElementAt(i).Id));
				Assert.That(actualAccountTypes.ElementAt(i).Name,
					Is.EqualTo(expAccountTypes.ElementAt(i).Name));
			}

			Assert.That(actualCurrencies.Count(),
				Is.EqualTo(expCurrencies.Count()));

			for (int i = 0; i < expCurrencies.Count(); i++)
			{
				Assert.That(actualCurrencies.ElementAt(i).Id,
					Is.EqualTo(expCurrencies.ElementAt(i).Id));
				Assert.That(actualCurrencies.ElementAt(i).Name,
					Is.EqualTo(expCurrencies.ElementAt(i).Name));
			}
		}

		private static void CheckAccountDetailsViewModel(
			AccountDetailsViewModel? viewModel,
			AccountDetailsServiceModel serviceModel,
			bool isUserAdmin, bool isPostRequest = false)
		{
			Assert.That(viewModel, Is.Not.Null);
			Assert.That(viewModel.Id, Is.EqualTo(serviceModel.Id));
			Assert.That(viewModel.Name, Is.EqualTo(serviceModel.Name));
			Assert.That(viewModel.Balance, Is.EqualTo(serviceModel.Balance));
			Assert.That(viewModel.CurrencyName, Is.EqualTo(serviceModel.CurrencyName));
			Assert.That(viewModel.OwnerId, Is.EqualTo(serviceModel.OwnerId));

			if (isPostRequest)
			{
				Assert.That(viewModel.StartDate, Is.EqualTo(serviceModel.StartDate));
				Assert.That(viewModel.EndDate, Is.EqualTo(serviceModel.EndDate));
			}

			Assert.That(viewModel.Transactions.Count(), Is.EqualTo(serviceModel.Transactions.Count()));
			for (int i = 0; i < serviceModel.Transactions.Count(); i++)
			{
				Assert.That(viewModel.Transactions.ElementAt(i).Id,
					Is.EqualTo(serviceModel.Transactions.ElementAt(i).Id));
				Assert.That(viewModel.Transactions.ElementAt(i).Amount,
					Is.EqualTo(serviceModel.Transactions.ElementAt(i).Amount));
				Assert.That(viewModel.Transactions.ElementAt(i).Refference,
					Is.EqualTo(serviceModel.Transactions.ElementAt(i).Refference));
				Assert.That(viewModel.Transactions.ElementAt(i).TransactionType,
					Is.EqualTo(serviceModel.Transactions.ElementAt(i).TransactionType));
				Assert.That(viewModel.Transactions.ElementAt(i).AccountCurrencyName,
					Is.EqualTo(serviceModel.Transactions.ElementAt(i).AccountCurrencyName));
				Assert.That(viewModel.Transactions.ElementAt(i).CreatedOn,
					Is.EqualTo(serviceModel.Transactions.ElementAt(i).CreatedOn));
			}

			Assert.That(viewModel.Pagination.TotalElements, Is.EqualTo(serviceModel.TotalAccountTransactions));
			Assert.That(viewModel.Pagination.ElementsName, Is.EqualTo(PaginationConstants.TransactionsName));
			Assert.That(viewModel.Pagination.ElementsPerPage, Is.EqualTo(PaginationConstants.TransactionsPerPage));
			Assert.That(viewModel.Pagination.Page, Is.EqualTo(1));

			Assert.That(viewModel.ApiTransactionsEndpoint, Is.EqualTo(HostConstants.ApiAccountTransactionsUrl));

			Assert.That(viewModel.Routing.Area, isUserAdmin ? Is.EqualTo("Admin") : Is.EqualTo(string.Empty));
			Assert.That(viewModel.Routing.Controller, Is.EqualTo("Accounts"));
			Assert.That(viewModel.Routing.Action, Is.EqualTo("AccountDetails"));

			string expectedReturnUrl = (isUserAdmin
				? "/Admin/Accounts/AccountDetails/"
				: "/Accounts/AccountDetails/")
				+ serviceModel.Id;

			Assert.That(viewModel.Routing.ReturnUrl, Is.EqualTo(expectedReturnUrl));
		}
	}
}
