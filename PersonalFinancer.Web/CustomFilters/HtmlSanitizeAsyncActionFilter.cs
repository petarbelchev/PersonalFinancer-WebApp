﻿namespace PersonalFinancer.Web.CustomFilters
{
	using Ganss.Xss;
	using Microsoft.AspNetCore.Mvc.Filters;
	using PersonalFinancer.Web.CustomAttributes;
	using System.Reflection;
	using System.Threading.Tasks;

	public class HtmlSanitizeAsyncActionFilter : IAsyncActionFilter
	{
		public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			string[] httpMethodsForSanitize = new string[] { "POST", "PUT", "PATCH" };

			if (httpMethodsForSanitize.Contains(context.HttpContext.Request.Method) == false
				|| context.ActionDescriptor.FilterDescriptors.Any(f => f.Filter is NoHtmlSanitizingAttribute))
			{
				return next();
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
					argumentValue = sanitizer.Sanitize((argumentValue as string)!);

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

					property.SetValue(argumentValue, sanitizer.Sanitize(propertyValue));
				}
			}

			return next();
		}
	}
}