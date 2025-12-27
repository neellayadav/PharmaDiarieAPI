using System;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.DataAccessContract;
using PharmaDiaries.Models;

namespace PharmaDiaries.Bussiness
{
    public class CompanyBusiness : ICompanyBusiness
    {
        private ICompanyRepository _irepository;

        public CompanyBusiness(ICompanyRepository irepository)
        {
            this._irepository = irepository;

        }

        public Task<bool> CheckCompanyNameExistsAsync(string name, int? excludingCompId = null)
        {
            return _irepository.CheckCompanyNameExistsAsync(name, excludingCompId);
        }

        public Task<int> CreateCompanyAsync(CompanyCreateRequest request)
        {
            return _irepository.CreateCompanyAsync(request);
        }

        public Task<bool> DeleteCompanyAsync(int compId, int modifiedBy)
        {
            return _irepository.DeleteCompanyAsync(compId, modifiedBy);
        }

        public Task<List<CompanyModel>> GetActiveCompaniesAsync()
        {
            return _irepository.GetActiveCompaniesAsync();
        }

        public Task<List<CompanyModel>> GetAllCompaniesAsync()
        {
            return _irepository.GetAllCompaniesAsync();
        }

        public Task<CompanyModel?> GetCompanyByIdAsync(int compId)
        {
            return _irepository.GetCompanyByIdAsync(compId);
        }

        public Task<CompanyCountResponse> GetCompanyCountAsync()
        {
            return _irepository.GetCompanyCountAsync();
        }

        public Task<List<CompanyModel>> SearchCompaniesByNameAsync(string searchTerm)
        {
            return _irepository.SearchCompaniesByNameAsync(searchTerm);
        }

        public Task<bool> UpdateCompanyAsync(CompanyUpdateRequest request)
        {
            return _irepository.UpdateCompanyAsync(request);
        }

        public Task<bool> UpdateCompanyStatusAsync(CompanyStatusUpdateRequest request)
        {
            return _irepository.UpdateCompanyStatusAsync(request);
        }

        public Task<bool> UpdateLocationTrackerSettingAsync(int compId, bool isEnabled, int modifiedBy)
        {
            return _irepository.UpdateLocationTrackerSettingAsync(compId, isEnabled, modifiedBy);
        }

        public Task<bool> GetLocationTrackerSettingAsync(int compId)
        {
            return _irepository.GetLocationTrackerSettingAsync(compId);
        }
    }
}
