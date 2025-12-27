using ClosedXML.Excel;
using PharmaDiaries.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace PharmaDiaries.Utils
{

    public class Reports
    {
        private readonly string _reportFolder;

        public Reports(string reportFolder)
        {
            _reportFolder = reportFolder;
            if (!Directory.Exists(_reportFolder))
                Directory.CreateDirectory(_reportFolder);
        }

        public async Task<string> GenerateMonthlyReportAsync(List<Report> data, MonthlyReportRequest monRepReq)
        {
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monRepReq.Month).ToUpper();
            string year = monRepReq.Yr.ToString();
            string fileName = $"{monthName}_{year}.xlsx";
            var filePath = Path.Combine(_reportFolder, fileName);

            await Task.Run(() =>
            {
                using (var workbook = new XLWorkbook())
                {
                    var sheet = workbook.Worksheets.Add("Monthly Report");

                    // Add headers
                    sheet.Cell(1, 1).Value = "Company Code";
                    sheet.Cell(1, 2).Value = "Transaction ID";
                    sheet.Cell(1, 3).Value = "UserName";
                    sheet.Cell(1, 4).Value = "Created On";
                    sheet.Cell(1, 5).Value = "Head Quater";
                    sheet.Cell(1, 6).Value = "Patch Name";
                    sheet.Cell(1, 7).Value = "Visited";
                    sheet.Cell(1, 8).Value = "Customer Name";
                    sheet.Cell(1, 9).Value = "Emp. Seq. No.";
                    sheet.Cell(1, 10).Value = "Colleague Name";
                    sheet.Cell(1, 11).Value = "Prod. Seq. No.";
                    sheet.Cell(1, 12).Value = "ProductDesc";
                    sheet.Cell(1, 13).Value = "Remarks";

                    // Style header row
                    var headerRow = sheet.Range(1, 1, 1, 13);
                    headerRow.Style.Font.Bold = true;
                    headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

                    // Add data rows
                    for (int i = 0; i < data.Count; i++)
                    {
                        int j = 1;
                        sheet.Cell(i + 2, j++).Value = data[i].CompID;
                        sheet.Cell(i + 2, j++).Value = (i > 0 && data[i].TransID == data[i - 1].TransID) ? "" : data[i].TransID;
                        sheet.Cell(i + 2, j++).Value = (i > 0 && data[i].TransID == data[i - 1].TransID) ? "" : data[i].UserName;
                        sheet.Cell(i + 2, j++).Value = (i > 0 && data[i].TransID == data[i - 1].TransID) ? "" : data[i].CreatedOn.ToString("dd-MM-yyyy hh:mm");
                        sheet.Cell(i + 2, j++).Value = (i > 0 && data[i].TransID == data[i - 1].TransID) ? "" : data[i].HQcode;
                        sheet.Cell(i + 2, j++).Value = (i > 0 && data[i].TransID == data[i - 1].TransID) ? "" : data[i].PatchName;
                        sheet.Cell(i + 2, j++).Value = (i > 0 && data[i].TransID == data[i - 1].TransID) ? "" : data[i].Visited;
                        sheet.Cell(i + 2, j++).Value = (i > 0 && data[i].TransID == data[i - 1].TransID) ? "" : data[i].CustName;
                        if (i > 0 && data[i].TransID == data[i - 1].TransID && data[i].EmpSeqNo == data[i - 1].EmpSeqNo)
                        {
                            sheet.Cell(i + 2, j++).Value = "";
                        }
                        else
                        {
                            sheet.Cell(i + 2, j++).Value = data[i].EmpSeqNo;
                        }
                        sheet.Cell(i + 2, j++).Value = (i > 0 && data[i].TransID == data[i - 1].TransID && data[i].EmpSeqNo == data[i - 1].EmpSeqNo) ? "" : data[i].ColleagueName;
                        sheet.Cell(i + 2, j++).Value = data[i].ProdSeqNo;
                        sheet.Cell(i + 2, j++).Value = data[i].ProductDesc;
                        sheet.Cell(i + 2, j++).Value = data[i].Remarks;
                    }

                    // Auto-fit columns
                    sheet.Columns().AdjustToContents();

                    workbook.SaveAs(filePath);
                }
            });

            return filePath;
        }

        // Generate Employee Monthly Report
        public async Task<string> GenerateEmpMonthlyReportAsync(List<Report> data, EmpMonthlyReportRequest request)
        {
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(request.Month).ToUpper();
            string fileName = $"{request.CompID}{request.UID}{monthName}{request.Year}.xlsx";
            var filePath = Path.Combine(_reportFolder, fileName);

            await Task.Run(() =>
            {
                using (var workbook = new XLWorkbook())
                {
                    var sheet = workbook.Worksheets.Add($"{monthName} Report");
                    AddReportHeaders(sheet);
                    PopulateReportData(sheet, data);
                    sheet.Columns().AdjustToContents();
                    workbook.SaveAs(filePath);
                }
            });

            return filePath;
        }

        // Generate Company Monthly Report (same as existing but with updated naming)
        public async Task<string> GenerateFWMonthlyReportAsync(List<Report> data, MonthlyReportRequest request)
        {
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(request.Month).ToUpper();
            string fileName = $"{request.CompID}{monthName}{request.Yr}.xlsx";
            var filePath = Path.Combine(_reportFolder, fileName);

            await Task.Run(() =>
            {
                using (var workbook = new XLWorkbook())
                {
                    var sheet = workbook.Worksheets.Add($"{monthName} Report");
                    AddReportHeaders(sheet);
                    PopulateReportData(sheet, data);
                    sheet.Columns().AdjustToContents();
                    workbook.SaveAs(filePath);
                }
            });

            return filePath;
        }

        // Generate Employee Yearly Report with separate worksheets for each month
        public async Task<string> GenerateEmpYearlyReportAsync(Dictionary<int, List<Report>> monthlyData, EmpYearlyReportRequest request)
        {
            string fileName = $"{request.CompID}{request.UID}{request.Year}.xlsx";
            var filePath = Path.Combine(_reportFolder, fileName);

            await Task.Run(() =>
            {
                using (var workbook = new XLWorkbook())
                {
                    // Create a worksheet for each month
                    for (int month = 1; month <= 12; month++)
                    {
                        string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                        var sheet = workbook.Worksheets.Add(monthName);
                        AddReportHeaders(sheet);

                        if (monthlyData.ContainsKey(month) && monthlyData[month].Count > 0)
                        {
                            PopulateReportData(sheet, monthlyData[month]);
                        }

                        sheet.Columns().AdjustToContents();
                    }

                    workbook.SaveAs(filePath);
                }
            });

            return filePath;
        }

        // Generate Company Yearly Report with separate worksheets for each month
        public async Task<string> GenerateFWYearlyReportAsync(Dictionary<int, List<Report>> monthlyData, YearlyReportRequest request)
        {
            string fileName = $"{request.CompID}{request.Year}.xlsx";
            var filePath = Path.Combine(_reportFolder, fileName);

            await Task.Run(() =>
            {
                using (var workbook = new XLWorkbook())
                {
                    // Create a worksheet for each month
                    for (int month = 1; month <= 12; month++)
                    {
                        string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                        var sheet = workbook.Worksheets.Add(monthName);
                        AddReportHeaders(sheet);

                        if (monthlyData.ContainsKey(month) && monthlyData[month].Count > 0)
                        {
                            PopulateReportData(sheet, monthlyData[month]);
                        }

                        sheet.Columns().AdjustToContents();
                    }

                    workbook.SaveAs(filePath);
                }
            });

            return filePath;
        }

        // Generate Financial Year Report with separate worksheets for each month in the range
        public async Task<string> GenerateFinancialYearReportAsync(Dictionary<string, List<Report>> monthlyData, FinancialYearReportRequest request)
        {
            string fromMonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(request.FromMonth);
            string toMonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(request.ToMonth);
            string fileName = $"{fromMonthName}{request.FromYear}_{toMonthName}{request.ToYear}.xlsx";
            var filePath = Path.Combine(_reportFolder, fileName);

            await Task.Run(() =>
            {
                using (var workbook = new XLWorkbook())
                {
                    // Loop through all months in the financial year range
                    int currentYear = request.FromYear;
                    int currentMonth = request.FromMonth;

                    while (currentYear < request.ToYear || (currentYear == request.ToYear && currentMonth <= request.ToMonth))
                    {
                        string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(currentMonth);
                        string sheetName = $"{monthName} {currentYear}";
                        string dataKey = $"{currentYear}-{currentMonth}";

                        var sheet = workbook.Worksheets.Add(sheetName);
                        AddReportHeaders(sheet);

                        if (monthlyData.ContainsKey(dataKey) && monthlyData[dataKey].Count > 0)
                        {
                            PopulateReportData(sheet, monthlyData[dataKey]);
                        }

                        sheet.Columns().AdjustToContents();

                        // Move to next month
                        currentMonth++;
                        if (currentMonth > 12)
                        {
                            currentMonth = 1;
                            currentYear++;
                        }
                    }

                    workbook.SaveAs(filePath);
                }
            });

            return filePath;
        }

        // Helper method to add standard report headers
        private void AddReportHeaders(IXLWorksheet sheet)
        {
            sheet.Cell(1, 1).Value = "Company Code";
            sheet.Cell(1, 2).Value = "Transaction ID";
            sheet.Cell(1, 3).Value = "UserName";
            sheet.Cell(1, 4).Value = "Created On";
            sheet.Cell(1, 5).Value = "Head Quater";
            sheet.Cell(1, 6).Value = "Patch Name";
            sheet.Cell(1, 7).Value = "Visited";
            sheet.Cell(1, 8).Value = "Customer Name";
            sheet.Cell(1, 9).Value = "Emp. Seq. No.";
            sheet.Cell(1, 10).Value = "Colleague Name";
            sheet.Cell(1, 11).Value = "Prod. Seq. No.";
            sheet.Cell(1, 12).Value = "ProductDesc";
            sheet.Cell(1, 13).Value = "Remarks";

            // Style header row
            var headerRow = sheet.Range(1, 1, 1, 13);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        // Helper method to populate report data
        private void PopulateReportData(IXLWorksheet sheet, List<Report> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                int j = 1;
                sheet.Cell(i + 2, j++).Value = data[i].CompID;
                sheet.Cell(i + 2, j++).Value = (i > 0 && data[i].TransID == data[i - 1].TransID) ? "" : data[i].TransID;
                sheet.Cell(i + 2, j++).Value = (i > 0 && data[i].TransID == data[i - 1].TransID) ? "" : data[i].UserName;
                sheet.Cell(i + 2, j++).Value = (i > 0 && data[i].TransID == data[i - 1].TransID) ? "" : data[i].CreatedOn.ToString("dd-MM-yyyy hh:mm");
                sheet.Cell(i + 2, j++).Value = (i > 0 && data[i].TransID == data[i - 1].TransID) ? "" : data[i].HQcode;
                sheet.Cell(i + 2, j++).Value = (i > 0 && data[i].TransID == data[i - 1].TransID) ? "" : data[i].PatchName;
                sheet.Cell(i + 2, j++).Value = (i > 0 && data[i].TransID == data[i - 1].TransID) ? "" : data[i].Visited;
                sheet.Cell(i + 2, j++).Value = (i > 0 && data[i].TransID == data[i - 1].TransID) ? "" : data[i].CustName;
                if (i > 0 && data[i].TransID == data[i - 1].TransID && data[i].EmpSeqNo == data[i - 1].EmpSeqNo)
                {
                    sheet.Cell(i + 2, j++).Value = "";
                }
                else
                {
                    sheet.Cell(i + 2, j++).Value = data[i].EmpSeqNo;
                }
                sheet.Cell(i + 2, j++).Value = (i > 0 && data[i].TransID == data[i - 1].TransID && data[i].EmpSeqNo == data[i - 1].EmpSeqNo) ? "" : data[i].ColleagueName;
                sheet.Cell(i + 2, j++).Value = data[i].ProdSeqNo;
                sheet.Cell(i + 2, j++).Value = data[i].ProductDesc;
                sheet.Cell(i + 2, j++).Value = data[i].Remarks;
            }
        }
    }

}

