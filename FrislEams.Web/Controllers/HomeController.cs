using FrislEams.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrislEams.Web.Controllers;

public class HomeController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.TotalAssets = await db.Assets.CountAsync();
        ViewBag.PendingAssignments = await db.AssetAssignments.CountAsync(a => a.Status == "Pending");
        ViewBag.Alerts = await db.RfidEvents.CountAsync(r => r.AlertTriggered);
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}
