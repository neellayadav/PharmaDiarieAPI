using System;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.Models;

namespace PharmaDiariesAPI.Controllers.worktype
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class SalesController : ControllerBase
    {
        private readonly ISalesBusiness _salesBusiness;

        public SalesController(ISalesBusiness salesBusiness)
        {
            _salesBusiness = salesBusiness;
        }

        // ===================== COMBINED CREATE =====================

        [HttpPost("CreateSale")]
        public IActionResult CreateSale([FromBody] CreateSaleRequest request)
        {
            try
            {
                if (request.Details == null || request.Details.Count == 0)
                    return Ok(new ApiResponse<string> { Success = false, Message = "At least one detail item is required" });

                var result = _salesBusiness.CreateSale(request);
                if (string.IsNullOrEmpty(result.SalesID))
                    return Ok(new ApiResponse<string> { Success = false, Message = "Failed to create sale" });

                return Ok(new ApiResponse<CreateSaleResponse>
                {
                    Success = true,
                    Message = $"Sale created with {result.ItemIDs.Count} items",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = ex.Message });
            }
        }

        // ===================== HEADER =====================

        [HttpPost("CreateHeader")]
        public IActionResult CreateHeader([FromBody] SalesHeaderCreateRequest request)
        {
            try
            {
                var salesId = _salesBusiness.CreateHeader(request);
                if (string.IsNullOrEmpty(salesId))
                    return Ok(new ApiResponse<string> { Success = false, Message = "Failed to create sales header" });

                return Ok(new ApiResponse<string> { Success = true, Message = "Sales header created", Data = salesId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("GetHeaderById")]
        public IActionResult GetHeaderById(int compId, string salesId, int uid)
        {
            try
            {
                var result = _salesBusiness.GetHeaderById(compId, salesId, uid);
                if (result == null)
                    return Ok(new ApiResponse<SalesHeaderModel> { Success = false, Message = "Record not found" });

                return Ok(new ApiResponse<SalesHeaderModel> { Success = true, Message = "Success", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("GetHeaderList")]
        public IActionResult GetHeaderList(int compId, int? uid, int? custId, string? type, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var result = _salesBusiness.GetHeaderList(compId, uid, custId, type, fromDate, toDate);
                return Ok(new ApiResponse<List<SalesHeaderModel>> { Success = true, Message = "Success", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("UpdateHeader")]
        public IActionResult UpdateHeader([FromBody] SalesHeaderUpdateRequest request)
        {
            try
            {
                var result = _salesBusiness.UpdateHeader(request);
                return Ok(new ApiResponse<bool> { Success = result, Message = result ? "Updated successfully" : "Update failed", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("DeleteHeader")]
        public IActionResult DeleteHeader([FromBody] SalesHeaderDeleteRequest request)
        {
            try
            {
                var result = _salesBusiness.DeleteHeader(request);
                return Ok(new ApiResponse<bool> { Success = result, Message = result ? "Deleted successfully" : "Delete failed", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = ex.Message });
            }
        }

        // ===================== DETAIL =====================

        [HttpPost("CreateDetail")]
        public IActionResult CreateDetail([FromBody] SalesDetailCreateRequest request)
        {
            try
            {
                var itemId = _salesBusiness.CreateDetail(request);
                if (itemId == 0)
                    return Ok(new ApiResponse<int> { Success = false, Message = "Failed to create detail" });

                return Ok(new ApiResponse<int> { Success = true, Message = "Detail created", Data = itemId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("GetDetailById")]
        public IActionResult GetDetailById(int itemId)
        {
            try
            {
                var result = _salesBusiness.GetDetailById(itemId);
                if (result == null)
                    return Ok(new ApiResponse<SalesDetailModel> { Success = false, Message = "Record not found" });

                return Ok(new ApiResponse<SalesDetailModel> { Success = true, Message = "Success", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("GetDetailList")]
        public IActionResult GetDetailList(string salesId)
        {
            try
            {
                var result = _salesBusiness.GetDetailList(salesId);
                return Ok(new ApiResponse<List<SalesDetailModel>> { Success = true, Message = "Success", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("UpdateDetail")]
        public IActionResult UpdateDetail([FromBody] SalesDetailUpdateRequest request)
        {
            try
            {
                var result = _salesBusiness.UpdateDetail(request);
                return Ok(new ApiResponse<bool> { Success = result, Message = result ? "Updated successfully" : "Update failed", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("DeleteDetail")]
        public IActionResult DeleteDetail([FromBody] SalesDetailDeleteRequest request)
        {
            try
            {
                var result = _salesBusiness.DeleteDetail(request.ItemID);
                return Ok(new ApiResponse<bool> { Success = result, Message = result ? "Deleted successfully" : "Delete failed", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = ex.Message });
            }
        }
    }
}
