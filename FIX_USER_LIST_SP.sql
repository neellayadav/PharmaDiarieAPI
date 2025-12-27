-- ========================================================================================================================================
-- FIX: usp_UserList - Add IsCompAdmin alias
-- ========================================================================================================================================

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
      ,[isCompAdmin] AS [IsCompAdmin]
FROM [mcMaster].[Users]
	WHERE IsActive = cast(1 as bit)
           AND UID != 100

END
