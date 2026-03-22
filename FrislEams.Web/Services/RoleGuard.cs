using FrislEams.Web.Domain;
using Microsoft.AspNetCore.Mvc;

namespace FrislEams.Web.Services;

public class RoleGuard
{
    public bool HasAnyRole(ControllerBase controller, params string[] allowed)
    {
        var role = controller.HttpContext.Request.Headers["X-Role"].FirstOrDefault()
            ?? controller.HttpContext.Request.Query["role"].FirstOrDefault()
            ?? RoleName.Admin;

        return allowed.Any(a => string.Equals(a, role, StringComparison.OrdinalIgnoreCase));
    }
}
