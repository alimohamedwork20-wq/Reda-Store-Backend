using System.Security.Claims;

namespace Reda.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out var id) || id <= 0)
            {
                throw new UnauthorizedAccessException("Authenticated user id is missing or invalid.");
            }

            return id;
        }

        public static string? GetUserRole(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Role);
        }
    }
}
