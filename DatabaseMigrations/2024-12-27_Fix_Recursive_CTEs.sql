-- =============================================
-- FIX: Recursive CTE Stored Procedures
-- Date: 2024-12-27
-- Issue: LEFT JOIN not allowed in recursive part of CTE
-- Solution: Move JOINs outside the CTE
-- =============================================

-- =============================================
-- 1. FIX usp_GetAllSubordinates
-- =============================================
CREATE OR ALTER PROCEDURE [mcDCR].[usp_GetAllSubordinates]
    @compID INT,
    @managerUID INT,
    @directOnly BIT = 0  -- 0 = all levels, 1 = direct reports only
AS
BEGIN
    SET NOCOUNT ON;

    -- First, get the hierarchy using only INNER JOINs in recursive part
    ;WITH SubordinateHierarchy AS (
        -- Base case: direct reports (no JOINs needed here, just get UIDs)
        SELECT
            UID,
            ReportingManagerID,
            1 AS Level
        FROM [mcMaster].[Users]
        WHERE CompID = @compID
          AND ReportingManagerID = @managerUID
          AND IsActive = 1

        UNION ALL

        -- Recursive case: indirect reports (only if @directOnly = 0)
        -- No LEFT JOINs allowed here - just get the hierarchy
        SELECT
            u.UID,
            u.ReportingManagerID,
            sh.Level + 1
        FROM [mcMaster].[Users] u
        INNER JOIN SubordinateHierarchy sh ON u.ReportingManagerID = sh.UID
        WHERE u.CompID = @compID
          AND u.IsActive = 1
          AND @directOnly = 0
    )
    -- Now JOIN to get additional info OUTSIDE the CTE
    SELECT
        sh.UID,
        u.Name,
        u.HeadQuater,
        u.RoleID,
        l.description AS RoleName,
        u.ReportingManagerID,
        m.Name AS ReportingManagerName,
        sh.Level
    FROM SubordinateHierarchy sh
    INNER JOIN [mcMaster].[Users] u ON sh.UID = u.UID
    LEFT JOIN [mcMaster].[Lookup] l ON u.RoleID = l.code AND l.type = 'EMPLOYEE_ROLE' AND l.CompID = @compID
    LEFT JOIN [mcMaster].[Users] m ON u.ReportingManagerID = m.UID AND m.CompID = @compID
    ORDER BY sh.Level, u.Name
END
GO

-- =============================================
-- 2. FIX usp_GetReportingChain
-- =============================================
CREATE OR ALTER PROCEDURE [mcDCR].[usp_GetReportingChain]
    @compID INT,
    @userUID INT
AS
BEGIN
    SET NOCOUNT ON;

    -- First, get the upward chain using only necessary JOINs
    ;WITH ReportingChain AS (
        -- Base case: immediate manager
        SELECT
            m.UID,
            m.ReportingManagerID,
            1 AS Level
        FROM [mcMaster].[Users] u
        INNER JOIN [mcMaster].[Users] m ON u.ReportingManagerID = m.UID AND m.CompID = u.CompID
        WHERE u.CompID = @compID
          AND u.UID = @userUID
          AND u.IsActive = 1
          AND m.IsActive = 1

        UNION ALL

        -- Recursive case: manager's manager (no LEFT JOINs)
        SELECT
            m.UID,
            m.ReportingManagerID,
            rc.Level + 1
        FROM ReportingChain rc
        INNER JOIN [mcMaster].[Users] m ON rc.ReportingManagerID = m.UID
        WHERE m.CompID = @compID AND m.IsActive = 1
    )
    -- Now JOIN to get additional info OUTSIDE the CTE
    SELECT
        rc.UID,
        u.Name,
        u.HeadQuater,
        u.RoleID,
        l.description AS RoleName,
        u.ReportingManagerID,
        rc.Level
    FROM ReportingChain rc
    INNER JOIN [mcMaster].[Users] u ON rc.UID = u.UID
    LEFT JOIN [mcMaster].[Lookup] l ON u.RoleID = l.code AND l.type = 'EMPLOYEE_ROLE' AND l.CompID = @compID
    ORDER BY rc.Level
END
GO

PRINT 'Recursive CTE fixes applied successfully!'
