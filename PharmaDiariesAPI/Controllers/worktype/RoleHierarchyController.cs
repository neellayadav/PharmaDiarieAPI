using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.Models;

namespace PharmaDiariesAPI.Controllers.worktype
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class RoleHierarchyController : Controller
    {
        private readonly IConfiguration _iconfiguration;
        private readonly IRoleHierarchyBusiness _roleHierarchyBusiness;

        public RoleHierarchyController(IConfiguration iconfiguration, IRoleHierarchyBusiness roleHierarchyBusiness)
        {
            _iconfiguration = iconfiguration;
            _roleHierarchyBusiness = roleHierarchyBusiness;
        }

        /// <summary>
        /// Gets all distinct roles from Lookup table with their current ranking (if any)
        /// </summary>
        [HttpGet("DistinctRoles")]
        public IActionResult GetDistinctRoles()
        {
            try
            {
                var result = _roleHierarchyBusiness.GetDistinctRoles();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Gets all ranked roles in hierarchy order
        /// </summary>
        [HttpGet("List")]
        public IActionResult GetRoleHierarchyList()
        {
            try
            {
                var result = _roleHierarchyBusiness.GetRoleHierarchyList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Saves/updates a single role hierarchy ranking
        /// </summary>
        [HttpPost("Save")]
        public IActionResult SaveRoleHierarchy([FromBody] RoleHierarchySaveRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.RoleName))
                {
                    return BadRequest(new { error = "RoleName is required" });
                }

                var result = _roleHierarchyBusiness.SaveRoleHierarchy(request);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Saves/updates multiple role hierarchy rankings in batch
        /// </summary>
        [HttpPost("SaveBatch")]
        public IActionResult SaveRoleHierarchyBatch([FromBody] RoleHierarchyBatchSaveRequest request)
        {
            try
            {
                if (request.RoleRankings == null || request.RoleRankings.Count == 0)
                {
                    return BadRequest(new { error = "RoleRankings list is required" });
                }

                var result = _roleHierarchyBusiness.SaveRoleHierarchyBatch(request);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
