namespace PersonalFinancer.Tests.Helpers
{
	using NUnit.Framework;
	using PersonalFinancer.Web.Extensions;
	using System.Security.Claims;

	[TestFixture]
	internal class ClaimsPrincipalExtensionsTests
	{
		[Test]
		public void GetUserUsername_ShouldReturnCorrectData()
		{
			//Assert
			var claim = new Claim(ClaimTypes.Name, "User Full Name");
			var identity = new ClaimsIdentity(new[] { claim });
			var claimsPrincipal = new ClaimsPrincipal(identity);

			//Act
			string result = claimsPrincipal.GetUserUsername();

			//Assert
			Assert.That(result, Is.EqualTo(claim.Value));
		}

		[Test]
		public void IsAuthenticated_ShouldReturnCorrectData_WhenThePrincipalIsUnauthenticated()
		{
			//Assert
			var identity = new ClaimsIdentity();
			var claimsPrincipal = new ClaimsPrincipal(identity);

			//Act
			bool actual = claimsPrincipal.IsAuthenticated();

			//Assert
			Assert.That(actual, Is.False);
		}

		[Test]
		public void IsAuthenticated_ShouldReturnCorrectData_WhenThePrincipalIsAuthenticated()
		{
			//Assert
			var identity = new ClaimsIdentity("Test Authentication");
			var claimsPrincipal = new ClaimsPrincipal(identity);

			//Act
			bool actual = claimsPrincipal.IsAuthenticated();

			//Assert
			Assert.That(actual, Is.True);
		}

		[Test]
		public void IsAuthenticated_ShouldReturnCorrectData_WhenThePrincipalIsNull()
		{
			//Assert
			var claimsPrincipal = new ClaimsPrincipal();

			//Act
			bool actual = claimsPrincipal.IsAuthenticated();

			//Assert
			Assert.That(actual, Is.False);
		}
	}
}
