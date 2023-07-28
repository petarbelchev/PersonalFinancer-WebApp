namespace PersonalFinancer.Tests.Helpers
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Abstractions;
	using Microsoft.AspNetCore.Mvc.Filters;
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using Microsoft.AspNetCore.Routing;
	using Moq;
	using Newtonsoft.Json;
	using NUnit.Framework;
	using PersonalFinancer.Web.CustomAttributes;
	using PersonalFinancer.Web.CustomFilters;
	using PersonalFinancer.Web.Models.Message;

	[TestFixture]
	internal class HtmlSanitizeAsyncActionFilterTests
	{
		private static readonly ReplyInputModel inputModelForSanitize = new()
		{
			MessageId = null!,
			ReplyContent = "<'123'>"
		};

		private readonly IDictionary<string, object?> arguments = new Dictionary<string, object?>
		{
			{ "id", Guid.NewGuid() },
			{ "name", "some name" },
			{ "amount", 100 },
			{ "createdOn", null },
			{ "forSanitize", "<script>alert('You are hacked!!!')</script>" },
			{ "inputModel", inputModelForSanitize }
		};

		private Mock<ActionExecutionDelegate> nextMock;
		private HtmlSanitizeAsyncActionFilter actionFilter;

		[SetUp]
		public void SetUp()
		{
			this.nextMock = new Mock<ActionExecutionDelegate>();
			this.actionFilter = new HtmlSanitizeAsyncActionFilter();
		}

		[Test]
		[TestCase("GET")]
		[TestCase("HEAD")]
		[TestCase("DELETE")]
		[TestCase("CONNECT")]
		[TestCase("OPTIONS")]
		[TestCase("TRACE")]
		public async Task ShouldContinueExecutionWithoutSanitizing_WhenTheContextMethodDoesNotRequireSanitization(string method)
		{
			//Arrange
			var httpContext = new DefaultHttpContext();
			httpContext.Request.Method = method;

			var actionContext = new ActionContext(
				httpContext,
				Mock.Of<RouteData>(),
				Mock.Of<ActionDescriptor>(),
				new ModelStateDictionary());

			var context = new ActionExecutingContext(
				actionContext,
				new List<IFilterMetadata>(),
				new Dictionary<string, object?>(this.arguments),
				Mock.Of<Controller>());

			//Act
			await this.actionFilter.OnActionExecutionAsync(context, this.nextMock.Object);

			//Assert
			this.nextMock.Verify(x => x(), Times.Once);
			Assert.That(JsonConvert.SerializeObject(context.ActionArguments),
				Is.EqualTo(JsonConvert.SerializeObject(this.arguments)));
		}

		[Test]
		[TestCase("POST")]
		[TestCase("PUT")]
		[TestCase("PATCH")]
		public async Task ShouldContinueExecutionWithoutSanitizing_WhenTheActionHaveNoHtmlSanitizingAttribute(string method)
		{
			//Arrange
			var httpContext = new DefaultHttpContext();
			httpContext.Request.Method = method;

			var actualDescriptor = new ActionDescriptor();
			actualDescriptor.FilterDescriptors = new List<FilterDescriptor>
			{
				new FilterDescriptor(new NoHtmlSanitizingAttribute(), 0)
			};

			var actionContext = new ActionContext(
				httpContext,
				Mock.Of<RouteData>(),
				actualDescriptor,
				new ModelStateDictionary());

			var context = new ActionExecutingContext(
				actionContext,
				new List<IFilterMetadata>(),
				new Dictionary<string, object?>(this.arguments),
				Mock.Of<Controller>());

			//Act
			await this.actionFilter.OnActionExecutionAsync(context, this.nextMock.Object);

			//Assert
			this.nextMock.Verify(x => x(), Times.Once);
			Assert.That(JsonConvert.SerializeObject(context.ActionArguments), 
				Is.EqualTo(JsonConvert.SerializeObject(this.arguments)));
		}

		[Test]
		[TestCase("POST")]
		[TestCase("PUT")]
		[TestCase("PATCH")]
		public async Task ShouldSanitizeTheArguments(string method)
		{
			//Arrange
			var httpContext = new DefaultHttpContext();
			httpContext.Request.Method = method;

			var actionContext = new ActionContext(
				httpContext,
				Mock.Of<RouteData>(),
				Mock.Of<ActionDescriptor>(),
				new ModelStateDictionary());

			var context = new ActionExecutingContext(
				actionContext,
				new List<IFilterMetadata>(),
				this.arguments,
				Mock.Of<Controller>());

			//Act
			await this.actionFilter.OnActionExecutionAsync(context, this.nextMock.Object);

			//Assert
			this.nextMock.Verify(x => x(), Times.Once);
			Assert.Multiple(() =>
			{
				Assert.That(context.ActionArguments["forSanitize"], Is.EqualTo(string.Empty));
				Assert.That(inputModelForSanitize.ReplyContent, Is.EqualTo("&lt;'123'&gt;"));
			});
		}
	}
}
