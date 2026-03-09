using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.Models;

namespace PharmaDiariesAPI.Controllers.worktype
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class ExpenseReportController : Controller
    {
        private IConfiguration _iconfiguration;
        private IExpenseBusiness IResultData;

        public ExpenseReportController(IConfiguration iconfiguration, IExpenseBusiness iresultdata)
        {
            _iconfiguration = iconfiguration;
            IResultData = iresultdata;
        }

        [HttpGet("GetAllClaims")]
        public async Task<IActionResult> GetAllClaims(int compID, int? month, int? year, int? uid, string? status)
        {
            try
            {
                var data = await this.IResultData.GetAllClaimsAsync(compID, month, year, uid, status);
                return Ok(new ApiResponse<List<ExpenseClaimModel>> { Success = true, Message = "Success", Data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("SettleClaim")]
        public async Task<IActionResult> SettleClaim([FromBody] ClaimSettleRequest request)
        {
            try
            {
                var result = await this.IResultData.SettleClaimAsync(request);
                if (result.Status == "ERROR")
                    return BadRequest(new ApiResponse<SpStatusResponse> { Success = false, Message = result.Message, Data = result });
                return Ok(new ApiResponse<SpStatusResponse> { Success = true, Message = result.Message, Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("GetSettlements")]
        public async Task<IActionResult> GetSettlements(int compID, int month, int year)
        {
            try
            {
                var data = await this.IResultData.GetSettlementsByMonthAsync(compID, month, year);
                return Ok(new ApiResponse<List<SettlementModel>> { Success = true, Message = "Success", Data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }
    }
}
