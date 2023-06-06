using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.User;
using PersonalFinancer.Tests.Mocks;
using System.Security.Claims;

namespace PersonalFinancer.Tests
{
	[TestFixture]
	abstract class ControllersUnitTestsBase
	{
		protected IMapper mapper = ControllersMapperMock.Instance;
		protected Mock<IAccountsService> accountsServiceMock;
		protected Mock<IUsersService> usersServiceMock;
		protected Mock<ClaimsPrincipal> userMock;
		
		protected string userId = "user Id";

		[SetUp]
		protected void SetUpBase()
		{			
			this.accountsServiceMock = new Mock<IAccountsService>();
			this.usersServiceMock = new Mock<IUsersService>();
			userMock = new Mock<ClaimsPrincipal>();

			userMock.Setup(x => x.FindFirst(ClaimTypes.NameIdentifier))
				.Returns(new Claim(ClaimTypes.NameIdentifier, userId));
		}

		protected void CheckModelStateErrors(
			ModelStateDictionary modelState, string key, string errorMessage)
		{
			Assert.That(modelState.Keys.Count(), Is.EqualTo(1));
			Assert.That(modelState.Keys.First(), Is.EqualTo(key));
			Assert.That(modelState.Values.Count(), Is.EqualTo(1));
			Assert.That(modelState.Values.First().Errors.Count, Is.EqualTo(1));

			Assert.That(modelState.Values.First().Errors.First().ErrorMessage,
				Is.EqualTo(errorMessage));
		}

		protected void CheckTempDataMessage(
			ITempDataDictionary tempData, string expectedMessage)
		{
			Assert.That(tempData.Keys.Count(), Is.EqualTo(1));
			Assert.That(tempData.Keys.First(), Is.EqualTo("successMsg"));
			Assert.That(tempData.Values.Count(), Is.EqualTo(1));
			Assert.That(tempData.Values.First(), Is.EqualTo(expectedMessage));
		}

		protected void CheckRouteValues(RouteValueDictionary routeValues, string key, string value)
		{			
			Assert.That(routeValues, Is.Not.Null);
			Assert.That(routeValues.Keys.Count, Is.EqualTo(1));
			Assert.That(routeValues.ContainsKey(key), Is.True);
			Assert.That(routeValues.Values.Count, Is.EqualTo(1));
			Assert.That(routeValues.Values.First(), Is.EqualTo(value));
		}
	}
}
