using FrislEams.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FrislEams.Web.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<AssetCategory> AssetCategories => Set<AssetCategory>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Staff> Staff => Set<Staff>();
    public DbSet<RfidTag> RfidTags => Set<RfidTag>();
    public DbSet<AssetAssignment> AssetAssignments => Set<AssetAssignment>();
    public DbSet<AssetStatusHistory> AssetStatusHistories => Set<AssetStatusHistory>();
    public DbSet<AssetRequest> AssetRequests => Set<AssetRequest>();
    public DbSet<RepairRequest> RepairRequests => Set<RepairRequest>();
    public DbSet<LoanRequest> LoanRequests => Set<LoanRequest>();
    public DbSet<ExitGrant> ExitGrants => Set<ExitGrant>();
    public DbSet<RfidEvent> RfidEvents => Set<RfidEvent>();
    public DbSet<AuditSession> AuditSessions => Set<AuditSession>();
    public DbSet<AuditResult> AuditResults => Set<AuditResult>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<ProcurementRecord> ProcurementRecords => Set<ProcurementRecord>();
    public DbSet<IntegrationEventLog> IntegrationEventLogs => Set<IntegrationEventLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Asset>().HasIndex(a => a.TagCode).IsUnique();
        modelBuilder.Entity<RfidTag>().HasIndex(r => r.RfidCode).IsUnique();
        modelBuilder.Entity<RfidTag>().HasIndex(r => r.AssetId).IsUnique();
        modelBuilder.Entity<AssetStatusHistory>().HasIndex(h => h.ChangedAt);
        modelBuilder.Entity<RfidEvent>().HasIndex(e => e.EventTime);
        modelBuilder.Entity<AuditResult>().HasIndex(a => new { a.AuditSessionId, a.AssetId }).IsUnique();
        modelBuilder.Entity<ProcurementRecord>().HasIndex(p => p.ExternalReference);
        modelBuilder.Entity<IntegrationEventLog>().HasIndex(i => i.CreatedAt);

        base.OnModelCreating(modelBuilder);
    }
}
