namespace PersonalFinancer.Web.Infrastructure
{
	using System.Security.Claims;

	public static class ClaimsPrincipalExtensions
	{
		/// <summary>
		/// Returns User's identifier, or null when user does not exist.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public static string Id(this ClaimsPrincipal user)
			=> user.FindFirstValue(ClaimTypes.NameIdentifier);
	}
}
