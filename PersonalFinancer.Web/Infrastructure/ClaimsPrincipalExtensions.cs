using System.Security.Claims;

namespace PersonalFinancer.Web.Infrastructure
{
	public static class ClaimsPrincipalExtensions
	{
		public static string Id(this ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.NameIdentifier);
	}
}
