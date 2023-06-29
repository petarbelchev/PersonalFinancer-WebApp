namespace PersonalFinancer.Web.ModelBinders
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class DecimalModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            string? fieldValue = bindingContext.ValueProvider.GetValue(bindingContext.FieldName).FirstValue;

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