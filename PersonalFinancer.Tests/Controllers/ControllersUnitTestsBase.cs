namespace PersonalFinancer.Tests.Controllers
{
	using AutoMapper;
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using Microsoft.AspNetCore.Mvc.ViewFeatures;
	using Microsoft.AspNetCore.Routing;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Services.Accounts;
	using PersonalFinancer.Services.User;
	using PersonalFinancer.Tests.Mocks;
	using System.Security.Claims;

	[TestFixture]
	abstract class ControllersUnitTestsBase
	{
		protected IMapper mapper = ControllersMapperMock.Instance;
		protected Mock<IAccountsUpdateService> accountsUpdateServiceMock;
		protected Mock<IAccountsInfoService> accountsInfoServiceMock;
		protected Mock<IUsersService> usersServiceMock;
		protected Mock<ClaimsPrincipal> userMock;

		protected Guid userId = Guid.NewGuid();

		[SetUp]
		protected void SetUpBase()
		{
			this.accountsUpdateServiceMock = new Mock<IAccountsUpdateService>();
			this.accountsInfoServiceMock = new Mock<IAccountsInfoService>();
			this.usersServiceMock = new Mock<IUsersService>();
			this.userMock = new Mock<ClaimsPrincipal>();

			this.userMock.Setup(x => x
				.FindFirst(ClaimTypes.NameIdentifier))
				.Returns(new Claim(ClaimTypes.NameIdentifier, this.userId.ToString()));
		}

		protected static void CheckModelStateErrors(
			ModelStateDictionary modelState, string key, string errorMessage)
		{
			Assert.Multiple(() =>
			{
				Assert.That(modelState.Keys.Count(), Is.EqualTo(1));
				Assert.That(modelState.Keys.First(), Is.EqualTo(key));
				Assert.That(modelState.Values.Count(), Is.EqualTo(1));
				Assert.That(modelState.Values.First().Errors, Has.Count.EqualTo(1));
				Assert.That(modelState.Values.First().Errors.First().ErrorMessage,
					Is.EqualTo(errorMessage));
			});
		}

		protected static void CheckTempDataMessage(
			ITempDataDictionary tempData, string expectedMessage)
		{
			Assert.Multiple(() =>
			{
				Assert.That(tempData.Keys, Has.Count.EqualTo(1));
				Assert.That(tempData.Keys.First(), Is.EqualTo(ResponseMessages.TempDataKey));
				Assert.That(tempData.Values, Has.Count.EqualTo(1));
				Assert.That(tempData.Values.First(), Is.EqualTo(expectedMessage));
			});
		}

		protected static void CheckRouteValues(
			RouteValueDictionary routeValues, string key, Guid value)
		{
			Assert.Multiple(() =>
			{
				Assert.That(routeValues, Is.Not.Null);
				Assert.That(routeValues.Keys, Has.Count.EqualTo(1));
				Assert.That(routeValues.ContainsKey(key), Is.True);
				Assert.That(routeValues.Values, Has.Count.EqualTo(1));
				Assert.That(routeValues.Values.First(), Is.EqualTo(value));
			});
		}
	}
}
