using System;
using System.Collections.Generic;

namespace PharmaDiaries.Models
{
    // ========================
    // Response Models (GET)
    // ========================

    public class SalesHeaderModel
    {
        public int? CompID { get; set; }
        public string? SalesID { get; set; }
        public int? UID { get; set; }
        public string? UserName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? CustID { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerType { get; set; }
        public string? Type { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public List<SalesDetailModel>? Details { get; set; }
    }

    public class SalesDetailModel
    {
        public int? ItemID { get; set; }
        public string? SalesID { get; set; }
        public int? ProductID { get; set; }
        public string? ProductName { get; set; }
        public double? ProductPrice { get; set; }
        public double? MRP { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    // ========================
    // Request Models (INSERT / UPDATE / DELETE)
    // ========================

    public class SalesHeaderCreateRequest
    {
        public int CompID { get; set; }
        public int UID { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int CustID { get; set; }
        public string Type { get; set; } = "PRIMARY";
        public int CreatedBy { get; set; }
    }

    public class SalesHeaderUpdateRequest
    {
        public int CompID { get; set; }
        public string SalesID { get; set; } = string.Empty;
        public int UID { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int CustID { get; set; }
        public string Type { get; set; } = "PRIMARY";
        public int ModifiedBy { get; set; }
    }

    public class SalesHeaderDeleteRequest
    {
        public int CompID { get; set; }
        public string SalesID { get; set; } = string.Empty;
        public int UID { get; set; }
        public int ModifiedBy { get; set; }
    }

    public class SalesDetailCreateRequest
    {
        public string SalesID { get; set; } = string.Empty;
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public int CreatedBy { get; set; }
    }

    public class SalesDetailUpdateRequest
    {
        public int ItemID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public int ModifiedBy { get; set; }
    }

    public class SalesDetailDeleteRequest
    {
        public int ItemID { get; set; }
    }

    // ========================
    // Combined Create (Header + Details in one call)
    // ========================

    public class CreateSaleRequest
    {
        public int CompID { get; set; }
        public int UID { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int CustID { get; set; }
        public string Type { get; set; } = "PRIMARY";
        public int CreatedBy { get; set; }
        public List<CreateSaleDetailItem> Details { get; set; } = new();
    }

    public class CreateSaleDetailItem
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class CreateSaleResponse
    {
        public string SalesID { get; set; } = string.Empty;
        public List<int> ItemIDs { get; set; } = new();
    }
}
