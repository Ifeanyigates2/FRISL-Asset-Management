using Microsoft.AspNetCore.Mvc;

namespace FrislEams.Web.Controllers;

public class AccountController : Controller
{
    private static readonly Dictionary<string, (string Password, string Role, string DisplayName)> DemoUsers = new()
    {
        ["admin"]   = ("admin123",   "Admin",   "Admin User"),
        ["auditor"] = ("audit123",   "Auditor", "Audit Officer"),
        ["staff"]   = ("staff123",   "Staff",   "Staff Member"),
        ["viewer"]  = ("viewer123",  "Viewer",  "Read-Only Viewer"),
    };

    [HttpGet("/Account/Login")]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost("/Account/Login")]
    public IActionResult Login(string username, string password, string? returnUrl = null)
    {
        if (!string.IsNullOrWhiteSpace(username) &&
            DemoUsers.TryGetValue(username.ToLower().Trim(), out var user) &&
            user.Password == password)
        {
            HttpContext.Session.SetString("UserName", user.DisplayName);
            HttpContext.Session.SetString("UserRole", user.Role);
            TempData["Success"] = $"Welcome, {user.DisplayName}!";
            return Redirect(returnUrl ?? "/");
        }

        ViewBag.ReturnUrl = returnUrl;
        ViewBag.Error = "Invalid username or password. Please try again.";
        return View();
    }

    [HttpGet("/Account/Logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
