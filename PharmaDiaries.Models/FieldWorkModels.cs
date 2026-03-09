using System;
using System.Collections.Generic;

namespace PharmaDiaries.Models
{
    public class FieldWorkHeader
    {
        public int? CompID { get; set; }

        public string? TransID { get; set; }

        public int? UID { get; set; }

        public string? HQcode { get; set; }

        public string? PatchName { get; set; }

        public bool? IsActive { get; set; }

        public int? CustID { get; set; }

        public string? Visited { get; set; }

        public string? Remarks { get; set; }

        public List<FieldworkEmpDT>? EmpDTs { get; set; }

        public List<FieldworkProdDT>? ProdDTs { get; set; }

        // Location tracking fields
        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public double? LocationAccuracy { get; set; }

        // Orders for POB (Personal Order Booking)
        public List<OrderModel>? Orders { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }


    public class FieldworkEmpDT
    {
        public string? TransID { get; set; }

        public int? SNo { get; set; }

        public int? UID { get; set; }

        public bool? IsActive { get; set; }


    }

    public class FieldworkProdDT
    {
        public string? TransID { get; set; }

        public int? SNo { get; set; }

        public int? Prodcode { get; set; }

        public bool? IsActive { get; set; }

    }

    public class FieldworkOthers
    {
        public int? CompID { get; set; }

        public string? TransID { get; set; }

        public int? UID { get; set; }

        public string? WTCode { get; set; }

        public string? Remarks { get; set; }

        public bool? IsActive { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

    }

    public class FWSummary
    {
        public int Daily { get; set; }

        public int Weekly { get; set; }

        public int Monthly { get; set; }

        public decimal DailyPOB { get; set; }

        public decimal WeeklyPOB { get; set; }

        public decimal MonthlyPOB { get; set; }
    }



    //*************** Models for Reports *****************

    public class FWHeader4Report
    {
        public int? CompID { get; set; }

        public string? TransID { get; set; }

        public int? UID { get; set; }

        public string? UserName { get; set; }

        public string? HQcode { get; set; }

        public string? PatchName { get; set; }

        public bool? IsActive { get; set; }

        public int? CustID { get; set; }

        public string? CustName { get; set; }

        public string? Visited { get; set; }

        public string? Remarks { get; set; }

        // Location tracking fields
        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public double? LocationAccuracy { get; set; }

        //public List<FWEmpDT4Report>? EmpDTs { get; set; }

        //public List<FWProdDT4Report>? ProdDTs { get; set; }

        //public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? EmpSeqNo { get; set; }

        public int? Colleague { get; set; }

        public string? ColleagueName { get; set; }

        public int? ProdSeqNo { get; set; }

        public int? ProdCode { get; set; }

        public string? ProductDesc { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }

    // ============== POB (Personal Order Booking) Models ==============

    public class OrderModel
    {
        public int? OrderID { get; set; }

        public int? CompID { get; set; }

        public string? TransID { get; set; }

        public int? ProductID { get; set; }

        public string? ProductName { get; set; }

        public string? ProductPack { get; set; }

        public decimal? MRP { get; set; }

        public decimal? StockistPrice { get; set; }

        public int? Quantity { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? TotalAmount { get; set; }

        public string orderType { get; set; } = "POB";

        public bool? IsActive { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }

    // ============== DCR Date Approval Models ==============

    public class DCRDateRequest
    {
        public int? RequestID { get; set; }

        public int? CompID { get; set; }

        public int? EmployeeID { get; set; }

        public string? EmployeeName { get; set; }

        public string? HeadQuarter { get; set; }

        public DateTime? RequestedDate { get; set; }

        public string? Reason { get; set; }

        public string? Status { get; set; }  // Pending, Approved, Rejected

        public DateTime? RequestedOn { get; set; }

        public int? ApprovedBy { get; set; }

        public string? ApprovedByName { get; set; }

        public DateTime? ApprovedOn { get; set; }

        public string? RejectionReason { get; set; }
    }

    public class FWEmpDT4Report
    {
        public string? TransID { get; set; }

        public int? SNo { get; set; }

        public int? UID { get; set; }

        public string? Name { get; set; }

        public bool? IsActive { get; set; }

    }

    public class FWProdDT4Report
    {
        public string? TransID { get; set; }

        public int? SNo { get; set; }

        public int? Prodcode { get; set; }

        public string? Description { get; set; }

        public bool? IsActive { get; set; }

    }


}