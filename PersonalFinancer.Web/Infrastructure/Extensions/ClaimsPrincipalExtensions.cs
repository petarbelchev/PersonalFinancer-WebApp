namespace PersonalFinancer.Web.Infrastructure.Extensions
{
    using System.Security.Claims;
    using static PersonalFinancer.Data.Constants.RoleConstants;

    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Returns User's identifier, or null when user does not exist.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static Guid Id(this ClaimsPrincipal user)
            => Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

        public static bool IsAdmin(this ClaimsPrincipal user)
            => user.IsInRole(AdminRoleName);

        public static string GetUserName(this ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.Name);
    }
}
