namespace PersonalFinancer.Tests.Helpers
{
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using Microsoft.Extensions.Primitives;
	using Moq;
	using NUnit.Framework;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Web.CustomModelBinders;

	[TestFixture]
	internal class DateTimeModelBinderTests
	{
		private readonly string fieldName = "CreatedOn";
		private Mock<ModelBindingContext> bindingContext;
		private DateTimeModelBinder modelBinder;

		[SetUp]
		public void SetUp()
		{
			this.bindingContext = new Mock<ModelBindingContext>();
			this.modelBinder = new DateTimeModelBinder();
		}

		[Test]
		public async Task BindModelAsync_ShouldBindCorrectly_WhenValidDateTimeValueIsProvided()
		{
			// Arrange
			this.bindingContext
				.SetupGet(x => x.FieldName)
				.Returns(this.fieldName);

			var expectedValue = DateTime.Now;

			var values = new ValueProviderResult(new StringValues(expectedValue.ToString()));

			this.bindingContext
				.Setup(x => x.ValueProvider.GetValue(this.fieldName))
				.Returns(values);

			DateTime? actualValue = null;

			this.bindingContext
				.SetupSet(x => x.Result = It.IsAny<ModelBindingResult>())
				.Callback((ModelBindingResult result) => actualValue = result.Model as DateTime?);

			// Act
			await this.modelBinder.BindModelAsync(this.bindingContext.Object);

			// Assert
			Assert.That(actualValue, Is.Not.Null);
			Assert.That(actualValue?.ToString("s"), Is.EqualTo(expectedValue.ToString("s")));
		}

		[Test]
		[TestCase("invalid date")]
		[TestCase("")]
		[TestCase(null)]
		public async Task BindModelAsync_ShouldFail_WhenInvalidDateTimeValueIsProvided(string value)
		{
			// Arrange
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
					Is.EqualTo(ValidationMessages.InvalidDate));
			});
		}
	}
}
