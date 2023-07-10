namespace PersonalFinancer.Web.CustomFilters
{
	using Microsoft.AspNetCore.Mvc.Filters;
	using PersonalFinancer.Web.CustomAttributes;
	using System.Net;
	using System.Reflection;
	using System.Threading.Tasks;

	public class HtmlEncodeAsyncActionFilter : IAsyncActionFilter
	{
		public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			string[] httpMethodsForSanitize = new string[] { "POST", "PUT", "PATCH" };

			if (!httpMethodsForSanitize.Contains(context.HttpContext.Request.Method)
				|| context.ActionDescriptor.FilterDescriptors.Any(f => f.Filter is NotRequireHtmlEncodingAttribute))
			{
				return next();
			}

			foreach (string key in context.ActionArguments.Keys)
			{
				object? argumentValue = context.ActionArguments[key];

				if (argumentValue == null)
					continue;

				Type argumentValueType = argumentValue.GetType();

				if (argumentValueType == typeof(string))
				{
					argumentValue = WebUtility.HtmlEncode(argumentValue as string);

					continue;
				}

				IEnumerable<PropertyInfo> typeProperties = argumentValueType.GetProperties()
					.Where(p => p.GetCustomAttribute<RequireHtmlEncodingAttribute>() != null);

				foreach (PropertyInfo property in typeProperties)
				{
					if (property.PropertyType == typeof(string))
					{
						string? propertyValue = property.GetValue(argumentValue)?.ToString();
						property.SetValue(argumentValue, WebUtility.HtmlEncode(propertyValue));
					}
				}
			}

			return next();
		}
	}
}
