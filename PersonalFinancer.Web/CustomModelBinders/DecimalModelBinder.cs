namespace PersonalFinancer.Web.CustomModelBinders
{
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using System.Globalization;

	public class DecimalModelBinder : IModelBinder
	{
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			string? fieldValue = bindingContext.ValueProvider.GetValue(bindingContext.FieldName).FirstValue;
			fieldValue = fieldValue?.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
			fieldValue = fieldValue?.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

			if (decimal.TryParse(fieldValue, out decimal resultValue))
			{
				bindingContext.Result = ModelBindingResult.Success(resultValue);
			}
			else
			{
				bindingContext.Result = ModelBindingResult.Failed();

				bindingContext.ModelState.AddModelError(
					bindingContext.FieldName,
					$"The {bindingContext.FieldName} is required and must be a number.");
			}

			return Task.CompletedTask;
		}
	}
}