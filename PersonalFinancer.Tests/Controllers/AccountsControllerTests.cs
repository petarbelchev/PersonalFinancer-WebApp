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

		private static AccountDetailsDTO expectedAccountDetailsDto;

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

				CreateEditAccountViewModel viewModel = viewResult.Model as CreateEditAccountViewModel ??
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
			var inputModel = new CreateEditAccountViewModel
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
		public async Task Create_ShouldReturnBadRequest_WhenOwnerIdIsDifferentFromUserId()
		{
			//Arrange
			var inputModel = new CreateEditAccountViewModel
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
			var inputModel = new CreateEditAccountViewModel
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
					It.Is<CreateEditAccountInputDTO>(a =>
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
				Assert.That(result.ActionName, Is.EqualTo("Details"));
				AssertTempDataMessageIsEqual(this.controller.TempData, "You create a new account successfully!");

				Assert.That(result.RouteValues, Is.Not.Null);
				AssertRouteValueIsEqual(result.RouteValues!, "id", newAccId);
			});
		}

		[Test]
		public async Task Create_ShouldCatchExceptionAndReturnViewResultWithModelErrors_WhenTryToCreateAccountWithExistingName()
		{
			//Arrange
			var inputModel = new CreateEditAccountViewModel
			{
				Name = "Test Account",
				Balance = 100,
				CurrencyId = Guid.NewGuid(),
				AccountTypeId = Guid.NewGuid(),
				OwnerId = this.userId
			};

			string errorMessage = string.Format(ExceptionMessages.ExistingUserEntityName, "account", inputModel.Name);

			this.accountsUpdateServiceMock.Setup(x => x
				.CreateAccountAsync(It.Is<CreateEditAccountInputDTO>(
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

				CreateEditAccountViewModel viewModel = viewResult.Model as CreateEditAccountViewModel ??
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
				.GetAccountDetailsAsync(expectedAccountDetailsDto.Id, this.userId, false))
				.ReturnsAsync(expectedAccountDetailsDto);

			//Act
			var viewResult = (ViewResult)await this.controller.Details(expectedAccountDetailsDto.Id, null);

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
			string returnUrl = "return url";

			this.userMock.Setup(x => x.IsInRole(AdminRoleName)).Returns(true);

			this.accountsInfoServiceMock.Setup(x => x
				.GetAccountDetailsAsync(expectedAccountDetailsDto.Id, this.userId, true))
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
		public async Task AccountDetailsOnGet_ShouldReturnBadRequest_WhenUserNotOwnerOrAdminOrAccountDoesNotExist()
		{
			//Arrange
			this.userMock.Setup(x => x
				.IsInRole(AdminRoleName))
				.Returns(false);

			this.accountsInfoServiceMock.Setup(x => x
				.GetAccountDetailsAsync(It.IsAny<Guid>(), this.userId, false))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await this.controller.Details(Guid.NewGuid(), null);

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
				FromLocalTime = DateTime.Now.AddMonths(-1),
				ToLocalTime = DateTime.Now
			};

			expectedAccountDetailsDto.OwnerId = Guid.NewGuid();

			this.userMock.Setup(x => x
				.IsInRole(AdminRoleName))
				.Returns(true);

			this.accountsInfoServiceMock.Setup(x => x
				.GetAccountDetailsAsync(expectedAccountDetailsDto.Id, this.userId, true))
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
		public async Task AccountDetailsOnPost_ShouldReturnViewModel_WhenUserIsOwner()
		{
			//Arrange
			var inputModel = new AccountDetailsInputModel
			{
				Id = expectedAccountDetailsDto.Id,
				FromLocalTime = DateTime.Now.AddMonths(-1),
				ToLocalTime = DateTime.Now
			};

			expectedAccountDetailsDto.OwnerId = this.userId;

			this.userMock.Setup(x => x.IsInRole(AdminRoleName)).Returns(false);

			this.accountsInfoServiceMock.Setup(x => x
				.GetAccountDetailsAsync(expectedAccountDetailsDto.Id, this.userId, false))
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
		public async Task AccountDetailsOnPost_ShouldReturnBadRequest_WhenUserNotOwnerOrAdminOrAccountDoesNotExist()
		{
			//Arrange
			var inputModel = new AccountDetailsInputModel
			{
				Id = expectedAccountDetailsDto.Id,
				FromLocalTime = DateTime.Now.AddMonths(-1),
				ToLocalTime = DateTime.Now
			};

			this.userMock.Setup(x => x.IsInRole(AdminRoleName)).Returns(false);

			this.accountsInfoServiceMock.Setup(x => x
				.GetAccountDetailsAsync(expectedAccountDetailsDto.Id, this.userId, false))
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

		[Test]
		public async Task AccountDetailsOnPost_ShouldReturnViewModelWithErrors_WhenDatesAreInvalidButAccountExists()
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
		public async Task AccountDetailsOnPost_ShouldReturnBadRequest_WhenModelIsInvalidAndAccountDoesNotExist()
		{
			//Arrange
			Guid accountId = Guid.NewGuid();
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
			var result = (BadRequestResult)await this.controller.Delete(Guid.NewGuid(), null);

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
			var result = (BadRequestResult)await this.controller.Delete(Guid.NewGuid(), null);

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
				ShouldDeleteTransactions = false
			};

			this.userMock.Setup(x => x
				.IsInRole(AdminRoleName))
				.Returns(false);

			this.controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (LocalRedirectResult)await this.controller.Delete(inputModel);

			this.accountsUpdateServiceMock.Verify(x => x
				.DeleteAccountAsync(accountId, this.userId, false, false),
				Times.Once);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Url, Is.EqualTo("/"));

				AssertTempDataMessageIsEqual(this.controller.TempData, "Your account was successfully deleted!");
			});
		}

		[Test]
		public async Task DeleteOnPost_ShouldRedirectWhenAccountIsDeletedAndUserIsAdmin()
		{
			//Arrange
			var accountId = Guid.NewGuid();
			var ownerId = Guid.NewGuid();
			var inputModel = new DeleteAccountInputModel
			{
				Id = accountId,
				ConfirmButton = "button",
				ShouldDeleteTransactions = false,
				ReturnUrl = "/Admin/Users/Details/" + ownerId,
			};

			this.userMock.Setup(x => x
				.IsInRole(AdminRoleName))
				.Returns(true);

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
		public async Task DeleteOnPost_ShouldReturnBadRequest_WhenModelIsInvalid()
		{
			//Arrange
			var inputModel = new DeleteAccountInputModel
			{
				Id = Guid.NewGuid(),
				ConfirmButton = "accept",
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

			this.accountsInfoServiceMock.Setup(x => x
				.GetAccountFormDataAsync(accId, this.userId, false))
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
		public async Task EditAccountOnGet_ShouldReturnBadRequest_WhenServiceThrowException()
		{
			//Arrange
			this.accountsInfoServiceMock.Setup(x => x
				.GetAccountFormDataAsync(It.IsAny<Guid>(), this.userId, false))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await this.controller.Edit(Guid.NewGuid());

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
			var inputModel = new CreateEditAccountViewModel
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
		public async Task EditAccountOnPost_ShouldReturnBadRequest_WhenUserIsNotOwnerOrAdmin()
		{
			//Arrange
			var accId = Guid.NewGuid();
			var inputFormModel = new CreateEditAccountViewModel
			{
				Name = "Account name",
				Balance = 100,
				OwnerId = Guid.NewGuid(),
				CurrencyId = Guid.NewGuid(),
				AccountTypeId = Guid.NewGuid()
			};

			this.userMock.Setup(x => x.IsInRole(AdminRoleName)).Returns(false);

			//Act
			var viewResult = (BadRequestResult)await this.controller.Edit(accId, inputFormModel);

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
			var inputFormModel = new CreateEditAccountInputModel
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
			var result = (RedirectToActionResult)await this.controller.Edit(accId, inputFormModel);

			this.accountsUpdateServiceMock.Verify(x => x.EditAccountAsync(accId,
				It.Is<CreateEditAccountInputDTO>(m =>
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
				Assert.That(result.ActionName, Is.EqualTo("Details"));
				AssertRouteValueIsEqual(result.RouteValues!, "Id", accId);
				AssertTempDataMessageIsEqual(this.controller.TempData, "Your account was successfully edited!");
			});
		}

		[Test]
		public async Task EditAccountOnPost_ShouldPassToServiceModelAndRedirectToReturnUrl_WhenUserIsAdminAndAccountIsEdited()
		{
			//Arrange
			var accId = Guid.NewGuid();
			var ownerId = Guid.NewGuid();
			var inputFormModel = new CreateEditAccountInputModel
			{
				Name = "Account name",
				Balance = 100,
				OwnerId = ownerId,
				CurrencyId = Guid.NewGuid(),
				AccountTypeId = Guid.NewGuid(),
			};

			this.userMock.Setup(x => x.IsInRole(AdminRoleName)).Returns(true);
			this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			//Act
			var result = (RedirectToActionResult)await this.controller.Edit(accId, inputFormModel);

			this.accountsUpdateServiceMock.Verify(x => x.EditAccountAsync(accId,
				It.Is<CreateEditAccountInputDTO>(m =>
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
				Assert.That(result.ActionName, Is.EqualTo("Details"));
				AssertRouteValueIsEqual(result.RouteValues!, "Id", accId);
				AssertTempDataMessageIsEqual(this.controller.TempData, "You successfully edited user's account!");
			});
		}

		[Test]
		public async Task EditAccountOnPost_ShouldCatchServiceArgumentExceptionAndReturnModelWithErrors_WhenUserIsAdminAndAccountNameExist()
		{
			//Arrange
			var accId = Guid.NewGuid();
			var ownerId = Guid.NewGuid();
			var inputModel = new CreateEditAccountInputModel
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

			this.accountsUpdateServiceMock.Setup(x => x
				.EditAccountAsync(accId, It.Is<CreateEditAccountInputDTO>(m =>
					m.CurrencyId == inputModel.CurrencyId
					&& m.Balance == inputModel.Balance
					&& m.OwnerId == inputModel.OwnerId
					&& m.AccountTypeId == inputModel.AccountTypeId
					&& m.Name == inputModel.Name)))
				.Throws<ArgumentException>();

			this.usersServiceMock.Setup(x => x
				.GetUserAccountTypesAndCurrenciesDropdownsAsync(ownerId))
				.ReturnsAsync(expAccountTypesAndCurrencies);

			//Act
			var viewResult = (ViewResult)await this.controller.Edit(accId, inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				AssertModelStateErrorIsEqual(viewResult.ViewData.ModelState, nameof(inputModel.Name),
					string.Format(ExceptionMessages.AdminExistingUserEntityName, "account", inputModel.Name));

				CreateEditAccountViewModel viewModel = viewResult.Model as CreateEditAccountViewModel ??
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
			var inputModel = new CreateEditAccountInputModel
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
				It.Is<CreateEditAccountInputDTO>(m =>
					m.CurrencyId == inputModel.CurrencyId
					&& m.Balance == inputModel.Balance
					&& m.OwnerId == inputModel.OwnerId
					&& m.AccountTypeId == inputModel.AccountTypeId
					&& m.Name == inputModel.Name)))
				.Throws(new ArgumentException(errorMessage));

			Guid userId = inputModel.OwnerId ?? throw new InvalidOperationException();

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
		public async Task EditAccountOnPost_ShouldCatchServiceInvalidOperationExceptionAndReturnBadRequest_WhenAccountDoesNotExist()
		{
			//Arrange
			var accId = Guid.NewGuid();
			var inputFormModel = new CreateEditAccountViewModel
			{
				Name = "Account name",
				Balance = 100,
				OwnerId = this.userId,
				CurrencyId = Guid.NewGuid(),
				AccountTypeId = Guid.NewGuid(),
			};

			this.userMock.Setup(x => x.IsInRole(AdminRoleName)).Returns(false);

			this.accountsUpdateServiceMock.Setup(x => x.EditAccountAsync(accId,
				It.Is<CreateEditAccountInputDTO>(m =>
					m.CurrencyId == inputFormModel.CurrencyId
					&& m.Balance == inputFormModel.Balance
					&& m.OwnerId == inputFormModel.OwnerId
					&& m.AccountTypeId == inputFormModel.AccountTypeId
					&& m.Name == inputFormModel.Name)))
				.Throws<InvalidOperationException>();

			//Act
			var result = (BadRequestResult)await this.controller.Edit(accId, inputFormModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.StatusCode, Is.EqualTo(400));
			});
		}
	}
}
