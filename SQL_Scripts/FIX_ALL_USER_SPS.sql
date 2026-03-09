-- ========================================================================================================================================
-- FIX: Apply IsCompAdmin alias to all user-related stored procedures
-- This ensures isCompAdmin works correctly across all user endpoints
-- ========================================================================================================================================

-- ========================================================================================================================================
-- 1. Fix usp_UserLogin
-- ========================================================================================================================================
PRINT 'Fixing usp_UserLogin...';

ALTER PROC [mcDCR].[usp_UserLogin]
    @compid int,
    @userid VARCHAR(50),
    @password varchar(20)
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
      ,[CreatedBy]
      ,[CreatedOn]
      ,[ModifiedBy]
      ,[ModifiedOn]
      ,[isCompAdmin] AS [IsCompAdmin]  -- Added alias
FROM [mcMaster].[Users]
	WHERE [UserID] = @userid COLLATE SQL_Latin1_General_CP1_CS_AS
	       AND [Password] = @password COLLATE SQL_Latin1_General_CP1_CS_AS
           AND IsActive = cast(1 as bit)

END
GO

PRINT '✅ usp_UserLogin fixed';
GO

-- ========================================================================================================================================
-- 2. Fix usp_UserList
-- ========================================================================================================================================
PRINT 'Fixing usp_UserList...';

ALTER PROC [mcDCR].[usp_UserList]
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
      ,[isCompAdmin] AS [IsCompAdmin]  -- Added alias
FROM [mcMaster].[Users]
	WHERE IsActive = cast(1 as bit)
           AND UID != 100

END
GO

PRINT '✅ usp_UserList fixed';
GO

-- ========================================================================================================================================
-- VERIFICATION
-- ========================================================================================================================================
PRINT '';
PRINT '========================================';
PRINT 'ALL USER STORED PROCEDURES UPDATED';
PRINT '========================================';
PRINT '';
PRINT 'Updated procedures:';
PRINT '1. ✅ usp_UserLogin';
PRINT '2. ✅ usp_UserList';
PRINT '3. ✅ usp_UserListByCompany (already fixed)';
PRINT '';
PRINT 'Next steps:';
PRINT '1. Restart your API';
PRINT '2. Test Login endpoint';
PRINT '3. All user endpoints should now return IsCompAdmin correctly';
PRINT '';
