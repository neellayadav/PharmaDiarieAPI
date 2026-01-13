-- =====================================================
-- Role Hierarchy Feature - Database Migration Script
-- Date: 2025-01-05
-- Description: Creates RoleHierarchy table and related SPs
--              for managing role rankings across the application
-- =====================================================

-- =====================================================
-- STEP 1: Create RoleHierarchy Table
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RoleHierarchy' AND schema_id = SCHEMA_ID('mcMaster'))
BEGIN
    CREATE TABLE [mcMaster].[RoleHierarchy] (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        RoleName NVARCHAR(100) NOT NULL,
        RankLevel INT NOT NULL,
        IsActive BIT DEFAULT 1,
        CreatedBy INT,
        CreatedOn DATETIME DEFAULT GETDATE(),
        ModifiedBy INT,
        ModifiedOn DATETIME,
        CONSTRAINT UQ_RoleHierarchy_RoleName UNIQUE (RoleName)
    );
    PRINT 'Table [mcMaster].[RoleHierarchy] created successfully.';
END
ELSE
BEGIN
    PRINT 'Table [mcMaster].[RoleHierarchy] already exists.';
END
GO

-- =====================================================
-- STEP 2: SP to Get Distinct Roles from Lookup Table
-- Returns all unique role names from EMPLOYEE_ROLE lookup
-- =====================================================
 CREATE OR ALTER PROCEDURE [mcMaster].[usp_GetDistinctRoles]
  AS
  BEGIN
      SET NOCOUNT ON;

      ;WITH DistinctRoles AS (
          SELECT DISTINCT
              l.description AS RoleName,
              rh.RankLevel,
              rh.Id AS HierarchyId
          FROM [mcMaster].[Lookup] l
          LEFT JOIN [mcMaster].[RoleHierarchy] rh ON l.description = rh.RoleName AND rh.IsActive = 1
          WHERE l.type = 'EMPLOYEE_ROLE'
            AND l.IsActive = 1
            AND l.description IS NOT NULL
            AND LEN(LTRIM(RTRIM(l.description))) > 0
      )
      SELECT RoleName, RankLevel, HierarchyId
      FROM DistinctRoles
      ORDER BY
          CASE WHEN RankLevel IS NULL THEN 1 ELSE 0 END,
          RankLevel,
          RoleName;
  END
  GO
PRINT 'Stored Procedure [mcMaster].[usp_GetDistinctRoles] created/updated.';
GO

-- =====================================================
-- STEP 3: SP to Get Role Hierarchy List
-- Returns all ranked roles ordered by rank level
-- =====================================================
CREATE OR ALTER PROCEDURE [mcMaster].[usp_RoleHierarchyList]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        RoleName,
        RankLevel,
        IsActive,
        CreatedBy,
        CreatedOn,
        ModifiedBy,
        ModifiedOn
    FROM [mcMaster].[RoleHierarchy]
    WHERE IsActive = 1
    ORDER BY RankLevel, RoleName;
END
GO
PRINT 'Stored Procedure [mcMaster].[usp_RoleHierarchyList] created/updated.';
GO

-- =====================================================
-- STEP 4: SP to Save/Update Role Hierarchy
-- Inserts new or updates existing role ranking
-- =====================================================
CREATE OR ALTER PROCEDURE [mcMaster].[usp_RoleHierarchySave]
    @RoleName NVARCHAR(100),
    @RankLevel INT,
    @ModifiedBy INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [mcMaster].[RoleHierarchy] WHERE RoleName = @RoleName)
    BEGIN
        -- Update existing
        UPDATE [mcMaster].[RoleHierarchy]
        SET RankLevel = @RankLevel,
            ModifiedBy = @ModifiedBy,
            ModifiedOn = GETDATE()
        WHERE RoleName = @RoleName;

        SELECT 'Updated' AS Result, Id, RoleName, RankLevel
        FROM [mcMaster].[RoleHierarchy]
        WHERE RoleName = @RoleName;
    END
    ELSE
    BEGIN
        -- Insert new
        INSERT INTO [mcMaster].[RoleHierarchy] (RoleName, RankLevel, IsActive, CreatedBy, CreatedOn)
        VALUES (@RoleName, @RankLevel, 1, @ModifiedBy, GETDATE());

        SELECT 'Inserted' AS Result, Id, RoleName, RankLevel
        FROM [mcMaster].[RoleHierarchy]
        WHERE RoleName = @RoleName;
    END
END
GO
PRINT 'Stored Procedure [mcMaster].[usp_RoleHierarchySave] created/updated.';
GO

-- =====================================================
-- STEP 5: SP to Save Multiple Role Rankings (Batch)
-- Accepts a list of roles with rankings
-- =====================================================
CREATE OR ALTER PROCEDURE [mcMaster].[usp_RoleHierarchySaveBatch]
    @RoleRankings NVARCHAR(MAX),  -- JSON format: [{"RoleName":"CEO","RankLevel":1},...]
    @ModifiedBy INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Parse JSON and upsert each role
    MERGE [mcMaster].[RoleHierarchy] AS target
    USING (
        SELECT
            JSON_VALUE(value, '$.RoleName') AS RoleName,
            CAST(JSON_VALUE(value, '$.RankLevel') AS INT) AS RankLevel
        FROM OPENJSON(@RoleRankings)
        WHERE JSON_VALUE(value, '$.RoleName') IS NOT NULL
          AND JSON_VALUE(value, '$.RankLevel') IS NOT NULL
    ) AS source
    ON target.RoleName = source.RoleName
    WHEN MATCHED THEN
        UPDATE SET
            RankLevel = source.RankLevel,
            ModifiedBy = @ModifiedBy,
            ModifiedOn = GETDATE()
    WHEN NOT MATCHED THEN
        INSERT (RoleName, RankLevel, IsActive, CreatedBy, CreatedOn)
        VALUES (source.RoleName, source.RankLevel, 1, @ModifiedBy, GETDATE());

    -- Return updated list
    SELECT Id, RoleName, RankLevel, IsActive
    FROM [mcMaster].[RoleHierarchy]
    WHERE IsActive = 1
    ORDER BY RankLevel;
