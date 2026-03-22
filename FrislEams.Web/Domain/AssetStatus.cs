namespace FrislEams.Web.Domain;

public enum AssetStatus
{
    UnregisteredUnassigned,
    RegisteredUnassigned,
    AssignedPendingConfirmation,
    ActiveAssigned,
    RecalledToStorage,
    UnderRepair,
    PendingReplacement,
    Damaged,
    LoanedOutInternal,
    LoanedOutExternal,
    UnderProcurement,
    Retired,
    Sold,
    Discarded
}
