using FrislEams.Web.Data;
using FrislEams.Web.Domain;
using Microsoft.EntityFrameworkCore;

namespace FrislEams.Web.Services;

public class AdminDashboardMetrics
{
    public int TotalAssets { get; set; }
    public int ActiveAssigned { get; set; }
    public int RegisteredUnassigned { get; set; }
    public int UnderRepair { get; set; }
    public int LoanedOut { get; set; }
    public int Damaged { get; set; }
    public int Retired { get; set; }
    public int TotalValue { get; set; }
    public int PendingAssignmentConfirmations { get; set; }
    public int PendingRepairRequests { get; set; }
    public int PendingAssetRequests { get; set; }
    public int PendingLoanRequests { get; set; }
    public int ExitAlerts { get; set; }
    public int UnreadNotifications { get; set; }
    public int WarrantyExpiringSoon { get; set; }
    public int TotalStaff { get; set; }
    public int TotalDepartments { get; set; }
    public Dictionary<string, int> AssetsByCategory { get; set; } = new();
    public Dictionary<string, int> AssetsByStatus { get; set; } = new();
    public Dictionary<string, int> AssetsByDepartment { get; set; } = new();
}

public class DashboardService(AppDbContext db)
{
    public async Task<AdminDashboardMetrics> GetAdminMetricsFullAsync()
    {
        var threeMonths = DateTime.UtcNow.AddMonths(3);
        var assets = await db.Assets
            .Include(a => a.AssetCategory)
            .Include(a => a.CurrentDepartment)
            .ToListAsync();

        var metrics = new AdminDashboardMetrics
        {
            TotalAssets = assets.Count,
            ActiveAssigned = assets.Count(a => a.CurrentStatus == AssetStatus.ActiveAssigned),
            RegisteredUnassigned = assets.Count(a => a.CurrentStatus == AssetStatus.RegisteredUnassigned),
            UnderRepair = assets.Count(a => a.CurrentStatus == AssetStatus.UnderRepair),
            LoanedOut = assets.Count(a => a.CurrentStatus is AssetStatus.LoanedOutInternal or AssetStatus.LoanedOutExternal),
            Damaged = assets.Count(a => a.CurrentStatus == AssetStatus.Damaged),
            Retired = assets.Count(a => a.CurrentStatus is AssetStatus.Retired or AssetStatus.Sold or AssetStatus.Discarded),
            PendingAssignmentConfirmations = assets.Count(a => a.CurrentStatus == AssetStatus.AssignedPendingConfirmation),
            WarrantyExpiringSoon = assets.Count(a => a.WarrantyExpiryDate.HasValue && a.WarrantyExpiryDate.Value <= threeMonths && a.WarrantyExpiryDate.Value >= DateTime.UtcNow),
            PendingRepairRequests = await db.RepairRequests.CountAsync(r => r.Status == "Pending Admin Review"),
            PendingAssetRequests = await db.AssetRequests.CountAsync(r => r.Status.StartsWith("Pending")),
            PendingLoanRequests = await db.LoanRequests.CountAsync(l => l.Status == "Pending"),
            ExitAlerts = await db.RfidEvents.CountAsync(e => e.AlertTriggered),
            UnreadNotifications = await db.Notifications.CountAsync(n => !n.IsRead && n.TargetRole == "Admin"),
            TotalStaff = await db.Staff.CountAsync(s => s.IsActive),
            TotalDepartments = await db.Departments.CountAsync(),
            AssetsByCategory = assets
                .Where(a => a.AssetCategory != null)
                .GroupBy(a => a.AssetCategory!.Name)
                .ToDictionary(g => g.Key, g => g.Count()),
            AssetsByDepartment = assets
                .Where(a => a.CurrentDepartment != null)
                .GroupBy(a => a.CurrentDepartment!.Name)
                .ToDictionary(g => g.Key, g => g.Count()),
            AssetsByStatus = assets
                .GroupBy(a => a.CurrentStatus.ToString())
                .ToDictionary(g => g.Key, g => g.Count())
        };

        if (assets.Any(a => a.PurchaseCost.HasValue))
            metrics.TotalValue = (int)(assets.Where(a => a.PurchaseCost.HasValue).Sum(a => a.PurchaseCost!.Value) / 1_000_000);

        return metrics;
    }

    // Legacy method for Blazor dashboard
    public async Task<Dictionary<string, int>> GetAdminMetricsAsync()
    {
        var m = await GetAdminMetricsFullAsync();
        return new Dictionary<string, int>
        {
            ["Total Assets"] = m.TotalAssets,
            ["Active – Assigned"] = m.ActiveAssigned,
            ["Registered – Unassigned"] = m.RegisteredUnassigned,
            ["Under Repair"] = m.UnderRepair,
            ["Loaned Out"] = m.LoanedOut,
            ["Damaged"] = m.Damaged,
            ["Pending Confirmations"] = m.PendingAssignmentConfirmations,
            ["Pending Repairs"] = m.PendingRepairRequests,
            ["Pending Requests"] = m.PendingAssetRequests,
            ["RFID Alerts"] = m.ExitAlerts,
            ["Warranty Expiring Soon"] = m.WarrantyExpiringSoon
        };
    }
}
