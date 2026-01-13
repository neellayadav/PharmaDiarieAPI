using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.Models;
using PharmaDiaries.Utils;

namespace PharmaDiariesAPI.Controllers.worktype
{

    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class ReportsController : Controller
    {
        private readonly IConfiguration _Iconfiguration;
        private readonly IReportBusiness _IResultData;
        //private readonly IWebHostEnvironment _env;

        //public ReportsController(IConfiguration iconfiguration, IReportBusiness iResultdata, IWebHostEnvironment env)
        public ReportsController(IConfiguration iconfiguration, IReportBusiness iResultdata)
        {
            _Iconfiguration = iconfiguration;
            _IResultData = iResultdata;
            //_env = env;

            //var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "reports");
            //_reportService = new MonthlyReportService(folder);
        }

        [HttpPost("MonthlyReport")]
        public Task<IActionResult> GetMonthlyReport(MonthlyReportRequest monRepReq)
        {
            //Console.WriteLine($"Base Directory: {_env.WebRootPath}");

            var downloadUrl = this._IResultData.GetMonthlyReport(monRepReq);
            //var downloadUrl = $"{_baseUrl}{fileName}";

            return Task.FromResult<IActionResult>(Ok(new
            {
                message = "Report generated successfully",
                downloadLink = downloadUrl
            }));
        }

        /// <summary>
        /// Generate Employee Monthly Report
        /// </summary>
        /// <param name="request">Employee monthly report request parameters</param>
        /// <returns>URL of the generated Excel report</returns>
        [HttpPost("FWEmpMonthly")]
        public Task<IActionResult> FWEmpMonthly(EmpMonthlyReportRequest request)
        {
            var downloadUrl = this._IResultData.GetEmpMonthlyReport(request);

            return Task.FromResult<IActionResult>(Ok(new
            {
                message = "Employee monthly report generated successfully",
                downloadLink = downloadUrl
            }));
        }

        /// <summary>
        /// Generate Company Monthly Report (All Employees)
        /// </summary>
        /// <param name="request">Company monthly report request parameters</param>
        /// <returns>URL of the generated Excel report</returns>
        [HttpPost("FWMonthly")]
        public Task<IActionResult> FWMonthly(MonthlyReportRequest request)
        {
            var downloadUrl = this._IResultData.GetFWMonthlyReport(request);

            return Task.FromResult<IActionResult>(Ok(new
            {
                message = "Company monthly report generated successfully",
                downloadLink = downloadUrl
            }));
        }

        /// <summary>
        /// Generate Employee Yearly Report with monthly worksheets
        /// </summary>
        /// <param name="request">Employee yearly report request parameters</param>
        /// <returns>URL of the generated Excel report</returns>
        [HttpPost("FWEmpYearly")]
        public Task<IActionResult> FWEmpYearly(EmpYearlyReportRequest request)
        {
            var downloadUrl = this._IResultData.GetEmpYearlyReport(request);

            return Task.FromResult<IActionResult>(Ok(new
            {
                message = "Employee yearly report generated successfully",
                downloadLink = downloadUrl
            }));
        }

        /// <summary>
        /// Generate Company Yearly Report (All Employees) with monthly worksheets
        /// </summary>
        /// <param name="request">Company yearly report request parameters</param>
        /// <returns>URL of the generated Excel report</returns>
        [HttpPost("FWYearly")]
        public Task<IActionResult> FWYearly(YearlyReportRequest request)
        {
            var downloadUrl = this._IResultData.GetFWYearlyReport(request);

            return Task.FromResult<IActionResult>(Ok(new
            {
                message = "Company yearly report generated successfully",
                downloadLink = downloadUrl
            }));
        }

        /// <summary>
        /// Generate Financial Year Report with monthly worksheets
        /// </summary>
        /// <param name="request">Financial year report request parameters</param>
        /// <returns>URL of the generated Excel report</returns>
        [HttpPost("FinancialYearly")]
        public Task<IActionResult> FinancialYearly(FinancialYearReportRequest request)
        {
            var downloadUrl = this._IResultData.GetFinancialYearReport(request);

            return Task.FromResult<IActionResult>(Ok(new
            {
                message = "Financial year report generated successfully",
                downloadLink = downloadUrl
            }));
        }

        /// <summary>
        /// Get Field Work Customer Call Summary
        /// </summary>
        /// <param name="request">Customer call summary request parameters</param>
        /// <returns>List of customer call summary records</returns>
        [HttpPost("FWCustomerCallSummary")]
        public Task<IActionResult> FWCustomerCallSummary(FWCustomerCallSummaryRequestModel request)
        {
            var data = this._IResultData.GetCustomerCallSummary(request);

            return Task.FromResult<IActionResult>(Ok(new
            {
                message = "Customer call summary retrieved successfully",
                data = data
            }));
        }

        /// <summary>
        /// Download a generated report file
        /// </summary>
        /// <param name="fileName">The name of the file to download (e.g., "2000NOVEMBER2024.xlsx")</param>
        /// <param name="compId">Company ID</param>
        /// <returns>The Excel file as a downloadable attachment</returns>
        [HttpGet("Download/{compId}/{fileName}")]
        public IActionResult DownloadReport(int compId, string fileName)
        {
            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var filePath = Path.Combine(baseDirectory, $"Reports/{compId}/reports/", fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new
                    {
                        message = "File not found",
                        filePath = filePath
                    });
                }

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error downloading file",
                    error = ex.Message
                });
            }
        }
    }
}

//namespace YourNamespace.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class ReportsController : ControllerBase
//    {
//        private readonly MonthlyReportService _reportService;
//        private readonly string _baseUrl = "https://yourdomain.com/reports/"; // Change to your domain

//        public ReportsController()
//        {
//            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "reports");
//            _reportService = new MonthlyReportService(folder);
//        }

//        [HttpGet("generate-monthly-report")]
//        public async Task<IActionResult> GenerateReport()
//        {
//            var fileName = await _reportService.GenerateMonthlyReportAsync();
//            var downloadUrl = $"{_baseUrl}{fileName}";

//            return Ok(new
//            {
//                message = "Report generated successfully",
//                downloadLink = downloadUrl
//            });
//        }
//    }
//}

