-- ========================================================================================================================================
-- REMOVE DASHBOARD FROM SCREEN PERMISSIONS
-- Date: 2025-11-19
-- Purpose: Dashboard should be accessible to ALL users by default, not part of permission system
-- ========================================================================================================================================

PRINT '';
PRINT '========================================';
PRINT 'REMOVING DASHBOARD FROM PERMISSIONS';
PRINT '========================================';
PRINT '';

-- Remove Dashboard from ScreenMaster table
PRINT 'Removing Dashboard screen from ScreenMaster...';
DELETE FROM McMaster.ScreenMaster
WHERE ScreenName = 'Dashboard' OR ScreenRoute = '/dashboard' OR ScreenRoute = '/Dashboard';
PRINT '✅ Dashboard removed from ScreenMaster';
GO

-- Remove any existing Dashboard permissions
PRINT 'Removing Dashboard permissions from UserScreenPermissions...';
DELETE FROM McMaster.UserScreenPermissions
WHERE ScreenID IN (
    SELECT ScreenID FROM McMaster.ScreenMaster
    WHERE ScreenName = 'Dashboard' OR ScreenRoute = '/dashboard' OR ScreenRoute = '/Dashboard'
);
PRINT '✅ Dashboard permissions cleaned up';
GO

-- ========================================================================================================================================
-- VERIFICATION
-- ========================================================================================================================================
PRINT '';
PRINT '========================================';
PRINT 'VERIFICATION';
PRINT '========================================';
PRINT '';

PRINT 'Remaining screens in ScreenMaster:';
SELECT ScreenID, ScreenName, ScreenRoute, IsActive
FROM McMaster.ScreenMaster
WHERE IsActive = 1
ORDER BY ScreenID;

PRINT '';
PRINT '========================================';
PRINT '✅ UPDATE COMPLETE';
PRINT '========================================';
PRINT '';
PRINT 'Note: Dashboard is now accessible to ALL users by default.';
PRINT 'Only the screens in ScreenMaster require permissions.';
PRINT '';
