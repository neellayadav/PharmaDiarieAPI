using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PharmaDiaries.DataAccess;
using PharmaDiaries.Models;
using System;

namespace PharmaDiariesAPI.Controllers.worktype
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class DCRDateRequestController : ControllerBase
    {
        private readonly IDCRDateRequestRepository _repository;

        public DCRDateRequestController(IDCRDateRequestRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Create a new DCR date request (Employee submits request for past date entry)
        /// </summary>
        [HttpPost("CreateRequest")]
        public IActionResult CreateRequest([FromBody] DCRDateRequest request)
        {
            try
            {
                var requestId = _repository.CreateRequest(request);
                if (requestId > 0)
                {
                    return Ok(new { success = true, message = "Request submitted successfully", requestId = requestId });
                }
                return BadRequest(new { success = false, message = "Failed to create request. A request for this date may already exist." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Approve a DCR date request (Admin only)
        /// </summary>
        [HttpPost("Approve")]
        public IActionResult Approve([FromQuery] int requestId, [FromQuery] int approvedBy)
        {
            try
            {
                var result = _repository.Approve(requestId, approvedBy);
                return Ok(new { success = result, message = result ? "Request approved successfully" : "Failed to approve request" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Reject a DCR date request (Admin only)
        /// </summary>
        [HttpPost("Reject")]
        public IActionResult Reject([FromQuery] int requestId, [FromQuery] int approvedBy, [FromQuery] string? rejectionReason)
        {
            try
            {
                var result = _repository.Reject(requestId, approvedBy, rejectionReason);
                return Ok(new { success = result, message = result ? "Request rejected" : "Failed to reject request" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get all requests for an employee
        /// </summary>
        [HttpGet("GetByEmployee")]
        public IActionResult GetByEmployee([FromQuery] int compId, [FromQuery] int employeeId)
        {
            try
            {
                var requests = _repository.GetByEmployee(compId, employeeId);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get all pending requests for a company (Admin view)
        /// </summary>
        [HttpGet("GetPendingRequests")]
        public IActionResult GetPendingRequests([FromQuery] int compId)
        {
            try
            {
                var requests = _repository.GetPendingRequests(compId);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get all requests with optional filters (Admin view)
        /// </summary>
        [HttpGet("GetAllRequests")]
        public IActionResult GetAllRequests(
            [FromQuery] int compId,
            [FromQuery] string? status,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            try
            {
                var requests = _repository.GetAllRequests(compId, status, fromDate, toDate);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Check if a specific date is approved for DCR entry
        /// </summary>
        [HttpGet("IsDateApproved")]
        public IActionResult IsDateApproved([FromQuery] int compId, [FromQuery] int employeeId, [FromQuery] DateTime requestedDate)
        {
            try
            {
                var isApproved = _repository.IsDateApproved(compId, employeeId, requestedDate);
                return Ok(new { isApproved = isApproved });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
