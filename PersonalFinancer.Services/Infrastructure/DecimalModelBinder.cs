using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PersonalFinancer.Services.Infrastructure
{
	public class DecimalModelBinder : IModelBinder
	{
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			if (decimal.TryParse(bindingContext.ValueProvider.GetValue("Amount").FirstValue,
				out decimal amountValue))
			{
				bindingContext.Result = ModelBindingResult.Success(amountValue);
			}
			else
			{
				bindingContext.Result = ModelBindingResult.Failed();
			}

			return Task.CompletedTask;
		}
	}
}