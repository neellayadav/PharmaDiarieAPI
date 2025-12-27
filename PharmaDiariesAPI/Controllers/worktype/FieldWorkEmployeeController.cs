using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.Bussiness;

namespace PharmaDiariesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class FieldWorkEmployee : ControllerBase
    {
        private readonly IConfiguration _iconfiguration;
        private readonly IFWEmpDTBusiness IResultData;
        public FieldWorkEmployee(IConfiguration iconfiguration, IFWEmpDTBusiness iResultData)
        {
            _iconfiguration = iconfiguration;
             IResultData= iResultData;

        }

        [HttpGet("GetAllFWEmployees")]
        public IActionResult GetAllFieldWorkEmployees(string transId)
        {
            var result = this.IResultData.GetAllFieldworkEmp(transId);
            return Ok(result);
        }
    }
}

