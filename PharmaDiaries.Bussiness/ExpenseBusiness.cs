using PharmaDiaries.BusinessContract;
using PharmaDiaries.DataAccessContract;
using PharmaDiaries.Models;

namespace PharmaDiaries.Bussiness
{
    public class ExpenseBusiness : IExpenseBusiness
    {
        private IExpenseRepository _irepository;

        public ExpenseBusiness(IExpenseRepository irepository)
        {
            this._irepository = irepository;
        }

        public Task<ExpenseCalculateResponse> CalculateExpenseAsync(ExpenseCalculateRequest request) => _irepository.CalculateExpenseAsync(request);

        public Task<ExpenseClaimCreateResponse> CreateClaimAsync(ExpenseClaimCreateRequest request) => _irepository.CreateClaimAsync(request);
        public Task<bool> UpdateClaimAsync(ExpenseClaimUpdateRequest request) => _irepository.UpdateClaimAsync(request);
        public Task<List<ExpenseClaimModel>> GetClaimsByUIDAsync(int compID, int uid, int? month, int? year, string? status) => _irepository.GetClaimsByUIDAsync(compID, uid, month, year, status);
        public Task<List<ExpenseClaimModel>> GetAllClaimsAsync(int compID, int? month, int? year, int? uid, string? status) => _irepository.GetAllClaimsAsync(compID, month, year, uid, status);
        public Task<ExpenseClaimModel?> GetClaimByIDAsync(int compID, int claimID) => _irepository.GetClaimByIDAsync(compID, claimID);
        public Task<List<ExpenseClaimLineModel>> GetClaimLinesByClaimIDAsync(int compID, int claimID) => _irepository.GetClaimLinesByClaimIDAsync(compID, claimID);
        public Task<List<ApprovalActionModel>> GetApprovalHistoryByClaimIDAsync(int compID, int claimID) => _irepository.GetApprovalHistoryByClaimIDAsync(compID, claimID);
        public Task<SpStatusResponse> SubmitClaimAsync(ClaimSubmitRequest request) => _irepository.SubmitClaimAsync(request);
        public Task<SpStatusResponse> ResubmitClaimAsync(ClaimSubmitRequest request) => _irepository.ResubmitClaimAsync(request);

        public Task<ClaimLineInsertResponse> AddClaimLineAsync(ClaimLineAddRequest request) => _irepository.AddClaimLineAsync(request);
        public Task<ClaimLineInsertResponse> UpdateClaimLineAsync(ClaimLineUpdateRequest request) => _irepository.UpdateClaimLineAsync(request);
        public Task<SpStatusResponse> DeleteClaimLineAsync(int compID, int lineID, int modifiedBy) => _irepository.DeleteClaimLineAsync(compID, lineID, modifiedBy);

        public Task<int> InsertAttachmentAsync(ClaimAttachmentInsertRequest request) => _irepository.InsertAttachmentAsync(request);
        public Task<List<ClaimAttachmentModel>> GetAttachmentsByLineAsync(int compID, int lineID) => _irepository.GetAttachmentsByLineAsync(compID, lineID);

        public Task<List<ExpenseClaimModel>> GetPendingApprovalsAsync(int compID, int approverUID) => _irepository.GetPendingApprovalsAsync(compID, approverUID);
        public Task<SpStatusResponse> ApproveClaimAsync(ClaimApproveRequest request) => _irepository.ApproveClaimAsync(request);
        public Task<SpStatusResponse> RejectClaimAsync(ClaimRejectRequest request) => _irepository.RejectClaimAsync(request);
        public Task<SpStatusResponse> ReturnClaimAsync(ClaimReturnRequest request) => _irepository.ReturnClaimAsync(request);
        public Task<SpStatusResponse> BulkApproveAsync(BulkApproveRequest request) => _irepository.BulkApproveAsync(request);
        public Task<List<ApprovalActionModel>> GetApprovalHistoryAsync(int compID, int claimID) => _irepository.GetApprovalHistoryAsync(compID, claimID);

        public Task<SpStatusResponse> SettleClaimAsync(ClaimSettleRequest request) => _irepository.SettleClaimAsync(request);
        public Task<List<SettlementModel>> GetSettlementsByMonthAsync(int compID, int month, int year) => _irepository.GetSettlementsByMonthAsync(compID, month, year);
    }
}
