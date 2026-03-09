using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.Models;

namespace PharmaDiariesAPI.Controllers.worktype
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class ExpenseController : Controller
    {
        private IConfiguration _iconfiguration;
        private IExpenseBusiness IResultData;

        public ExpenseController(IConfiguration iconfiguration, IExpenseBusiness iresultdata)
        {
            _iconfiguration = iconfiguration;
            IResultData = iresultdata;
        }

        // ===== CALCULATION =====

        [HttpPost("Calculate")]
        public async Task<IActionResult> Calculate([FromBody] ExpenseCalculateRequest request)
        {
            try
            {
                var result = await this.IResultData.CalculateExpenseAsync(request);
                if (result.Status == "ERROR")
                    return BadRequest(new ApiResponse<ExpenseCalculateResponse> { Success = false, Message = result.Message, Data = result });
                return Ok(new ApiResponse<ExpenseCalculateResponse> { Success = true, Message = "Calculated", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        // ===== CLAIMS =====

        [HttpPost("CreateClaim")]
        public async Task<IActionResult> CreateClaim([FromBody] ExpenseClaimCreateRequest request)
        {
            try
            {
                var result = await this.IResultData.CreateClaimAsync(request);
                if (result.ClaimID > 0)
                    return Ok(new ApiResponse<ExpenseClaimCreateResponse> { Success = true, Message = "Claim created", Data = result });
                return BadRequest(new ApiResponse<string> { Success = false, Message = "Failed to create claim" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("UpdateClaim")]
        public async Task<IActionResult> UpdateClaim([FromBody] ExpenseClaimUpdateRequest request)
        {
            try
            {
                var result = await this.IResultData.UpdateClaimAsync(request);
                if (result)
                    return Ok(new ApiResponse<bool> { Success = true, Message = "Claim updated", Data = true });
                return NotFound(new ApiResponse<bool> { Success = false, Message = "Claim not found or not editable" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("GetMyClaims")]
        public async Task<IActionResult> GetMyClaims(int compID, int uid, int? month, int? year, string? status)
        {
            try
            {
                var data = await this.IResultData.GetClaimsByUIDAsync(compID, uid, month, year, status);
                return Ok(new ApiResponse<List<ExpenseClaimModel>> { Success = true, Message = "Success", Data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("GetClaimDetail")]
        public async Task<IActionResult> GetClaimDetail(int compID, int claimID)
        {
            try
            {
                var claim = await this.IResultData.GetClaimByIDAsync(compID, claimID);
                if (claim == null)
                    return NotFound(new ApiResponse<string> { Success = false, Message = "Claim not found" });

                var lines = await this.IResultData.GetClaimLinesByClaimIDAsync(compID, claimID);
                var history = await this.IResultData.GetApprovalHistoryByClaimIDAsync(compID, claimID);

                var result = new
                {
                    Claim = claim,
                    Lines = lines,
                    ApprovalHistory = history
                };
                return Ok(new ApiResponse<object> { Success = true, Message = "Success", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        // ===== CLAIM LINES =====

        [HttpPost("AddLine")]
        public async Task<IActionResult> AddLine([FromBody] ClaimLineAddRequest request)
        {
            try
            {
                var result = await this.IResultData.AddClaimLineAsync(request);
                if (result.Status == "ERROR")
                    return BadRequest(new ApiResponse<ClaimLineInsertResponse> { Success = false, Message = result.Message, Data = result });
                return Ok(new ApiResponse<ClaimLineInsertResponse> { Success = true, Message = "Line added", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("UpdateLine")]
        public async Task<IActionResult> UpdateLine([FromBody] ClaimLineUpdateRequest request)
        {
            try
            {
                var result = await this.IResultData.UpdateClaimLineAsync(request);
                if (result.Status == "ERROR")
                    return BadRequest(new ApiResponse<ClaimLineInsertResponse> { Success = false, Message = result.Message, Data = result });
                return Ok(new ApiResponse<ClaimLineInsertResponse> { Success = true, Message = "Line updated", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("DeleteLine")]
        public async Task<IActionResult> DeleteLine(int compID, int lineID, int modifiedBy)
        {
            try
            {
                var result = await this.IResultData.DeleteClaimLineAsync(compID, lineID, modifiedBy);
                if (result.Status == "ERROR")
                    return BadRequest(new ApiResponse<SpStatusResponse> { Success = false, Message = result.Message, Data = result });
                return Ok(new ApiResponse<SpStatusResponse> { Success = true, Message = result.Message, Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        // ===== SUBMIT =====

        [HttpPost("SubmitClaim")]
        public async Task<IActionResult> SubmitClaim([FromBody] ClaimSubmitRequest request)
        {
            try
            {
                var result = await this.IResultData.SubmitClaimAsync(request);
                if (result.Status == "ERROR")
                    return BadRequest(new ApiResponse<SpStatusResponse> { Success = false, Message = result.Message, Data = result });
                return Ok(new ApiResponse<SpStatusResponse> { Success = true, Message = result.Message, Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("ResubmitClaim")]
        public async Task<IActionResult> ResubmitClaim([FromBody] ClaimSubmitRequest request)
        {
            try
            {
                var result = await this.IResultData.ResubmitClaimAsync(request);
                if (result.Status == "ERROR")
                    return BadRequest(new ApiResponse<SpStatusResponse> { Success = false, Message = result.Message, Data = result });
                return Ok(new ApiResponse<SpStatusResponse> { Success = true, Message = result.Message, Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        // ===== TRAVEL MODES (convenience endpoint) =====

        [HttpGet("GetTravelModes")]
        public async Task<IActionResult> GetTravelModes([FromServices] IExpensePolicyBusiness policyBusiness)
        {
            try
            {
                var data = await policyBusiness.GetTravelModesAsync();
                return Ok(new ApiResponse<List<TravelModeModel>> { Success = true, Message = "Success", Data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        // ===== ATTACHMENTS =====

        [HttpPost("UploadReceipt")]
        public async Task<IActionResult> UploadReceipt([FromBody] ClaimAttachmentInsertRequest request)
        {
            try
            {
                var id = await this.IResultData.InsertAttachmentAsync(request);
                if (id > 0)
                    return Ok(new ApiResponse<int> { Success = true, Message = "Receipt uploaded", Data = id });
                return BadRequest(new ApiResponse<int> { Success = false, Message = "Failed to upload receipt" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("GetAttachments")]
        public async Task<IActionResult> GetAttachments(int compID, int lineID)
        {
            try
            {
                var data = await this.IResultData.GetAttachmentsByLineAsync(compID, lineID);
                return Ok(new ApiResponse<List<ClaimAttachmentModel>> { Success = true, Message = "Success", Data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" });
            }
        }
    }
}
