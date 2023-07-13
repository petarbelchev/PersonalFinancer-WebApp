namespace PersonalFinancer.Web.CustomAttributes
{
	using Microsoft.AspNetCore.Mvc.Filters;

	[AttributeUsage(AttributeTargets.Method)]
	public class NoHtmlSanitizingAttribute : ActionFilterAttribute
	{ }
}
