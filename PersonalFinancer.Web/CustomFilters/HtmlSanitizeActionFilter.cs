namespace PersonalFinancer.Web.CustomFilters
{
	using Ganss.Xss;
	using Microsoft.AspNetCore.Mvc.Filters;
	using PersonalFinancer.Web.CustomAttributes;
	using System.Reflection;

	public class HtmlSanitizeActionFilter : IActionFilter
	{
		public void OnActionExecuting(ActionExecutingContext context)
		{
			string[] httpMethodsForSanitize = new string[] { "POST", "PUT", "PATCH" };

			if (httpMethodsForSanitize.Contains(context.HttpContext.Request.Method) == false
				|| context.ActionDescriptor.FilterDescriptors.Any(f => f.Filter is NoHtmlSanitizingAttribute))
			{
				return;
			}

			var sanitizer = new HtmlSanitizer();

			foreach (string key in context.ActionArguments.Keys)
			{
				object? argumentValue = context.ActionArguments[key];

				if (argumentValue == null)
					continue;

				Type argumentValueType = argumentValue.GetType();

				if (argumentValueType == typeof(string))
				{
					context.ActionArguments[key] = sanitizer.Sanitize((argumentValue as string)!);

					continue;
				}

				IEnumerable<PropertyInfo> typeProperties = argumentValueType
					.GetProperties()
					.Where(p => p.PropertyType == typeof(string));

				foreach (PropertyInfo property in typeProperties)
				{
					string? propertyValue = property.GetValue(argumentValue)?.ToString();

					if (propertyValue == null)
						continue;

					property.SetValue(context.ActionArguments[key], sanitizer.Sanitize(propertyValue));
				}
			}
		}

		public void OnActionExecuted(ActionExecutedContext context) { }
	}
}
