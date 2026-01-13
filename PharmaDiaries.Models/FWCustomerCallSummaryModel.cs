using System;

namespace PharmaDiaries.Models
{
    /// <summary>
    /// Model for Field Work Customer Call Summary response
    /// Maps to SP: [mcDCR].[usp_FWCustomersCallSummary]
    /// </summary>
    public class FWCustomerCallSummaryModel
    {
        public DateTime? VisitedOn { get; set; }
        public string? EmpName { get; set; }
        public string? CustName { get; set; }
    }

    /// <summary>
    /// Request model for Field Work Customer Call Summary API
    /// </summary>
    public class FWCustomerCallSummaryRequestModel
    {
        public int? CompId { get; set; }
        public int? CustID { get; set; }
        public int? UID { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
