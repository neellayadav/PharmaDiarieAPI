-- =============================================
-- Reporting Manager Feature - Database Migration
-- Date: 2024-12-27
--
-- Changes:
--   1. ALTER Users table - Add RoleID and ReportingManagerID columns
--   2. ALTER usp_UserInsert - Include new columns
--   3. ALTER usp_UserUpdate - Include new columns
--   4. ALTER usp_UserListByCompany - Return role and manager info with names
--   5. CREATE usp_GetPotentialManagers - Get list of potential managers for dropdown
--   6. CREATE usp_GetAllSubordinates - Get all subordinates for a manager
--   7. CREATE usp_GetReportingChain - Get upward reporting chain
-- =============================================

-- =============================================
-- 1. ALTER Users table (if columns don't exist)
-- =============================================
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('mcMaster.Users') AND name = 'RoleID')
BEGIN
    ALTER TABLE mcMaster.Users ADD RoleID INT NULL
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('mcMaster.Users') AND name = 'ReportingManagerID')
BEGIN
    ALTER TABLE mcMaster.Users ADD ReportingManagerID INT NULL
END
GO

-- =============================================
-- 2. ALTER usp_UserInsert
-- =============================================
ALTER PROCEDURE [mcDCR].[usp_UserInsert]
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
    @roleID INT = NULL,
    @reportingManagerID INT = NULL,
    @createdBy INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [mcMaster].[Users]
    (
        CompID, UserID, Password, Name, HeadQuater, Address1, Locality,
        CityOrTown, Pincode, District, State, Country, Mobile, Telephone,
        ProfileImageURL, IsActive, IsCompAdmin, RoleID, ReportingManagerID,
        CreatedBy, CreatedOn
    )
    VALUES
    (
        @compID, @userID, @password, @name, @headquater, @address1, @locality,
        @cityOrTown, @pincode, @district, @state, @country, @mobile, @telephone,
        @ProfileImageURL, 1, @isCompAdmin, @roleID, @reportingManagerID,
        @createdBy, SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')
    )
END
GO

-- =============================================
-- 3. ALTER usp_UserUpdate
-- =============================================
ALTER PROCEDURE [mcDCR].[usp_UserUpdate]
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
    @roleID INT = NULL,
    @reportingManagerID INT = NULL,
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
        RoleID = @roleID,
        ReportingManagerID = @reportingManagerID,
        ModifiedBy = @modifiedBy,
        ModifiedOn = SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')
    WHERE
        CompID = @compID
        AND UID = @UID
END
GO

-- =============================================
-- 4. ALTER usp_UserListByCompany - Include role and manager info
-- =============================================
ALTER PROCEDURE [mcDCR].[usp_UserListByCompany]
    @compID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.UID,
        u.CompID,
        u.UserID,
        u.Password,
        u.Name,
        u.HeadQuater,
        u.Address1,
        u.Locality,
        u.CityOrTown,
        u.Pincode,
        u.District,
        u.State,
        u.Country,
        u.Mobile,
        u.Telephone,
        u.ProfileImageURL,
        u.IsActive,
        u.IsCompAdmin,
        u.RoleID,
        l.description AS RoleName,
        u.ReportingManagerID,
        m.Name AS ReportingManagerName,
        u.CreatedBy,
        u.CreatedOn,
        u.ModifiedBy,
        u.ModifiedOn
    FROM [mcMaster].[Users] u
    LEFT JOIN [mcMaster].[Lookup] l ON u.RoleID = l.code AND l.type = 'EMPLOYEE_ROLE' AND l.CompID = u.CompID
    LEFT JOIN [mcMaster].[Users] m ON u.ReportingManagerID = m.UID AND m.CompID = u.CompID
    WHERE u.CompID = @compID AND u.IsActive = 1
    ORDER BY u.Name
END
GO

-- =============================================
-- 5. CREATE usp_GetPotentialManagers
-- Returns users who can be reporting managers for a given user
-- Excludes: self, own subordinates (to prevent circular references)
-- =============================================
CREATE OR ALTER PROCEDURE [mcDCR].[usp_GetPotentialManagers]
    @compID INT,
    @currentUID INT = NULL  -- NULL for new employee, UID for editing existing
AS
BEGIN
    SET NOCOUNT ON;

    -- Get all subordinates of current user (if editing) to exclude them
    ;WITH Subordinates AS (
        -- Base case: direct reports
        SELECT UID
        FROM [mcMaster].[Users]
        WHERE CompID = @compID
          AND ReportingManagerID = @currentUID
          AND IsActive = 1
          AND @currentUID IS NOT NULL

        UNION ALL

        -- Recursive case: reports of reports
        SELECT u.UID
        FROM [mcMaster].[Users] u
        INNER JOIN Subordinates s ON u.ReportingManagerID = s.UID
        WHERE u.CompID = @compID AND u.IsActive = 1
    )
    SELECT
        u.UID,
        u.Name,
        u.HeadQuater,
        l.description AS RoleName
    FROM [mcMaster].[Users] u
    LEFT JOIN [mcMaster].[Lookup] l ON u.RoleID = l.code AND l.type = 'EMPLOYEE_ROLE' AND l.CompID = u.CompID
    WHERE u.CompID = @compID
      AND u.IsActive = 1
      AND u.UID != ISNULL(@currentUID, 0)  -- Exclude self
      AND u.UID NOT IN (SELECT UID FROM Subordinates)  -- Exclude subordinates
    ORDER BY u.Name
