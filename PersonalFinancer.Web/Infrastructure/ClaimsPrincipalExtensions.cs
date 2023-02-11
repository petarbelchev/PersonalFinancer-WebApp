namespace PersonalFinancer.Web.Infrastructure
{
	using System.Security.Claims;

	public static class ClaimsPrincipalExtensions
	{
		public static string Id(this ClaimsPrincipal user)
			=> user.FindFirstValue(ClaimTypes.NameIdentifier);
	}
}
