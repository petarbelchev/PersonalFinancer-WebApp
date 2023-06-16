﻿namespace PersonalFinancer.Tests.Controllers
{
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
    using static PersonalFinancer.Data.Constants;
    using static PersonalFinancer.Services.Infrastructure.Constants;
    using static PersonalFinancer.Web.Infrastructure.Constants;

    [TestFixture]
    internal class AccountsControllerTests : ControllersUnitTestsBase
    {
        private AccountsController controller;

        private readonly UserAccountTypesAndCurrenciesServiceModel expAccTypesAndCurrencies = new()
        {
            AccountTypes = new AccountTypeServiceModel[]
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
                },
            Currencies = new CurrencyServiceModel[]
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
                }
        };
        private readonly AccountDetailsServiceModel expAccDetailsDto = new()
        {
            Id = Guid.NewGuid(),
            Balance = 100,
            CurrencyName = "Currency",
            Name = "Account Name",
            Transactions = new TransactionTableServiceModel[]
                {
                    new TransactionTableServiceModel
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
                .Setup(x => x.GetUserAccountTypesAndCurrenciesAsync(this.userId))
                .ReturnsAsync(this.expAccTypesAndCurrencies);

            this.controller = new AccountsController(
                this.accountsServiceMock.Object,
                this.mapper,
                this.usersServiceMock.Object)
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
            Assert.That(viewResult, Is.Not.Null);
            this.CheckAccountFormViewModel(viewResult.Model as AccountFormViewModel);
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

                CheckModelStateErrors(
                    viewResult.ViewData.ModelState,
                    nameof(inputModel.Balance),
                    "Balance is invalid.");

                this.CheckAccountFormViewModel(viewResult.Model as AccountFormViewModel, inputModel);
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

            this.accountsServiceMock.Setup(x => x
                .CreateAccountAsync(
                    It.Is<AccountFormShortServiceModel>(a =>
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
                CheckTempDataMessage(this.controller.TempData, "You create a new account successfully!");
            });

            Assert.That(result.RouteValues, Is.Not.Null);
            CheckRouteValues(result.RouteValues, "id", newAccId);
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

            this.accountsServiceMock
                .Setup(x => x.CreateAccountAsync(It.IsAny<AccountFormShortServiceModel>()))
                .Throws<ArgumentException>();

            //Act
            var viewResult = (ViewResult)await this.controller.Create(inputModel);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(viewResult, Is.Not.Null);
                CheckModelStateErrors(
                    viewResult.ViewData.ModelState,
                    nameof(inputModel.Name),
                    "You already have Account with that name.");
                this.CheckAccountFormViewModel(viewResult.Model as AccountFormViewModel, inputModel);
            });
        }

        [Test]
        public async Task AccountDetailsOnGet_ShouldReturnViewModel_WhenUserIsOwner()
        {
            //Arrange
            this.expAccDetailsDto.OwnerId = this.userId;

            this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

            this.accountsServiceMock.Setup(x => x
                .GetAccountDetailsAsync(
                    this.expAccDetailsDto.Id,
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    this.userId,
                    false))
                .ReturnsAsync(this.expAccDetailsDto);

            //Act
            var viewResult = (ViewResult)await this.controller.AccountDetails(this.expAccDetailsDto.Id);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(viewResult, Is.Not.Null);
                CheckAccountDetailsViewModel(
                    viewResult.Model as AccountDetailsViewModel,
                    this.expAccDetailsDto,
                    isUserAdmin: false);
            });
        }

        [Test]
        public async Task AccountDetailsOnGet_ShouldReturnViewModel_WhenUserIsNotOwnerButIsAdmin()
        {
            //Arrange
            this.expAccDetailsDto.OwnerId = Guid.NewGuid();

            this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(true);

            this.accountsServiceMock.Setup(x => x
                .GetAccountDetailsAsync(this.expAccDetailsDto.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>(), this.userId, true))
                .ReturnsAsync(this.expAccDetailsDto);

            //Act
            var viewResult = (ViewResult)await this.controller.AccountDetails(this.expAccDetailsDto.Id);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(viewResult, Is.Not.Null);
                CheckAccountDetailsViewModel(
                    viewResult.Model as AccountDetailsViewModel,
                    this.expAccDetailsDto,
                    isUserAdmin: true);
            });
        }

        [Test]
        public async Task AccountDetailsOnGet_ShouldReturnBadRequest_WhenUserNotOwnerOrAdminOrAccountDoesNotExist()
        {
            //Arrange
            this.userMock.Setup(x => x
                .IsInRole(RoleConstants.AdminRoleName))
                .Returns(false);

            this.accountsServiceMock.Setup(x => x
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
                Id = this.expAccDetailsDto.Id,
                StartDate = DateTime.UtcNow.AddMonths(-1),
                EndDate = DateTime.UtcNow
            };
            this.expAccDetailsDto.OwnerId = Guid.NewGuid();
            this.expAccDetailsDto.StartDate = inputModel.StartDate;
            this.expAccDetailsDto.EndDate = inputModel.EndDate;

            this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(true);

            this.accountsServiceMock.Setup(x => x
                .GetAccountDetailsAsync(this.expAccDetailsDto.Id, inputModel.StartDate, inputModel.EndDate, this.userId, true))
                .ReturnsAsync(this.expAccDetailsDto);

            //Act
            var viewResult = (ViewResult)await this.controller.AccountDetails(inputModel);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(viewResult, Is.Not.Null);
                CheckAccountDetailsViewModel(
                    viewResult.Model as AccountDetailsViewModel,
                    this.expAccDetailsDto,
                    isUserAdmin: true,
                    isPostRequest: true);
            });
        }

        [Test]
        public async Task AccountDetailsOnPost_ShouldReturnViewModel_WhenUserIsOwner()
        {
            //Arrange
            var inputModel = new AccountDetailsInputModel
            {
                Id = this.expAccDetailsDto.Id,
                StartDate = DateTime.UtcNow.AddMonths(-1),
                EndDate = DateTime.UtcNow
            };
            this.expAccDetailsDto.OwnerId = this.userId;
            this.expAccDetailsDto.StartDate = inputModel.StartDate;
            this.expAccDetailsDto.EndDate = inputModel.EndDate;

            this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

            this.accountsServiceMock.Setup(x => x
                .GetAccountDetailsAsync(this.expAccDetailsDto.Id, inputModel.StartDate, inputModel.EndDate, this.userId, false))
                .ReturnsAsync(this.expAccDetailsDto);

            //Act
            var viewResult = (ViewResult)await this.controller.AccountDetails(inputModel);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(viewResult, Is.Not.Null);
                CheckAccountDetailsViewModel(
                    viewResult.Model as AccountDetailsViewModel,
                    this.expAccDetailsDto,
                    isUserAdmin: false,
                    isPostRequest: true);
            });
        }

        [Test]
        public async Task AccountDetailsOnPost_ShouldReturnBadRequest_WhenUserNotOwnerOrAdminOrAccountDoesNotExist()
        {
            //Arrange
            var inputModel = new AccountDetailsInputModel
            {
                Id = this.expAccDetailsDto.Id,
                StartDate = DateTime.UtcNow.AddMonths(-1),
                EndDate = DateTime.UtcNow
            };

            this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

            this.accountsServiceMock.Setup(x => x
                .GetAccountDetailsAsync(this.expAccDetailsDto.Id, inputModel.StartDate, inputModel.EndDate, this.userId, false))
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
                Id = this.expAccDetailsDto.Id,
                EndDate = DateTime.UtcNow
            };

            var serviceReturnModel = new AccountDetailsShortServiceModel
            {
                Name = this.expAccDetailsDto.Name,
                Balance = this.expAccDetailsDto.Balance,
                CurrencyName = this.expAccDetailsDto.CurrencyName
            };

            this.controller.ModelState.AddModelError(nameof(inputModel.StartDate), "Start Date is invalid");

            this.accountsServiceMock
                .Setup(x => x.GetAccountShortDetailsAsync(this.expAccDetailsDto.Id))
                .ReturnsAsync(serviceReturnModel);

            //Act
            var viewResult = (ViewResult)await this.controller.AccountDetails(inputModel);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(viewResult, Is.Not.Null);
                CheckModelStateErrors(
                    viewResult.ViewData.ModelState,
                    nameof(inputModel.StartDate),
                    "Start Date is invalid");

                var model = viewResult.Model as AccountDetailsViewModel;
                Assert.That(model, Is.Not.Null);
                Assert.That(model!.Name, Is.EqualTo(serviceReturnModel.Name));
                Assert.That(model.Balance, Is.EqualTo(serviceReturnModel.Balance));
                Assert.That(model.CurrencyName, Is.EqualTo(serviceReturnModel.CurrencyName));
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

            this.accountsServiceMock
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
                .IsInRole(RoleConstants.AdminRoleName))
                .Returns(false);

            this.accountsServiceMock.Setup(x => x
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
                .IsInRole(RoleConstants.AdminRoleName))
                .Returns(false);

            this.accountsServiceMock.Setup(x => x
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
                .IsInRole(RoleConstants.AdminRoleName))
                .Returns(false);

            this.controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            //Act
            var result =
                (RedirectToActionResult)await this.controller.Delete(inputModel);

            this.accountsServiceMock
                .Verify(x => x.DeleteAccountAsync(accountId, this.userId, false, false),
                    Times.Once);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.ControllerName, Is.EqualTo("Home"));
                Assert.That(result.ActionName, Is.EqualTo("Index"));

                CheckTempDataMessage(this.controller.TempData, "Your account was successfully deleted!");
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
                .IsInRole(RoleConstants.AdminRoleName))
                .Returns(true);

            this.accountsServiceMock.Setup(x => x
                .GetOwnerIdAsync(accountId))
                .ReturnsAsync(ownerId);

            this.controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            //Act
            var result = (LocalRedirectResult)await this.controller.Delete(inputModel);

            this.accountsServiceMock
                .Verify(x => x.DeleteAccountAsync(accountId, this.userId, true, false),
                    Times.Once);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Url, Is.EqualTo("/Admin/Users/Details/" + ownerId));

                CheckTempDataMessage(this.controller.TempData, "You successfully delete user's account!");
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
                .IsInRole(RoleConstants.AdminRoleName))
                .Returns(false);

            this.accountsServiceMock.Setup(x =>
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
                .IsInRole(RoleConstants.AdminRoleName))
                .Returns(false);

            this.accountsServiceMock.Setup(x => x
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
            var expServiceDto = new AccountFormServiceModel
            {
                Name = "name",
                AccountTypeId = Guid.NewGuid(),
                CurrencyId = Guid.NewGuid(),
                Balance = 100,
                OwnerId = this.userId,
                AccountTypes = new AccountTypeServiceModel[]
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
                },
                Currencies = new CurrencyServiceModel[]
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
                }
            };

            this.accountsServiceMock.Setup(x => x
                .GetAccountFormDataAsync(accId, this.userId, false))
                .ReturnsAsync(expServiceDto);

            //Act
            var viewResult = (ViewResult)await this.controller.EditAccount(accId);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(viewResult, Is.Not.Null);
                var model = viewResult.Model as AccountFormViewModel;
                Assert.That(model, Is.Not.Null);
                Assert.That(model!.Balance, Is.EqualTo(expServiceDto.Balance));
                Assert.That(model.CurrencyId, Is.EqualTo(expServiceDto.CurrencyId));
                Assert.That(model.OwnerId, Is.EqualTo(expServiceDto.OwnerId));
                Assert.That(model.AccountTypeId, Is.EqualTo(expServiceDto.AccountTypeId));
                this.CheckAccountTypesAndCurrencies(
                    model.AccountTypes, model.Currencies,
                    expServiceDto.AccountTypes, expServiceDto.Currencies);
            });
        }

        [Test]
        public async Task EditAccountOnGet_ShouldReturnBadRequest_WhenServiceThrowException()
        {
            //Arrange
            this.accountsServiceMock.Setup(x => x
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
            var inputFormModel = new AccountFormViewModel
            {
                //Name = null,
                Balance = 100,
                OwnerId = this.userId,
                CurrencyId = Guid.NewGuid(),
                AccountTypeId = Guid.NewGuid(),
            };

            this.controller.ModelState.AddModelError(nameof(inputFormModel.Name), "Name is invalid.");

            //Act
            var viewResult = (ViewResult)await this.controller.EditAccount(accId, inputFormModel, returnUrl);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(viewResult, Is.Not.Null);
                CheckModelStateErrors(viewResult.ViewData.ModelState, nameof(inputFormModel.Name), "Name is invalid.");
                this.CheckAccountFormViewModel(viewResult.Model as AccountFormViewModel, inputFormModel);
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

            this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

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

            this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            //Act
            var result = (LocalRedirectResult)await this.controller.EditAccount(accId, inputFormModel, returnUrl);

            this.accountsServiceMock.Verify(x => x.EditAccountAsync(accId,
                It.Is<AccountFormShortServiceModel>(m =>
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
                CheckTempDataMessage(this.controller.TempData, "Your account was successfully edited!");
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

            this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(true);
            this.accountsServiceMock.Setup(x => x.GetOwnerIdAsync(accId)).ReturnsAsync(ownerId);
            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            //Act
            var result = (LocalRedirectResult)await this.controller.EditAccount(accId, inputFormModel, returnUrl);

            this.accountsServiceMock.Verify(x => x.EditAccountAsync(accId,
                It.Is<AccountFormShortServiceModel>(m =>
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
                CheckTempDataMessage(this.controller.TempData, "You successfully edited user's account!");
            });
        }

        [Test]
        public async Task EditAccountOnPost_ShouldCatchServiceArgumentExceptionAndReturnModelWithErrors_WhenUserIsAdminAndAccountNameExist()
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

            this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(true);
            this.accountsServiceMock.Setup(x => x.GetOwnerIdAsync(accId)).ReturnsAsync(ownerId);

            this.accountsServiceMock.Setup(x => x.EditAccountAsync(accId,
                It.Is<AccountFormShortServiceModel>(m =>
                    m.CurrencyId == inputFormModel.CurrencyId
                    && m.Balance == inputFormModel.Balance
                    && m.OwnerId == inputFormModel.OwnerId
                    && m.AccountTypeId == inputFormModel.AccountTypeId
                    && m.Name == inputFormModel.Name)))
                .Throws<ArgumentException>();

            Guid userId = inputFormModel.OwnerId ?? throw new InvalidOperationException();

            this.usersServiceMock.Setup(x => x
                .GetUserAccountTypesAndCurrenciesAsync(userId))
                .ReturnsAsync(this.expAccTypesAndCurrencies);

            //Act
            var result = (ViewResult)await this.controller.EditAccount(accId, inputFormModel, returnUrl);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);

                CheckModelStateErrors(result.ViewData.ModelState, nameof(inputFormModel.Name),
                    $"The user already have Account with \"{inputFormModel.Name}\" name.");

                this.CheckAccountFormViewModel(result.Model as AccountFormViewModel, inputFormModel);
            });
        }

        [Test]
        public async Task EditAccountOnPost_ShouldCatchServiceArgumentExceptionAndReturnModelWithErrors_WhenUserIsOwnerAndAccountNameExist()
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

            this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

            this.accountsServiceMock.Setup(x => x.EditAccountAsync(accId,
                It.Is<AccountFormShortServiceModel>(m =>
                    m.CurrencyId == inputFormModel.CurrencyId
                    && m.Balance == inputFormModel.Balance
                    && m.OwnerId == inputFormModel.OwnerId
                    && m.AccountTypeId == inputFormModel.AccountTypeId
                    && m.Name == inputFormModel.Name)))
                .Throws<ArgumentException>();

			Guid userId = inputFormModel.OwnerId ?? throw new InvalidOperationException();

			this.usersServiceMock.Setup(x => x
                .GetUserAccountTypesAndCurrenciesAsync(userId))
                .ReturnsAsync(this.expAccTypesAndCurrencies);

            //Act
            var result = (ViewResult)await this.controller.EditAccount(accId, inputFormModel, returnUrl);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);

                CheckModelStateErrors(result.ViewData.ModelState, nameof(inputFormModel.Name),
                    $"You already have Account with \"{inputFormModel.Name}\" name.");

                this.CheckAccountFormViewModel(result.Model as AccountFormViewModel, inputFormModel);
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

            this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);

            this.accountsServiceMock.Setup(x => x.EditAccountAsync(accId,
                It.Is<AccountFormShortServiceModel>(m =>
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
                this.CheckAccountTypesAndCurrencies(
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

            for (int i = 0; i < this.expAccTypesAndCurrencies.AccountTypes.Count(); i++)
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
                Assert.That(viewModel.Transactions.ElementAt(i).Reference,
                    Is.EqualTo(serviceModel.Transactions.ElementAt(i).Reference));
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
