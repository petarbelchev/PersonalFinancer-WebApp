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
	using static PersonalFinancer.Common.Constants.RoleConstants;
	using static PersonalFinancer.Common.Constants.UrlPathConstants;

	[TestFixture]
	internal class HomeControllerTests : ControllersUnitTestsBase
	{
		private readonly UserDashboardDTO expUserDashboard = new()
		{
			Accounts = new AccountCardDTO[]
			{
				new AccountCardDTO
				{
					Id = Guid.NewGuid(),
					Balance = 100,
					CurrencyName = "Test Currency",
					Name = "Test Name",
					OwnerId = Guid.NewGuid()
				}
			},
			CurrenciesCashFlow = new CurrencyCashFlowWithExpensesByCategoriesDTO[]
			{
				new CurrencyCashFlowWithExpensesByCategoriesDTO
				{
					Name = "Test Currency",
					Incomes = 200,
					Expenses = 100,
					ExpensesByCategories = new CategoryExpensesDTO[]
					{
						new CategoryExpensesDTO
						{
							CategoryName = "Food",
							ExpensesAmount = 100
						}
					}
				}
			},
			LastTransactions = new TransactionTableDTO[]
			{
				new TransactionTableDTO
				{
					Id = Guid.NewGuid(),
					AccountCurrencyName = "Test Currency",
					Amount = 100,
					CategoryName = "Test Category",
					CreatedOnLocalTime = DateTime.Now,
					Reference = "Test Reference",
					TransactionType = TransactionType.Expense.ToString()
				}
			}
		};
		private readonly AccountCardDTO[] expAccountCard = new AccountCardDTO[]
		{
			new AccountCardDTO
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

			this.usersServiceMock.Setup(x => x
				.GetUserAccountsCardsAsync(this.userId))
				.ReturnsAsync(this.expAccountCard);

			this.controller = new HomeController(this.usersServiceMock.Object, this.mapper)
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
			this.userMock.Setup(x => x.IsInRole(AdminRoleName)).Returns(false);
			this.userMock.Setup(x => x.Identity!.IsAuthenticated).Returns(true);

			//Act
			var viewResult = (ViewResult)await this.controller.Index();
			UserDashboardViewModel viewModel = viewResult.Model as UserDashboardViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

			//Assert
			AssertSamePropertiesValuesAreEqual(viewModel, this.expUserDashboard);
		}

		[Test]
		public async Task Index_ShouldReturnEmptyResult_WhenUserIsNotAuthenticated()
		{
			//Arrange
			this.userMock.Setup(x => x.IsInRole(AdminRoleName)).Returns(false);
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
			this.userMock.Setup(x => x.IsInRole(AdminRoleName)).Returns(true);

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
				FromLocalTime = DateTime.Now.AddDays(-1),
				ToLocalTime = DateTime.Now
			};

			//Act
			var viewResult = (ViewResult)await this.controller.Index(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(viewResult, Is.Not.Null);

				UserDashboardViewModel viewModel = viewResult.Model as UserDashboardViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(viewModel, this.expUserDashboard);
			});
		}

		[Test]
		public async Task Index_ShouldReturnCorrectViewModel_WhenInputIsInvalid()
		{
			//Arrange
			var inputModel = new DateFilterModel
			{
				FromLocalTime = DateTime.Now,
				ToLocalTime = DateTime.Now.AddDays(-1)
			};
			string errorMessage = "error message";

			//Act
			this.controller.ModelState.AddModelError("error", errorMessage);
			var viewResult = (ViewResult)await this.controller.Index(inputModel);

			//Assert
			Assert.Multiple(() =>
			{
				AssertModelStateErrorIsEqual(viewResult.ViewData.ModelState, "error", errorMessage);

				UserDashboardViewModel viewModel = viewResult.Model as UserDashboardViewModel ??
					throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				Assert.That(viewModel.CurrenciesCashFlow.Count, Is.EqualTo(0));
				Assert.That(viewModel.LastTransactions.Count, Is.EqualTo(0));
				Assert.That(viewModel.FromLocalTime, Is.EqualTo(inputModel.FromLocalTime));
				Assert.That(viewModel.ToLocalTime, Is.EqualTo(inputModel.ToLocalTime));

				AssertSamePropertiesValuesAreEqual(viewModel.Accounts, this.expAccountCard);
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
					Is.EqualTo(BadRequestImgPath));
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
					Is.EqualTo(InternalServerErrorImgPath));
			});
		}
	}
}
