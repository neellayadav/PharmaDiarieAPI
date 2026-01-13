-- =====================================================
-- Fix: Create DCRLocationAudit table WITHOUT FK constraints
-- Date: 2025-01-06
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

    PRINT 'Table [mcDCR].[DCRLocationAudit] created successfully (without FK constraints)';
END
ELSE
BEGIN
    PRINT 'Table [mcDCR].[DCRLocationAudit] already exists';
END
GO
