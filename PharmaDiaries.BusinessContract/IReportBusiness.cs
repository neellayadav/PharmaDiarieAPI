using System;
using System.Collections.Generic;
using PharmaDiaries.Models;

namespace PharmaDiaries.BusinessContract
{
    public interface IReportBusiness
    {
        public string GetMonthlyReport(MonthlyReportRequest monRepReq);
        public string GetEmpMonthlyReport(EmpMonthlyReportRequest request);
        public string GetFWMonthlyReport(MonthlyReportRequest request);
        public string GetEmpYearlyReport(EmpYearlyReportRequest request);
        public string GetFWYearlyReport(YearlyReportRequest request);
        public string GetFinancialYearReport(FinancialYearReportRequest request);
    }
}

