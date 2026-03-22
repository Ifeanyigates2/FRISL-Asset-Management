using FrislEams.Web.Data;
using FrislEams.Web.Domain;
using FrislEams.Web.Models;
using FrislEams.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrislEams.Web.Controllers;

public class WorkflowController(
    AppDbContext db,
    AssetLifecycleService lifecycleService,
    RfidMonitoringService rfidMonitoringService,
    RoleGuard roleGuard) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Requests()
    {
        ViewBag.Staff = await db.Staff.ToListAsync();
        ViewBag.Assets = await db.Assets.ToListAsync();
        var requests = await db.AssetRequests.OrderByDescending(r => r.CreatedAt).ToListAsync();
        return View(requests);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRequest(AssetRequest vm)
    {
        vm.CreatedAt = DateTime.UtcNow;
        vm.Status = "Pending Department Approval";
        db.AssetRequests.Add(vm);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Requests));
    }

    [HttpGet]
    public async Task<IActionResult> Repairs()
    {
        ViewBag.Assets = await db.Assets.Where(a => a.CurrentStatus == AssetStatus.ActiveAssigned || a.CurrentStatus == AssetStatus.Damaged).ToListAsync();
        ViewBag.Staff = await db.Staff.ToListAsync();
        var repairs = await db.RepairRequests.Include(r => r.Asset).OrderByDescending(r => r.CreatedAt).ToListAsync();
        return View(repairs);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RaiseRepair(RepairRequestVm vm)
    {
        db.RepairRequests.Add(new RepairRequest
        {
            AssetId = vm.AssetId,
            ReportedByStaffId = vm.ReportedByStaffId,
            Description = vm.Description,
            Severity = vm.Severity,
            PreferredAction = vm.PreferredAction,
            Status = "Pending Admin Review"
        });
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Repairs));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveRepair(ApproveRepairVm vm)
    {
        if (!roleGuard.HasAnyRole(this, RoleName.Admin))
        {
            return Forbid();
        }

        var repair = await db.RepairRequests.Include(r => r.Asset).FirstOrDefaultAsync(r => r.Id == vm.RepairRequestId);
        if (repair?.Asset is null)
        {
            return NotFound();
        }

        repair.ApprovedAction = vm.Action;
        repair.Status = $"Approved: {vm.Action}";

        if (vm.Action.Equals("Repair", StringComparison.OrdinalIgnoreCase))
        {
            lifecycleService.ChangeStatus(repair.Asset, AssetStatus.UnderRepair, vm.Reason, "Admin");
        }
        else if (vm.Action.Equals("Replacement", StringComparison.OrdinalIgnoreCase))
        {
            lifecycleService.ChangeStatus(repair.Asset, AssetStatus.PendingReplacement, vm.Reason, "Admin");
        }
        else if (vm.Action.Equals("Discard", StringComparison.OrdinalIgnoreCase))
        {
            lifecycleService.ChangeStatus(repair.Asset, AssetStatus.Discarded, vm.Reason, "Admin");
        }

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Repairs));
    }

    [HttpGet]
    public async Task<IActionResult> Loans()
    {
        ViewBag.Assets = await db.Assets.Where(a => a.CurrentStatus == AssetStatus.ActiveAssigned).ToListAsync();
        ViewBag.Staff = await db.Staff.ToListAsync();
        var loans = await db.LoanRequests.Include(l => l.Asset).OrderByDescending(l => l.Id).ToListAsync();
        return View(loans);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RaiseLoan(LoanRequestVm vm)
    {
        db.LoanRequests.Add(new LoanRequest
        {
            AssetId = vm.AssetId,
            RequestedByStaffId = vm.RequestedByStaffId,
            LoanType = vm.LoanType,
            StartDate = vm.StartDate,
            ExpectedReturnDate = vm.ExpectedReturnDate,
            Purpose = vm.Purpose,
            Destination = vm.Destination,
            ResponsiblePerson = vm.ResponsiblePerson,
            Status = "Pending"
        });

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Loans));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveLoan(ApproveLoanVm vm)
    {
        if (!roleGuard.HasAnyRole(this, RoleName.Admin, RoleName.DepartmentHead))
        {
            return Forbid();
        }

        var loan = await db.LoanRequests.Include(l => l.Asset).FirstOrDefaultAsync(l => l.Id == vm.LoanRequestId);
        if (loan?.Asset is null)
        {
            return NotFound();
        }

        loan.ApprovedBy = vm.ApprovedBy;
        loan.Status = "Approved";

        var isExternal = loan.LoanType.Equals("External", StringComparison.OrdinalIgnoreCase);
        lifecycleService.ChangeStatus(
            loan.Asset,
            isExternal ? AssetStatus.LoanedOutExternal : AssetStatus.LoanedOutInternal,
            "Loan approved",
            vm.ApprovedBy);

        if (isExternal)
        {
            db.ExitGrants.Add(new ExitGrant
            {
                AssetId = loan.AssetId,
                LoanRequestId = loan.Id,
                GrantedBy = vm.ApprovedBy,
                GrantStartDate = loan.StartDate,
                GrantEndDate = loan.ExpectedReturnDate,
                IsActive = true
            });
        }

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Loans));
    }

    [HttpGet]
    public async Task<IActionResult> Rfid()
    {
        var eventsLog = await db.RfidEvents.OrderByDescending(e => e.EventTime).Take(100).ToListAsync();
        return View(eventsLog);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RfidScan(RfidScanVm vm)
    {
        await rfidMonitoringService.ProcessDoorScanAsync(vm.RfidCode.Trim(), vm.DoorLocation.Trim());
        return RedirectToAction(nameof(Rfid));
    }
}
