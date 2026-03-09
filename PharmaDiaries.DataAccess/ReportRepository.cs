using System;
using Logicon.Kaidu.Platform.Helpers;
using System.Data;
using Microsoft.Extensions.Configuration;
using PharmaDiaries.DataAccessContract;
using PharmaDiaries.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using PharmaDiaries.DataAccessContract.Repository;
using DocumentFormat.OpenXml.EMMA;
using PharmaDiaries.Utils;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc;
//using System.IO;

namespace PharmaDiaries.DataAccess
{
	public class ReportRepository : IReportRepository
	{
		private IConfiguration _configuration;
        private string _PharmaDiaries_ConnectionString;
        private string _baseUrl;

        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        public ReportRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._PharmaDiaries_ConnectionString = configuration["connectionStrings:APIconnectionString"]!.ToString();
            this._baseUrl = configuration["ReportSettings:BaseUrl"]!.ToString();
        }

        // Helper method to convert file path to API download URL
        private string ConvertToDownloadUrl(string filePath)
        {
            // Extract company ID and filename from the path
            // From: h:\root\home\ragsarma-001\www\PharmadiarieApiUAT\Reports/2000/reports/2000NOVEMBER2024.xlsx
            // To: http://pharmadiarieapiuat-001-site1.ltempurl.com/api/Reports/Download/2000/2000NOVEMBER2024.xlsx

            var fileName = Path.GetFileName(filePath);

            // Extract compID from the path (Reports/{compID}/reports/)
            var relativePath = filePath.Replace(baseDirectory, "").Replace("\\", "/");
            var pathParts = relativePath.Split('/');

            // Find the compID (should be after "Reports" folder)
            string compID = "";
            for (int i = 0; i < pathParts.Length; i++)
            {
                if (pathParts[i].Equals("Reports", StringComparison.OrdinalIgnoreCase) && i + 1 < pathParts.Length)
                {
                    compID = pathParts[i + 1];
                    break;
                }
            }

            return $"{_baseUrl}/api/Reports/Download/{compID}/{fileName}";
        }

        string IReportRepository.GetMonthlyReport(MonthlyReportRequest monRepReq)
        {
            try
            { 
                var dSet = new List<Report>();
                DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[mcDCR].[usp_FWHDMonthlyList]", monRepReq.CompID, monRepReq.Month, monRepReq.Yr);
                dSet = DataTableHelper.ConvertDataTable<Report>(ds.Tables[0]);
                //var exactPath = Path.Combine($"{monRepReq.baseDom}", $"Reports/{monRepReq.Yr}/");

                //Console.WriteLine($"Base Directory: {baseDirectory}");

                var exactPath = Path.Combine(baseDirectory, $"Reports/{monRepReq.Yr}/");
                //Console.WriteLine($"Exact Directory: {exactPath}");

                Reports _reports = new Reports(exactPath);
                var filePath = _reports.GenerateMonthlyReportAsync(dSet, monRepReq).Result;
                return ConvertToDownloadUrl(filePath);

            }
            catch (Exception ex)
            {
                //transaction.Commit();
                throw ex;
            }
        }

