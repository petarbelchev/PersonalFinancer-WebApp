namespace PersonalFinancer.Tests.Controllers
{
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

    [TestFixture]
    internal class HomeControllerTests : ControllersUnitTestsBase
    {
        private readonly UserDashboardServiceModel expUserDashboard = new()
        {
            Accounts = new AccountCardServiceModel[]
            {
                new AccountCardServiceModel
                {
                    Id = Guid.NewGuid(),
                    Balance = 100,
                    CurrencyName = "Test Currency",
                    Name = "Test Name",
                    OwnerId = Guid.NewGuid()
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
                    Id = Guid.NewGuid(),
                    AccountCurrencyName = "Test Currency",
                    Amount = 100,
                    CategoryName = "Test Category",
                    CreatedOn = DateTime.UtcNow,
                    Reference = "Test Reference",
                    TransactionType = TransactionType.Expense.ToString()
                }
            }
        };
        private readonly AccountCardServiceModel[] expAccountCard = new AccountCardServiceModel[]
        {
            new AccountCardServiceModel
            {
                Id = Guid.NewGuid(),
                Balance = 150,
                CurrencyName = "Test Currency Name",
                Name = "Test Name",
                OwnerId = Guid.NewGuid()
            }
        };

        private HomeController controller;

        [SetUp]
        public void SetUp()
        {
            this.usersServiceMock.Setup(x => x
                .GetUserDashboardDataAsync(this.userId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(this.expUserDashboard);

            this.accountsServiceMock.Setup(x => x
                .GetUserAccountsAsync(this.userId))
                .ReturnsAsync(this.expAccountCard);

            this.controller = new HomeController(this.usersServiceMock.Object, this.accountsServiceMock.Object)
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
        public async Task Index_ShouldReturnCorrectModel_WhenUserIsAuthenticatedUser()
        {
            //Arrange			
            this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);
            this.userMock.Setup(x => x.Identity!.IsAuthenticated).Returns(true);

            //Act
            var viewResult = (ViewResult)await this.controller.Index();
            var actual = viewResult.Model as UserDashboardViewModel;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(actual, Is.Not.Null);

                Assert.That(actual!.Accounts.Count(),
                    Is.EqualTo(this.expUserDashboard.Accounts.Count()));

                for (int i = 0; i < this.expUserDashboard.Accounts.Count(); i++)
                {
                    Assert.That(actual.Accounts.ElementAt(i).Id,
                        Is.EqualTo(this.expUserDashboard.Accounts.ElementAt(i).Id));
                    Assert.That(actual.Accounts.ElementAt(i).Name,
                        Is.EqualTo(this.expUserDashboard.Accounts.ElementAt(i).Name));
                    Assert.That(actual.Accounts.ElementAt(i).CurrencyName,
                        Is.EqualTo(this.expUserDashboard.Accounts.ElementAt(i).CurrencyName));
                    Assert.That(actual.Accounts.ElementAt(i).Balance,
                        Is.EqualTo(this.expUserDashboard.Accounts.ElementAt(i).Balance));
                    Assert.That(actual.Accounts.ElementAt(i).OwnerId,
                        Is.EqualTo(this.expUserDashboard.Accounts.ElementAt(i).OwnerId));
                }

                Assert.That(actual.CurrenciesCashFlow.Count(),
                    Is.EqualTo(this.expUserDashboard.CurrenciesCashFlow.Count()));

                for (int i = 0; i < this.expUserDashboard.CurrenciesCashFlow.Count(); i++)
                {
                    Assert.That(actual.CurrenciesCashFlow.ElementAt(i).Name,
                        Is.EqualTo(this.expUserDashboard.CurrenciesCashFlow.ElementAt(i).Name));
                    Assert.That(actual.CurrenciesCashFlow.ElementAt(i).Incomes,
                        Is.EqualTo(this.expUserDashboard.CurrenciesCashFlow.ElementAt(i).Incomes));
                    Assert.That(actual.CurrenciesCashFlow.ElementAt(i).Expenses,
                        Is.EqualTo(this.expUserDashboard.CurrenciesCashFlow.ElementAt(i).Expenses));
                }

                Assert.That(actual.Transactions.Count(),
                    Is.EqualTo(this.expUserDashboard.LastTransactions.Count()));

                for (int i = 0; i < this.expUserDashboard.LastTransactions.Count(); i++)
                {
                    Assert.That(actual.Transactions.ElementAt(i).Amount,
                        Is.EqualTo(this.expUserDashboard.LastTransactions.ElementAt(i).Amount));
                    Assert.That(actual.Transactions.ElementAt(i).Reference,
                        Is.EqualTo(this.expUserDashboard.LastTransactions.ElementAt(i).Reference));
                    Assert.That(actual.Transactions.ElementAt(i).TransactionType,
                        Is.EqualTo(this.expUserDashboard.LastTransactions.ElementAt(i).TransactionType));
                    Assert.That(actual.Transactions.ElementAt(i).AccountCurrencyName,
                        Is.EqualTo(this.expUserDashboard.LastTransactions.ElementAt(i).AccountCurrencyName));
                    Assert.That(actual.Transactions.ElementAt(i).CategoryName,
                        Is.EqualTo(this.expUserDashboard.LastTransactions.ElementAt(i).CategoryName));
                    Assert.That(actual.Transactions.ElementAt(i).CreatedOn,
                        Is.EqualTo(this.expUserDashboard.LastTransactions.ElementAt(i).CreatedOn));
                    Assert.That(actual.Transactions.ElementAt(i).Id,
                        Is.EqualTo(this.expUserDashboard.LastTransactions.ElementAt(i).Id));
                }
            });
        }

        [Test]
        public async Task Index_ShouldReturnEmptyResult_WhenUserIsNotAuthenticated()
        {
            //Arrange
            this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(false);
            this.userMock.Setup(x => x.Identity!.IsAuthenticated).Returns(false);

            //Act
            var viewResult = (ViewResult)await this.controller.Index();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(viewResult, Is.Not.Null);
                Assert.That(viewResult.Model, Is.Null);
            });
        }

        [Test]
        public async Task Index_ShouldRedirectToAdminArea_WhenUserIsAdmin()
        {
            //Arrange
            this.userMock.Setup(x => x.IsInRole(RoleConstants.AdminRoleName)).Returns(true);

            //Act
            var redirectResult = (LocalRedirectResult)await this.controller.Index();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(redirectResult, Is.Not.Null);
                Assert.That(redirectResult.Url, Is.EqualTo("/Admin"));
            });
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
            var viewResult = (ViewResult)await this.controller.Index(inputModel);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(viewResult, Is.Not.Null);

                var actual = viewResult.Model as UserDashboardViewModel;
                Assert.That(actual, Is.Not.Null);

                Assert.That(actual!.Accounts.Count(),
                    Is.EqualTo(this.expUserDashboard.Accounts.Count()));

                for (int i = 0; i < this.expUserDashboard.Accounts.Count(); i++)
                {
                    Assert.That(actual.Accounts.ElementAt(i).Id,
                        Is.EqualTo(this.expUserDashboard.Accounts.ElementAt(i).Id));
                    Assert.That(actual.Accounts.ElementAt(i).Name,
                        Is.EqualTo(this.expUserDashboard.Accounts.ElementAt(i).Name));
                    Assert.That(actual.Accounts.ElementAt(i).CurrencyName,
                        Is.EqualTo(this.expUserDashboard.Accounts.ElementAt(i).CurrencyName));
                    Assert.That(actual.Accounts.ElementAt(i).Balance,
                        Is.EqualTo(this.expUserDashboard.Accounts.ElementAt(i).Balance));
                    Assert.That(actual.Accounts.ElementAt(i).OwnerId,
                        Is.EqualTo(this.expUserDashboard.Accounts.ElementAt(i).OwnerId));
                }

                Assert.That(actual.CurrenciesCashFlow.Count(),
                    Is.EqualTo(this.expUserDashboard.CurrenciesCashFlow.Count()));

                for (int i = 0; i < this.expUserDashboard.CurrenciesCashFlow.Count(); i++)
                {
                    Assert.That(actual.CurrenciesCashFlow.ElementAt(i).Name,
                        Is.EqualTo(this.expUserDashboard.CurrenciesCashFlow.ElementAt(i).Name));
                    Assert.That(actual.CurrenciesCashFlow.ElementAt(i).Incomes,
                        Is.EqualTo(this.expUserDashboard.CurrenciesCashFlow.ElementAt(i).Incomes));
                    Assert.That(actual.CurrenciesCashFlow.ElementAt(i).Expenses,
                        Is.EqualTo(this.expUserDashboard.CurrenciesCashFlow.ElementAt(i).Expenses));
                }

                Assert.That(actual.Transactions.Count(),
                    Is.EqualTo(this.expUserDashboard.LastTransactions.Count()));

                for (int i = 0; i < this.expUserDashboard.LastTransactions.Count(); i++)
                {
                    Assert.That(actual.Transactions.ElementAt(i).Amount,
                        Is.EqualTo(this.expUserDashboard.LastTransactions.ElementAt(i).Amount));
                    Assert.That(actual.Transactions.ElementAt(i).Reference,
                        Is.EqualTo(this.expUserDashboard.LastTransactions.ElementAt(i).Reference));
                    Assert.That(actual.Transactions.ElementAt(i).TransactionType,
                        Is.EqualTo(this.expUserDashboard.LastTransactions.ElementAt(i).TransactionType));
                    Assert.That(actual.Transactions.ElementAt(i).AccountCurrencyName,
                        Is.EqualTo(this.expUserDashboard.LastTransactions.ElementAt(i).AccountCurrencyName));
                    Assert.That(actual.Transactions.ElementAt(i).CategoryName,
                        Is.EqualTo(this.expUserDashboard.LastTransactions.ElementAt(i).CategoryName));
                    Assert.That(actual.Transactions.ElementAt(i).CreatedOn,
                        Is.EqualTo(this.expUserDashboard.LastTransactions.ElementAt(i).CreatedOn));
                    Assert.That(actual.Transactions.ElementAt(i).Id,
                        Is.EqualTo(this.expUserDashboard.LastTransactions.ElementAt(i).Id));
                }
            });
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
            this.controller.ModelState.AddModelError("error", errorMessage);
            var viewResult = (ViewResult)await this.controller.Index(inputModel);
            var actual = viewResult.Model as UserDashboardViewModel;
            var modelStateErrors = viewResult.ViewData.ModelState.Values.First().Errors;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(modelStateErrors, Has.Count.EqualTo(1));
                Assert.That(modelStateErrors.First().ErrorMessage, Is.EqualTo(errorMessage));

                Assert.That(actual, Is.Not.Null);
                Assert.That(actual!.CurrenciesCashFlow.Count, Is.EqualTo(0));
                Assert.That(actual.Transactions.Count, Is.EqualTo(0));
                Assert.That(actual.StartDate, Is.EqualTo(inputModel.StartDate));
                Assert.That(actual.EndDate, Is.EqualTo(inputModel.EndDate));
                Assert.That(actual.Accounts.Count, Is.EqualTo(this.expAccountCard.Length));

                for (int i = 0; i < this.expAccountCard.Length; i++)
                {
                    Assert.That(actual.Accounts.ElementAt(i).Id,
                        Is.EqualTo(this.expAccountCard[i].Id));
                    Assert.That(actual.Accounts.ElementAt(i).Name,
                        Is.EqualTo(this.expAccountCard[i].Name));
                    Assert.That(actual.Accounts.ElementAt(i).Balance,
                        Is.EqualTo(this.expAccountCard[i].Balance));
                    Assert.That(actual.Accounts.ElementAt(i).CurrencyName,
                        Is.EqualTo(this.expAccountCard[i].CurrencyName));
                    Assert.That(actual.Accounts.ElementAt(i).OwnerId,
                        Is.EqualTo(this.expAccountCard[i].OwnerId));
                }
            });
        }

        [Test]
        public void Error_ShouldReturnCorrectErrorImgUrl_WhenStatusCodeIs400()
        {
            //Act
            var viewResult = (ViewResult)this.controller.Error(400);
            ICollection<string> viewBagKeys = viewResult.ViewData.Keys;
            ICollection<object?> viewBagValues = viewResult.ViewData.Values;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(viewResult.ViewData, Has.Count.EqualTo(1));
                Assert.That(viewBagKeys, Has.Count.EqualTo(1));
                Assert.That(viewBagValues, Has.Count.EqualTo(1));
                Assert.That(viewBagValues.First(),
                    Is.EqualTo(HostConstants.BadRequestImgUrl));
            });
        }

        [Test]
        public void Error_ShouldReturnCorrectErrorImgUrl_WhenStatusCodeIsNot400()
        {
            //Act
            IActionResult result = this.controller.Error(500);
            var viewResult = (ViewResult)result;
            ICollection<string> viewBagKeys = viewResult.ViewData.Keys;
            ICollection<object?> viewBagValues = viewResult.ViewData.Values;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(viewResult.ViewData, Has.Count.EqualTo(1));
                Assert.That(viewBagKeys, Has.Count.EqualTo(1));
                Assert.That(viewBagValues, Has.Count.EqualTo(1));
                Assert.That(viewBagValues.First(),
                    Is.EqualTo(HostConstants.InternalServerErrorImgUrl));
            });
        }
    }
}
