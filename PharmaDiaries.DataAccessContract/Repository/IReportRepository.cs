using System;
using System.Collections.Generic;
using PharmaDiaries.Models;

namespace PharmaDiaries.DataAccessContract.Repository
{
	public interface IReportRepository
	{
		public string GetMonthlyReport(MonthlyReportRequest monRepReq);
		public string GetEmpMonthlyReport(EmpMonthlyReportRequest request);
		public string GetFWMonthlyReport(MonthlyReportRequest request);
		public string GetEmpYearlyReport(EmpYearlyReportRequest request);
		public string GetFWYearlyReport(YearlyReportRequest request);
		public string GetFinancialYearReport(FinancialYearReportRequest request);
		public List<FWCustomerCallSummaryModel> GetCustomerCallSummary(FWCustomerCallSummaryRequestModel request);
	}
}

