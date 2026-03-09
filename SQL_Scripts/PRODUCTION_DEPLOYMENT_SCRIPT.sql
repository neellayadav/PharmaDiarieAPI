-- ========================================================================================================================================
-- PRODUCTION DEPLOYMENT SCRIPT
-- Date: 2025-11-18
-- Feature: Company Admin & Screen-Based Permissions
--
-- IMPORTANT: This script contains ALL fixes that were applied to staging
-- Execute this on PRODUCTION database after successful staging testing
-- ========================================================================================================================================

-- ========================================================================================================================================
-- BACKUP REMINDER
-- ========================================================================================================================================
PRINT '⚠️  IMPORTANT: Have you backed up the production database?';
PRINT 'If NO, press Ctrl+C to cancel and backup first!';
PRINT 'Waiting 5 seconds...';
WAITFOR DELAY '00:00:05';
GO

-- ========================================================================================================================================
-- PART 1: ALTER TABLE (Already done in Phase 1)
-- ========================================================================================================================================
PRINT '';
PRINT '========================================';
PRINT 'PART 1: Table Structure';
PRINT '========================================';
PRINT '';

-- Check if isCompAdmin column already exists
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_SCHEMA = 'McMaster'
               AND TABLE_NAME = 'Users'
               AND COLUMN_NAME = 'isCompAdmin')
BEGIN
    PRINT 'Adding isCompAdmin column to Users table...';
    ALTER TABLE McMaster.Users ADD isCompAdmin BIT NULL DEFAULT 0;
    PRINT '✅ Column added';
END
ELSE
BEGIN
    PRINT '✅ isCompAdmin column already exists';
END
GO

-- Update existing admin users
PRINT 'Updating existing admin users (UID < 105)...';
UPDATE McMaster.Users SET isCompAdmin = 1 WHERE UID < 105 AND (isCompAdmin IS NULL OR isCompAdmin = 0);
PRINT '✅ Admin users updated';
GO

-- Update NULL values to 0
PRINT 'Setting NULL values to 0...';
UPDATE McMaster.Users SET isCompAdmin = 0 WHERE isCompAdmin IS NULL;
PRINT '✅ NULL values fixed';
GO

-- ========================================================================================================================================
-- PART 2: CREATE TABLES (Already done in Phase 1)
-- ========================================================================================================================================
PRINT '';
PRINT '========================================';
PRINT 'PART 2: Screen Management Tables';
PRINT '========================================';
PRINT '';

-- Check and create ScreenMaster table
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES
               WHERE TABLE_SCHEMA = 'McMaster'
               AND TABLE_NAME = 'ScreenMaster')
BEGIN
    PRINT 'Creating ScreenMaster table...';
    CREATE TABLE McMaster.ScreenMaster (
        ScreenID INT IDENTITY(1,1) PRIMARY KEY,
        ScreenName NVARCHAR(255) NOT NULL,
        ScreenRoute NVARCHAR(255) NOT NULL,
        ScreenDescription NVARCHAR(500) NULL,
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE()
    );
    PRINT '✅ ScreenMaster table created';
END
ELSE
BEGIN
    PRINT '✅ ScreenMaster table already exists';
END
GO

-- Check and create UserScreenPermissions table
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES
               WHERE TABLE_SCHEMA = 'McMaster'
               AND TABLE_NAME = 'UserScreenPermissions')
BEGIN
    PRINT 'Creating UserScreenPermissions table...';
    CREATE TABLE McMaster.UserScreenPermissions (
        PermissionID INT IDENTITY(1,1) PRIMARY KEY,
        UserID INT NOT NULL,
        ScreenID INT NOT NULL,
        HasAccess BIT DEFAULT 1,
        CreatedBy INT NULL,
        CreatedOn DATETIME DEFAULT GETDATE(),
        ModifiedBy INT NULL,
        ModifiedOn DATETIME NULL
    );
    PRINT '✅ UserScreenPermissions table created';
END
ELSE
BEGIN
    PRINT '✅ UserScreenPermissions table already exists';
END
GO