        string IReportRepository.GetEmpMonthlyReport(EmpMonthlyReportRequest request)
        {
            try
            {
                var dSet = new List<Report>();
                DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[mcDCR].[usp_FWHDEmpMonthlyList]",
                    request.CompID, request.UID, request.Month, request.Year);
                dSet = DataTableHelper.ConvertDataTable<Report>(ds.Tables[0]);

                var exactPath = Path.Combine(baseDirectory, $"Reports/{request.CompID}/reports/");

                Reports _reports = new Reports(exactPath);
                var filePath = _reports.GenerateEmpMonthlyReportAsync(dSet, request).Result;
                return ConvertToDownloadUrl(filePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        string IReportRepository.GetFWMonthlyReport(MonthlyReportRequest request)
        {
            try
            {
                var dSet = new List<Report>();
                DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[mcDCR].[usp_FWHDMonthlyList]",
                    request.CompID, request.Month, request.Yr);
                dSet = DataTableHelper.ConvertDataTable<Report>(ds.Tables[0]);

                var exactPath = Path.Combine(baseDirectory, $"Reports/{request.CompID}/reports/");

                Reports _reports = new Reports(exactPath);
                var filePath = _reports.GenerateFWMonthlyReportAsync(dSet, request).Result;
                return ConvertToDownloadUrl(filePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        string IReportRepository.GetEmpYearlyReport(EmpYearlyReportRequest request)
        {
            try
            {
                // Fetch data for all 12 months
                var monthlyData = new Dictionary<int, List<Report>>();

                for (int month = 1; month <= 12; month++)
                {
                    DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[mcDCR].[usp_FWHDEmpYearlyList]",
                        request.CompID, request.UID, request.Year);
                    var dataForMonth = DataTableHelper.ConvertDataTable<Report>(ds.Tables[0]);

                    // Filter data for the current month
                    var filteredData = dataForMonth.Where(r => r.CreatedOn.Month == month).ToList();
                    monthlyData[month] = filteredData;
                }

                var exactPath = Path.Combine(baseDirectory, $"Reports/{request.CompID}/reports/");

                Reports _reports = new Reports(exactPath);
                var filePath = _reports.GenerateEmpYearlyReportAsync(monthlyData, request).Result;
                return ConvertToDownloadUrl(filePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        string IReportRepository.GetFWYearlyReport(YearlyReportRequest request)
        {
            try
            {
                // Fetch all data for the year
                DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[mcDCR].[usp_FWHDYearlyList]",
                    request.CompID, request.Year);
                var yearData = DataTableHelper.ConvertDataTable<Report>(ds.Tables[0]);

                // Group data by month
                var monthlyData = new Dictionary<int, List<Report>>();
                for (int month = 1; month <= 12; month++)
                {
                    var filteredData = yearData.Where(r => r.CreatedOn.Month == month).ToList();
                    monthlyData[month] = filteredData;
                }

                var exactPath = Path.Combine(baseDirectory, $"Reports/{request.CompID}/reports/");

                Reports _reports = new Reports(exactPath);
                var filePath = _reports.GenerateFWYearlyReportAsync(monthlyData, request).Result;
                return ConvertToDownloadUrl(filePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        string IReportRepository.GetFinancialYearReport(FinancialYearReportRequest request)
        {
            try
            {
                // Fetch all data for the financial year range
                DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[mcDCR].[usp_FWHDFinancialYearList]",
                    request.CompID, request.FromMonth, request.FromYear, request.ToMonth, request.ToYear);
                var allData = DataTableHelper.ConvertDataTable<Report>(ds.Tables[0]);

                // Group data by year-month
                var monthlyData = new Dictionary<string, List<Report>>();

                int currentYear = request.FromYear;
                int currentMonth = request.FromMonth;

                while (currentYear < request.ToYear || (currentYear == request.ToYear && currentMonth <= request.ToMonth))
                {
                    string dataKey = $"{currentYear}-{currentMonth}";
                    var filteredData = allData.Where(r => r.CreatedOn.Year == currentYear && r.CreatedOn.Month == currentMonth).ToList();
                    monthlyData[dataKey] = filteredData;

                    // Move to next month
                    currentMonth++;
                    if (currentMonth > 12)
                    {
                        currentMonth = 1;
                        currentYear++;
                    }
                }

                var exactPath = Path.Combine(baseDirectory, $"Reports/{request.CompID}/reports/");

                Reports _reports = new Reports(exactPath);
                var filePath = _reports.GenerateFinancialYearReportAsync(monthlyData, request).Result;
                return ConvertToDownloadUrl(filePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        List<FWCustomerCallSummaryModel> IReportRepository.GetCustomerCallSummary(FWCustomerCallSummaryRequestModel request)
        {
            try
            {
                DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[mcDCR].[usp_FWCustomersCallSummary]",
                    request.CompId, request.CustID, request.UID, request.DateFrom, request.DateTo);
                var data = DataTableHelper.ConvertDataTable<FWCustomerCallSummaryModel>(ds.Tables[0]);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        PaginatedReportResponse IReportRepository.GetEmpMonthlyData(EmpMonthlyDataRequest request)
        {
            try
            {
                DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[mcDCR].[usp_FWHDEmpMonthlyDataList]",
                    request.CompID, request.UID, request.Month, request.Year, request.Page, request.PageSize);

                int totalCount = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalCount"]);
                var data = DataTableHelper.ConvertDataTable<ReportDataItem>(ds.Tables[1]);

                return new PaginatedReportResponse
                {
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        PaginatedReportResponse IReportRepository.GetMonthlyData(MonthlyDataRequest request)
        {
            try
            {
                DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[mcDCR].[usp_FWHDMonthlyDataList]",
                    request.CompID, request.Month, request.Year, request.Page, request.PageSize);

                int totalCount = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalCount"]);
                var data = DataTableHelper.ConvertDataTable<ReportDataItem>(ds.Tables[1]);

                return new PaginatedReportResponse
                {
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        PaginatedReportResponse IReportRepository.GetEmpYearlyData(EmpYearlyDataRequest request)
        {
            try
            {
                DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[mcDCR].[usp_FWHDEmpYearlyDataList]",
                    request.CompID, request.UID, request.Year, request.Page, request.PageSize);

                int totalCount = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalCount"]);
                var data = DataTableHelper.ConvertDataTable<ReportDataItem>(ds.Tables[1]);

                return new PaginatedReportResponse
                {
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        PaginatedReportResponse IReportRepository.GetYearlyData(YearlyDataRequest request)
        {
            try
            {
                DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[mcDCR].[usp_FWHDYearlyDataList]",
                    request.CompID, request.Year, request.Page, request.PageSize);

                int totalCount = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalCount"]);
                var data = DataTableHelper.ConvertDataTable<ReportDataItem>(ds.Tables[1]);

                return new PaginatedReportResponse
                {
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        PaginatedReportResponse IReportRepository.GetFinancialYearData(FinancialYearDataRequest request)
        {
            try
            {
                DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[mcDCR].[usp_FWHDFinancialYearDataList]",
                    request.CompID, request.FromMonth, request.FromYear, request.ToMonth, request.ToYear, request.Page, request.PageSize);

                int totalCount = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalCount"]);
                var data = DataTableHelper.ConvertDataTable<ReportDataItem>(ds.Tables[1]);

                return new PaginatedReportResponse
                {
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

