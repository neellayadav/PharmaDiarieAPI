using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PharmaDiariesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class FieldworkHeader : ControllerBase
    {
        private readonly IConfiguration _iconfiguration;
        private readonly IFWHDBusiness IResultData;
        public FieldworkHeader(IConfiguration iconfiguration, IFWHDBusiness iResultData)
        {
            _iconfiguration = iconfiguration;
            IResultData = iResultData;

        }

        [HttpGet("FWHeaderList")]
        public IActionResult FWHeaderList()
        {
            var result = this.IResultData.FWHeaderList();
            return Ok(result);
        }

        [HttpPost("Save")]
        public IActionResult Save(FieldWorkHeader fwheader)
        {
            var result = this.IResultData.Save(fwheader);
            return Ok(result);
        }

        [HttpPost("Delete")]
        public IActionResult Delete(int compid, String transid, String userid)
        {
            var result = this.IResultData.Delete( compid,  transid,  userid);
            return Ok(result);
        }

        [HttpPost("OtherWorkSave")]
        public IActionResult OtherworkSave(FieldworkOthers fwOthers)
        {
            var result = this.IResultData.OtherworkSave(fwOthers);
            return Ok(result);
        }

        [HttpPost("GetEmpDateWiseFW")]
        public IActionResult GetEmpDateWiseFW(int compId,int uid, String empWorkDate, String periodOf)
        {
            var result = this.IResultData.GetEmpDateWiseFW(compId, uid, empWorkDate, periodOf );
            return Ok(result);
        }

        [HttpGet("GetFieldWorkSummary")]
        public IActionResult GetFieldWorkSummary(int compId, int uid)
        {
            var result = this.IResultData.GetFieldworkSummary(compId, uid);
            return Ok(result);
        }

        [HttpGet("GetFWMonthlyReport")]
        public IActionResult GetFWMonthlyReport(int compId, int monthOf, int yearOf)
        {
            var result = this.IResultData.GetFWMonthlyReport(compId, monthOf, yearOf);
            return Ok(result);
        }

    }
}