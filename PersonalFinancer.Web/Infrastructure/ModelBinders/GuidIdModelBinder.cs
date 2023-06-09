namespace PersonalFinancer.Web.Infrastructure.ModelBinders
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class GuidIdModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            string? fieldValue = bindingContext.ValueProvider.GetValue(bindingContext.FieldName).FirstValue;

            if (fieldValue != Guid.Empty.ToString())
            {
                bindingContext.Result = ModelBindingResult.Success(fieldValue);
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();

                bindingContext.ModelState.AddModelError(
                    bindingContext.FieldName,
                    $"{bindingContext.FieldName} is required.");
            }

            return Task.CompletedTask;
        }
    }
}
