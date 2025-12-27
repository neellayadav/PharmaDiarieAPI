using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PharmaDiaries.BusinessContract;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PharmaDiariesAPI.Controllers.worktype
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class LookupController : Controller
    {
        private readonly IConfiguration _iconfiguration;
        private readonly ILookupBusiness IResultData;
        public LookupController(IConfiguration iconfiguration, ILookupBusiness iResultData)
        {
            _iconfiguration = iconfiguration;
            IResultData = iResultData;

        }

        [HttpGet("LookupsList")]
        public IActionResult GetLookups()
        {
            var result = this.IResultData.Lookuplist();
            return Ok(result);
        }

        [HttpGet("LookupsByType")]
        public IActionResult GetLookupsByType(String type)
        {
            var result = this.IResultData.LookupListByType(type);
            return Ok(result);
        }



    }
}

