using System;
using PharmaDiaries.Models;

namespace PharmaDiaries.BusinessContract
{
	public interface ICompanyBusiness
	{

        public Task<int> CreateCompanyAsync(CompanyCreateRequest request);

        public Task<CompanyModel?> GetCompanyByIdAsync(int compId);

        public Task<List<CompanyModel>> GetAllCompaniesAsync();

        public Task<List<CompanyModel>> GetActiveCompaniesAsync();

        public Task<List<CompanyModel>> SearchCompaniesByNameAsync(string searchTerm);

        public Task<bool> UpdateCompanyAsync(CompanyUpdateRequest request);

        public Task<bool> UpdateCompanyStatusAsync(CompanyStatusUpdateRequest request);

        public Task<bool> DeleteCompanyAsync(int compId, int modifiedBy);

        public Task<bool> CheckCompanyNameExistsAsync(string name, int? excludingCompId = null);

        public Task<CompanyCountResponse> GetCompanyCountAsync();

        public Task<bool> UpdateLocationTrackerSettingAsync(int compId, bool isEnabled, int modifiedBy);

        public Task<bool> GetLocationTrackerSettingAsync(int compId);

        public Task<GeoFenceSettingsResponse?> GetGeoFenceSettingsAsync(int compId);

        public Task<bool> UpdateGeoFenceSettingsAsync(GeoFenceSettingsRequest request);

        public Task<bool> UpdateCompanySettingsAsync(CompanySettingsUpdateRequest request);
	}
}

