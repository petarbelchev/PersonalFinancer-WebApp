namespace Microsoft.Extensions.DependencyInjection
{
    using System.Security.Claims;
    using static PersonalFinancer.Common.Constants.RoleConstants;

    public static class ClaimsPrincipalExtensions
	{
		public static string GetUserUsername(this ClaimsPrincipal user)
			=> user.FindFirstValue(ClaimTypes.Name);

		public static string Id(this ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.NameIdentifier);

        public static Guid IdToGuid(this ClaimsPrincipal user)
            => Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

        public static bool IsAdmin(this ClaimsPrincipal user)
            => user.IsInRole(AdminRoleName);

        public static bool IsAuthenticated(this ClaimsPrincipal user)
            => user.Identity?.IsAuthenticated ?? false;
    }
}
