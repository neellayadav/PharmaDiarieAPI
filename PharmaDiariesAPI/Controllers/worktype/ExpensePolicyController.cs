using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.Models;

namespace PharmaDiariesAPI.Controllers.worktype
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class ExpensePolicyController : Controller
    {
        private IConfiguration _iconfiguration;
        private IExpensePolicyBusiness IResultData;

        public ExpensePolicyController(IConfiguration iconfiguration, IExpensePolicyBusiness iresultdata)
        {
            _iconfiguration = iconfiguration;
            IResultData = iresultdata;
        }

        // ===== TRAVEL MODES =====

        [HttpPost("CreateTravelMode")]
        public async Task<IActionResult> CreateTravelMode([FromBody] TravelModeCreateRequest request)
        {
            try
            {
                var id = await this.IResultData.CreateTravelModeAsync(request);
                if (id > 0)
                    return Ok(new ApiResponse<int> { Success = true, Message = "Travel mode created", Data = id });
                return BadRequest(new ApiResponse<int> { Success = false, Message = "Failed to create travel mode" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("UpdateTravelMode")]
        public async Task<IActionResult> UpdateTravelMode([FromBody] TravelModeUpdateRequest request)
        {
            try
            {
                var result = await this.IResultData.UpdateTravelModeAsync(request);
                if (result)
                    return Ok(new ApiResponse<bool> { Success = true, Message = "Travel mode updated", Data = true });
                return NotFound(new ApiResponse<bool> { Success = false, Message = "Travel mode not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("GetTravelModes")]
        public async Task<IActionResult> GetTravelModes()
        {
            try
            {
                var data = await this.IResultData.GetTravelModesAsync();
                return Ok(new ApiResponse<List<TravelModeModel>> { Success = true, Message = "Success", Data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        // ===== ALLOWANCE POLICY =====

        [HttpPost("CreatePolicy")]
        public async Task<IActionResult> CreatePolicy([FromBody] AllowancePolicyCreateRequest request)
        {
            try
            {
                var id = await this.IResultData.CreatePolicyAsync(request);
                if (id > 0)
                    return Ok(new ApiResponse<int> { Success = true, Message = "Policy created", Data = id });
                return BadRequest(new ApiResponse<int> { Success = false, Message = "Failed to create policy" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("UpdatePolicy")]
        public async Task<IActionResult> UpdatePolicy([FromBody] AllowancePolicyUpdateRequest request)
        {
            try
            {
                var result = await this.IResultData.UpdatePolicyAsync(request);
                if (result)
                    return Ok(new ApiResponse<bool> { Success = true, Message = "Policy updated", Data = true });
                return NotFound(new ApiResponse<bool> { Success = false, Message = "Policy not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("GetPolicies")]
        public async Task<IActionResult> GetPolicies(int compID)
        {
            try
            {
                var data = await this.IResultData.GetPoliciesAsync(compID);
                return Ok(new ApiResponse<List<AllowancePolicyModel>> { Success = true, Message = "Success", Data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        // ===== DA RATES =====

        [HttpPost("SaveDARates")]
        public async Task<IActionResult> SaveDARates([FromBody] List<DARateSaveRequest> rates)
        {
            try
            {
                int saved = 0;
                foreach (var rate in rates)
                {
                    var result = await this.IResultData.SaveDARateAsync(rate);
                    if (result) saved++;
                }
                return Ok(new ApiResponse<int> { Success = true, Message = $"{saved} DA rates saved", Data = saved });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("GetDARates")]
        public async Task<IActionResult> GetDARates(int compID, int policyID)
        {
            try
            {
                var data = await this.IResultData.GetDARatesByPolicyAsync(compID, policyID);
                return Ok(new ApiResponse<List<DARateModel>> { Success = true, Message = "Success", Data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        // ===== TA RATES =====

        [HttpPost("SaveTARates")]
        public async Task<IActionResult> SaveTARates([FromBody] List<TARateSaveRequest> rates)
        {
            try
            {
                int saved = 0;
                foreach (var rate in rates)
                {
                    var result = await this.IResultData.SaveTARateAsync(rate);
                    if (result) saved++;
                }
                return Ok(new ApiResponse<int> { Success = true, Message = $"{saved} TA rates saved", Data = saved });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("GetTARates")]
        public async Task<IActionResult> GetTARates(int compID, int policyID)
        {
            try
            {
                var data = await this.IResultData.GetTARatesByPolicyAsync(compID, policyID);
                return Ok(new ApiResponse<List<TARateModel>> { Success = true, Message = "Success", Data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        // ===== EMPLOYEE DA RATES =====

        [HttpPost("SaveEmployeeDARates")]
        public async Task<IActionResult> SaveEmployeeDARates([FromBody] List<EmployeeDARateSaveRequest> rates)
        {
            try
            {
                int saved = 0;
                foreach (var rate in rates)
                {
                    var result = await this.IResultData.SaveEmployeeDARateAsync(rate);
                    if (result) saved++;
                }
                return Ok(new ApiResponse<int> { Success = true, Message = $"{saved} employee DA rates saved", Data = saved });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("GetEmployeeDARates")]
        public async Task<IActionResult> GetEmployeeDARates(int compID, int policyID, int? uid = null)
        {
            try
            {
                var data = await this.IResultData.GetEmployeeDARatesAsync(compID, policyID, uid);
                return Ok(new ApiResponse<List<EmployeeDARateModel>> { Success = true, Message = "Success", Data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        // ===== EMPLOYEE TA RATES =====

        [HttpPost("SaveEmployeeTARates")]
        public async Task<IActionResult> SaveEmployeeTARates([FromBody] List<EmployeeTARateSaveRequest> rates)
        {
            try
            {
                int saved = 0;
                foreach (var rate in rates)
                {
                    var result = await this.IResultData.SaveEmployeeTARateAsync(rate);
                    if (result) saved++;
                }
                return Ok(new ApiResponse<int> { Success = true, Message = $"{saved} employee TA rates saved", Data = saved });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("GetEmployeeTARates")]
        public async Task<IActionResult> GetEmployeeTARates(int compID, int policyID, int? uid = null)
        {
            try
            {
                var data = await this.IResultData.GetEmployeeTARatesAsync(compID, policyID, uid);
                return Ok(new ApiResponse<List<EmployeeTARateModel>> { Success = true, Message = "Success", Data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }
    }
}
