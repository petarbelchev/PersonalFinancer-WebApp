using System.Security.Claims;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Infrastructure.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Returns User's identifier, or null when user does not exist.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string Id(this ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.NameIdentifier);

        public static bool IsAdmin(this ClaimsPrincipal user)
            => user.IsInRole(AdminRoleName);
    }
}
