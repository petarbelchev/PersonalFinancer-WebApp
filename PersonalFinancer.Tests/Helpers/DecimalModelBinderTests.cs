namespace PersonalFinancer.Tests.Helpers
{
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using Microsoft.Extensions.Primitives;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Web.CustomModelBinders;

	[TestFixture]
	internal class DecimalModelBinderTests
	{
		private readonly string fieldName = "Amount";
		private Mock<ModelBindingContext> bindingContext;
		private DecimalModelBinder modelBinder;

		[SetUp]
		public void SetUp()
		{
			this.bindingContext = new Mock<ModelBindingContext>();
			this.modelBinder = new DecimalModelBinder();
		}

		[Test]
		[TestCase("123,45")]
		[TestCase("123.45")]
		public async Task BindModelAsync_ShouldBindCorrectly_WhenValidDecimalValueIsProvided(string value)
		{
			// Arrange
			this.bindingContext
				.SetupGet(x => x.FieldName)
				.Returns(this.fieldName);

			var values = new ValueProviderResult(new StringValues(value));

			this.bindingContext
				.Setup(x => x.ValueProvider.GetValue(this.fieldName))
				.Returns(values);

			decimal expectedValue = 123.45m;

			// Act
			await this.modelBinder.BindModelAsync(this.bindingContext.Object);

			// Assert
			this.bindingContext.VerifySet(
				x => x.Result = ModelBindingResult.Success(expectedValue),
				"Binding was unsuccessful.");
		}

		[Test]
		[TestCase("123,,45")]
		[TestCase(null)]
		public async Task BindModelAsync_ShouldFail_WhenInvalidDecimalValueIsProvided(string value)
		{
			// Arrange
			this.bindingContext.Object.ModelState = new ModelStateDictionary();

			this.bindingContext
				.SetupGet(x => x.FieldName)
				.Returns(this.fieldName);

			var values = new ValueProviderResult(new StringValues(value));

			this.bindingContext
				.Setup(x => x.ValueProvider.GetValue(this.fieldName))
				.Returns(values);

			var modelStateDictionary = new ModelStateDictionary();

			this.bindingContext
				.Setup(x => x.ModelState)
				.Returns(modelStateDictionary);

			// Act
			await this.modelBinder.BindModelAsync(this.bindingContext.Object);

			// Assert
			this.bindingContext.VerifySet(
				x => x.Result = ModelBindingResult.Failed(),
				"Binding should be unsuccessful");

			Assert.Multiple(() =>
			{
				Assert.That(modelStateDictionary.Keys.Count(), Is.EqualTo(1));
				Assert.That(modelStateDictionary.Keys.First(), Is.EqualTo(this.fieldName));
				Assert.That(modelStateDictionary.Values.Count(), Is.EqualTo(1));
				Assert.That(modelStateDictionary.Values.First().Errors, Has.Count.EqualTo(1));

				Assert.That(modelStateDictionary.Values.First().Errors.First().ErrorMessage, 
					Is.EqualTo($"The {this.fieldName} is required and must be a number."));
			});
		}
	}
}
