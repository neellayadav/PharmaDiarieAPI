using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PharmaDiaries.BusinessContract;

namespace PharmaDiariesAPI.Controllers.worktype
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class AppController : Controller
    {
        private readonly IConfiguration _iconfiguration;
        private readonly IAppBusiness IResultData;

        public AppController(IConfiguration iconfiguration, IAppBusiness iResultData)
        {
            _iconfiguration = iconfiguration;
            IResultData = iResultData;
        }

        [HttpGet("VersionCheck")]
        public IActionResult VersionCheck(string platform, string currentVersion)
        {
            var result = this.IResultData.CheckVersion(platform, currentVersion);
            if (result == null)
                return Ok(new { needsUpdate = false });

            return Ok(result);
        }
    }
}
