-- =====================================================
-- Fix: Update stored procedures to use IST timezone
-- Date: 2025-01-06
-- =====================================================

-- SP to Update Customer Location (with IST)
CREATE OR ALTER PROCEDURE [mcMaster].[usp_UpdateCustomerLocation]
    @CompID INT,
    @CustID INT,
    @Latitude DECIMAL(10, 7),
    @Longitude DECIMAL(10, 7),
    @ModifiedBy INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [mcMaster].[Customers]
    SET Latitude = @Latitude,
        Longitude = @Longitude,
        ModifiedBy = @ModifiedBy,
        ModifiedOn = SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')
    WHERE CompID = @CompID AND CustID = @CustID;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
PRINT 'Updated [mcMaster].[usp_UpdateCustomerLocation] with IST timezone';
GO

-- SP to Update Company GeoFence Settings (with IST)
CREATE OR ALTER PROCEDURE [mcMaster].[usp_UpdateCompanyGeoFenceSettings]
    @CompID INT,
    @IsLocationTrackerEnabled BIT,
    @GeoFenceRadiusMeters INT,
    @GPSAccuracyThreshold INT,
    @ModifiedBy INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [mcMaster].[Company]
    SET IsLocationTrackerEnabled = @IsLocationTrackerEnabled,
        GeoFenceRadiusMeters = @GeoFenceRadiusMeters,
        GPSAccuracyThreshold = @GPSAccuracyThreshold,
        ModifiedBy = @ModifiedBy,
        ModifiedOn = SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')
    WHERE CompID = @CompID;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
PRINT 'Updated [mcMaster].[usp_UpdateCompanyGeoFenceSettings] with IST timezone';
GO

PRINT '';
PRINT 'All stored procedures updated to use IST timezone (+05:30)';
GO
