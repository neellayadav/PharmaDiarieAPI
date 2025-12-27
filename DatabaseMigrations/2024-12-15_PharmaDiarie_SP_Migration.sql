-- ============================================================================
-- PharmaDiarie Database Migration Script
-- Date: 2025-12-15
-- Description: Stored Procedure updates for Location Tracking, Products Enhancement,
--              POB (Orders), and DCR Date Approval features
-- ============================================================================

-- ============================================================================
-- SECTION 1: FIELDWORK LOCATION TRACKING
-- ============================================================================

-- 1.1 Update usp_FieldworkHDInsert to include location parameters
ALTER PROC [mcDCR].[usp_FieldworkHDInsert]
    @CompID int = NULL,
    @TransID varchar(20),
    @UID int,
    @HQcode VARCHAR(30),
    @PatchName VARCHAR(75),
    @CustId int,
    @Visited VARCHAR(20),
    @Remarks VARCHAR(210),
    @IsActive bit = 1,
    @CreatedBy int = NULL,
    @CreatedOn datetime = NULL,
    @Latitude DECIMAL(10,8) = NULL,
    @Longitude DECIMAL(11,8) = NULL,
    @LocationAccuracy FLOAT = NULL
AS
BEGIN
    INSERT INTO [mcDCR].[FieldworkHD] (
        [CompID], [TransID], [UID], [CustID], [Visited], [Remarks],
        [HQcode], [PatchName], [IsActive], [CreatedBy], [CreatedOn],
        [Latitude], [Longitude], [LocationAccuracy])
    SELECT @CompID, @TransID, @UID, @CustId, @Visited, @Remarks,
           @HQcode, @PatchName, @IsActive, @CreatedBy,
           SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30'),
           @Latitude, @Longitude, @LocationAccuracy
END
GO

-- 1.2 Update usp_FieldworkSave to include location parameters
ALTER PROC [mcDCR].[usp_FieldworkSave]
    @CompID int,
    @UID int,
    @HQcode VARCHAR(30),
    @PatchName VARCHAR(75),
    @IsActive bit,
    @CustId int,
    @Visited VARCHAR(20),
    @Remarks VARCHAR(210),
    @CreatedBy int = NULL,
    @CreatedOn datetime = NULL,
    @Latitude DECIMAL(10,8) = NULL,
    @Longitude DECIMAL(11,8) = NULL,
    @LocationAccuracy FLOAT = NULL,
    @TransID varchar(20) OUTPUT
