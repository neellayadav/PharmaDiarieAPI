using PharmaDiaries.BusinessContract;
using PharmaDiaries.DataAccessContract;
using PharmaDiaries.Models;

namespace PharmaDiaries.Bussiness
{
    public class ExpensePolicyBusiness : IExpensePolicyBusiness
    {
        private IExpensePolicyRepository _irepository;

        public ExpensePolicyBusiness(IExpensePolicyRepository irepository)
        {
            this._irepository = irepository;
        }

        public Task<int> CreateTravelModeAsync(TravelModeCreateRequest request) => _irepository.CreateTravelModeAsync(request);
        public Task<bool> UpdateTravelModeAsync(TravelModeUpdateRequest request) => _irepository.UpdateTravelModeAsync(request);
        public Task<List<TravelModeModel>> GetTravelModesAsync() => _irepository.GetTravelModesAsync();

        public Task<int> CreatePolicyAsync(AllowancePolicyCreateRequest request) => _irepository.CreatePolicyAsync(request);
        public Task<bool> UpdatePolicyAsync(AllowancePolicyUpdateRequest request) => _irepository.UpdatePolicyAsync(request);
        public Task<List<AllowancePolicyModel>> GetPoliciesAsync(int compID) => _irepository.GetPoliciesAsync(compID);
        public Task<AllowancePolicyModel?> GetActivePolicyAsync(int compID, DateTime asOfDate) => _irepository.GetActivePolicyAsync(compID, asOfDate);

        public Task<bool> SaveDARateAsync(DARateSaveRequest request) => _irepository.SaveDARateAsync(request);
        public Task<List<DARateModel>> GetDARatesByPolicyAsync(int compID, int policyID) => _irepository.GetDARatesByPolicyAsync(compID, policyID);

        public Task<bool> SaveTARateAsync(TARateSaveRequest request) => _irepository.SaveTARateAsync(request);
        public Task<List<TARateModel>> GetTARatesByPolicyAsync(int compID, int policyID) => _irepository.GetTARatesByPolicyAsync(compID, policyID);

        public Task<bool> SaveEmployeeDARateAsync(EmployeeDARateSaveRequest request) => _irepository.SaveEmployeeDARateAsync(request);
        public Task<List<EmployeeDARateModel>> GetEmployeeDARatesAsync(int compID, int policyID, int? uid = null) => _irepository.GetEmployeeDARatesAsync(compID, policyID, uid);

        public Task<bool> SaveEmployeeTARateAsync(EmployeeTARateSaveRequest request) => _irepository.SaveEmployeeTARateAsync(request);
        public Task<List<EmployeeTARateModel>> GetEmployeeTARatesAsync(int compID, int policyID, int? uid = null) => _irepository.GetEmployeeTARatesAsync(compID, policyID, uid);
    }
}
