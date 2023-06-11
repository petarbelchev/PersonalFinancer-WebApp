﻿namespace PersonalFinancer.Web.Infrastructure.Extensions
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
        public static string Id(this ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.NameIdentifier);

        /// <summary>
        /// Returns User's identifier, or null when user does not exist.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static Guid IdToGuid(this ClaimsPrincipal user)
            => Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

        public static bool IsAdmin(this ClaimsPrincipal user)
            => user.IsInRole(AdminRoleName);

        /// <summary>
        /// Returns User's username, or null when user does not exist.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GetUserName(this ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.Name);
    }
}
