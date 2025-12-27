using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.DataAccessContract;
using PharmaDiaries.Models;

namespace PharmaDiariesAPI.Controllers.worktype
{

    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class CompanyController : Controller
    {
        private readonly IConfiguration _iconfiguration;
        private readonly ICompanyBusiness IResultData;
        public CompanyController(IConfiguration iconfiguration, ICompanyBusiness iResultData)
        {
            _iconfiguration = iconfiguration;
            IResultData = iResultData;

        }

        [HttpPost("Insert")]
        public async Task<ActionResult<ApiResponse<int>>> CreateCompany(CompanyCreateRequest request)
        {
            try
            {
                // Check if company name already exists
                var nameExists = await this.IResultData.CheckCompanyNameExistsAsync(request.Name);
                if (nameExists)
                {
                    return BadRequest(new ApiResponse<int>
                    {
                        Success = false,
                        Message = "Company name already exists"
                    });
                }

                var compId = await this.IResultData.CreateCompanyAsync(request);
                return Ok(new ApiResponse<int>
                {
                    Success = true,
                    Message = "Company created successfully",
                    Data = compId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<int>
                {
                    Success = false,
                    Message = $"Error creating company: {ex.Message}"
                });
            }
        }

        [HttpGet("GetCompanyById")]
        public async Task<ActionResult<ApiResponse<CompanyModel>>> GetCompanyById(int id)
        {
            try
            {
                var company = await this.IResultData.GetCompanyByIdAsync(id);
                if (company == null)
                {
                    return NotFound(new ApiResponse<CompanyModel>
                    {
                        Success = false,
                        Message = "Company not found"
                    });
                }

                return Ok(new ApiResponse<CompanyModel>
                {
                    Success = true,
                    Message = "Company retrieved successfully",
                    Data = company
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<CompanyModel>
                {
                    Success = false,
                    Message = $"Error retrieving company: {ex.Message}"
                });
            }
        }

        [HttpGet("GetAllCompanies")]
        public async Task<ActionResult<ApiResponse<List<CompanyModel>>>> GetAllCompanies()
        {
            try
            {
                var companies = await this.IResultData.GetAllCompaniesAsync();
                return Ok(new ApiResponse<List<CompanyModel>>
                {
                    Success = true,
                    Message = "Companies retrieved successfully",
                    Data = companies
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<CompanyModel>>
                {
                    Success = false,
                    Message = $"Error retrieving companies: {ex.Message}"
                });
            }
        }

        [HttpGet("active")]
        public async Task<ActionResult<ApiResponse<List<CompanyModel>>>> GetActiveCompanies()
        {
            try
            {
                var companies = await this.IResultData.GetActiveCompaniesAsync();
                return Ok(new ApiResponse<List<CompanyModel>>
                {
                    Success = true,
                    Message = "Active companies retrieved successfully",
                    Data = companies
                }) ;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<CompanyModel>>
                {
                    Success = false,
                    Message = $"Error retrieving active companies: {ex.Message}"
                });
            }
        }

        [HttpGet("search/{searchTerm}")]
        public async Task<ActionResult<ApiResponse<List<CompanyModel>>>> SearchCompanies(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
                {
                    return BadRequest(new ApiResponse<List<CompanyModel>>
                    {
                        Success = false,
                        Message = "Search term must be at least 2 characters long"
                    });
                }

                var companies = await this.IResultData.SearchCompaniesByNameAsync(searchTerm);
                return Ok(new ApiResponse<List<CompanyModel>>
                {
                    Success = true,
                    Message = "Companies search completed successfully",
                    Data = companies
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<CompanyModel>>
                {
                    Success = false,
                    Message = $"Error searching companies: {ex.Message}"
                });
            }
        }

        [HttpPut("Update")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateCompany(CompanyUpdateRequest request)
        {
            try
            {
                // Check if company exists
                var existingCompany = await this.IResultData.GetCompanyByIdAsync(request.CompID);
                if (existingCompany == null)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Company not found"
                    });
                }

                // Check if company name already exists (excluding current company)
                var nameExists = await this.IResultData.CheckCompanyNameExistsAsync(request.Name, request.CompID);
                if (nameExists)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Company name already exists"
                    });
                }

                var updated = await this.IResultData.UpdateCompanyAsync(request);
                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Company updated successfully",
                    Data = updated
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Error updating company: {ex.Message}"
                });
            }
        }

        [HttpPatch("Status")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateCompanyStatus(CompanyStatusUpdateRequest request)
        {
            try
            {
                var updated = await this.IResultData.UpdateCompanyStatusAsync(request);
                if (!updated)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Company not found"
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Company status updated successfully",
                    Data = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Error updating company status: {ex.Message}"
                });
            }
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCompany(int id, [FromQuery] int modifiedBy)
        {
            try
            {
                var deleted = await this.IResultData.DeleteCompanyAsync(id, modifiedBy);
                if (!deleted)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Company not found"
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Company deleted successfully",
                    Data = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting company: {ex.Message}"
                });
            }
        }

        [HttpPost("CheckName")]
        public async Task<ActionResult<ApiResponse<NameExistsResponse>>> CheckCompanyNameExists(NameExistsRequest request)
        {
            try
            {
                var exists = await this.IResultData.CheckCompanyNameExistsAsync(request.Name, request.ExcludingCompID);
                return Ok(new ApiResponse<NameExistsResponse>
                {
                    Success = true,
                    Message = "Name check completed",
                    Data = new NameExistsResponse { Exists = exists }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<NameExistsResponse>
                {
                    Success = false,
                    Message = $"Error checking company name: {ex.Message}"
                });
            }
        }

        [HttpGet("Count")]
        public async Task<ActionResult<ApiResponse<CompanyCountResponse>>> GetCompanyCount()
        {
            try
            {
                var count = await this.IResultData.GetCompanyCountAsync();
                return Ok(new ApiResponse<CompanyCountResponse>
                {
                    Success = true,
                    Message = "Company count retrieved successfully",
                    Data = count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<CompanyCountResponse>
                {
                    Success = false,
                    Message = $"Error retrieving company count: {ex.Message}"
                });
            }
        }

        [HttpGet("GetLocationTrackerSetting")]
        public async Task<ActionResult<ApiResponse<bool>>> GetLocationTrackerSetting([FromQuery] int compId)
        {
            try
            {
                var isEnabled = await this.IResultData.GetLocationTrackerSettingAsync(compId);
                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Location tracker setting retrieved successfully",
                    Data = isEnabled
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Error retrieving location tracker setting: {ex.Message}"
                });
            }
        }

        [HttpPost("UpdateLocationTrackerSetting")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateLocationTrackerSetting([FromQuery] int compId, [FromQuery] bool isEnabled, [FromQuery] int modifiedBy)
        {
            try
            {
                var updated = await this.IResultData.UpdateLocationTrackerSettingAsync(compId, isEnabled, modifiedBy);
                if (!updated)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Company not found"
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Location tracker setting updated successfully",
                    Data = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Error updating location tracker setting: {ex.Message}"
                });
            }
        }
    }
}