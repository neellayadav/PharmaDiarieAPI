using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PharmaDiariesAPI.Controllers.worktype
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class CustomerController : Controller
    {
        private readonly IConfiguration _iconfiguration;
        private readonly ICustomerBusiness IResultData;
        public CustomerController(IConfiguration iconfiguration, ICustomerBusiness iResultData)
        {
            _iconfiguration = iconfiguration;
            IResultData = iResultData;

        }

        [HttpGet("CustomersListByCompType")]
        public IActionResult CustomersListByCompType(int compid, String custType)
        {
            var result = this.IResultData.CustomerListByCompType(compid, custType);
            return Ok(result);
        }

        [HttpPost("Save")]
        public IActionResult Save(CustomerModel custModel)
        {
            var result = this.IResultData.Save(custModel);
            return Ok(result);
        }

        [HttpPost("Update")]
        public IActionResult Update(CustomerModel custModel)
        {
            var result = this.IResultData.Update(custModel);
            return Ok(result);
        }

        [HttpPost("Delete")]
        public IActionResult Delete(DeleteCustomerModel delCustMod)
        {
            var result = this.IResultData.Delete(delCustMod);
            return Ok(result);
        }

        [HttpGet("GetLocation")]
        public IActionResult GetCustomerLocation([FromQuery] int compId, [FromQuery] int custId)
        {
            try
            {
                var result = this.IResultData.GetCustomerLocation(compId, custId);
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Customer not found" });
                }
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("UpdateLocation")]
        public IActionResult UpdateCustomerLocation([FromBody] CustomerLocationUpdateRequest request)
        {
            try
            {
                var result = this.IResultData.UpdateCustomerLocation(request);
                return Ok(new { success = result, message = result ? "Location updated successfully" : "Failed to update location" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

    }
}
