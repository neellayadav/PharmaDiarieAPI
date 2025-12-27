-- =============================================
-- FIX: User Stored Procedures - Correct Schema & Timestamp
-- Date: 2024-12-17
--
-- Corrections:
--   Table: [mcMaster].[Users] (NOT [mcDCR].[Users])
--   Timestamp: SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30') (NOT GETDATE())
-- =============================================

-- =============================================
-- 1. Fix usp_UserInsert
-- =============================================
CREATE OR ALTER PROCEDURE [mcDCR].[usp_UserInsert]
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
    @ProfileImageURL NVARCHAR(500) = NULL,
    @isCompAdmin BIT,
    @createdBy INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [mcMaster].[Users]
    (
        CompID, UserID, Password, Name, HeadQuater, Address1, Locality,
        CityOrTown, Pincode, District, State, Country, Mobile, Telephone,
        ProfileImageURL, IsActive, IsCompAdmin, CreatedBy, CreatedOn
    )
    VALUES
    (
        @compID, @userID, @password, @name, @headquater, @address1, @locality,
        @cityOrTown, @pincode, @district, @state, @country, @mobile, @telephone,
        @ProfileImageURL, 1, @isCompAdmin, @createdBy, SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')
    )
END
GO

PRINT 'Fixed [mcDCR].[usp_UserInsert]';
GO

-- =============================================
-- 2. Fix usp_UserUpdate
-- =============================================
CREATE OR ALTER PROCEDURE [mcDCR].[usp_UserUpdate]
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
    @ProfileImageURL NVARCHAR(500) = NULL,
    @isCompAdmin BIT,
    @modifiedBy INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [mcMaster].[Users]
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
        ProfileImageURL = @ProfileImageURL,
        IsCompAdmin = @isCompAdmin,
        ModifiedBy = @modifiedBy,
        ModifiedOn = SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')
    WHERE
        CompID = @compID
        AND UID = @UID
END
GO

PRINT 'Fixed [mcDCR].[usp_UserUpdate]';
GO

-- =============================================
-- 3. Fix usp_UserList
-- =============================================
CREATE OR ALTER PROCEDURE [mcDCR].[usp_UserList]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        UID, CompID, UserID, Password, Name, HeadQuater,
        Address1, Locality, CityOrTown, Pincode, District, State, Country,
        Mobile, Telephone, ProfileImageURL, IsActive, IsCompAdmin,
        CreatedBy, CreatedOn, ModifiedBy, ModifiedOn
    FROM [mcMaster].[Users]
    WHERE IsActive = 1
    ORDER BY Name
END
GO

PRINT 'Fixed [mcDCR].[usp_UserList]';
GO

-- =============================================
-- 4. Fix usp_UserListByCompany
-- =============================================
CREATE OR ALTER PROCEDURE [mcDCR].[usp_UserListByCompany]
    @compID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        UID, CompID, UserID, Password, Name, HeadQuater,
        Address1, Locality, CityOrTown, Pincode, District, State, Country,
        Mobile, Telephone, ProfileImageURL, IsActive, IsCompAdmin,
        CreatedBy, CreatedOn, ModifiedBy, ModifiedOn
    FROM [mcMaster].[Users]
    WHERE CompID = @compID AND IsActive = 1
    ORDER BY Name
END
GO

PRINT 'Fixed [mcDCR].[usp_UserListByCompany]';
GO

-- =============================================
-- 5. Fix usp_UserLogin
-- =============================================
CREATE OR ALTER PROCEDURE [mcDCR].[usp_UserLogin]
    @compID INT,
    @userID NVARCHAR(50),
    @password NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        UID, CompID, UserID, Password, Name, HeadQuater,
        Address1, Locality, CityOrTown, Pincode, District, State, Country,
        Mobile, Telephone, ProfileImageURL, IsActive, IsCompAdmin,
        CreatedBy, CreatedOn, ModifiedBy, ModifiedOn
    FROM [mcMaster].[Users]
    WHERE
        CompID = @compID
        AND UserID = @userID
        AND Password = @password
        AND IsActive = 1
END
GO

PRINT 'Fixed [mcDCR].[usp_UserLogin]';
GO

-- =============================================
-- 6. Fix usp_UserBasicInsert
-- =============================================
CREATE OR ALTER PROCEDURE [mcDCR].[usp_UserBasicInsert]
    @compID INT,
    @userID NVARCHAR(50),
    @password NVARCHAR(100),
    @name NVARCHAR(100),
    @mobile NVARCHAR(20),
    @headquater NVARCHAR(200),
    @createdBy INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [mcMaster].[Users]
    (
        CompID, UserID, Password, Name, Mobile, HeadQuater,
        IsActive, IsCompAdmin, CreatedBy, CreatedOn
    )
    VALUES
    (
        @compID, @userID, @password, @name, @mobile, @headquater,
        1, 0, @createdBy, SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')
    )

    SELECT
        UID, CompID, UserID, Password, Name, HeadQuater,
        Address1, Locality, CityOrTown, Pincode, District, State, Country,
        Mobile, Telephone, ProfileImageURL, IsActive, IsCompAdmin,
        CreatedBy, CreatedOn, ModifiedBy, ModifiedOn
    FROM [mcMaster].[Users]
    WHERE CompID = @compID AND UserID = @userID
END
GO

PRINT 'Fixed [mcDCR].[usp_UserBasicInsert]';
GO

-- =============================================
-- 7. Ensure ProfileImageURL column exists in [mcMaster].[Users]
-- =============================================
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[mcMaster].[Users]') AND name = 'ProfileImageURL')
BEGIN
    ALTER TABLE [mcMaster].[Users] ADD ProfileImageURL NVARCHAR(500) NULL;
    PRINT 'Added ProfileImageURL column to [mcMaster].[Users]';
END
ELSE
BEGIN
    PRINT 'ProfileImageURL column already exists in [mcMaster].[Users]';
END
GO

PRINT '==============================================';
PRINT 'All User SPs fixed successfully!';
PRINT 'Table: [mcMaster].[Users]';
PRINT 'Timestamp: SWITCHOFFSET(SYSDATETIMEOFFSET(), +05:30)';
PRINT '==============================================';
GO
