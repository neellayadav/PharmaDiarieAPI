-- =====================================================
-- Geo-Fencing Feature - Database Migration Script
-- Date: 2025-01-06
-- Description: Adds geo-fencing capabilities for DCR validation
--              - Company settings for radius and accuracy
--              - Customer location coordinates
--              - Location audit table
-- =====================================================

-- =====================================================
-- STEP 1: Add GeoFencing columns to Company table
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[mcMaster].[Company]') AND name = 'GeoFenceRadiusMeters')
BEGIN
    ALTER TABLE [mcMaster].[Company]
    ADD GeoFenceRadiusMeters INT NOT NULL DEFAULT 100;
    PRINT 'Column GeoFenceRadiusMeters added to Company table (default: 100 meters)';
END
ELSE
BEGIN
    PRINT 'Column GeoFenceRadiusMeters already exists in Company table';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[mcMaster].[Company]') AND name = 'GPSAccuracyThreshold')
BEGIN
    ALTER TABLE [mcMaster].[Company]
    ADD GPSAccuracyThreshold INT NOT NULL DEFAULT 50;
    PRINT 'Column GPSAccuracyThreshold added to Company table (default: 50 meters)';
END
ELSE
BEGIN
    PRINT 'Column GPSAccuracyThreshold already exists in Company table';
END
GO

-- =====================================================
-- STEP 2: Add Location columns to Customers table
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[mcMaster].[Customers]') AND name = 'Latitude')
BEGIN
    ALTER TABLE [mcMaster].[Customers]
    ADD Latitude DECIMAL(10, 7) NULL;
    PRINT 'Column Latitude added to Customers table';
END
ELSE
BEGIN
    PRINT 'Column Latitude already exists in Customers table';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[mcMaster].[Customers]') AND name = 'Longitude')
BEGIN
    ALTER TABLE [mcMaster].[Customers]
    ADD Longitude DECIMAL(10, 7) NULL;
    PRINT 'Column Longitude added to Customers table';
END
ELSE
BEGIN
    PRINT 'Column Longitude already exists in Customers table';
END
GO

-- =====================================================
-- STEP 3: Create DCRLocationAudit table
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DCRLocationAudit' AND schema_id = SCHEMA_ID('mcDCR'))
BEGIN
    CREATE TABLE [mcDCR].[DCRLocationAudit] (
        AuditID INT IDENTITY(1,1) PRIMARY KEY,
        CompID INT NOT NULL,
        TransID VARCHAR(50) NOT NULL,
        UID INT NOT NULL,
        CustID INT NOT NULL,

        -- MR's captured location
        CapturedLatitude DECIMAL(10, 7) NULL,
        CapturedLongitude DECIMAL(10, 7) NULL,
        CapturedAccuracy FLOAT NULL,

        -- Customer's expected location
        ExpectedLatitude DECIMAL(10, 7) NULL,
        ExpectedLongitude DECIMAL(10, 7) NULL,

        -- Validation results
        DistanceMeters FLOAT NULL,
        AllowedRadiusMeters INT NULL,
        ValidationStatus VARCHAR(20) NOT NULL, -- 'PASSED', 'FAILED', 'SKIPPED', 'NO_CUSTOMER_LOCATION'
        ValidationMessage NVARCHAR(500) NULL,

        -- Device info
        DeviceInfo NVARCHAR(500) NULL,

        -- Timestamps
        CreatedOn DATETIME DEFAULT SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')
    );

    -- Create indexes for common queries
    CREATE INDEX IX_DCRLocationAudit_CompID ON [mcDCR].[DCRLocationAudit](CompID);
    CREATE INDEX IX_DCRLocationAudit_UID ON [mcDCR].[DCRLocationAudit](UID);
    CREATE INDEX IX_DCRLocationAudit_TransID ON [mcDCR].[DCRLocationAudit](TransID);
    CREATE INDEX IX_DCRLocationAudit_CreatedOn ON [mcDCR].[DCRLocationAudit](CreatedOn);
    CREATE INDEX IX_DCRLocationAudit_ValidationStatus ON [mcDCR].[DCRLocationAudit](ValidationStatus);

    PRINT 'Table [mcDCR].[DCRLocationAudit] created successfully';
END
ELSE
BEGIN
    PRINT 'Table [mcDCR].[DCRLocationAudit] already exists';
END
GO

-- =====================================================
-- STEP 4: SP to Update Customer Location
-- =====================================================
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
PRINT 'Stored Procedure [mcMaster].[usp_UpdateCustomerLocation] created/updated';
GO

-- =====================================================
-- STEP 5: SP to Get Customer Location
-- =====================================================
CREATE OR ALTER PROCEDURE [mcMaster].[usp_GetCustomerLocation]
    @CompID INT,
    @CustID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CustID,
        Name,
        Latitude,
        Longitude
    FROM [mcMaster].[Customers]
    WHERE CompID = @CompID AND CustID = @CustID AND IsActive = 1;
