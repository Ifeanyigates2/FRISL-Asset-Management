using FrislEams.Web.Data;
using FrislEams.Web.Domain;
using FrislEams.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FrislEams.Web.Services;

public class RfidMonitoringService(AppDbContext db)
{
    public async Task<(string Outcome, bool Alert)> ProcessDoorScanAsync(string rfidCode, string doorLocation)
    {
        var tag = await db.RfidTags.Include(r => r.Asset).FirstOrDefaultAsync(r => r.RfidCode == rfidCode && r.IsActive);

        if (tag is null)
        {
            db.RfidEvents.Add(new RfidEvent
            {
                RfidCode = rfidCode,
                EventType = "ExternalAsset",
                DoorLocation = doorLocation,
                ProcessedStatus = "Unknown RFID - logged as external",
                AlertTriggered = false
            });
            await db.SaveChangesAsync();
            return ("External asset logged", false);
        }

        var asset = tag.Asset!;
        var activeGrant = await db.ExitGrants.AnyAsync(g => g.AssetId == asset.Id && g.IsActive && g.GrantEndDate >= DateTime.UtcNow);
        var allowed = activeGrant || asset.CurrentStatus == AssetStatus.LoanedOutExternal;

        if (allowed)
        {
            db.RfidEvents.Add(new RfidEvent
            {
                RfidCode = rfidCode,
                AssetId = asset.Id,
                EventType = "AuthorizedExitOrReturn",
                DoorLocation = doorLocation,
                ProcessedStatus = "Authorized",
                AlertTriggered = false
            });
            await db.SaveChangesAsync();
            return ("Authorized movement", false);
        }

        db.RfidEvents.Add(new RfidEvent
        {
            RfidCode = rfidCode,
            AssetId = asset.Id,
            EventType = "UnauthorizedAttempt",
            DoorLocation = doorLocation,
            ProcessedStatus = "Blocked - no active exit grant",
            AlertTriggered = true
        });

        db.Notifications.Add(new Notification
        {
            TargetRole = RoleName.Admin,
            Message = $"Unauthorized RFID exit attempt for {asset.TagCode} at {doorLocation}."
        });

        await db.SaveChangesAsync();
        return ("Unauthorized attempt", true);
    }
}
