
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
    public class LoginController : Controller
    {
        private readonly IConfiguration _iconfiguration;
        private readonly ILoginBusiness IResultData;
        public LoginController(IConfiguration iconfiguration, ILoginBusiness iResultData)
        {
            _iconfiguration = iconfiguration;
            IResultData = iResultData;

        }

        [HttpPost("Validate")]
        public IActionResult Validate(LoginUserModel lumodel)
        {
            var result = this.IResultData.Validate(lumodel);
            return Ok(result);
        }

        [HttpPost("SignUp")]
        public IActionResult SignUp(SignUpModel uModel)
        {
            var result = this.IResultData.SignUp(uModel);
            return Ok(result);
        }

    }
}

