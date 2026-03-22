using FrislEams.Web.Domain;

namespace FrislEams.Web.Models;

public class AssetCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int UsefulLifeYears { get; set; }
}

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

public class Location
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

public class Staff
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public Department? Department { get; set; }
}

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

public class Asset
{
    public int Id { get; set; }
    public string TagCode { get; set; } = string.Empty;
    public string AssetName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int AssetCategoryId { get; set; }
    public AssetCategory? AssetCategory { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal? PurchaseCost { get; set; }
    public string? GlCode { get; set; }
    public string StateOfPurchase { get; set; } = "New";
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public string? SerialNumber { get; set; }
    public DateTime? WarrantyExpiryDate { get; set; }
    public int? ExpectedServiceYears { get; set; }
    public string CurrentCondition { get; set; } = "Good";
    public AssetStatus CurrentStatus { get; set; } = AssetStatus.UnregisteredUnassigned;
    public int? CurrentLocationId { get; set; }
    public Location? CurrentLocation { get; set; }
    public int? CurrentDepartmentId { get; set; }
    public Department? CurrentDepartment { get; set; }
    public int? CurrentCustodianId { get; set; }
    public Staff? CurrentCustodian { get; set; }
    public string? InvoicePath { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class RfidTag
{
    public int Id { get; set; }
    public int AssetId { get; set; }
    public Asset? Asset { get; set; }
    public string RfidCode { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}

public class AssetAssignment
{
    public int Id { get; set; }
    public int AssetId { get; set; }
    public Asset? Asset { get; set; }
    public int? AssignedToStaffId { get; set; }
    public Staff? AssignedToStaff { get; set; }
    public int? AssignedToDepartmentId { get; set; }
    public Department? AssignedToDepartment { get; set; }
    public int AssignedLocationId { get; set; }
    public Location? AssignedLocation { get; set; }
    public string AssignedCondition { get; set; } = "Good";
    public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpectedReturnDate { get; set; }
    public DateTime? ConfirmationDate { get; set; }
    public string? ConfirmedCondition { get; set; }
    public int? ConfirmedByStaffId { get; set; }
    public Staff? ConfirmedByStaff { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
}

public class AssetStatusHistory
{
    public int Id { get; set; }
    public int AssetId { get; set; }
    public Asset? Asset { get; set; }
    public AssetStatus PreviousStatus { get; set; }
    public AssetStatus NewStatus { get; set; }
    public string ChangedBy { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public int? LocationSnapshotId { get; set; }
    public int? DepartmentSnapshotId { get; set; }
    public int? CustodianSnapshotId { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}

public class AssetRequest
{
    public int Id { get; set; }
    public string RequestType { get; set; } = string.Empty;
    public int RequestedByStaffId { get; set; }
    public Staff? RequestedByStaff { get; set; }
    public int DepartmentId { get; set; }
    public Department? Department { get; set; }
    public int? AssetId { get; set; }
    public Asset? Asset { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending Department Approval";
    public string? ApprovedByDepartmentHead { get; set; }
    public string? ApprovedByAdmin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class RepairRequest
{
    public int Id { get; set; }
    public int AssetId { get; set; }
    public Asset? Asset { get; set; }
    public int ReportedByStaffId { get; set; }
    public Staff? ReportedByStaff { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = "Low";
    public string PreferredAction { get; set; } = "Repair";
    public string Status { get; set; } = "Pending Admin Review";
    public string? ApprovedAction { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class LoanRequest
{
    public int Id { get; set; }
    public int AssetId { get; set; }
    public Asset? Asset { get; set; }
    public int RequestedByStaffId { get; set; }
    public Staff? RequestedByStaff { get; set; }
    public string LoanType { get; set; } = "Internal";
    public DateTime StartDate { get; set; }
    public DateTime ExpectedReturnDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string ResponsiblePerson { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public string? ApprovedBy { get; set; }
}

public class ExitGrant
{
    public int Id { get; set; }
    public int AssetId { get; set; }
    public Asset? Asset { get; set; }
    public int? LoanRequestId { get; set; }
    public LoanRequest? LoanRequest { get; set; }
    public string GrantedBy { get; set; } = string.Empty;
    public DateTime GrantStartDate { get; set; }
    public DateTime GrantEndDate { get; set; }
    public bool IsActive { get; set; } = true;
}

public class RfidEvent
{
    public int Id { get; set; }
    public string RfidCode { get; set; } = string.Empty;
    public int? AssetId { get; set; }
    public Asset? Asset { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string DoorLocation { get; set; } = string.Empty;
    public string ProcessedStatus { get; set; } = string.Empty;
    public bool AlertTriggered { get; set; }
    public DateTime EventTime { get; set; } = DateTime.UtcNow;
}

public class AuditSession
{
    public int Id { get; set; }
    public string AuditType { get; set; } = string.Empty;
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public string InitiatedBy { get; set; } = string.Empty;
    public string Status { get; set; } = "Open";
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
}

public class AuditResult
{
    public int Id { get; set; }
    public int AuditSessionId { get; set; }
    public AuditSession? AuditSession { get; set; }
    public int AssetId { get; set; }
    public Asset? Asset { get; set; }
    public string SeenStatus { get; set; } = string.Empty;
    public string? PhysicalCondition { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Notification
{
    public int Id { get; set; }
    public string TargetRole { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ProcurementRecord
{
    public int Id { get; set; }
    public string ExternalReference { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public string VendorCode { get; set; } = string.Empty;
    public string AssetName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public AssetCategory? Category { get; set; }
    public decimal UnitCost { get; set; }
    public int Quantity { get; set; }
    public string GlCode { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public string Status { get; set; } = "Synced";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class IntegrationEventLog
{
    public int Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string SourceSystem { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public string ProcessingStatus { get; set; } = "Queued";
    public string? ProcessingNote { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
}
