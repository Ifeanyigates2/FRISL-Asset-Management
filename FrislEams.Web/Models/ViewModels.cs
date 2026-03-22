using System.ComponentModel.DataAnnotations;
using FrislEams.Web.Domain;

namespace FrislEams.Web.Models;

public class AssetRegistrationVm
{
    [Required]
    public string AssetName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [Required]
    public int AssetCategoryId { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal? PurchaseCost { get; set; }
    public string? GlCode { get; set; }
    public string StateOfPurchase { get; set; } = "New";
    public int? SupplierId { get; set; }
    public string? SerialNumber { get; set; }
    public DateTime? WarrantyExpiryDate { get; set; }
    public int? ExpectedServiceYears { get; set; }
    public string CurrentCondition { get; set; } = "Good";
    public int InitialLocationId { get; set; }
    public int? InitialDepartmentId { get; set; }
    [Required]
    public string RfidCode { get; set; } = string.Empty;
}

public class StatusChangeVm
{
    public int AssetId { get; set; }
    public AssetStatus NextStatus { get; set; }
    [Required]
    public string Reason { get; set; } = string.Empty;
    public string ChangedBy { get; set; } = "Admin";
}

public class AssignmentInitiateVm
{
    public int AssetId { get; set; }
    public int? AssignedToStaffId { get; set; }
    public int? AssignedToDepartmentId { get; set; }
    [Required]
    public int AssignedLocationId { get; set; }
    [Required]
    public string AssignedCondition { get; set; } = "Good";
    public DateTime? ExpectedReturnDate { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class AssignmentConfirmVm
{
    public int AssignmentId { get; set; }
    [Required]
    public int ConfirmedByStaffId { get; set; }
    [Required]
    public string ConfirmedCondition { get; set; } = "Good";
}

public class RepairRequestVm
{
    public int AssetId { get; set; }
    public int ReportedByStaffId { get; set; }
    [Required]
    public string Description { get; set; } = string.Empty;
    [Required]
    public string Severity { get; set; } = "Low";
    [Required]
    public string PreferredAction { get; set; } = "Repair";
}

public class ApproveRepairVm
{
    public int RepairRequestId { get; set; }
    [Required]
    public string Action { get; set; } = "Repair";
    [Required]
    public string Reason { get; set; } = string.Empty;
}

public class LoanRequestVm
{
    public int AssetId { get; set; }
    public int RequestedByStaffId { get; set; }
    [Required]
    public string LoanType { get; set; } = "Internal";
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime ExpectedReturnDate { get; set; } = DateTime.Today.AddDays(7);
    [Required]
    public string Purpose { get; set; } = string.Empty;
    [Required]
    public string Destination { get; set; } = string.Empty;
    [Required]
    public string ResponsiblePerson { get; set; } = string.Empty;
}

public class ApproveLoanVm
{
    public int LoanRequestId { get; set; }
    [Required]
    public string ApprovedBy { get; set; } = "Admin";
}

public class RfidScanVm
{
    [Required]
    public string RfidCode { get; set; } = string.Empty;
    [Required]
    public string DoorLocation { get; set; } = "Main Gate";
}

public class StartAuditVm
{
    [Required]
    public string AuditType { get; set; } = "Departmental";
    public int? DepartmentId { get; set; }
    [Required]
    public string InitiatedBy { get; set; } = "Auditor";
}

public class SubmitAuditResultVm
{
    public int AuditSessionId { get; set; }
    public int AssetId { get; set; }
    [Required]
    public string SeenStatus { get; set; } = "Seen";
    public string? PhysicalCondition { get; set; }
    public string? Notes { get; set; }
}

public class ReportFilterVm
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? DepartmentId { get; set; }
    public int? CategoryId { get; set; }
    public string? Status { get; set; }
}

public class DepreciationRowVm
{
    public int AssetId { get; set; }
    public string TagCode { get; set; } = string.Empty;
    public string AssetName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal PurchaseCost { get; set; }
    public int UsefulLifeYears { get; set; }
    public decimal AnnualDepreciation { get; set; }
    public decimal AccumulatedDepreciation { get; set; }
    public decimal NetBookValue { get; set; }
}

public class ProcurementItemVm
{
    [Required]
    public string ExternalReference { get; set; } = string.Empty;
    [Required]
    public string VendorName { get; set; } = string.Empty;
    public string VendorCode { get; set; } = string.Empty;
    [Required]
    public string AssetName { get; set; } = string.Empty;
    [Required]
    public int CategoryId { get; set; }
    public decimal UnitCost { get; set; }
    public int Quantity { get; set; } = 1;
    public string GlCode { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow.Date;
}

public class ProcurementSyncVm
{
    [Required]
    public string SourceSystem { get; set; } = "ERP";
    [Required]
    public List<ProcurementItemVm> Items { get; set; } = [];
}

public class RfidBatchScanVm
{
    [Required]
    public string SourceSystem { get; set; } = "RFIDDoorSystem";
    [Required]
    public List<RfidScanVm> Scans { get; set; } = [];
}
