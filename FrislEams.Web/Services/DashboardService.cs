using FrislEams.Web.Data;
using FrislEams.Web.Domain;
using Microsoft.EntityFrameworkCore;

namespace FrislEams.Web.Services;

public class DashboardService(AppDbContext db)
{
    public async Task<Dictionary<string, int>> GetAdminMetricsAsync()
    {
        return new Dictionary<string, int>
        {
            ["Total Assets"] = await db.Assets.CountAsync(),
            ["Active Assigned"] = await db.Assets.CountAsync(a => a.CurrentStatus == AssetStatus.ActiveAssigned),
            ["Loaned Out"] = await db.Assets.CountAsync(a => a.CurrentStatus == AssetStatus.LoanedOutExternal || a.CurrentStatus == AssetStatus.LoanedOutInternal),
            ["Under Repair"] = await db.Assets.CountAsync(a => a.CurrentStatus == AssetStatus.UnderRepair),
            ["Pending Requests"] = await db.AssetRequests.CountAsync(r => r.Status.Contains("Pending")),
            ["Exit Alerts"] = await db.RfidEvents.CountAsync(e => e.AlertTriggered)
        };
    }
}
