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
    public class AreasController : Controller
    {

        private readonly IConfiguration _Iconfiguration;
        private readonly IAreasBusiness _IResultData;
        public AreasController(IConfiguration iconfiguration, IAreasBusiness iResultdata)
        {
            _Iconfiguration = iconfiguration;
            _IResultData = iResultdata;
        }

        [HttpGet("GetRegionList")]
        public IActionResult RegionList(int compid)
        {
            var result = this._IResultData.RegionList(compid);
            return Ok(result);
        }

        [HttpGet("GetHeadQuaterList")]
        public IActionResult HeadQuaterList(int compid)
        {
            var result = this._IResultData.HeadQuaterList(compid);
            return Ok(result);
        }

        [HttpGet("GetHeadQuaterListByRegion")]
        public IActionResult HeadQuaterListByRegion(int compid, String region)
        {
            var result = this._IResultData.HeadQuarterListByRegion(compid, region);
            return Ok(result);
        }

        [HttpGet("GetPatchListByHeadQuater")]
        public IActionResult PatchListByHeadQuater(int compid, String hQuater)
        {
            var result = this._IResultData.PatchListByHeadQuater(compid, hQuater);
            return Ok(result);
        }

        [HttpGet("GetAreaList")]
        public IActionResult AreaList(int compid)
        {
            var result = this._IResultData.AreaList(compid);
            return Ok(result);
        }

        [HttpPost("Save")]
        public IActionResult Save([FromBody] AreasModel areaModel)
        {
            var result = this._IResultData.Save(areaModel);
            return Ok(result);
        }

        [HttpPost("Update")]
        public IActionResult Update([FromBody] AreasModel areaModel)
        {
            var result = this._IResultData.Update(areaModel);
            return Ok(result);
        }

        [HttpPost("Delete")]
        public IActionResult Delete([FromBody] AreasModel areaModel)
        {
            var result = this._IResultData.Delete(areaModel.AreaID, areaModel.CompID.GetValueOrDefault(), areaModel.ModifiedBy.GetValueOrDefault());
            return Ok(result);
        }

    }
}

