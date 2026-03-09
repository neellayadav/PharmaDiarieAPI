using System;
using System.Collections.Generic;

namespace PharmaDiaries.Models
{
    // ===========================================
    // TRAVEL MODE
    // ===========================================
    public class TravelModeModel
    {
        public int TravelModeID { get; set; }
        public int CompID { get; set; } = 0;
        public string ModeName { get; set; } = string.Empty;
        public string ModeType { get; set; } = string.Empty; // PER_KM or ACTUAL_FARE
        public bool RequiresReceipt { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class TravelModeCreateRequest
    {
        public string ModeName { get; set; } = string.Empty;
        public string ModeType { get; set; } = string.Empty;
        public bool RequiresReceipt { get; set; } = false;
        public int CreatedBy { get; set; }
    }

    public class TravelModeUpdateRequest
    {
        public int TravelModeID { get; set; }
        public string ModeName { get; set; } = string.Empty;
        public string ModeType { get; set; } = string.Empty;
        public bool RequiresReceipt { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public int ModifiedBy { get; set; }
    }

    // ===========================================
    // ALLOWANCE POLICY
    // ===========================================
    public class AllowancePolicyModel
    {
        public int PolicyID { get; set; }
        public int CompID { get; set; }
        public string PolicyName { get; set; } = string.Empty;
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsActive { get; set; } = true;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class AllowancePolicyCreateRequest
    {
        public int CompID { get; set; }
        public string PolicyName { get; set; } = string.Empty;
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public int CreatedBy { get; set; }
    }

    public class AllowancePolicyUpdateRequest
    {
        public int PolicyID { get; set; }
        public int CompID { get; set; }
        public string PolicyName { get; set; } = string.Empty;
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsActive { get; set; } = true;
        public int ModifiedBy { get; set; }
    }

    // ===========================================
    // DA RATE
    // ===========================================
    public class DARateModel
    {
        public int DARateID { get; set; }
        public int CompID { get; set; }
        public int PolicyID { get; set; }
        public int RoleID { get; set; }
        public string? RoleName { get; set; }
        public string LocationType { get; set; } = string.Empty;
        public decimal FullDayRate { get; set; }
        public decimal HalfDayRate { get; set; }
        public bool IsActive { get; set; } = true;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class DARateSaveRequest
    {
        public int CompID { get; set; }
        public int PolicyID { get; set; }
        public int RoleID { get; set; }
        public string LocationType { get; set; } = string.Empty;
        public decimal FullDayRate { get; set; }
        public decimal HalfDayRate { get; set; }
        public int ModifiedBy { get; set; }
    }

    // ===========================================
    // TA RATE
    // ===========================================
    public class TARateModel
    {
        public int TARateID { get; set; }
        public int CompID { get; set; }
        public int PolicyID { get; set; }
        public int RoleID { get; set; }
        public string? RoleName { get; set; }
        public int TravelModeID { get; set; }
        public string? ModeName { get; set; }
        public string? ModeType { get; set; }
        public decimal RatePerUnit { get; set; }
        public decimal? MaxDailyLimit { get; set; }
        public decimal? MaxMonthlyLimit { get; set; }
        public bool IsActive { get; set; } = true;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class TARateSaveRequest
    {
        public int CompID { get; set; }
        public int PolicyID { get; set; }
        public int RoleID { get; set; }
        public int TravelModeID { get; set; }
        public decimal RatePerUnit { get; set; }
        public decimal? MaxDailyLimit { get; set; }
        public decimal? MaxMonthlyLimit { get; set; }
        public int ModifiedBy { get; set; }
    }

    // ===========================================
    // EXPENSE CALCULATION
    // ===========================================
    public class ExpenseCalculateRequest
    {
        public int CompID { get; set; }
        public int UID { get; set; }
        public DateTime DutyDate { get; set; }
        public string PatchName { get; set; } = string.Empty;
        public bool IsHalfDay { get; set; } = false;
        public int? TravelModeID { get; set; }
        public decimal TravelKM { get; set; } = 0;
        public decimal ActualFare { get; set; } = 0;
    }

    public class ExpenseCalculateResponse
    {
        public string Status { get; set; } = string.Empty;
        public string? Message { get; set; }
        public int PolicyID { get; set; }
        public string? HQCode { get; set; }
        public string LocationType { get; set; } = string.Empty;
        public decimal DARateApplied { get; set; }
        public decimal DAAmount { get; set; }
        public decimal TARateApplied { get; set; }
        public decimal TAAmount { get; set; }
        public decimal LineTotal { get; set; }
    }

    // ===========================================
    // EXPENSE CLAIM (HEADER)
    // ===========================================
    public class ExpenseClaimModel
    {
        public int ClaimID { get; set; }
        public int CompID { get; set; }
        public string ClaimNumber { get; set; } = string.Empty;
        public int UID { get; set; }
        public string? EmployeeName { get; set; }
        public string? HeadQuater { get; set; }
        public int? RoleID { get; set; }
        public string? RoleName { get; set; }
        public int ClaimMonth { get; set; }
        public int ClaimYear { get; set; }
        public decimal TotalDAAmount { get; set; }
        public decimal TotalTAAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? SubmittedOn { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public DateTime? SettledOn { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class ExpenseClaimCreateRequest
    {
        public int CompID { get; set; }
        public int UID { get; set; }
        public int ClaimMonth { get; set; }
        public int ClaimYear { get; set; }
        public int CreatedBy { get; set; }
    }

    public class ExpenseClaimCreateResponse
    {
        public int ClaimID { get; set; }
        public string ClaimNumber { get; set; } = string.Empty;
    }

    public class ExpenseClaimUpdateRequest
    {
        public int ClaimID { get; set; }
        public int CompID { get; set; }
        public int ClaimMonth { get; set; }
        public int ClaimYear { get; set; }
        public int ModifiedBy { get; set; }
    }

    // ===========================================
    // EXPENSE CLAIM LINE
    // ===========================================
    public class ExpenseClaimLineModel
    {
        public int LineID { get; set; }
        public int ClaimID { get; set; }
        public DateTime DutyDate { get; set; }
        public string HQCode { get; set; } = string.Empty;
        public string PatchName { get; set; } = string.Empty;
        public string LocationType { get; set; } = string.Empty;
        public bool IsHalfDay { get; set; }
        public int PolicyIDUsed { get; set; }
        public decimal DARateApplied { get; set; }
        public decimal DAAmount { get; set; }
        public int? TravelModeID { get; set; }
        public string? TravelModeName { get; set; }
        public string? ModeType { get; set; }
        public decimal TravelKM { get; set; }
        public decimal ActualFare { get; set; }
        public decimal TARateApplied { get; set; }
        public decimal TAAmount { get; set; }
        public decimal? AutoCalculatedKM { get; set; }
        public string KMSource { get; set; } = "MANUAL";
        public decimal LineTotal { get; set; }
        public string? Remarks { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class ClaimLineAddRequest
    {
        public int ClaimID { get; set; }
        public int CompID { get; set; }
        public int UID { get; set; }
        public DateTime DutyDate { get; set; }
        public string PatchName { get; set; } = string.Empty;
        public bool IsHalfDay { get; set; } = false;
        public int? TravelModeID { get; set; }
        public decimal TravelKM { get; set; } = 0;
        public decimal ActualFare { get; set; } = 0;
        public decimal? AutoCalculatedKM { get; set; }
        public string KMSource { get; set; } = "MANUAL";
        public string? Remarks { get; set; }
        public int CreatedBy { get; set; }
    }

    public class ClaimLineUpdateRequest
    {
        public int LineID { get; set; }
        public int CompID { get; set; }
        public string PatchName { get; set; } = string.Empty;
        public bool IsHalfDay { get; set; } = false;
        public int? TravelModeID { get; set; }
        public decimal TravelKM { get; set; } = 0;
        public decimal ActualFare { get; set; } = 0;
        public decimal? AutoCalculatedKM { get; set; }
        public string KMSource { get; set; } = "MANUAL";
        public string? Remarks { get; set; }
        public int ModifiedBy { get; set; }
    }

    public class ClaimLineInsertResponse
    {
        public string Status { get; set; } = string.Empty;
        public string? Message { get; set; }
        public int LineID { get; set; }
        public string? LocationType { get; set; }
        public decimal DARateApplied { get; set; }
        public decimal DAAmount { get; set; }
        public decimal TARateApplied { get; set; }
        public decimal TAAmount { get; set; }
        public decimal LineTotal { get; set; }
    }

    // ===========================================
    // CLAIM ACTIONS
    // ===========================================
    public class ClaimSubmitRequest
    {
        public int ClaimID { get; set; }
        public int CompID { get; set; }
        public int UID { get; set; }
    }

    public class ClaimApproveRequest
    {
        public int ClaimID { get; set; }
        public int CompID { get; set; }
        public int ApproverUID { get; set; }
        public string? Remarks { get; set; }
    }

    public class ClaimRejectRequest
    {
        public int ClaimID { get; set; }
        public int CompID { get; set; }
        public int ApproverUID { get; set; }
        public string? Remarks { get; set; }
    }

    public class ClaimReturnRequest
    {
        public int ClaimID { get; set; }
        public int CompID { get; set; }
        public int ApproverUID { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }

    public class BulkApproveRequest
    {
        public int CompID { get; set; }
        public int ApproverUID { get; set; }
        public string ClaimIDs { get; set; } = string.Empty; // comma-separated
        public string? Remarks { get; set; }
    }

    public class ClaimSettleRequest
    {
        public int ClaimID { get; set; }
        public int CompID { get; set; }
        public int SettledByUID { get; set; }
        public string? PaymentRef { get; set; }
    }

    // ===========================================
    // APPROVAL ACTION
    // ===========================================
    public class ApprovalActionModel
    {
        public int ActionID { get; set; }
        public int ClaimID { get; set; }
        public int ApproverUID { get; set; }
        public string ApproverName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public DateTime ActionOn { get; set; }
    }

    // ===========================================
    // SETTLEMENT
    // ===========================================
    public class SettlementModel
    {
        public int SettlementID { get; set; }
        public int ClaimID { get; set; }
        public string? ClaimNumber { get; set; }
        public int UID { get; set; }
        public string? EmployeeName { get; set; }
        public decimal SettledAmount { get; set; }
        public string? PaymentRef { get; set; }
        public DateTime SettledOn { get; set; }
        public string? SettledByName { get; set; }
    }

    // ===========================================
    // ATTACHMENT
    // ===========================================
    public class ClaimAttachmentModel
    {
        public int AttachmentID { get; set; }
        public int LineID { get; set; }
        public int CompID { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileURL { get; set; } = string.Empty;
        public int? FileSize { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class ClaimAttachmentInsertRequest
    {
        public int LineID { get; set; }
        public int CompID { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileURL { get; set; } = string.Empty;
        public int? FileSize { get; set; }
        public int CreatedBy { get; set; }
    }

    // ===========================================
    // EMPLOYEE DA RATE (Employee-Specific Override)
    // ===========================================
    public class EmployeeDARateModel
    {
        public int EmployeeDARateID { get; set; }
        public int CompID { get; set; }
        public int PolicyID { get; set; }
        public int UID { get; set; }
        public string? EmployeeName { get; set; }
        public string LocationType { get; set; } = string.Empty;
        public decimal FullDayRate { get; set; }
        public decimal HalfDayRate { get; set; }
        public bool IsActive { get; set; } = true;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class EmployeeDARateSaveRequest
    {
        public int CompID { get; set; }
        public int PolicyID { get; set; }
        public int UID { get; set; }
        public string LocationType { get; set; } = string.Empty;
        public decimal FullDayRate { get; set; }
        public decimal HalfDayRate { get; set; }
        public int ModifiedBy { get; set; }
    }

    // ===========================================
    // EMPLOYEE TA RATE (Employee-Specific Override)
    // ===========================================
    public class EmployeeTARateModel
    {
        public int EmployeeTARateID { get; set; }
        public int CompID { get; set; }
        public int PolicyID { get; set; }
        public int UID { get; set; }
        public string? EmployeeName { get; set; }
        public int TravelModeID { get; set; }
        public string? ModeName { get; set; }
        public string? ModeType { get; set; }
        public decimal RatePerUnit { get; set; }
        public decimal? MaxDailyLimit { get; set; }
        public decimal? MaxMonthlyLimit { get; set; }
        public bool IsActive { get; set; } = true;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class EmployeeTARateSaveRequest
    {
        public int CompID { get; set; }
        public int PolicyID { get; set; }
        public int UID { get; set; }
        public int TravelModeID { get; set; }
        public decimal RatePerUnit { get; set; }
        public decimal? MaxDailyLimit { get; set; }
        public decimal? MaxMonthlyLimit { get; set; }
        public int ModifiedBy { get; set; }
    }

    // ===========================================
    // SP STATUS RESPONSE (generic)
    // ===========================================
    public class SpStatusResponse
    {
        public string Status { get; set; } = string.Empty;
        public string? Message { get; set; }
    }
}
