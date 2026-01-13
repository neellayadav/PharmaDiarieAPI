using System;

namespace PharmaDiaries.Models
{
    /// <summary>
    /// Model for DCR Location Audit records
    /// </summary>
    public class DCRLocationAuditModel
    {
        public int AuditID { get; set; }
        public int CompID { get; set; }
        public string? TransID { get; set; }
        public int UID { get; set; }
        public int CustID { get; set; }

        // MR's captured location
        public decimal? CapturedLatitude { get; set; }
        public decimal? CapturedLongitude { get; set; }
        public double? CapturedAccuracy { get; set; }

        // Customer's expected location
        public decimal? ExpectedLatitude { get; set; }
        public decimal? ExpectedLongitude { get; set; }

        // Validation results
        public double? DistanceMeters { get; set; }
        public int? AllowedRadiusMeters { get; set; }
        public string? ValidationStatus { get; set; } // PASSED, FAILED, SKIPPED, NO_CUSTOMER_LOCATION
        public string? ValidationMessage { get; set; }

        // Device info
        public string? DeviceInfo { get; set; }

        // Timestamp
        public DateTime? CreatedOn { get; set; }
    }

    /// <summary>
    /// Request model for inserting location audit
    /// </summary>
    public class LocationAuditRequest
    {
        public int CompID { get; set; }
        public string? TransID { get; set; }
        public int UID { get; set; }
        public int CustID { get; set; }
        public decimal? CapturedLatitude { get; set; }
        public decimal? CapturedLongitude { get; set; }
        public double? CapturedAccuracy { get; set; }
        public decimal? ExpectedLatitude { get; set; }
        public decimal? ExpectedLongitude { get; set; }
        public double? DistanceMeters { get; set; }
        public int? AllowedRadiusMeters { get; set; }
        public string ValidationStatus { get; set; } = "SKIPPED";
        public string? ValidationMessage { get; set; }
        public string? DeviceInfo { get; set; }
    }

    /// <summary>
    /// Response model for location validation
    /// </summary>
    public class LocationValidationResponse
    {
        public bool IsValid { get; set; }
        public string Status { get; set; } = string.Empty; // PASSED, FAILED, SKIPPED, NO_CUSTOMER_LOCATION
        public string Message { get; set; } = string.Empty;
        public double? DistanceMeters { get; set; }
        public int? AllowedRadiusMeters { get; set; }
        public bool CustomerHasLocation { get; set; }
    }

    /// <summary>
    /// Request model for validating location before DCR save
    /// </summary>
    public class LocationValidationRequest
    {
        public int CompID { get; set; }
        public int CustID { get; set; }
        public decimal CapturedLatitude { get; set; }
        public decimal CapturedLongitude { get; set; }
        public double? CapturedAccuracy { get; set; }
    }
}
