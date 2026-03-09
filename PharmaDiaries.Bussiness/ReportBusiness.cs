using System;
using System.Collections.Generic;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.DataAccessContract.Repository;
using PharmaDiaries.Models;

namespace PharmaDiaries.Bussiness
{
	public class ReportBusiness : IReportBusiness
	{
        private IReportRepository _reportRepository;

        public ReportBusiness(IReportRepository repository)
        {
            _reportRepository = repository;
        }

        string IReportBusiness.GetMonthlyReport(MonthlyReportRequest monRepReq)
        {
            return _reportRepository.GetMonthlyReport(monRepReq);
        }

        string IReportBusiness.GetEmpMonthlyReport(EmpMonthlyReportRequest request)
        {
            return _reportRepository.GetEmpMonthlyReport(request);
        }

        string IReportBusiness.GetFWMonthlyReport(MonthlyReportRequest request)
        {
            return _reportRepository.GetFWMonthlyReport(request);
        }

        string IReportBusiness.GetEmpYearlyReport(EmpYearlyReportRequest request)
        {
            return _reportRepository.GetEmpYearlyReport(request);
        }

        string IReportBusiness.GetFWYearlyReport(YearlyReportRequest request)
        {
            return _reportRepository.GetFWYearlyReport(request);
        }

        string IReportBusiness.GetFinancialYearReport(FinancialYearReportRequest request)
        {
            return _reportRepository.GetFinancialYearReport(request);
        }

        List<FWCustomerCallSummaryModel> IReportBusiness.GetCustomerCallSummary(FWCustomerCallSummaryRequestModel request)
        {
            return _reportRepository.GetCustomerCallSummary(request);
        }

        PaginatedReportResponse IReportBusiness.GetEmpMonthlyData(EmpMonthlyDataRequest request)
        {
            return _reportRepository.GetEmpMonthlyData(request);
        }

        PaginatedReportResponse IReportBusiness.GetMonthlyData(MonthlyDataRequest request)
        {
            return _reportRepository.GetMonthlyData(request);
        }

        PaginatedReportResponse IReportBusiness.GetEmpYearlyData(EmpYearlyDataRequest request)
        {
            return _reportRepository.GetEmpYearlyData(request);
        }

        PaginatedReportResponse IReportBusiness.GetYearlyData(YearlyDataRequest request)
        {
            return _reportRepository.GetYearlyData(request);
        }

        PaginatedReportResponse IReportBusiness.GetFinancialYearData(FinancialYearDataRequest request)
        {
            return _reportRepository.GetFinancialYearData(request);
        }
    }
}

