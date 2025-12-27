-- =============================================
-- Phase 2: Company Admin and Screen Permissions
-- Stored Procedures for SQL Server
-- =============================================

USE [YourDatabaseName]
GO

-- =============================================
-- 1. Update usp_UserInsert to include isCompAdmin
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[mcDCR].[usp_UserInsert]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [mcDCR].[usp_UserInsert]
GO

CREATE PROCEDURE [mcDCR].[usp_UserInsert]
    @compID INT,
    @userID NVARCHAR(50),
    @password NVARCHAR(100),
    @name NVARCHAR(100),
    @headquater NVARCHAR(200),
    @address1 NVARCHAR(200),
    @locality NVARCHAR(100),
    @cityOrTown NVARCHAR(100),
    @pincode INT,
    @district NVARCHAR(100),
    @state NVARCHAR(100),
    @country NVARCHAR(100),
    @mobile NVARCHAR(20),
    @telephone NVARCHAR(20),
    @isCompAdmin BIT,
    @createdBy INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [mcDCR].[Users]
    (
        CompID, UserID, Password, Name, HeadQuater, Address1, Locality,
        CityOrTown, Pincode, District, State, Country, Mobile, Telephone,
        IsActive, IsCompAdmin, CreatedBy, CreatedOn
    )
    VALUES
    (
        @compID, @userID, @password, @name, @headquater, @address1, @locality,
        @cityOrTown, @pincode, @district, @state, @country, @mobile, @telephone,
        1, @isCompAdmin, @createdBy, GETDATE()
    )
END
GO

-- =============================================
-- 2. Update usp_UserUpdate to include isCompAdmin
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[mcDCR].[usp_UserUpdate]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [mcDCR].[usp_UserUpdate]
GO

CREATE PROCEDURE [mcDCR].[usp_UserUpdate]
    @compID INT,
    @UID INT,
    @userID NVARCHAR(50),
    @password NVARCHAR(100),
    @name NVARCHAR(100),
    @headquater NVARCHAR(200),
    @address1 NVARCHAR(200),
    @locality NVARCHAR(100),
    @cityOrTown NVARCHAR(100),
    @pincode INT,
    @district NVARCHAR(100),
    @state NVARCHAR(100),
    @country NVARCHAR(100),
    @mobile NVARCHAR(20),
    @telephone NVARCHAR(20),
    @isCompAdmin BIT,
    @modifiedBy INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [mcDCR].[Users]
    SET
        UserID = @userID,
        Password = @password,
        Name = @name,
        HeadQuater = @headquater,
        Address1 = @address1,
        Locality = @locality,
        CityOrTown = @cityOrTown,
        Pincode = @pincode,
        District = @district,
        State = @state,
        Country = @country,
        Mobile = @mobile,
        Telephone = @telephone,
        IsCompAdmin = @isCompAdmin,
        ModifiedBy = @modifiedBy,
        ModifiedOn = GETDATE()
    WHERE
        CompID = @compID
        AND UID = @UID
END
GO

-- =============================================
-- 3. Update usp_UserSignUp to include isCompAdmin
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[mcDCR].[usp_UserSignUp]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [mcDCR].[usp_UserSignUp]
GO

CREATE PROCEDURE [mcDCR].[usp_UserSignUp]
    @compID INT,
    @userID NVARCHAR(50),
    @password NVARCHAR(100),
    @name NVARCHAR(100),
    @mobile NVARCHAR(20),
    @headquater NVARCHAR(200),
    @createdBy INT,
    @isCompAdmin BIT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [mcDCR].[Users]
    (
        CompID, UserID, Password, Name, Mobile, HeadQuater,
        IsActive, IsCompAdmin, CreatedBy, CreatedOn
    )
    VALUES
    (
        @compID, @userID, @password, @name, @mobile, @headquater,
        1, @isCompAdmin, @createdBy, GETDATE()
    )

    -- Return the newly created user
    SELECT
        UID, CompID, UserID, Password, Name, HeadQuater,
        Address1, Locality, CityOrTown, Pincode, District, State, Country,
        Mobile, Telephone, IsActive, IsCompAdmin, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn
    FROM [mcDCR].[Users]
    WHERE CompID = @compID AND UserID = @userID
END
GO

-- =============================================
-- 4. Update usp_UserLogin to return isCompAdmin
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[mcDCR].[usp_UserLogin]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [mcDCR].[usp_UserLogin]
GO

CREATE PROCEDURE [mcDCR].[usp_UserLogin]
    @compID INT,
    @userID NVARCHAR(50),
    @password NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        UID, CompID, UserID, Password, Name, HeadQuater,
        Address1, Locality, CityOrTown, Pincode, District, State, Country,
        Mobile, Telephone, IsActive, IsCompAdmin, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn
    FROM [mcDCR].[Users]
    WHERE
        CompID = @compID
        AND UserID = @userID
        AND Password = @password
        AND IsActive = 1
