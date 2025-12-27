-- =============================================
-- Migration: Add ProfileImageURL to User Stored Procedures
-- Date: 2024-12-17
-- Stored Procedures Schema: [mcDCR]
-- Table Schema: [mcMaster].[Users]
-- =============================================

-- =============================================
-- Step 1: Add ProfileImageURL column to Users table if not exists
-- =============================================
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[mcMaster].[Users]') AND name = 'ProfileImageURL')
BEGIN
    ALTER TABLE [mcMaster].[Users] ADD ProfileImageURL NVARCHAR(500) NULL;
    PRINT 'Added ProfileImageURL column to [mcMaster].[Users] table';
END
ELSE
BEGIN
    PRINT 'ProfileImageURL column already exists in [mcMaster].[Users] table';
END
GO

-- =============================================
-- Step 2: Update usp_UserInsert to include ProfileImageURL
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
        @ProfileImageURL, 1, @isCompAdmin, @createdBy, GETDATE()
    )
END
GO

PRINT 'Updated [mcDCR].[usp_UserInsert] with ProfileImageURL';
GO

-- =============================================
-- Step 3: Update usp_UserUpdate to include ProfileImageURL
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
        ModifiedOn = GETDATE()
    WHERE
        CompID = @compID
        AND UID = @UID
END
GO

PRINT 'Updated [mcDCR].[usp_UserUpdate] with ProfileImageURL';
GO

-- =============================================
-- Step 4: Update usp_UserList to return ProfileImageURL
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
        Mobile, Telephone, ProfileImageURL, IsActive, IsCompAdmin,
        CreatedBy, CreatedOn, ModifiedBy, ModifiedOn
    FROM [mcMaster].[Users]
    WHERE IsActive = 1
    ORDER BY Name
END
GO

PRINT 'Updated [mcDCR].[usp_UserList] with ProfileImageURL';
GO

-- =============================================
-- Step 5: Update usp_UserListByCompany to return ProfileImageURL
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
        Mobile, Telephone, ProfileImageURL, IsActive, IsCompAdmin,
        CreatedBy, CreatedOn, ModifiedBy, ModifiedOn
    FROM [mcMaster].[Users]
    WHERE CompID = @compID AND IsActive = 1
    ORDER BY Name
END
GO

PRINT 'Updated [mcDCR].[usp_UserListByCompany] with ProfileImageURL';
GO

-- =============================================
-- Step 6: Update usp_UserLogin to return ProfileImageURL
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

PRINT 'Updated [mcDCR].[usp_UserLogin] with ProfileImageURL';
GO

-- =============================================
-- Step 7: Update usp_UserBasicInsert (SignUp)
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[mcDCR].[usp_UserBasicInsert]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [mcDCR].[usp_UserBasicInsert]
GO

CREATE PROCEDURE [mcDCR].[usp_UserBasicInsert]
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
        1, 0, @createdBy, GETDATE()
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

PRINT 'Updated [mcDCR].[usp_UserBasicInsert] with ProfileImageURL';
GO

PRINT '==============================================';
PRINT 'Migration completed successfully!';
PRINT 'Stored Procedures: [mcDCR] schema';
PRINT 'Table: [mcMaster].[Users]';
PRINT '==============================================';
GO
