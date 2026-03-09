using System;
using System.ComponentModel;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.Models;

namespace PharmaDiariesAPI.Controllers.worktype
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowOrigin")]
    public class LoginLogController : ControllerBase
    {
        private readonly ILoginLogBusiness _loginLogBusiness;

        public LoginLogController(ILoginLogBusiness loginLogBusiness)
        {
            _loginLogBusiness = loginLogBusiness;
        }

        /// <summary>
        /// Insert new login log and return the complete LoginLogModel with auto-generated LogID
        /// </summary>
        [HttpPost("Save")]
        public ActionResult<ApiResponse<LoginLogModel>> Save([FromBody] LoginLogInsertRequest request)
        {
            try
            {
                var loginLog = _loginLogBusiness.Save(request);
                if (loginLog != null)
                {
                    return Ok(new ApiResponse<LoginLogModel>
                    {
                        Success = true,
                        Message = "Login log saved successfully",
                        Data = loginLog
                    });
                }
                return Ok(new ApiResponse<LoginLogModel>
                {
                    Success = false,
                    Message = "Failed to save login log"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<LoginLogModel>
                {
                    Success = false,
                    Message = $"Error saving login log: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Update logout time and return the complete updated LoginLogModel
        /// </summary>
        [HttpPut("Update")]
        public ActionResult<ApiResponse<LoginLogModel>> Update([FromBody] LoginLogUpdateRequest request)
        {
            try
            {
                var loginLog = _loginLogBusiness.Update(request);
                if (loginLog != null)
                {
                    return Ok(new ApiResponse<LoginLogModel>
                    {
                        Success = true,
                        Message = "Logout time updated successfully",
                        Data = loginLog
                    });
                }
                return Ok(new ApiResponse<LoginLogModel>
                {
                    Success = false,
                    Message = "Login log not found or failed to update"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<LoginLogModel>
                {
                    Success = false,
                    Message = $"Error updating login log: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get logs by CompID and UID
        /// </summary>
        [HttpGet("GetByCompUID")]
        public ActionResult<ApiResponse<List<LoginLogModel>>> GetByCompUID(int compId, int uid)
        {
            try
            {
                var result = _loginLogBusiness.GetByCompUID(compId, uid);
                return Ok(new ApiResponse<List<LoginLogModel>>
                {
                    Success = true,
                    Message = "Login logs retrieved successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<LoginLogModel>>
                {
                    Success = false,
                    Message = $"Error retrieving login logs: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get log by LogID
        /// </summary>
        [HttpGet("GetByLogId")]
        public ActionResult<ApiResponse<LoginLogModel>> GetByLogId(int logId)
        {
            try
            {
                var result = _loginLogBusiness.GetByLogId(logId);
                if (result != null)
                {
                    return Ok(new ApiResponse<LoginLogModel>
                    {
                        Success = true,
                        Message = "Login log retrieved successfully",
                        Data = result
                    });
                }
                return NotFound(new ApiResponse<LoginLogModel>
                {
                    Success = false,
                    Message = "Login log not found"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<LoginLogModel>
                {
                    Success = false,
                    Message = $"Error retrieving login log: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get logs by CompID, UID, Month and Year
        /// </summary>
        [HttpGet("GetByCompUIDMonthYear")]
        public ActionResult<ApiResponse<List<LoginLogModel>>> GetByCompUIDMonthYear(int compId, int uid, int monthOf, int yearOf)
        {
            try
            {
                var result = _loginLogBusiness.GetByCompUIDMonthYear(compId, uid, monthOf, yearOf);
                return Ok(new ApiResponse<List<LoginLogModel>>
                {
                    Success = true,
                    Message = "Login logs retrieved successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<LoginLogModel>>
                {
                    Success = false,
                    Message = $"Error retrieving login logs: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get logs by CompID, UID and specific Date
        /// </summary>
        /// <param name="compId">Company ID</param>
        /// <param name="uid">User ID</param>
        /// <param name="dateOf">Date in format: yyyy-MM-dd (e.g., 2026-02-07)</param>
        [HttpGet("GetByCompUIDDate")]
        public ActionResult<ApiResponse<List<LoginLogModel>>> GetByCompUIDDate(int compId, int uid, [DefaultValue("2026-02-07")] DateTime dateOf)
        {
            try
            {
                var result = _loginLogBusiness.GetByCompUIDDate(compId, uid, dateOf);
                return Ok(new ApiResponse<List<LoginLogModel>>
                {
                    Success = true,
                    Message = "Login logs retrieved successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<LoginLogModel>>
                {
                    Success = false,
                    Message = $"Error retrieving login logs: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get logs by CompID, Month and Year (all users)
        /// </summary>
        [HttpGet("GetByCompIdMonthYear")]
        public ActionResult<ApiResponse<List<LoginLogModel>>> GetByCompIdMonthYear(int compId, int monthOf, int yearOf)
        {
            try
            {
                var result = _loginLogBusiness.GetByCompIdMonthYear(compId, monthOf, yearOf);
                return Ok(new ApiResponse<List<LoginLogModel>>
                {
                    Success = true,
                    Message = "Login logs retrieved successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<LoginLogModel>>
                {
                    Success = false,
                    Message = $"Error retrieving login logs: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get logs by CompID and specific Date (all users)
        /// </summary>
        /// <param name="compId">Company ID</param>
        /// <param name="dateOf">Date in format: yyyy-MM-dd (e.g., 2026-02-07)</param>
        [HttpGet("GetByCompIdDate")]
        public ActionResult<ApiResponse<List<LoginLogModel>>> GetByCompIdDate(int compId, [DefaultValue("2026-02-07")] DateTime dateOf)
        {
            try
            {
                var result = _loginLogBusiness.GetByCompIdDate(compId, dateOf);
                return Ok(new ApiResponse<List<LoginLogModel>>
                {
                    Success = true,
                    Message = "Login logs retrieved successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<LoginLogModel>>
                {
                    Success = false,
                    Message = $"Error retrieving login logs: {ex.Message}"
                });
            }
        }
    }
}
