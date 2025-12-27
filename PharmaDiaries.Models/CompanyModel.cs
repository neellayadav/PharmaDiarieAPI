using System;
namespace PharmaDiaries.Models
{
	public class CompanyModel
	{
        public int CompID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address1 { get; set; }
        public string? Locality { get; set; }
        public string? CityOrTown { get; set; }
        public int? Pincode { get; set; }
        public string? District { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Mobile { get; set; }
        public string? Telephone { get; set; }
        public string? Fax { get; set; }
        public string? LogoURL { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsLocationTrackerEnabled { get; set; } = false;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class CompanyCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Address1 { get; set; }
        public string? Locality { get; set; }
        public string? CityOrTown { get; set; }
        public int? Pincode { get; set; }
        public string? District { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Mobile { get; set; }
        public string? Telephone { get; set; }
        public string? Fax { get; set; }
        public bool IsActive { get; set; } = true;
        public int CreatedBy { get; set; }
    }

    public class CompanyUpdateRequest
    {
        public int CompID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address1 { get; set; }
        public string? Locality { get; set; }
        public string? CityOrTown { get; set; }
        public int? Pincode { get; set; }
        public string? District { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Mobile { get; set; }
        public string? Telephone { get; set; }
        public string? Fax { get; set; }
        public string? LogoURL { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsLocationTrackerEnabled { get; set; }
        public int ModifiedBy { get; set; }
    }

    public class CompanyStatusUpdateRequest
    {
        public int CompID { get; set; }
        public bool IsActive { get; set; }
        public int ModifiedBy { get; set; }
    }

    public class CompanyCountResponse
    {
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public int InactiveCount { get; set; }
    }

    public class NameExistsRequest
    {
        public string Name { get; set; } = string.Empty;
        public int? ExcludingCompID { get; set; }
    }

    public class NameExistsResponse
    {
        public bool Exists { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}