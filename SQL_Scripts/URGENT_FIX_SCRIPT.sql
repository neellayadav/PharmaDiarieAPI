-- ========================================================================================================================================
-- URGENT FIX SCRIPT - Run this immediately to fix API errors
-- Date: 2025-11-18
-- Issues Fixed:
--   1. isCompAdmin returning NULL in GetUserListByComp API
--   2. Runtime error in Screen API (missing stored procedures)
-- ========================================================================================================================================

-- ========================================================================================================================================
-- FIX 1: Add isCompAdmin to usp_UserListByCompany
-- ========================================================================================================================================

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
      ,[isCompAdmin]
FROM [mcMaster].[Users]
	WHERE [CompID] = @compid
           AND IsActive = cast(1 as bit)
           AND UID != 100

END
GO

-- ========================================================================================================================================
-- FIX 2: Create missing Screen stored procedures
-- ========================================================================================================================================

-- Stored Procedure 1: usp_ScreenSync
-- Purpose: Sync a single screen (insert if not exists, update if exists)
-- ========================================================================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[McMaster].[usp_ScreenSync]') AND type in (N'P', N'PC'))
DROP PROCEDURE [McMaster].[usp_ScreenSync]
GO

CREATE PROCEDURE [McMaster].[usp_ScreenSync]
    @ScreenName NVARCHAR(255),
    @ScreenRoute NVARCHAR(255),
    @ScreenDescription NVARCHAR(500) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if screen already exists by route
    IF EXISTS (SELECT 1 FROM McMaster.ScreenMaster WHERE ScreenRoute = @ScreenRoute)
    BEGIN
        -- Update existing screen
        UPDATE McMaster.ScreenMaster
        SET ScreenName = @ScreenName,
            ScreenDescription = @ScreenDescription,
            IsActive = @IsActive
        WHERE ScreenRoute = @ScreenRoute;
    END
    ELSE
    BEGIN
        -- Insert new screen
        INSERT INTO McMaster.ScreenMaster (ScreenName, ScreenRoute, ScreenDescription, IsActive, CreatedDate)
        VALUES (@ScreenName, @ScreenRoute, @ScreenDescription, @IsActive, GETDATE());
    END
END
GO

-- ========================================================================================================================================
-- Stored Procedure 2: usp_GetActiveScreens
-- Purpose: Get all active screens
-- ========================================================================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[McMaster].[usp_GetActiveScreens]') AND type in (N'P', N'PC'))
DROP PROCEDURE [McMaster].[usp_GetActiveScreens]
GO

CREATE PROCEDURE [McMaster].[usp_GetActiveScreens]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ScreenID,
        ScreenName,
        ScreenRoute,
        ScreenDescription,
        IsActive,
        CreatedDate
    FROM McMaster.ScreenMaster
    WHERE IsActive = 1
    ORDER BY ScreenID;
END
GO

-- ========================================================================================================================================
-- Stored Procedure 3: usp_SaveUserScreenPermissions
-- Purpose: Save user screen permissions (delete old, insert new)
-- ========================================================================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[McMaster].[usp_SaveUserScreenPermissions]') AND type in (N'P', N'PC'))
DROP PROCEDURE [McMaster].[usp_SaveUserScreenPermissions]
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

        -- Delete existing permissions for this user
        DELETE FROM McMaster.UserScreenPermissions WHERE UserID = @UserID;

        -- Insert new permissions if ScreenIDs provided
        IF LEN(@ScreenIDs) > 0
        BEGIN
            -- Split comma-separated ScreenIDs and insert
            INSERT INTO McMaster.UserScreenPermissions (UserID, ScreenID, HasAccess, CreatedBy, CreatedOn)
            SELECT
                @UserID,
                CAST(value AS INT),
                1,
                @CreatedBy,
                GETDATE()
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

-- ========================================================================================================================================
-- Stored Procedure 4: usp_GetUserScreenPermissions
-- Purpose: Get user's assigned screen permissions
-- ========================================================================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[McMaster].[usp_GetUserScreenPermissions]') AND type in (N'P', N'PC'))
DROP PROCEDURE [McMaster].[usp_GetUserScreenPermissions]
GO

CREATE PROCEDURE [McMaster].[usp_GetUserScreenPermissions]
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.PermissionID,
        p.UserID,
        p.ScreenID,
        s.ScreenName,
        s.ScreenRoute,
        s.ScreenDescription,
        p.HasAccess
    FROM McMaster.UserScreenPermissions p
    INNER JOIN McMaster.ScreenMaster s ON p.ScreenID = s.ScreenID
    WHERE p.UserID = @UserID
      AND p.HasAccess = 1
      AND s.IsActive = 1
    ORDER BY s.ScreenID;
END
GO

-- ========================================================================================================================================
-- VERIFICATION QUERIES
-- ========================================================================================================================================
PRINT 'Stored procedures created successfully!';
PRINT '';
PRINT 'Verification:';
SELECT
    ROUTINE_SCHEMA,
    ROUTINE_NAME
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_TYPE = 'PROCEDURE'
  AND (ROUTINE_NAME LIKE '%Screen%' OR ROUTINE_NAME = 'usp_UserListByCompany')
ORDER BY ROUTINE_NAME;
