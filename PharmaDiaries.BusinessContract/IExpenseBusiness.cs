using PharmaDiaries.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PharmaDiaries.BusinessContract
{
    public interface IExpenseBusiness
    {
        public Task<ExpenseCalculateResponse> CalculateExpenseAsync(ExpenseCalculateRequest request);

        public Task<ExpenseClaimCreateResponse> CreateClaimAsync(ExpenseClaimCreateRequest request);
        public Task<bool> UpdateClaimAsync(ExpenseClaimUpdateRequest request);
        public Task<List<ExpenseClaimModel>> GetClaimsByUIDAsync(int compID, int uid, int? month, int? year, string? status);
        public Task<List<ExpenseClaimModel>> GetAllClaimsAsync(int compID, int? month, int? year, int? uid, string? status);
        public Task<ExpenseClaimModel?> GetClaimByIDAsync(int compID, int claimID);
        public Task<List<ExpenseClaimLineModel>> GetClaimLinesByClaimIDAsync(int compID, int claimID);
        public Task<List<ApprovalActionModel>> GetApprovalHistoryByClaimIDAsync(int compID, int claimID);
        public Task<SpStatusResponse> SubmitClaimAsync(ClaimSubmitRequest request);
        public Task<SpStatusResponse> ResubmitClaimAsync(ClaimSubmitRequest request);

        public Task<ClaimLineInsertResponse> AddClaimLineAsync(ClaimLineAddRequest request);
        public Task<ClaimLineInsertResponse> UpdateClaimLineAsync(ClaimLineUpdateRequest request);
        public Task<SpStatusResponse> DeleteClaimLineAsync(int compID, int lineID, int modifiedBy);

        public Task<int> InsertAttachmentAsync(ClaimAttachmentInsertRequest request);
        public Task<List<ClaimAttachmentModel>> GetAttachmentsByLineAsync(int compID, int lineID);

        public Task<List<ExpenseClaimModel>> GetPendingApprovalsAsync(int compID, int approverUID);
        public Task<SpStatusResponse> ApproveClaimAsync(ClaimApproveRequest request);
        public Task<SpStatusResponse> RejectClaimAsync(ClaimRejectRequest request);
        public Task<SpStatusResponse> ReturnClaimAsync(ClaimReturnRequest request);
        public Task<SpStatusResponse> BulkApproveAsync(BulkApproveRequest request);
        public Task<List<ApprovalActionModel>> GetApprovalHistoryAsync(int compID, int claimID);

        public Task<SpStatusResponse> SettleClaimAsync(ClaimSettleRequest request);
        public Task<List<SettlementModel>> GetSettlementsByMonthAsync(int compID, int month, int year);
    }
}
