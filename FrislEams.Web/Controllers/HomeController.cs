using FrislEams.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrislEams.Web.Controllers;

public class HomeController(DashboardService dashboardService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var metrics = await dashboardService.GetAdminMetricsFullAsync();
        return View(metrics);
    }

    public IActionResult Error()
    {
        return View();
    }
}
