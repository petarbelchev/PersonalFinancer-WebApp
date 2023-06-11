﻿namespace PersonalFinancer.Web.Infrastructure.ModelBinders
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class DateTimeModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            string? fieldValue = bindingContext.ValueProvider.GetValue(bindingContext.FieldName).FirstValue;

            if (DateTime.TryParse(fieldValue, out DateTime resultValue))
            {
                bindingContext.Result = ModelBindingResult.Success(resultValue);
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();

                bindingContext.ModelState.AddModelError(
                    bindingContext.FieldName, $"Please enter a valid date.");
            }

            return Task.CompletedTask;
        }
    }
}