-- ========================================================================================================================================
-- Database Verification Script
-- Run this to verify all database changes were applied correctly
-- ========================================================================================================================================

PRINT '========================================='
PRINT 'DATABASE VERIFICATION SCRIPT'
PRINT '========================================='
PRINT ''

-- ========================================================================================================================================
-- 1. Verify Tables Exist
-- ========================================================================================================================================
PRINT '1. Verifying Tables...'
PRINT '----------------------'

SELECT
    TABLE_SCHEMA,
    TABLE_NAME,
    'EXISTS' AS Status
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME IN ('Users', 'ScreenMaster', 'UserScreenPermissions')
ORDER BY TABLE_NAME;

PRINT ''

-- ========================================================================================================================================
-- 2. Verify isCompAdmin column exists in Users table
-- ========================================================================================================================================
PRINT '2. Verifying isCompAdmin column in Users table...'
PRINT '---------------------------------------------------'

SELECT
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'McMaster'
  AND TABLE_NAME = 'Users'
  AND COLUMN_NAME = 'isCompAdmin';

IF @@ROWCOUNT = 0
BEGIN
    PRINT '❌ ERROR: isCompAdmin column NOT found in Users table!'
END
ELSE
BEGIN
    PRINT '✅ SUCCESS: isCompAdmin column exists'
END

PRINT ''

-- ========================================================================================================================================
-- 3. Verify Stored Procedures Exist
-- ========================================================================================================================================
PRINT '3. Verifying Stored Procedures...'
PRINT '----------------------------------'

SELECT
    ROUTINE_SCHEMA AS [Schema],
    ROUTINE_NAME AS [Procedure Name],
    CREATED AS [Created Date],
    LAST_ALTERED AS [Last Modified]
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_TYPE = 'PROCEDURE'
  AND (
      ROUTINE_NAME IN (
          'usp_UserListByCompany',
          'usp_UserLogin',
          'usp_ScreenSync',
          'usp_GetActiveScreens',
          'usp_SaveUserScreenPermissions',
          'usp_GetUserScreenPermissions'
      )
  )
ORDER BY ROUTINE_NAME;

DECLARE @ExpectedCount INT = 6;
DECLARE @ActualCount INT;

SELECT @ActualCount = COUNT(*)
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_TYPE = 'PROCEDURE'
  AND ROUTINE_NAME IN (
      'usp_UserListByCompany',
      'usp_UserLogin',
      'usp_ScreenSync',
      'usp_GetActiveScreens',
      'usp_SaveUserScreenPermissions',
      'usp_GetUserScreenPermissions'
  );

PRINT ''
IF @ActualCount = @ExpectedCount
BEGIN
    PRINT '✅ SUCCESS: All ' + CAST(@ExpectedCount AS VARCHAR) + ' stored procedures exist'
END
ELSE
BEGIN
    PRINT '❌ ERROR: Expected ' + CAST(@ExpectedCount AS VARCHAR) + ' procedures, found ' + CAST(@ActualCount AS VARCHAR)
END

PRINT ''

-- ========================================================================================================================================
-- 4. Verify usp_UserListByCompany includes isCompAdmin
-- ========================================================================================================================================
PRINT '4. Verifying usp_UserListByCompany includes isCompAdmin...'
PRINT '------------------------------------------------------------'

DECLARE @ProcDefinition NVARCHAR(MAX);
SELECT @ProcDefinition = OBJECT_DEFINITION(OBJECT_ID('[mcDCR].[usp_UserListByCompany]'));

IF @ProcDefinition LIKE '%isCompAdmin%'
BEGIN
    PRINT '✅ SUCCESS: usp_UserListByCompany includes isCompAdmin column'
END
ELSE
BEGIN
    PRINT '❌ ERROR: usp_UserListByCompany does NOT include isCompAdmin column'
    PRINT 'Run URGENT_FIX_SCRIPT.sql to fix this issue'
END

PRINT ''

-- ========================================================================================================================================
-- 5. Check Users table data
-- ========================================================================================================================================
PRINT '5. Checking Users table data...'
PRINT '--------------------------------'

SELECT
    COUNT(*) AS [Total Users],
    SUM(CASE WHEN isCompAdmin = 1 THEN 1 ELSE 0 END) AS [Admin Users],
    SUM(CASE WHEN isCompAdmin = 0 THEN 1 ELSE 0 END) AS [Regular Users],
    SUM(CASE WHEN isCompAdmin IS NULL THEN 1 ELSE 0 END) AS [NULL Values (Need Fix)]
FROM McMaster.Users;

PRINT ''

-- If there are NULL values, show them
DECLARE @NullCount INT;
SELECT @NullCount = COUNT(*) FROM McMaster.Users WHERE isCompAdmin IS NULL;

IF @NullCount > 0
BEGIN
    PRINT '⚠️  WARNING: Found ' + CAST(@NullCount AS VARCHAR) + ' users with NULL isCompAdmin'
    PRINT 'Showing first 10 users with NULL values:'
    PRINT ''

    SELECT TOP 10
        UID,
        UserID,
        Name,
        isCompAdmin
    FROM McMaster.Users
    WHERE isCompAdmin IS NULL;

    PRINT ''
    PRINT 'To fix NULL values, run:'
    PRINT 'UPDATE McMaster.Users SET isCompAdmin = 0 WHERE isCompAdmin IS NULL;'
END
ELSE
BEGIN
    PRINT '✅ SUCCESS: No NULL values in isCompAdmin column'
END

PRINT ''

-- ========================================================================================================================================
-- 6. Check ScreenMaster table
-- ========================================================================================================================================
PRINT '6. Checking ScreenMaster table...'
PRINT '-----------------------------------'

SELECT
    COUNT(*) AS [Total Screens],
    SUM(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END) AS [Active Screens],
    SUM(CASE WHEN IsActive = 0 THEN 1 ELSE 0 END) AS [Inactive Screens]
FROM McMaster.ScreenMaster;

PRINT ''

-- ========================================================================================================================================
-- 7. Check UserScreenPermissions table
-- ========================================================================================================================================
PRINT '7. Checking UserScreenPermissions table...'
PRINT '-------------------------------------------'

SELECT
    COUNT(*) AS [Total Permission Records],
    COUNT(DISTINCT UserID) AS [Users with Permissions],
    COUNT(DISTINCT ScreenID) AS [Screens Assigned]
FROM McMaster.UserScreenPermissions;

PRINT ''

-- ========================================================================================================================================
-- FINAL SUMMARY
-- ========================================================================================================================================
PRINT '========================================='
PRINT 'VERIFICATION COMPLETE'
PRINT '========================================='
PRINT ''
PRINT 'Review the results above:'
PRINT '1. All tables should exist'
PRINT '2. isCompAdmin column should exist'
PRINT '3. All 6 stored procedures should exist'
PRINT '4. usp_UserListByCompany should include isCompAdmin'
PRINT '5. Check for NULL values in isCompAdmin (fix if needed)'
PRINT '6. ScreenMaster and UserScreenPermissions tables should exist'
PRINT ''
PRINT 'If everything looks good, proceed to test the API endpoints.'
PRINT 'If issues found, run URGENT_FIX_SCRIPT.sql'
PRINT ''
