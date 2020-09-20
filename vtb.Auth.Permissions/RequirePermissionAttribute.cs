using Microsoft.AspNetCore.Authorization;

namespace vtb.Auth.Permissions
{
    public sealed class RequirePermissionAttribute : AuthorizeAttribute
    {
        public RequirePermissionAttribute(string permissionName)
            : base($"Require_{permissionName}_Permission")
        {
        }
    }
}