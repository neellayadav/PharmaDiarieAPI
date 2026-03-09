-- =============================================
-- LoginLogs Stored Procedures
-- Schema: [mcDCR]
-- Table: [mcDCR].[LoginLogs]
-- Created: February 2026
-- =============================================

-- =============================================
-- 1. INSERT - Create new login log
-- =============================================
CREATE OR ALTER PROCEDURE [mcDCR].[usp_LoginLogInsert]
    @CompID INT,
    @UID INT,
    @Source VARCHAR(10)
    
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [mcDCR].[LoginLogs]
    (
        [CompID],
        [UID],
        [Source],
        [LoginDT] 
    )
    VALUES
    (
        @CompID,
        @UID,
        @Source,
        SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')
    );

    -- Return the newly created LogID
    SELECT SCOPE_IDENTITY() AS LogID;
END
GO

-- =============================================
-- 2. UPDATE - Update LogoutDT only
-- =============================================
CREATE OR ALTER PROCEDURE [mcDCR].[usp_LoginLogUpdate]
    @LogID INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [mcDCR].[LoginLogs]
    SET [LogoutDT] = SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30') 
    WHERE [LogID] = @LogID;

    -- Return affected rows count
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- =============================================
-- 3. GET BY CompID + UID
-- =============================================
CREATE OR ALTER PROCEDURE [mcDCR].[usp_LoginLogGetByCompUID]
    @CompID INT,
    @UID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [LogID],
        [CompID],
        [UID],
        [Source],
        [LoginDT],
        [LogoutDT]
    FROM [mcDCR].[LoginLogs]
    WHERE [CompID] = @CompID
      AND [UID] = @UID
    ORDER BY [LoginDT] DESC;
END
GO

-- =============================================
-- 4. GET BY LogID
-- =============================================
CREATE OR ALTER PROCEDURE [mcDCR].[usp_LoginLogGetByLogId]
    @LogID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [LogID],
        [CompID],
        [UID],
        [Source],
        [LoginDT],
        [LogoutDT]
    FROM [mcDCR].[LoginLogs]
    WHERE [LogID] = @LogID;
END
GO

-- =============================================
-- 5. GET BY CompID + UID + MonthOf + YearOf
-- =============================================
CREATE OR ALTER PROCEDURE [mcDCR].[usp_LoginLogGetByCompUIDMonthYear]
    @CompID INT,
    @UID INT,
    @MonthOf INT,
    @YearOf INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [LogID],
        [CompID],
        [UID],
        [Source],
        [LoginDT],
        [LogoutDT]
    FROM [mcDCR].[LoginLogs]
    WHERE [CompID] = @CompID
      AND [UID] = @UID
      AND MONTH([LoginDT]) = @MonthOf
      AND YEAR([LoginDT]) = @YearOf
    ORDER BY [LoginDT] DESC;
END
GO

-- =============================================
-- 6. GET BY CompID + UID + DateOf (specific date)
-- =============================================
CREATE OR ALTER PROCEDURE [mcDCR].[usp_LoginLogGetByCompUIDDate]
    @CompID INT,
    @UID INT,
    @DateOf DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [LogID],
        [CompID],
        [UID],
        [Source],
        [LoginDT],
        [LogoutDT]
    FROM [mcDCR].[LoginLogs]
    WHERE [CompID] = @CompID
      AND [UID] = @UID
      AND CAST([LoginDT] AS DATE) = @DateOf
    ORDER BY [LoginDT] DESC;
END
GO