-- ========================================================================================================================================
-- PART 3: UPDATE USER STORED PROCEDURES WITH IsCompAdmin ALIAS
-- ========================================================================================================================================
PRINT '';
PRINT '========================================';
PRINT 'PART 3: User Stored Procedures';
PRINT '========================================';
PRINT '';

-- 3.1: usp_UserLogin
PRINT 'Updating usp_UserLogin...';
ALTER PROC [mcDCR].[usp_UserLogin]
    @compid int,
    @userid VARCHAR(50),
    @password varchar(20)
AS
BEGIN
    SELECT [CompID], [UID], [UserID]
          ,[Password]
          ,[Name]
          ,[HeadQuater]
          ,[Address1]
          ,[Locality]
          ,[CityOrTown]
          ,[Pincode]
          ,[District]
          ,[State]
          ,[Country]
          ,[Mobile]
          ,[Telephone]
          ,[IsActive]
          ,[CreatedBy]
          ,[CreatedOn]
          ,[ModifiedBy]
          ,[ModifiedOn]
          ,[isCompAdmin] AS [IsCompAdmin]
    FROM [mcMaster].[Users]
    WHERE [UserID] = @userid COLLATE SQL_Latin1_General_CP1_CS_AS
          AND [Password] = @password COLLATE SQL_Latin1_General_CP1_CS_AS
          AND IsActive = cast(1 as bit)
END
GO
PRINT '✅ usp_UserLogin updated';

-- 3.2: usp_UserList
PRINT 'Updating usp_UserList...';
ALTER PROC [mcDCR].[usp_UserList]
AS
BEGIN
    SELECT [CompID], [UID], [UserID]
          ,[Password]
          ,[Name]
          ,[HeadQuater]
          ,[Address1]
          ,[Locality]
          ,[CityOrTown]
          ,[Pincode]
          ,[District]
          ,[State]
          ,[Country]
          ,[Mobile]
          ,[Telephone]
          ,[IsActive]
          ,[isCompAdmin] AS [IsCompAdmin]
    FROM [mcMaster].[Users]
    WHERE IsActive = cast(1 as bit)
          AND UID != 100
END
GO
PRINT '✅ usp_UserList updated';

-- 3.3: usp_UserListByCompany
PRINT 'Updating usp_UserListByCompany...';
ALTER PROC [mcDCR].[usp_UserListByCompany]
    @compid int
AS
BEGIN
    SELECT [CompID], [UID], [UserID]
          ,[Password]
          ,[Name]
          ,[HeadQuater]
          ,[Address1]
          ,[Locality]
          ,[CityOrTown]
          ,[Pincode]
          ,[District]
          ,[State]
          ,[Country]
          ,[Mobile]
          ,[Telephone]
          ,[IsActive]
          ,[isCompAdmin] AS [IsCompAdmin]
    FROM [mcMaster].[Users]
    WHERE [CompID] = @compid
          AND IsActive = cast(1 as bit)
          AND UID != 100
END
GO
PRINT '✅ usp_UserListByCompany updated';

-- ========================================================================================================================================
-- PART 4: CREATE SCREEN STORED PROCEDURES
-- ========================================================================================================================================
PRINT '';
PRINT '========================================';
PRINT 'PART 4: Screen Management Procedures';
PRINT '========================================';
PRINT '';

-- 4.1: usp_ScreenSync
PRINT 'Creating usp_ScreenSync...';
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[McMaster].[usp_ScreenSync]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [McMaster].[usp_ScreenSync];
GO

CREATE PROCEDURE [McMaster].[usp_ScreenSync]
    @ScreenName NVARCHAR(255),
    @ScreenRoute NVARCHAR(255),
    @ScreenDescription NVARCHAR(500) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM McMaster.ScreenMaster WHERE ScreenRoute = @ScreenRoute)
    BEGIN
        UPDATE McMaster.ScreenMaster
        SET ScreenName = @ScreenName,
            ScreenDescription = @ScreenDescription,
            IsActive = @IsActive
        WHERE ScreenRoute = @ScreenRoute;
    END
    ELSE
    BEGIN
        INSERT INTO McMaster.ScreenMaster (ScreenName, ScreenRoute, ScreenDescription, IsActive, CreatedDate)
        VALUES (@ScreenName, @ScreenRoute, @ScreenDescription, @IsActive, GETDATE());
    END
