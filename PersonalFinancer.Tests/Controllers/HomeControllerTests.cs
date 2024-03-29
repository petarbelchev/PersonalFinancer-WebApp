﻿namespace PersonalFinancer.Tests.Controllers
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Data.Models.Enums;
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Services.Users.Models;
	using PersonalFinancer.Web.Controllers;
	using PersonalFinancer.Web.Models.Home;
	using PersonalFinancer.Web.Models.Shared;
	using static PersonalFinancer.Common.Constants.RoleConstants;

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
		private readonly AccountCardDTO[] expAccountsCards = new AccountCardDTO[]
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
			this.usersServiceMock
				.Setup(x => x.GetUserDashboardDataAsync(this.userId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
				.ReturnsAsync(this.expUserDashboard);

			this.usersServiceMock
				.Setup(x => x.GetUserAccountsCardsAsync(this.userId))
				.ReturnsAsync(this.expAccountsCards);

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
		public void Error_ShouldReturnViewModel()
		{
			//Arrange
			string traceIdentifier = this.controller.ControllerContext.HttpContext.TraceIdentifier;

			//Act
			var result = (ViewResult)this.controller.Error();
			var errorViewModel = result.Model as ErrorViewModel;

			//Assert
			Assert.That(errorViewModel, Is.Not.Null);
			Assert.That(errorViewModel.RequestId, Is.EqualTo(traceIdentifier));
		}

		[Test]
		public async Task Filtered_ShouldReturnCorrectViewModel_WhenInputIsValid()
		{
			//Arrange
			var inputModel = new DateFilterModel
			{
				FromLocalTime = DateTime.Now.AddDays(-1),
				ToLocalTime = DateTime.Now
			};

			//Act
			var viewResult = (ViewResult)await this.controller.Filtered(inputModel);

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
		public async Task Filtered_ShouldReturnCorrectViewModel_WhenInputIsInvalid()
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
			var viewResult = (ViewResult)await this.controller.Filtered(inputModel);

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

				AssertSamePropertiesValuesAreEqual(viewModel.Accounts, this.expAccountsCards);
			});
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public async Task Index_ShouldReturnCorrectModel_WhenTheUserIsNotAdmin(bool isAuthenticated)
		{
			//Arrange			
			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(false);

			this.userMock
				.Setup(x => x.Identity!.IsAuthenticated)
				.Returns(isAuthenticated);

			//Act
			var viewResult = (ViewResult)await this.controller.Index();

			//Assert
			if (isAuthenticated)
			{
				UserDashboardViewModel viewModel = viewResult.Model as UserDashboardViewModel ??
						throw new InvalidOperationException($"{nameof(viewResult.Model)} should not be null.");

				AssertSamePropertiesValuesAreEqual(viewModel, this.expUserDashboard);
			}
			else
			{
				Assert.Multiple(() =>
				{
					Assert.That(viewResult, Is.Not.Null);
					Assert.That(viewResult.Model, Is.Null);
				});
			}
		}

		[Test]
		public async Task Index_ShouldRedirectToAdminArea_WhenTheUserIsAdmin()
		{
			//Arrange
			this.userMock
				.Setup(x => x.IsInRole(AdminRoleName))
				.Returns(true);

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
		[TestCase(400)]
		[TestCase(401)]
		[TestCase(404)]
		public void StatusCodePage_ShouldReturnCorrectStatusCodePageViewModel(int statusCode)
		{
			//Arrange
			var expected = new StatusCodePageViewModel();

			if (statusCode == 400)
			{
				expected.Title = "Bad request";
				expected.Message = "Something went wrong. Please try again or contact us.";
			}
			else if (statusCode == 401)
			{
				expected.Title = "Access denied";
				expected.Message = "You do not have access to this resource.";
			}
			else if (statusCode == 404)
			{
				expected.Title = "Not found";
				expected.Message = "The page you are looking for does not exist.";
			}

			//Act
			var viewResult = (ViewResult)this.controller.StatusCodePage(statusCode);

			//Assert
			Assert.Multiple(() =>
			{
				var actual = viewResult.Model as StatusCodePageViewModel;
				AssertSamePropertiesValuesAreEqual(actual!, expected);
			});
		}
	}
}
