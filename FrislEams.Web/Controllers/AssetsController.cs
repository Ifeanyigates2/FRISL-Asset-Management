using FrislEams.Web.Data;
using FrislEams.Web.Domain;
using FrislEams.Web.Models;
using FrislEams.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrislEams.Web.Controllers;

public class AssetsController(
    AppDbContext db,
    TagCodeGenerator tagCodeGenerator,
    AssetLifecycleService lifecycleService,
    RoleGuard roleGuard) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var assets = await db.Assets
            .Include(a => a.AssetCategory)
            .Include(a => a.CurrentLocation)
            .OrderByDescending(a => a.Id)
            .ToListAsync();
        return View(assets);
    }

    [HttpGet]
    public async Task<IActionResult> Register()
    {
        ViewBag.Categories = await db.AssetCategories.ToListAsync();
        ViewBag.Locations = await db.Locations.ToListAsync();
        ViewBag.Departments = await db.Departments.ToListAsync();
        ViewBag.Suppliers = await db.Suppliers.ToListAsync();
        return View(new AssetRegistrationVm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(AssetRegistrationVm vm)
    {
        if (!roleGuard.HasAnyRole(this, RoleName.Admin))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return await Register();
        }

        var existingRfid = await db.RfidTags.AnyAsync(r => r.RfidCode == vm.RfidCode.Trim());
        if (existingRfid)
        {
            ModelState.AddModelError(nameof(vm.RfidCode), "RFID code must be unique.");
            return await Register();
        }

        var asset = new Asset
        {
            TagCode = tagCodeGenerator.Generate(vm.AssetCategoryId, vm.InitialDepartmentId),
            AssetName = vm.AssetName,
            Description = vm.Description,
            AssetCategoryId = vm.AssetCategoryId,
            PurchaseDate = vm.PurchaseDate,
            PurchaseCost = vm.PurchaseCost,
            GlCode = vm.GlCode,
            StateOfPurchase = vm.StateOfPurchase,
            SupplierId = vm.SupplierId,
            SerialNumber = vm.SerialNumber,
            WarrantyExpiryDate = vm.WarrantyExpiryDate,
            ExpectedServiceYears = vm.ExpectedServiceYears,
            CurrentCondition = vm.CurrentCondition,
            CurrentLocationId = vm.InitialLocationId,
            CurrentDepartmentId = vm.InitialDepartmentId,
            CurrentStatus = AssetStatus.UnregisteredUnassigned
        };

        db.Assets.Add(asset);
        await db.SaveChangesAsync();

        db.RfidTags.Add(new RfidTag { AssetId = asset.Id, RfidCode = vm.RfidCode.Trim() });
        lifecycleService.ChangeStatus(asset, AssetStatus.RegisteredUnassigned, "Asset registration completed", "Admin");
        await db.SaveChangesAsync();

        TempData["Success"] = $"Asset registered with tag {asset.TagCode}";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(StatusChangeVm vm)
    {
        if (!roleGuard.HasAnyRole(this, RoleName.Admin))
        {
            return Forbid();
        }

        var asset = await db.Assets.FindAsync(vm.AssetId);
        if (asset is null)
        {
            return NotFound();
        }

        var ok = lifecycleService.ChangeStatus(asset, vm.NextStatus, vm.Reason, vm.ChangedBy);
        if (!ok)
        {
            TempData["Error"] = "Invalid status transition or missing reason.";
            return RedirectToAction(nameof(Index));
        }

        await db.SaveChangesAsync();
        TempData["Success"] = "Status changed.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> History(int id)
    {
        var history = await db.AssetStatusHistories
            .Where(h => h.AssetId == id)
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync();
        ViewBag.Asset = await db.Assets.FindAsync(id);
        return View(history);
    }
}