END
GO

-- =============================================
-- 6. CREATE usp_GetAllSubordinates
-- Returns all subordinates (direct and indirect) for a manager
-- =============================================
CREATE OR ALTER PROCEDURE [mcDCR].[usp_GetAllSubordinates]
    @compID INT,
    @managerUID INT,
    @directOnly BIT = 0  -- 0 = all levels, 1 = direct reports only
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH SubordinateHierarchy AS (
        -- Base case: direct reports
        SELECT
            u.UID,
            u.Name,
            u.HeadQuater,
            u.RoleID,
            l.description AS RoleName,
            u.ReportingManagerID,
            m.Name AS ReportingManagerName,
            1 AS Level
        FROM [mcMaster].[Users] u
        LEFT JOIN [mcMaster].[Lookup] l ON u.RoleID = l.code AND l.type = 'EMPLOYEE_ROLE' AND l.CompID = u.CompID
        LEFT JOIN [mcMaster].[Users] m ON u.ReportingManagerID = m.UID AND m.CompID = u.CompID
        WHERE u.CompID = @compID
          AND u.ReportingManagerID = @managerUID
          AND u.IsActive = 1

        UNION ALL

        -- Recursive case: indirect reports (only if @directOnly = 0)
        SELECT
            u.UID,
            u.Name,
            u.HeadQuater,
            u.RoleID,
            l.description AS RoleName,
            u.ReportingManagerID,
            m.Name AS ReportingManagerName,
            sh.Level + 1
        FROM [mcMaster].[Users] u
        INNER JOIN SubordinateHierarchy sh ON u.ReportingManagerID = sh.UID
        LEFT JOIN [mcMaster].[Lookup] l ON u.RoleID = l.code AND l.type = 'EMPLOYEE_ROLE' AND l.CompID = u.CompID
        LEFT JOIN [mcMaster].[Users] m ON u.ReportingManagerID = m.UID AND m.CompID = u.CompID
        WHERE u.CompID = @compID
          AND u.IsActive = 1
          AND @directOnly = 0
    )
    SELECT * FROM SubordinateHierarchy
    ORDER BY Level, Name
END
GO

-- =============================================
-- 7. CREATE usp_GetReportingChain
-- Returns upward reporting chain (who do I report to, all the way up)
-- =============================================
CREATE OR ALTER PROCEDURE [mcDCR].[usp_GetReportingChain]
    @compID INT,
    @userUID INT
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH ReportingChain AS (
        -- Base case: immediate manager
        SELECT
            m.UID,
            m.Name,
            m.HeadQuater,
            m.RoleID,
            l.description AS RoleName,
            m.ReportingManagerID,
            1 AS Level
        FROM [mcMaster].[Users] u
        INNER JOIN [mcMaster].[Users] m ON u.ReportingManagerID = m.UID AND m.CompID = u.CompID
        LEFT JOIN [mcMaster].[Lookup] l ON m.RoleID = l.code AND l.type = 'EMPLOYEE_ROLE' AND l.CompID = m.CompID
        WHERE u.CompID = @compID
          AND u.UID = @userUID
          AND u.IsActive = 1
          AND m.IsActive = 1

        UNION ALL

        -- Recursive case: manager's manager
        SELECT
            m.UID,
            m.Name,
            m.HeadQuater,
            m.RoleID,
            l.description AS RoleName,
            m.ReportingManagerID,
            rc.Level + 1
        FROM ReportingChain rc
        INNER JOIN [mcMaster].[Users] m ON rc.ReportingManagerID = m.UID
        LEFT JOIN [mcMaster].[Lookup] l ON m.RoleID = l.code AND l.type = 'EMPLOYEE_ROLE' AND l.CompID = m.CompID
        WHERE m.CompID = @compID AND m.IsActive = 1
    )
    SELECT * FROM ReportingChain
    ORDER BY Level
END
GO

-- =============================================
-- 8. CREATE usp_GetTeamSummary
-- Returns count of direct reports and total team size
-- =============================================
CREATE OR ALTER PROCEDURE [mcDCR].[usp_GetTeamSummary]
    @compID INT,
    @managerUID INT
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH AllSubordinates AS (
        SELECT UID, 1 AS Level
        FROM [mcMaster].[Users]
        WHERE CompID = @compID
          AND ReportingManagerID = @managerUID
          AND IsActive = 1

        UNION ALL

        SELECT u.UID, a.Level + 1
        FROM [mcMaster].[Users] u
        INNER JOIN AllSubordinates a ON u.ReportingManagerID = a.UID
        WHERE u.CompID = @compID AND u.IsActive = 1
    )
    SELECT
        (SELECT COUNT(*) FROM AllSubordinates WHERE Level = 1) AS DirectReports,
        (SELECT COUNT(*) FROM AllSubordinates) AS TotalTeamSize
END
GO

PRINT 'Reporting Manager Feature migration completed successfully!'
