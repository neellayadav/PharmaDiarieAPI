using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class ScreenController : Controller
    {
        private readonly IConfiguration _Iconfiguration;
        private readonly IScreenBusiness _IResultData;

        public ScreenController(IConfiguration iconfiguration, IScreenBusiness iResultdata)
        {
            _Iconfiguration = iconfiguration;
            _IResultData = iResultdata;
        }

        [HttpPost("sync")]
        public IActionResult SyncScreens([FromBody] List<ScreenModel> screens)
        {
            var result = _IResultData.SyncScreens(screens);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetActiveScreens()
        {
            var result = _IResultData.GetActiveScreens();
            return Ok(result);
        }

        [HttpPost("users/{userId}/permissions")]
        public IActionResult SaveUserPermissions(int userId, [FromBody] UserScreenPermissionRequest request)
        {
            request.UserID = userId;
            var result = _IResultData.SaveUserScreenPermissions(request);
            return Ok(result);
        }

        [HttpGet("users/{userId}/permissions")]
        public IActionResult GetUserPermissions(int userId)
        {
            var result = _IResultData.GetUserScreenPermissions(userId);
            return Ok(result);
        }
    }
}
