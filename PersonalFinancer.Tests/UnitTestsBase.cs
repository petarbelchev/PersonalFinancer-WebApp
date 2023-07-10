namespace PersonalFinancer.Tests
{
	using Newtonsoft.Json;
	using NUnit.Framework;
	using System.Collections;
	using System.Reflection;

	[TestFixture]
	internal class UnitTestsBase
	{
		protected static void AssertAreEqualAsJson(object actual, object expected)
		{
			string actualAsJson = JsonConvert.SerializeObject(actual, Formatting.Indented);
			string expectedAsJson = JsonConvert.SerializeObject(expected, Formatting.Indented);

			Assert.That(actualAsJson, Is.EqualTo(expectedAsJson));
		}

		protected static void AssertSamePropertiesValuesAreEqual(object actual, object expected)
		{
			PropertyInfo[] propsToCompare = expected.GetType().GetProperties();

			Assert.Multiple(() =>
			{
				foreach (PropertyInfo propToCompare in propsToCompare)
				{
					PropertyInfo? actualProp = actual
						.GetType()
						.GetProperty(propToCompare.Name);

					if (actualProp == null)
						continue;

					object? expectedValue = propToCompare.GetValue(expected);
					object? actualValue = actualProp.GetValue(actual);

					if (actualProp.PropertyType != typeof(string) 
						&& actualProp.PropertyType.IsAssignableTo(typeof(IEnumerable)))
					{
						var expectedCollection = expectedValue as IEnumerable<object>;
						var actualCollection = actualValue as IEnumerable<object>;

						for (int i = 0; i < expectedCollection!.Count(); i++)
						{
							AssertSamePropertiesValuesAreEqual(
								actualCollection!.ElementAt(i), expectedCollection!.ElementAt(i));
						}

						continue;
					}

					if (actualProp.PropertyType == typeof(DateTime))
					{
						if (expectedValue == null)
							throw new InvalidOperationException($"{propToCompare.PropertyType} cannot be null.");

						if (actualValue == null)
							throw new InvalidOperationException($"{actualProp.PropertyType} cannot be null.");

						var expectedDateTime = (DateTime)expectedValue;
						expectedValue = expectedDateTime.ToUniversalTime();

						var actualDateTime = (DateTime)actualValue;
						actualValue = actualDateTime.ToUniversalTime();
					}

					Assert.That(actualValue, Is.EqualTo(expectedValue));
				}
			});
		}
	}
}