AS
BEGIN
    SELECT @TransID = 'TR' + CAST(@CompID as varchar) + FORMAT (SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30'), 'yyMMddhhmmss')

    BEGIN
        Exec [mcDCR].[usp_FieldworkHDInsert]
            @CompID, @TransID, @UID, @HQcode, @PatchName, @CustId, @Visited, @Remarks,
            @IsActive, @CreatedBy, @CreatedOn, @Latitude, @Longitude, @LocationAccuracy
    END
END
GO

-- 1.3 Update usp_FWEmpDateWiseList to return location data
ALTER PROC [mcDCR].[usp_FWEmpDateWiseList]
    @compID int,
    @uid int,
    @dateofWork VARCHAR(15),
    @period VARCHAR(10) = 'ALL'
AS
BEGIN
    IF @period = 'DAILY'
        BEGIN
            SELECT hd.[CompID], [TransID], [UID], [HQcode], [PatchName],
                   hd.[CustID], [Name] as CustName, [Visited], [Remarks],
                   hd.[CreatedBy], hd.[CreatedOn],
                   hd.[Latitude], hd.[Longitude], hd.[LocationAccuracy]
            FROM [mcDCR].[FieldworkHD] hd
            LEFT OUTER JOIN [mcMaster].[Customers] cust ON hd.CompID = cust.CompID AND hd.CustID = cust.CustID
            WHERE hd.[CompID] = @compID AND UID = @uid
                  AND FORMAT(hd.CreatedOn, 'dd-MM-yyyy') = FORMAT(SWITCHOFFSET(SYSDATETIMEOFFSET(),'+05:30'), 'dd-MM-yyyy')
            UNION ALL
            SELECT b.[CompID], [TransID], b.[UID], a.HeadQuater as HQcode,
                   [description] as PatchName, 0 as CustID, 'N/A' as CustName,
                   'OTHER' as Visited, [Remarks], b.[CreatedBy], b.[CreatedOn],
                   NULL as Latitude, NULL as Longitude, NULL as LocationAccuracy
            FROM [mcDCR].[OtherWorkTypes] b
            INNER JOIN [mcMaster].[Users] a ON b.UID = a.UID
            INNER JOIN [mcMaster].[Lookup] ON code = b.WT_Code
            WHERE b.[CompID] = @compID AND b.UID = @uid
                  AND FORMAT(b.CreatedOn, 'dd-MM-yyyy') = FORMAT(SWITCHOFFSET(SYSDATETIMEOFFSET(),'+05:30'), 'dd-MM-yyyy')
        END
    ELSE IF @period = 'WEEKLY'
        BEGIN
            SELECT hd.[CompID], [TransID], [UID], [HQcode], [PatchName],
                   hd.[CustID], [Name] as CustName, [Visited], [Remarks],
                   hd.[CreatedBy], hd.[CreatedOn],
                   hd.[Latitude], hd.[Longitude], hd.[LocationAccuracy]
            FROM [mcDCR].[FieldworkHD] hd
            LEFT OUTER JOIN [mcMaster].[Customers] cust ON hd.CompID = cust.CompID AND hd.CustID = cust.CustID
            WHERE hd.[CompID] = @compID AND UID = @uid
                  AND DateDiff(wk,hd.CreatedOn,SWITCHOFFSET(SYSDATETIMEOFFSET(),'+05:30')) = 0
            UNION ALL
            SELECT b.[CompID], [TransID], b.[UID], a.HeadQuater as HQcode,
                   [description] as PatchName, 0 as CustID, 'N/A' as CustName,
                   'OTHER' as Visited, [Remarks], b.[CreatedBy], b.[CreatedOn],
                   NULL as Latitude, NULL as Longitude, NULL as LocationAccuracy
            FROM [mcDCR].[OtherWorkTypes] b
            INNER JOIN [mcMaster].[Users] a ON b.UID = a.UID
            INNER JOIN [mcMaster].[Lookup] ON code = b.WT_Code
            WHERE b.[CompID] = @compID AND b.UID = @uid
                  AND DateDiff(wk,b.CreatedOn,SWITCHOFFSET(SYSDATETIMEOFFSET(),'+05:30')) = 0
        END
    ELSE IF @period = 'MONTHLY'
        BEGIN
            SELECT hd.[CompID], [TransID], [UID], [HQcode], [PatchName],
                   hd.[CustID], [Name] as CustName, [Visited], [Remarks],
                   hd.[CreatedBy], hd.[CreatedOn],
                   hd.[Latitude], hd.[Longitude], hd.[LocationAccuracy]
            FROM [mcDCR].[FieldworkHD] hd
            LEFT OUTER JOIN [mcMaster].[Customers] cust ON hd.CompID = cust.CompID AND hd.CustID = cust.CustID
            WHERE hd.[CompID] = @compID AND UID = @uid
                  AND FORMAT(hd.CreatedOn, 'MM-yyyy') = FORMAT(SWITCHOFFSET(SYSDATETIMEOFFSET(),'+05:30'), 'MM-yyyy')
            UNION ALL
            SELECT b.[CompID], [TransID], b.[UID], a.HeadQuater as HQcode,
                   [description] as PatchName, 0 as CustID, 'N/A' as CustName,
                   'OTHER' as Visited, [Remarks], b.[CreatedBy], b.[CreatedOn],
                   NULL as Latitude, NULL as Longitude, NULL as LocationAccuracy
            FROM [mcDCR].[OtherWorkTypes] b
            INNER JOIN [mcMaster].[Users] a ON b.UID = a.UID
            INNER JOIN [mcMaster].[Lookup] ON code = b.WT_Code
            WHERE b.[CompID] = @compID AND b.UID = @uid
                  AND FORMAT(b.CreatedOn, 'MM-yyyy') = FORMAT(SWITCHOFFSET(SYSDATETIMEOFFSET(),'+05:30'), 'MM-yyyy')
        END
    ELSE
        BEGIN
            SELECT hd.[CompID], [TransID], [UID], [HQcode], [PatchName],
                   hd.[CustID], [Name] as CustName, [Visited], [Remarks],
                   hd.[CreatedBy], hd.[CreatedOn],
                   hd.[Latitude], hd.[Longitude], hd.[LocationAccuracy]
            FROM [mcDCR].[FieldworkHD] hd
            LEFT OUTER JOIN [mcMaster].[Customers] cust ON hd.CompID = cust.CompID AND hd.CustID = cust.CustID
            WHERE hd.[CompID] = @compID AND UID = @uid
                  AND FORMAT(hd.CreatedOn, 'dd-MM-yyyy') = FORMAT(CAST(@dateofWork as DATE), 'dd-MM-yyyy')
            UNION ALL
            SELECT b.[CompID], [TransID], b.[UID], a.HeadQuater as HQcode,
                   [description] as PatchName, 0 as CustID, 'N/A' as CustName,
                   'OTHER' as Visited, [Remarks], b.[CreatedBy], b.[CreatedOn],
                   NULL as Latitude, NULL as Longitude, NULL as LocationAccuracy
            FROM [mcDCR].[OtherWorkTypes] b
            INNER JOIN [mcMaster].[Users] a ON b.UID = a.UID
            INNER JOIN [mcMaster].[Lookup] ON code = b.WT_Code
            WHERE b.[CompID] = @compID AND b.UID = @uid
                  AND FORMAT(b.CreatedOn, 'dd-MM-yyyy') = FORMAT(CAST(@dateofWork as DATE), 'dd-MM-yyyy')
        END
END
GO

-- ============================================================================
-- SECTION 2: PRODUCTS ENHANCEMENT (MRP + ImageURL)
-- ============================================================================

-- 2.1 Update usp_ProductInsert to include MRP and ImageURL
ALTER PROC [mcDCR].[usp_ProductInsert]
    @compid int,
    @desc VARCHAR(30),
    @type [varchar](30) NULL,
    @pack [varchar](30) NULL,
    @price [numeric](10,2) = 0.0,
    @mrp [numeric](10,2) = 0.0,
    @imageurl [varchar](500) = NULL,
    @CreatedBy [int] NULL
AS
BEGIN
    INSERT INTO [mcMaster].[Products](
        [CompID], [proddesc], [prodtype], [prodpack], [prodprice], [MRP], [ImageURL], [IsActive],
        [CreatedBy], [CreatedOn])
    SELECT @compid, @desc, @type, @pack, @price, @mrp, @imageurl, cast(1 as bit),
           @CreatedBy, SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')
END
GO

-- 2.2 Update usp_ProductUpdate to include MRP and ImageURL
ALTER PROC [mcDCR].[usp_ProductUpdate]
    @compid int,
    @code INT,
    @desc VARCHAR(30),
    @type [varchar](30) NULL,
    @pack [varchar](30) NULL,
    @price [numeric](10,2) = 0.0,
    @mrp [numeric](10,2) = 0.0,
    @imageurl [varchar](500) = NULL,
    @ModifiedBy [int] NULL
AS
BEGIN
    UPDATE [mcMaster].[Products]
    SET [proddesc] = @desc, [prodtype] = @type, [prodpack] = @pack,
        [prodprice] = @price, [MRP] = @mrp, [ImageURL] = @imageurl,
        [ModifiedBy] = @ModifiedBy, [ModifiedOn] = SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')
    WHERE [CompID] = @compid AND [prodcode] = @code
END
GO

-- 2.3 Update usp_ProductList to return MRP and ImageURL
ALTER PROC [mcDCR].[usp_ProductList]
    @compid int
AS
BEGIN
    SELECT [CompID], [prodcode], [proddesc], [prodtype], [prodpack],
           [prodprice], [MRP], [ImageURL],
           [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn]
    FROM [mcMaster].[Products]
    WHERE CompID = @compid AND isActive = Cast(1 as bit)
END
GO

-- ============================================================================
-- SECTION 3: POB (PERSONAL ORDER BOOKING) - ORDERS
-- ============================================================================

-- 3.1 Create usp_OrderInsert
CREATE PROC [mcDCR].[usp_OrderInsert]
    @CompID int,
    @TransID varchar(20),
    @ProductID int,
    @Quantity int,
    @UnitPrice decimal(10,2),
    @TotalAmount decimal(10,2),
    @CreatedBy int
AS
BEGIN
    INSERT INTO [mcDCR].[Orders] (
        [CompID], [TransID], [ProductID], [Quantity], [UnitPrice], [TotalAmount],
        [IsActive], [CreatedBy], [CreatedOn])
    VALUES (
        @CompID, @TransID, @ProductID, @Quantity, @UnitPrice, @TotalAmount,
        1, @CreatedBy, SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30'))
END
GO

-- 3.2 Create usp_OrderUpdate
CREATE PROC [mcDCR].[usp_OrderUpdate]
    @OrderID int,
    @Quantity int,
    @UnitPrice decimal(10,2),
    @TotalAmount decimal(10,2),
    @ModifiedBy int
AS
BEGIN
    UPDATE [mcDCR].[Orders]
    SET [Quantity] = @Quantity, [UnitPrice] = @UnitPrice, [TotalAmount] = @TotalAmount,
        [ModifiedBy] = @ModifiedBy, [ModifiedOn] = SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')
    WHERE [OrderID] = @OrderID
END
GO

-- 3.3 Create usp_OrderDelete
CREATE PROC [mcDCR].[usp_OrderDelete]
    @OrderID int,
    @ModifiedBy int
AS
BEGIN
    UPDATE [mcDCR].[Orders]
    SET [IsActive] = 0,
        [ModifiedBy] = @ModifiedBy, [ModifiedOn] = SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')
    WHERE [OrderID] = @OrderID
END
GO

-- 3.4 Create usp_OrderListByTransID
CREATE PROC [mcDCR].[usp_OrderListByTransID]
    @TransID varchar(20)
AS
BEGIN
    SELECT o.[OrderID], o.[CompID], o.[TransID], o.[ProductID],
           p.[proddesc] as ProductName, p.[prodpack] as ProductPack,
           p.[MRP], p.[prodprice] as StockistPrice,
           o.[Quantity], o.[UnitPrice], o.[TotalAmount],
           o.[CreatedBy], o.[CreatedOn], o.[ModifiedBy], o.[ModifiedOn]
    FROM [mcDCR].[Orders] o
    INNER JOIN [mcMaster].[Products] p ON o.ProductID = p.prodcode AND o.CompID = p.CompID
    WHERE o.[TransID] = @TransID AND o.[IsActive] = 1
END
GO

-- 3.5 Create usp_OrderListByCompany
CREATE PROC [mcDCR].[usp_OrderListByCompany]
    @CompID int,
    @FromDate date = NULL,
    @ToDate date = NULL
AS
BEGIN
    SELECT o.[OrderID], o.[CompID], o.[TransID], o.[ProductID],
           p.[proddesc] as ProductName, p.[prodpack] as ProductPack,
           p.[MRP], p.[prodprice] as StockistPrice,
           o.[Quantity], o.[UnitPrice], o.[TotalAmount],
           o.[CreatedBy], o.[CreatedOn], o.[ModifiedBy], o.[ModifiedOn]
    FROM [mcDCR].[Orders] o
    INNER JOIN [mcMaster].[Products] p ON o.ProductID = p.prodcode AND o.CompID = p.CompID
    WHERE o.[CompID] = @CompID AND o.[IsActive] = 1
          AND (@FromDate IS NULL OR CAST(o.CreatedOn AS DATE) >= @FromDate)
          AND (@ToDate IS NULL OR CAST(o.CreatedOn AS DATE) <= @ToDate)
    ORDER BY o.[CreatedOn] DESC
END
GO

-- ============================================================================
-- SECTION 4: DCR DATE APPROVAL SYSTEM
-- ============================================================================

-- 4.1 Create usp_DCRDateRequestInsert
CREATE PROC [mcDCR].[usp_DCRDateRequestInsert]
    @CompID int,
    @EmployeeID int,
    @RequestedDate date,
    @Reason varchar(500) = NULL
AS
BEGIN
    -- Check if request already exists for this date
    IF EXISTS (SELECT 1 FROM [mcDCR].[DCRDateRequests]
               WHERE CompID = @CompID AND EmployeeID = @EmployeeID
               AND RequestedDate = @RequestedDate AND Status IN ('Pending', 'Approved'))
    BEGIN
        RAISERROR('A request for this date already exists', 16, 1)
        RETURN
    END

    INSERT INTO [mcDCR].[DCRDateRequests] (
        [CompID], [EmployeeID], [RequestedDate], [Reason], [Status], [RequestedOn])
    VALUES (
        @CompID, @EmployeeID, @RequestedDate, @Reason, 'Pending',
        SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30'))

    SELECT SCOPE_IDENTITY() as RequestID
END
GO

-- 4.2 Create usp_DCRDateRequestApprove
CREATE PROC [mcDCR].[usp_DCRDateRequestApprove]
    @RequestID int,
    @ApprovedBy int
AS
BEGIN
    UPDATE [mcDCR].[DCRDateRequests]
    SET [Status] = 'Approved',
        [ApprovedBy] = @ApprovedBy,
        [ApprovedOn] = SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')
    WHERE [RequestID] = @RequestID AND [Status] = 'Pending'
END
GO

-- 4.3 Create usp_DCRDateRequestReject
CREATE PROC [mcDCR].[usp_DCRDateRequestReject]
    @RequestID int,
    @ApprovedBy int,
    @RejectionReason varchar(500) = NULL
AS
BEGIN
    UPDATE [mcDCR].[DCRDateRequests]
    SET [Status] = 'Rejected',
        [ApprovedBy] = @ApprovedBy,
        [ApprovedOn] = SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30'),
        [RejectionReason] = @RejectionReason
    WHERE [RequestID] = @RequestID AND [Status] = 'Pending'
END
GO

-- 4.4 Create usp_DCRDateRequestListByEmployee
CREATE PROC [mcDCR].[usp_DCRDateRequestListByEmployee]
    @CompID int,
    @EmployeeID int
AS
BEGIN
    SELECT r.[RequestID], r.[CompID], r.[EmployeeID],
           u.[Name] as EmployeeName,
           r.[RequestedDate], r.[Reason], r.[Status],
           r.[RequestedOn], r.[ApprovedBy],
           a.[Name] as ApprovedByName,
           r.[ApprovedOn], r.[RejectionReason]
    FROM [mcDCR].[DCRDateRequests] r
    INNER JOIN [mcMaster].[Users] u ON r.EmployeeID = u.UID AND r.CompID = u.CompID
    LEFT JOIN [mcMaster].[Users] a ON r.ApprovedBy = a.UID AND r.CompID = a.CompID
    WHERE r.[CompID] = @CompID AND r.[EmployeeID] = @EmployeeID
    ORDER BY r.[RequestedOn] DESC
END
GO

-- 4.5 Create usp_DCRDateRequestListPending (For Admins)
CREATE PROC [mcDCR].[usp_DCRDateRequestListPending]
    @CompID int
AS
BEGIN
    SELECT r.[RequestID], r.[CompID], r.[EmployeeID],
           u.[Name] as EmployeeName, u.[HeadQuater],
           r.[RequestedDate], r.[Reason], r.[Status],
           r.[RequestedOn]
    FROM [mcDCR].[DCRDateRequests] r
    INNER JOIN [mcMaster].[Users] u ON r.EmployeeID = u.UID AND r.CompID = u.CompID
    WHERE r.[CompID] = @CompID AND r.[Status] = 'Pending'
    ORDER BY r.[RequestedOn] ASC
END
GO

-- 4.6 Create usp_DCRDateRequestListAll (For Admins - with filters)
CREATE PROC [mcDCR].[usp_DCRDateRequestListAll]
    @CompID int,
    @Status varchar(20) = NULL,
    @FromDate date = NULL,
    @ToDate date = NULL
AS
BEGIN
    SELECT r.[RequestID], r.[CompID], r.[EmployeeID],
           u.[Name] as EmployeeName, u.[HeadQuater],
           r.[RequestedDate], r.[Reason], r.[Status],
           r.[RequestedOn], r.[ApprovedBy],
           a.[Name] as ApprovedByName,
           r.[ApprovedOn], r.[RejectionReason]
    FROM [mcDCR].[DCRDateRequests] r
    INNER JOIN [mcMaster].[Users] u ON r.EmployeeID = u.UID AND r.CompID = u.CompID
    LEFT JOIN [mcMaster].[Users] a ON r.ApprovedBy = a.UID AND r.CompID = a.CompID
    WHERE r.[CompID] = @CompID
          AND (@Status IS NULL OR r.[Status] = @Status)
          AND (@FromDate IS NULL OR r.[RequestedDate] >= @FromDate)
          AND (@ToDate IS NULL OR r.[RequestedDate] <= @ToDate)
    ORDER BY r.[RequestedOn] DESC
END
GO

-- 4.7 Create usp_DCRDateRequestCheckApproved (Check if date is approved for entry)
CREATE PROC [mcDCR].[usp_DCRDateRequestCheckApproved]
    @CompID int,
    @EmployeeID int,
    @RequestedDate date
AS
BEGIN
    SELECT CASE WHEN EXISTS (
        SELECT 1 FROM [mcDCR].[DCRDateRequests]
        WHERE CompID = @CompID AND EmployeeID = @EmployeeID
              AND RequestedDate = @RequestedDate AND Status = 'Approved'
    ) THEN 1 ELSE 0 END as IsApproved
END
GO

-- ============================================================================
-- SECTION 5: INDEXES FOR PERFORMANCE
-- ============================================================================

-- Create indexes for better query performance
CREATE INDEX IX_Orders_TransID ON [mcDCR].[Orders](TransID);
GO

CREATE INDEX IX_Orders_CompID ON [mcDCR].[Orders](CompID);
GO

CREATE INDEX IX_DCRDateRequests_CompID_EmployeeID ON [mcDCR].[DCRDateRequests](CompID, EmployeeID);
GO

CREATE INDEX IX_DCRDateRequests_Status ON [mcDCR].[DCRDateRequests](Status);
GO

CREATE INDEX IX_FieldworkHD_Location ON [mcDCR].[FieldworkHD](Latitude, Longitude)
WHERE Latitude IS NOT NULL AND Longitude IS NOT NULL;
GO

-- ============================================================================
-- END OF MIGRATION SCRIPT
-- ============================================================================

PRINT 'Migration completed successfully!'
GO

  ALTER PROC [mcDCR].[usp_CompanyUpdate]
      @CompID INT,
      @Name VARCHAR(60),
      @Address1 VARCHAR(60) = NULL,
      @Locality VARCHAR(30) = NULL,
      @CityOrTown VARCHAR(20) = NULL,
      @Pincode INT = NULL,
      @District VARCHAR(20) = NULL,
      @State VARCHAR(20) = NULL,
      @Country VARCHAR(20) = NULL,
      @Mobile VARCHAR(15) = NULL,
      @Telephone VARCHAR(15) = NULL,
      @Fax VARCHAR(15) = NULL,
      @LogoURL NVARCHAR(500) = NULL,
      @ModifiedBy INT
  AS
  BEGIN
      SET NOCOUNT ON;

      UPDATE [mcMaster].[Company]
      SET
          Name = @Name,
          Address1 = @Address1,
          Locality = @Locality,
          CityOrTown = @CityOrTown,
          Pincode = @Pincode,
          District = @District,
          State = @State,
          Country = @Country,
          Mobile = @Mobile,
          Telephone = @Telephone,
          Fax = @Fax,
          LogoURL = @LogoURL,
          ModifiedBy = @ModifiedBy,
          ModifiedOn = SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')
      WHERE CompID = @CompID
  END
GO
--  Update usp_CompanyGetById - Return LogoURL:

ALTER PROC [mcDCR].[usp_CompanyGetById]
      @CompID INT
  AS
  BEGIN
      SET NOCOUNT ON;

      SELECT
          CompID, Name, Address1, Locality, CityOrTown, Pincode,
          District, State, Country, Mobile, Telephone, Fax, LogoURL,
          IsActive, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn
      FROM [mcMaster].[Company]
      WHERE CompID = @CompID
  END
GO
--*******************************************************

 ALTER PROC [mcDCR].[usp_UserInsert]
      @compID int,
      @userID VARCHAR(50),
      @password VARCHAR(20),
      @name VARCHAR(50),
      @headquater VARCHAR(30),
      @address1 VARCHAR(60) = NULL,
      @locality VARCHAR(60) = NULL,
      @cityOrTown VARCHAR(9) = NULL,
      @pincode int = NULL,
      @district VARCHAR(30) = NULL,
      @state VARCHAR(30) = NULL,
      @country VARCHAR(30) = NULL,
      @mobile VARCHAR(15) = NULL,
      @telephone VARCHAR(15) = NULL,
      @ProfileImageURL NVARCHAR(500) = NULL,
      @isCompAdmin BIT = 0,
      @createdBy VARCHAR(30) = NULL
  AS
  BEGIN
      INSERT INTO [mcMaster].[Users](
          [CompID], [UserID], [Password], [Name], [HeadQuater],
          [Address1], [Locality], [CityOrTown], [Pincode], [District],
          [State], [Country], [Mobile], [Telephone], [ProfileImageURL],
          [IsActive], [isCompAdmin], [CreatedBy], [CreatedOn])
      SELECT
          @compID, @userID, @password, @name, @headquater,
          @address1, @locality, @cityOrTown, @pincode, @district,
          @state, @country, @mobile, @telephone, @ProfileImageURL,
          cast(1 as bit), @isCompAdmin, @createdBy,
          SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')
  END
  GO

--   2. Update usp_UserUpdate:
  ALTER PROC [mcDCR].[usp_UserUpdate]
      @compID INT,
      @UID INT,
      @userID VARCHAR(50),
      @password VARCHAR(20),
      @name VARCHAR(50),
      @headquater VARCHAR(30),
      @address1 VARCHAR(60) = NULL,
      @locality VARCHAR(60) = NULL,
      @cityOrTown VARCHAR(9) = NULL,
      @pincode INT = NULL,
      @district VARCHAR(30) = NULL,
      @state VARCHAR(30) = NULL,
      @country VARCHAR(30) = NULL,
      @mobile VARCHAR(15) = NULL,
      @telephone VARCHAR(15) = NULL,
      @ProfileImageURL NVARCHAR(500) = NULL,
      @isCompAdmin BIT = 0,
      @modifiedBy VARCHAR(30) = NULL
  AS
  BEGIN
      UPDATE [mcMaster].[Users]
      SET
          [UserID] = @userID,
          [Password] = @password,
          [Name] = @name,
          [HeadQuater] = @headquater,
          [Address1] = @address1,
          [Locality] = @locality,
          [CityOrTown] = @cityOrTown,
          [Pincode] = @pincode,
          [District] = @district,
          [State] = @state,
          [Country] = @country,
          [Mobile] = @mobile,
          [Telephone] = @telephone,
          [ProfileImageURL] = @ProfileImageURL,
          [isCompAdmin] = @isCompAdmin,
          [ModifiedBy] = @modifiedBy,
          [ModifiedOn] = SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')
      WHERE [CompID] = @compID AND [UID] = @UID
  END
  GO

--   3. Update usp_UserList:
  ALTER PROC [mcDCR].[usp_UserList]
  AS
  BEGIN
      SELECT [CompID], [UID], [UserID], [Password], [Name], [HeadQuater],
             [Address1], [Locality], [CityOrTown], [Pincode], [District],
             [State], [Country], [Mobile], [Telephone], [ProfileImageURL],
             [IsActive], [isCompAdmin] AS [IsCompAdmin]
      FROM [mcMaster].[Users]
      WHERE IsActive = cast(1 as bit) AND UID != 100
  END
  GO

--   4. Update usp_UserListByCompany:
  ALTER PROC [mcDCR].[usp_UserListByCompany]
      @compid int
  AS
  BEGIN
      SELECT [CompID], [UID], [UserID], [Password], [Name], [HeadQuater],
             [Address1], [Locality], [CityOrTown], [Pincode], [District],
             [State], [Country], [Mobile], [Telephone], [ProfileImageURL],
             [IsActive], [isCompAdmin] AS [IsCompAdmin]
      FROM [mcMaster].[Users]
      WHERE [CompID] = @compid AND IsActive = cast(1 as bit) AND UID != 100
  END
  GO

-- ****************************************************
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
          ,[ProfileImageURL]
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
  GO

-- ****************************************************

SELECT * FROM [mcMaster].[Users]


