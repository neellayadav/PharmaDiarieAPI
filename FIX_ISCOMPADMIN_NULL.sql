-- ========================================================================================================================================
-- FIX: isCompAdmin returning NULL issue
-- Problem: Column name case mismatch between database (isCompAdmin) and C# model (IsCompAdmin)
-- Solution: Add column alias in stored procedure to match C# property name
-- ========================================================================================================================================

-- Check current stored procedure definition
PRINT 'Current usp_UserListByCompany definition:'
PRINT '==========================================';
SELECT OBJECT_DEFINITION(OBJECT_ID('[mcDCR].[usp_UserListByCompany]'));
GO

-- Fix: Update the stored procedure to use proper column alias
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
      ,[isCompAdmin] AS [IsCompAdmin]  -- Added alias to match C# property name
FROM [mcMaster].[Users]
	WHERE [CompID] = @compid
           AND IsActive = cast(1 as bit)
           AND UID != 100

END
GO

PRINT 'Updated stored procedure with column alias';
PRINT 'Testing the fix...';
GO

-- Test the updated procedure
EXEC [mcDCR].[usp_UserListByCompany] @compid = 0;
GO

PRINT 'Fix applied successfully!';
PRINT 'Restart your API and test again.';
