using System;

namespace PharmaDiaries.Models
{
    public class Report
    {
        public int CompID { get; set; }

        public string? TransID { get; set; }

        public string? HQcode { get; set; }

        public string? PatchName { get; set; }

        public string? Visited { get; set; }

        public int? UID { get; set; }

        public string? UserName { get; set; }

        public int? CustID { get; set; }

        public string? CustName { get; set; }

        public string? Remarks { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }

        public int EmpSeqNo { get; set; }

        public int Colleague { get; set; }

        public string? ColleagueName { get; set; }

        public int ProdSeqNo { get; set; }

        public int ProductCode { get; set; }

        public string? ProductDesc { get; set; }
    }

    public class MonthlyReportRequest
    {
        //public string? baseDom { get; set; }

        public int CompID { get; set; }

        public int Month { get; set; }

        public int Yr { get; set; }
    }

    // Request model for employee-specific monthly report
    public class EmpMonthlyReportRequest
    {
        public int CompID { get; set; }
        public int UID { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    // Request model for employee-specific yearly report
    public class EmpYearlyReportRequest
    {
        public int CompID { get; set; }
        public int UID { get; set; }
        public int Year { get; set; }
    }

    // Request model for company yearly report
    public class YearlyReportRequest
    {
        public int CompID { get; set; }
        public int Year { get; set; }
    }

    // Request model for financial year report
    public class FinancialYearReportRequest
    {
        public int CompID { get; set; }
        public int FromMonth { get; set; }
        public int FromYear { get; set; }
        public int ToMonth { get; set; }
        public int ToYear { get; set; }
    }
}

