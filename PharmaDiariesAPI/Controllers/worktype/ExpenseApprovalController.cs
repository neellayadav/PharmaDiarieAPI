using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.Models;

namespace PharmaDiariesAPI.Controllers.worktype
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class ExpenseApprovalController : Controller
    {
        private IConfiguration _iconfiguration;
        private IExpenseBusiness IResultData;

        public ExpenseApprovalController(IConfiguration iconfiguration, IExpenseBusiness iresultdata)
        {
            _iconfiguration = iconfiguration;
            IResultData = iresultdata;
        }

        [HttpGet("GetPending")]
        public async Task<IActionResult> GetPending(int compID, int approverUID)
        {
            try
            {
                var data = await this.IResultData.GetPendingApprovalsAsync(compID, approverUID);
                return Ok(new ApiResponse<List<ExpenseClaimModel>> { Success = true, Message = "Success", Data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("Approve")]
        public async Task<IActionResult> Approve([FromBody] ClaimApproveRequest request)
        {
            try
            {
                var result = await this.IResultData.ApproveClaimAsync(request);
                if (result.Status == "ERROR")
                    return BadRequest(new ApiResponse<SpStatusResponse> { Success = false, Message = result.Message, Data = result });
                return Ok(new ApiResponse<SpStatusResponse> { Success = true, Message = result.Message, Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("Reject")]
        public async Task<IActionResult> Reject([FromBody] ClaimRejectRequest request)
        {
            try
            {
                var result = await this.IResultData.RejectClaimAsync(request);
                if (result.Status == "ERROR")
                    return BadRequest(new ApiResponse<SpStatusResponse> { Success = false, Message = result.Message, Data = result });
                return Ok(new ApiResponse<SpStatusResponse> { Success = true, Message = result.Message, Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("Return")]
        public async Task<IActionResult> Return([FromBody] ClaimReturnRequest request)
        {
            try
            {
                var result = await this.IResultData.ReturnClaimAsync(request);
                if (result.Status == "ERROR")
                    return BadRequest(new ApiResponse<SpStatusResponse> { Success = false, Message = result.Message, Data = result });
                return Ok(new ApiResponse<SpStatusResponse> { Success = true, Message = result.Message, Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("BulkApprove")]
        public async Task<IActionResult> BulkApprove([FromBody] BulkApproveRequest request)
        {
            try
            {
                var result = await this.IResultData.BulkApproveAsync(request);
                if (result.Status == "ERROR")
                    return BadRequest(new ApiResponse<SpStatusResponse> { Success = false, Message = result.Message, Data = result });
                return Ok(new ApiResponse<SpStatusResponse> { Success = true, Message = result.Message, Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("GetHistory")]
        public async Task<IActionResult> GetHistory(int compID, int claimID)
        {
            try
            {
                var data = await this.IResultData.GetApprovalHistoryAsync(compID, claimID);
                return Ok(new ApiResponse<List<ApprovalActionModel>> { Success = true, Message = "Success", Data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }
    }
}
