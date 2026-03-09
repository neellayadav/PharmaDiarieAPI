-- ========================================================================================================================================
-- FIX: usp_UserLogin - Add IsCompAdmin alias
-- ========================================================================================================================================

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
      ,[isCompAdmin] AS [IsCompAdmin]
FROM [mcMaster].[Users]
	WHERE [UserID] = @userid COLLATE SQL_Latin1_General_CP1_CS_AS
	       AND [Password] = @password COLLATE SQL_Latin1_General_CP1_CS_AS
           AND IsActive = cast(1 as bit)

END
