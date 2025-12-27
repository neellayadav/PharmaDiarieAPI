using System;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.Models;

namespace PharmaDiariesAPI.Controllers.worktype
{
	
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class ProductController : Controller
    {

        private readonly IConfiguration _Iconfiguration;
        private readonly IProductBusiness _IResultData;
        public ProductController(IConfiguration iconfiguration, IProductBusiness iResultdata)
        {
            _Iconfiguration = iconfiguration;
            _IResultData = iResultdata;
        }

        [HttpGet("GetProductList")]
        public IActionResult GetProductList(int compid)
        {
            var result = this._IResultData.GetProductList(compid);
            return Ok(result);
        }

        [HttpPost("Save")]
        public IActionResult Save(ProductModel prodModel)
        {
            var result = this._IResultData.Save(prodModel);
            return Ok(result);
        }

        [HttpPost("Update")]
        public IActionResult Update(ProductModel prodModel)
        {
            var result = this._IResultData.Update(prodModel);
            return Ok(result);
        }

        [HttpPost("Delete")]
        public IActionResult Delete(ProductModel prodModel) //(int compId, int prodCode, int modifiedBy)
        {
            var result = this._IResultData.Delete(prodModel);
            return Ok(result);
        }
    }
	
}

