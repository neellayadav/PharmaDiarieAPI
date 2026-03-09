using PharmaDiaries.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PharmaDiaries.BusinessContract
{
    public interface IExpensePolicyBusiness
    {
        public Task<int> CreateTravelModeAsync(TravelModeCreateRequest request);
        public Task<bool> UpdateTravelModeAsync(TravelModeUpdateRequest request);
        public Task<List<TravelModeModel>> GetTravelModesAsync();

        public Task<int> CreatePolicyAsync(AllowancePolicyCreateRequest request);
        public Task<bool> UpdatePolicyAsync(AllowancePolicyUpdateRequest request);
        public Task<List<AllowancePolicyModel>> GetPoliciesAsync(int compID);
        public Task<AllowancePolicyModel?> GetActivePolicyAsync(int compID, DateTime asOfDate);

        public Task<bool> SaveDARateAsync(DARateSaveRequest request);
        public Task<List<DARateModel>> GetDARatesByPolicyAsync(int compID, int policyID);

        public Task<bool> SaveTARateAsync(TARateSaveRequest request);
        public Task<List<TARateModel>> GetTARatesByPolicyAsync(int compID, int policyID);

        public Task<bool> SaveEmployeeDARateAsync(EmployeeDARateSaveRequest request);
        public Task<List<EmployeeDARateModel>> GetEmployeeDARatesAsync(int compID, int policyID, int? uid = null);

        public Task<bool> SaveEmployeeTARateAsync(EmployeeTARateSaveRequest request);
        public Task<List<EmployeeTARateModel>> GetEmployeeTARatesAsync(int compID, int policyID, int? uid = null);
    }
}
