using System.Security.Claims;

namespace Store.WebUI.Helpers
{
    public static class PermissionHelper
    {
        /// <summary>
        /// Checks whether the current user is authorized to view soft-deleted records.
        /// Currently returns true to simulate Admin access until full role management is active.
        /// </summary>
        public static bool CanViewDeletedRecords(ClaimsPrincipal? user)
        {
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                // Temporarily allow Admin access by default for development/testing as requested
                return true;
            }

            // When full role management is activated, verify Admin role or specific claim:
            // return user.IsInRole("Admin") || user.HasClaim("permission", "view_deleted");

            return true;
        }
    }
}
