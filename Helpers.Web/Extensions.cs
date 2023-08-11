using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace zblesk.Helpers.Web;

public static class Extensions
{
    /// <summary>
    /// Check if a Role exists, and if it doesn't, create it with the provided claims.
    /// </summary>
    public static async Task EnsureRoleExists(
        this RoleManager<IdentityRole> roleManager,
        string roleName,
        params Claim[] claims)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role == null)
        {
            role = new IdentityRole(roleName);
            await roleManager.CreateAsync(role);
            if (claims?.Length > 0)
                foreach (var claim in claims)
                    await roleManager.AddClaimAsync(role, claim);
        }
    }
}