END
GO

-- =============================================
-- 5. Create usp_ScreenSync - Sync screens from frontend
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[McMaster].[usp_ScreenSync]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [McMaster].[usp_ScreenSync]
GO

CREATE PROCEDURE [McMaster].[usp_ScreenSync]
    @ScreenName NVARCHAR(100),
    @ScreenRoute NVARCHAR(200),
    @ScreenDescription NVARCHAR(500),
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if screen already exists
    IF EXISTS (SELECT 1 FROM [McMaster].[ScreenMaster] WHERE ScreenRoute = @ScreenRoute)
    BEGIN
        -- Update existing screen
        UPDATE [McMaster].[ScreenMaster]
        SET
            ScreenName = @ScreenName,
            ScreenDescription = @ScreenDescription,
            IsActive = @IsActive
        WHERE ScreenRoute = @ScreenRoute
    END
    ELSE
    BEGIN
        -- Insert new screen
        INSERT INTO [McMaster].[ScreenMaster]
        (ScreenName, ScreenRoute, ScreenDescription, IsActive, CreatedDate)
        VALUES
        (@ScreenName, @ScreenRoute, @ScreenDescription, @IsActive, GETDATE())
    END
END
GO

-- =============================================
-- 6. Create usp_GetActiveScreens - Get all active screens
-- =============================================
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
    FROM [McMaster].[ScreenMaster]
    WHERE IsActive = 1
    ORDER BY ScreenName
END
GO

-- =============================================
-- 7. Create usp_SaveUserScreenPermissions - Save user screen permissions
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[McMaster].[usp_SaveUserScreenPermissions]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [McMaster].[usp_SaveUserScreenPermissions]
GO

CREATE PROCEDURE [McMaster].[usp_SaveUserScreenPermissions]
    @UserID INT,
    @ScreenIDs NVARCHAR(MAX),  -- Comma-separated list of ScreenIDs
    @CreatedBy INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION

    BEGIN TRY
        -- Delete existing permissions for this user
        DELETE FROM [McMaster].[UserScreenPermissions]
        WHERE UserID = @UserID

        -- Insert new permissions
        INSERT INTO [McMaster].[UserScreenPermissions]
        (UserID, ScreenID, HasAccess, CreatedBy, CreatedOn)
        SELECT
            @UserID,
            CAST(value AS INT) AS ScreenID,
            1,
            @CreatedBy,
            GETDATE()
        FROM STRING_SPLIT(@ScreenIDs, ',')
        WHERE RTRIM(value) <> ''

        COMMIT TRANSACTION

        SELECT 1 AS Success
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        RAISERROR(@ErrorMessage, 16, 1)
    END CATCH
END
GO

-- =============================================
-- 8. Create usp_GetUserScreenPermissions - Get user screen permissions
-- =============================================
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
        p.HasAccess,
        p.CreatedBy,
        p.CreatedOn,
        p.ModifiedBy,
        p.ModifiedOn,
        s.ScreenName,
        s.ScreenRoute,
        s.ScreenDescription
    FROM [McMaster].[UserScreenPermissions] p
    INNER JOIN [McMaster].[ScreenMaster] s ON p.ScreenID = s.ScreenID
    WHERE
        p.UserID = @UserID
        AND p.HasAccess = 1
        AND s.IsActive = 1
    ORDER BY s.ScreenName
END
GO

-- =============================================
-- 9. Update usp_UserList to include isCompAdmin
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[mcDCR].[usp_UserList]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [mcDCR].[usp_UserList]
GO

CREATE PROCEDURE [mcDCR].[usp_UserList]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        UID, CompID, UserID, Password, Name, HeadQuater,
        Address1, Locality, CityOrTown, Pincode, District, State, Country,
        Mobile, Telephone, IsActive, IsCompAdmin, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn
    FROM [mcDCR].[Users]
    WHERE IsActive = 1
    ORDER BY Name
END
GO

-- =============================================
-- 10. Update usp_UserListByCompany to include isCompAdmin
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[mcDCR].[usp_UserListByCompany]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [mcDCR].[usp_UserListByCompany]
GO

CREATE PROCEDURE [mcDCR].[usp_UserListByCompany]
    @compID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        UID, CompID, UserID, Password, Name, HeadQuater,
        Address1, Locality, CityOrTown, Pincode, District, State, Country,
        Mobile, Telephone, IsActive, IsCompAdmin, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn
    FROM [mcDCR].[Users]
    WHERE CompID = @compID AND IsActive = 1
    ORDER BY Name
END
GO

PRINT 'All stored procedures created successfully!'
GO