END
GO
PRINT '✅ usp_ScreenSync created';

-- 4.2: usp_GetActiveScreens
PRINT 'Creating usp_GetActiveScreens...';
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[McMaster].[usp_GetActiveScreens]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [McMaster].[usp_GetActiveScreens];
GO

CREATE PROCEDURE [McMaster].[usp_GetActiveScreens]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ScreenID, ScreenName, ScreenRoute, ScreenDescription, IsActive, CreatedDate
    FROM McMaster.ScreenMaster
    WHERE IsActive = 1
    ORDER BY ScreenID;
END
GO
PRINT '✅ usp_GetActiveScreens created';

-- 4.3: usp_SaveUserScreenPermissions
PRINT 'Creating usp_SaveUserScreenPermissions...';
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[McMaster].[usp_SaveUserScreenPermissions]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [McMaster].[usp_SaveUserScreenPermissions];
GO

CREATE PROCEDURE [McMaster].[usp_SaveUserScreenPermissions]
    @UserID INT,
    @ScreenIDs NVARCHAR(MAX),
    @CreatedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DELETE FROM McMaster.UserScreenPermissions WHERE UserID = @UserID;

        IF LEN(@ScreenIDs) > 0
        BEGIN
            INSERT INTO McMaster.UserScreenPermissions (UserID, ScreenID, HasAccess, CreatedBy, CreatedOn)
            SELECT @UserID, CAST(value AS INT), 1, @CreatedBy, GETDATE()
            FROM STRING_SPLIT(@ScreenIDs, ',')
            WHERE RTRIM(value) <> '';
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
PRINT '✅ usp_SaveUserScreenPermissions created';

-- 4.4: usp_GetUserScreenPermissions
PRINT 'Creating usp_GetUserScreenPermissions...';
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[McMaster].[usp_GetUserScreenPermissions]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [McMaster].[usp_GetUserScreenPermissions];
GO

CREATE PROCEDURE [McMaster].[usp_GetUserScreenPermissions]
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT p.PermissionID, p.UserID, p.ScreenID,
           s.ScreenName, s.ScreenRoute, s.ScreenDescription,
           p.HasAccess
    FROM McMaster.UserScreenPermissions p
    INNER JOIN McMaster.ScreenMaster s ON p.ScreenID = s.ScreenID
    WHERE p.UserID = @UserID
      AND p.HasAccess = 1
      AND s.IsActive = 1
    ORDER BY s.ScreenID;
END
GO
PRINT '✅ usp_GetUserScreenPermissions created';

-- ========================================================================================================================================
-- VERIFICATION
-- ========================================================================================================================================
PRINT '';
PRINT '========================================';
PRINT 'DEPLOYMENT COMPLETE - VERIFICATION';
PRINT '========================================';
PRINT '';

PRINT 'Tables:';
SELECT TABLE_SCHEMA, TABLE_NAME, 'EXISTS' AS Status
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME IN ('Users', 'ScreenMaster', 'UserScreenPermissions')
ORDER BY TABLE_NAME;

PRINT '';
PRINT 'Stored Procedures:';
SELECT ROUTINE_SCHEMA AS [Schema], ROUTINE_NAME AS [Procedure], LAST_ALTERED AS [Last Modified]
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_TYPE = 'PROCEDURE'
  AND ROUTINE_NAME IN (
      'usp_UserLogin',
      'usp_UserList',
      'usp_UserListByCompany',
      'usp_ScreenSync',
      'usp_GetActiveScreens',
      'usp_SaveUserScreenPermissions',
      'usp_GetUserScreenPermissions'
  )
ORDER BY ROUTINE_NAME;

PRINT '';
PRINT '========================================';
PRINT '✅ PRODUCTION DEPLOYMENT SUCCESSFUL';
PRINT '========================================';
PRINT '';
PRINT 'Next Steps:';
PRINT '1. Deploy .NET API code to production';
PRINT '2. Restart production API';
PRINT '3. Test production endpoints';
PRINT '4. Deploy Flutter app';
PRINT '';