END
GO
PRINT 'Stored Procedure [mcMaster].[usp_RoleHierarchySaveBatch] created/updated.';
GO

-- =====================================================
-- STEP 6: Modify GetPotentialManagers to filter by Rank
-- Adds roleId parameter and filters by hierarchy
-- =====================================================
CREATE OR ALTER PROCEDURE [mcDCR].[usp_GetPotentialManagers]
    @compID INT,
    @currentUID INT = NULL,
    @currentRoleID INT = NULL  -- NEW: Role ID of the employee being edited
AS
BEGIN
    SET NOCOUNT ON;

    -- Get current employee's RankLevel based on their role
    DECLARE @currentRankLevel INT = NULL;

    IF @currentRoleID IS NOT NULL
    BEGIN
        SELECT @currentRankLevel = rh.RankLevel
        FROM [mcMaster].[Lookup] l
        INNER JOIN [mcMaster].[RoleHierarchy] rh ON l.description = rh.RoleName AND rh.IsActive = 1
        WHERE l.CompID = @compID
          AND l.code = @currentRoleID
          AND l.type = 'EMPLOYEE_ROLE'
          AND l.IsActive = 1;
    END

    -- Get all subordinates of current user to exclude them (prevents circular references)
    ;WITH Subordinates AS (
        -- Direct reports
        SELECT UID
        FROM [mcMaster].[Users]
        WHERE CompID = @compID
          AND ReportingManagerID = @currentUID
          AND IsActive = 1
          AND @currentUID IS NOT NULL

        UNION ALL

        -- Recursive: subordinates of subordinates
        SELECT u.UID
        FROM [mcMaster].[Users] u
        INNER JOIN Subordinates s ON u.ReportingManagerID = s.UID
        WHERE u.CompID = @compID
          AND u.IsActive = 1
    )
    SELECT
        u.UID,
        u.Name,
        u.HeadQuater,
        l.description AS RoleName,
        rh.RankLevel
    FROM [mcMaster].[Users] u
    LEFT JOIN [mcMaster].[Lookup] l ON u.RoleID = l.code
        AND l.type = 'EMPLOYEE_ROLE'
        AND l.CompID = u.CompID
        AND l.IsActive = 1
    LEFT JOIN [mcMaster].[RoleHierarchy] rh ON l.description = rh.RoleName
        AND rh.IsActive = 1
    WHERE u.CompID = @compID
      AND u.IsActive = 1
      AND u.UID != ISNULL(@currentUID, 0)  -- Exclude self
      AND u.UID NOT IN (SELECT UID FROM Subordinates)  -- Exclude subordinates
      -- NEW: Filter by rank - only show users with higher authority (lower RankLevel)
      AND (
          @currentRankLevel IS NULL  -- No rank defined, show all
          OR rh.RankLevel IS NULL    -- Manager has no rank, show them
          OR rh.RankLevel < @currentRankLevel  -- Manager has higher authority
      )
    ORDER BY
        ISNULL(rh.RankLevel, 999),  -- Ranked users first
        u.Name;
END
GO
PRINT 'Stored Procedure [mcDCR].[usp_GetPotentialManagers] modified to support role hierarchy.';
GO

-- =====================================================
-- STEP 7: SP to Get Role Rank by Role ID (Helper)
-- =====================================================
CREATE OR ALTER PROCEDURE [mcMaster].[usp_GetRoleRank]
    @CompID INT,
    @RoleID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        l.code AS RoleID,
        l.description AS RoleName,
        ISNULL(rh.RankLevel, 999) AS RankLevel
    FROM [mcMaster].[Lookup] l
    LEFT JOIN [mcMaster].[RoleHierarchy] rh ON l.description = rh.RoleName AND rh.IsActive = 1
    WHERE l.CompID = @CompID
      AND l.code = @RoleID
      AND l.type = 'EMPLOYEE_ROLE'
      AND l.IsActive = 1;
END
GO
PRINT 'Stored Procedure [mcMaster].[usp_GetRoleRank] created/updated.';
GO

PRINT '';
PRINT '=====================================================';
PRINT 'Role Hierarchy Feature - Migration Complete!';
PRINT '=====================================================';
PRINT '';
PRINT 'Summary:';
PRINT '  - Table: [mcMaster].[RoleHierarchy]';
PRINT '  - SP: [mcMaster].[usp_GetDistinctRoles]';
PRINT '  - SP: [mcMaster].[usp_RoleHierarchyList]';
PRINT '  - SP: [mcMaster].[usp_RoleHierarchySave]';
PRINT '  - SP: [mcMaster].[usp_RoleHierarchySaveBatch]';
PRINT '  - SP: [mcMaster].[usp_GetRoleRank]';
PRINT '  - SP: [mcDCR].[usp_GetPotentialManagers] (Modified)';
PRINT '';
GO
