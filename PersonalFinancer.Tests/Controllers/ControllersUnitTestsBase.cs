namespace PersonalFinancer.Tests.Controllers
{
	using AutoMapper;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using Microsoft.AspNetCore.Mvc.ViewFeatures;
	using Microsoft.AspNetCore.Routing;
	using Microsoft.Extensions.Logging;
	using Moq;
	using Newtonsoft.Json;
	using NUnit.Framework;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Services.Accounts;
	using PersonalFinancer.Services.Users;
	using PersonalFinancer.Web;
	using PersonalFinancer.Web.Models.Shared;
	using System.Security.Claims;

	[TestFixture]
	abstract class ControllersUnitTestsBase : UnitTestsBase
	{
		protected IMapper mapper;
		protected Mock<IAccountsUpdateService> accountsUpdateServiceMock;
		protected Mock<IAccountsInfoService> accountsInfoServiceMock;
		protected Mock<IUsersService> usersServiceMock;
		protected Mock<ClaimsPrincipal> userMock;

		protected Guid userId = Guid.NewGuid();

		[SetUp]
		protected void SetUpBase()
		{
			var config = new MapperConfiguration(cfg => cfg.AddProfile<ControllerMappingProfile>());
			this.mapper = config.CreateMapper();

			this.accountsUpdateServiceMock = new Mock<IAccountsUpdateService>();
			this.accountsInfoServiceMock = new Mock<IAccountsInfoService>();
			this.usersServiceMock = new Mock<IUsersService>();
			this.userMock = new Mock<ClaimsPrincipal>();

			this.userMock
				.Setup(x => x.FindFirst(ClaimTypes.NameIdentifier))
				.Returns(new Claim(ClaimTypes.NameIdentifier, this.userId.ToString()));
		}

		protected static void AssertModelStateErrorIsEqual(ModelStateDictionary modelState, string key, string errorMessage)
		{
			Assert.Multiple(() =>
			{
				Assert.That(modelState.Keys.Count(), Is.EqualTo(1));
				Assert.That(modelState.Keys.First(), Is.EqualTo(key));
				Assert.That(modelState.Values.Count(), Is.EqualTo(1));
				Assert.That(modelState.Values.First().Errors, Has.Count.EqualTo(1));
				Assert.That(modelState.Values.First().Errors.First().ErrorMessage, Is.EqualTo(errorMessage));
			});
		}

		protected static void AssertPaginationModelIsEqual(
			PaginationModel model, string elementsName, int elementsPerPage, int totalElements, int page)
		{
			int firstElement = totalElements > 0
				? (elementsPerPage * (page - 1)) + 1
				: 0;

			int lastElement = elementsPerPage * page;

			if (lastElement > totalElements)
				lastElement = totalElements;

			int pages = totalElements / elementsPerPage;

			if (totalElements % elementsPerPage != 0)
				pages++;

			if (pages == 0)
				pages = 1;

			Assert.Multiple(() =>
			{
				Assert.That(model.FirstElement, Is.EqualTo(firstElement));
				Assert.That(model.LastElement, Is.EqualTo(lastElement));
				Assert.That(model.ElementsName, Is.EqualTo(elementsName));
				Assert.That(model.TotalElements, Is.EqualTo(totalElements));
				Assert.That(model.ElementsPerPage, Is.EqualTo(elementsPerPage));
				Assert.That(model.Page, Is.EqualTo(page));
				Assert.That(model.Pages, Is.EqualTo(pages));
			});
		}

		protected static void AssertTempDataMessageIsEqual(ITempDataDictionary tempData, string expectedMessage)
		{
			Assert.Multiple(() =>
			{
				Assert.That(tempData.Keys, Has.Count.EqualTo(1));
				Assert.That(tempData.Keys.First(), Is.EqualTo(ResponseMessages.TempDataKey));
				Assert.That(tempData.Values, Has.Count.EqualTo(1));
				Assert.That(tempData.Values.First(), Is.EqualTo(expectedMessage));
			});
		}

		protected static void AssertRouteValueIsEqual(
			RouteValueDictionary routeValues, string key, object value, int totalRouteValues = 1)
		{
			Assert.Multiple(() =>
			{
				Assert.That(routeValues, Is.Not.Null);
				Assert.That(routeValues.Keys, Has.Count.EqualTo(totalRouteValues));
				Assert.That(routeValues.ContainsKey(key), Is.True);
				Assert.That(routeValues.Values, Has.Count.EqualTo(totalRouteValues));
				Assert.That(routeValues.Values.Contains(value), Is.True);
			});
		}

		protected static bool ValidateObjectsAreEqual(object x, object y) 
			=> JsonConvert.SerializeObject(x).Equals(JsonConvert.SerializeObject(y));

		protected static void VerifyLoggerLogWarning<T>(Mock<ILogger<T>> logger, string expectedLogMessage)
			where T : ControllerBase
		{
			logger.Verify(
				x => x.Log(
					It.Is<LogLevel>(l => l == LogLevel.Warning),
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => 
						v.ToString()!
						.Contains(expectedLogMessage)),
					It.IsAny<Exception>(),
					(Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
				Times.Once);
		}
	}
}
