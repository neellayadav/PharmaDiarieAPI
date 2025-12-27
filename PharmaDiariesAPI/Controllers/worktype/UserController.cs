using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PharmaDiariesAPI.Controllers.worktype
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class UserController : Controller
    {

        private readonly IConfiguration _Iconfiguration;
        private readonly IUserBusiness _IResultData;
        public UserController(IConfiguration iconfiguration, IUserBusiness iResultdata)
        {
            _Iconfiguration = iconfiguration;
            _IResultData = iResultdata;
        }

        [HttpGet("GetUserList")]
        public IActionResult GetUserList()
        {
            var result = _IResultData.GetUserList();
            return Ok(result);
        }

        [HttpGet("GetUserListByComp")]
        public IActionResult GetUserListByComp(int compid)
        {
            var result = _IResultData.GetUserListBycomp(compid);
            return Ok(result);
        }

        [HttpPost("Save")]
        public IActionResult Save(UserModel uModel)
        {
            var result = _IResultData.Save(uModel);
            return Ok(result);
        }

        [HttpPost("Update")]
        public IActionResult Update(UserModel uModel)
        {
            var result = _IResultData.Update(uModel);
            return Ok(result);
        }

        [HttpPost("Delete")]
        public IActionResult Delete(UserModel uModel)
        {
            var result = _IResultData.Delete(uModel);
            return Ok(result);
        }

        //[HttpPost("SaveBasic")]
        //public IActionResult SaveBasic(UserModel uModel)
        //{
        //    var result = _IResultData.SaveBasic(uModel);
        //    return Ok(result);
        //}

        [HttpPost("ResetPassword")]
        public IActionResult ResetPassword(ResetPasswordModel luModel)
        {
            var result = _IResultData.ResetPassword(luModel);
            return Ok(result);
        }

    }
}
