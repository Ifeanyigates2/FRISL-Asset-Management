using System.Text;
using FrislEams.Web.Data;
using FrislEams.Web.Models;
using FrislEams.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrislEams.Web.Controllers;

public class ReportsController(AppDbContext db, ReportingService reportingService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(ReportFilterVm filter)
    {
        ViewBag.Departments = await db.Departments.ToListAsync();
        ViewBag.Categories = await db.AssetCategories.ToListAsync();
        ViewBag.CategoryCounts = await reportingService.GetAssetCountByCategoryAsync();

        var report = await reportingService.GetAssetsReportAsync(filter);
        ViewBag.Filter = filter;
        return View(report);
    }

    [HttpGet]
    public async Task<IActionResult> ExportCsv(ReportFilterVm filter)
    {
        var report = await reportingService.GetAssetsReportAsync(filter);
        var csv = reportingService.BuildAssetsCsv(report);
        return File(Encoding.UTF8.GetBytes(csv), "text/csv", $"assets-report-{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    [HttpGet]
    public async Task<IActionResult> Depreciation()
    {
        var report = await reportingService.GetDepreciationReportAsync();
        return View(report);
    }

    [HttpGet]
    public async Task<IActionResult> ExportDepreciationCsv()
    {
        var report = await reportingService.GetDepreciationReportAsync();
        var csv = reportingService.BuildDepreciationCsv(report);
        return File(Encoding.UTF8.GetBytes(csv), "text/csv", $"depreciation-report-{DateTime.UtcNow:yyyyMMdd}.csv");
    }
}