END
GO
PRINT 'Stored Procedure [mcMaster].[usp_GetCustomerLocation] created/updated';
GO

-- =====================================================
-- STEP 6: SP to Insert Location Audit Record
-- =====================================================
CREATE OR ALTER PROCEDURE [mcDCR].[usp_InsertLocationAudit]
    @CompID INT,
    @TransID VARCHAR(50),
    @UID INT,
    @CustID INT,
    @CapturedLatitude DECIMAL(10, 7) = NULL,
    @CapturedLongitude DECIMAL(10, 7) = NULL,
    @CapturedAccuracy FLOAT = NULL,
    @ExpectedLatitude DECIMAL(10, 7) = NULL,
    @ExpectedLongitude DECIMAL(10, 7) = NULL,
    @DistanceMeters FLOAT = NULL,
    @AllowedRadiusMeters INT = NULL,
    @ValidationStatus VARCHAR(20),
    @ValidationMessage NVARCHAR(500) = NULL,
    @DeviceInfo NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [mcDCR].[DCRLocationAudit] (
        CompID, TransID, UID, CustID,
        CapturedLatitude, CapturedLongitude, CapturedAccuracy,
        ExpectedLatitude, ExpectedLongitude,
        DistanceMeters, AllowedRadiusMeters,
        ValidationStatus, ValidationMessage, DeviceInfo
    )
    VALUES (
        @CompID, @TransID, @UID, @CustID,
        @CapturedLatitude, @CapturedLongitude, @CapturedAccuracy,
        @ExpectedLatitude, @ExpectedLongitude,
        @DistanceMeters, @AllowedRadiusMeters,
        @ValidationStatus, @ValidationMessage, @DeviceInfo
    );

    SELECT SCOPE_IDENTITY() AS AuditID;
END
GO
PRINT 'Stored Procedure [mcDCR].[usp_InsertLocationAudit] created/updated';
GO

-- =====================================================
-- STEP 7: SP to Get Company GeoFence Settings
-- =====================================================
CREATE OR ALTER PROCEDURE [mcMaster].[usp_GetCompanyGeoFenceSettings]
    @CompID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CompID,
        IsLocationTrackerEnabled,
        GeoFenceRadiusMeters,
        GPSAccuracyThreshold
    FROM [mcMaster].[Company]
    WHERE CompID = @CompID;
END
GO
PRINT 'Stored Procedure [mcMaster].[usp_GetCompanyGeoFenceSettings] created/updated';
GO

-- =====================================================
-- STEP 8: SP to Update Company GeoFence Settings
-- =====================================================
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
PRINT 'Stored Procedure [mcMaster].[usp_UpdateCompanyGeoFenceSettings] created/updated';
GO

-- =====================================================
-- STEP 9: Update existing Customer stored procedures
-- to include Latitude/Longitude
-- =====================================================

-- First, check if the customer insert/update SPs exist and need updating
-- We'll create a wrapper to get customer with location
CREATE OR ALTER PROCEDURE [mcMaster].[usp_GetCustomerWithLocation]
    @CompID INT,
    @CustID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CompID,
        CustID,
        Name,
        Type,
        IsListed,
        QUALIFICATION,
        Speciality,
        HeadQuater,
        PATCH,
        Address1,
        Locality,
        CityOrTown,
        Pincode,
        District,
        State,
        Country,
        Mobile,
        Telephone,
        Latitude,
        Longitude,
        IsActive
    FROM [mcMaster].[Customers]
    WHERE CompID = @CompID AND CustID = @CustID;
END
GO
PRINT 'Stored Procedure [mcMaster].[usp_GetCustomerWithLocation] created/updated';
GO

-- =====================================================
-- SUMMARY
-- =====================================================
PRINT '';
PRINT '=====================================================';
PRINT 'Geo-Fencing Feature - Migration Complete!';
PRINT '=====================================================';
PRINT '';
PRINT 'Changes Made:';
PRINT '  - Company table: Added GeoFenceRadiusMeters (default 100m)';
PRINT '  - Company table: Added GPSAccuracyThreshold (default 50m)';
PRINT '  - Customers table: Added Latitude, Longitude columns';
PRINT '  - Created DCRLocationAudit table for audit trail';
PRINT '';
PRINT 'Stored Procedures Created/Updated:';
PRINT '  - [mcMaster].[usp_UpdateCustomerLocation]';
PRINT '  - [mcMaster].[usp_GetCustomerLocation]';
PRINT '  - [mcMaster].[usp_GetCustomerWithLocation]';
PRINT '  - [mcMaster].[usp_GetCompanyGeoFenceSettings]';
PRINT '  - [mcMaster].[usp_UpdateCompanyGeoFenceSettings]';
PRINT '  - [mcDCR].[usp_InsertLocationAudit]';
PRINT '';
GO
