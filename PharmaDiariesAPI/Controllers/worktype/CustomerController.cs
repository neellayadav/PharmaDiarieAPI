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

    }
}
