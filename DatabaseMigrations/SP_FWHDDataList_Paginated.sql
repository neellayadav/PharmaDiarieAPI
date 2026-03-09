-- ========================================================================================================================================
-- NEW PAGINATED DATA SPs FOR FIELD WORK REPORTS (Web Table View)
-- Created: 2026-02-17
-- Purpose: Return JSON data with pagination instead of full dataset for Excel generation
-- New fields added: POBAmount (sum of order TotalAmount per TransID)
-- ========================================================================================================================================


-- ========================================================================================================================================
-- 1. [mcDCR].[usp_FWHDEmpMonthlyDataList] - Employee Monthly Report Data (Paginated)
-- ========================================================================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[mcDCR].[usp_FWHDEmpMonthlyDataList]') AND type = N'P')
    DROP PROCEDURE [mcDCR].[usp_FWHDEmpMonthlyDataList]
GO

CREATE PROC [mcDCR].[usp_FWHDEmpMonthlyDataList]
    @compID INT,
    @uid INT,
    @month INT,
    @yearOf INT,
    @page INT = 1,
    @pageSize INT = 50

AS

-- *********************************************************
-- EXEC [mcDCR].[usp_FWHDEmpMonthlyDataList] 2000, 104, 11, 2024, 1, 50
-- *********************************************************

BEGIN
    SET NOCOUNT ON;

    -- Result Set 1: Total Count
    SELECT COUNT(*) AS TotalCount
    FROM mcDCR.FieldworkHD fhd
    INNER JOIN mcMaster.Customers cust ON fhd.CustID = cust.CustID
        AND fhd.CompID = @compID AND fhd.UID = @uid
        AND MONTH(fhd.CreatedOn) = @month AND YEAR(fhd.CreatedOn) = @yearOf
    INNER JOIN mcMaster.Users usr ON fhd.CreatedBy = usr.UID
    LEFT OUTER JOIN mcDCR.FieldworkEmpDT empDT ON fhd.TransID = empDt.TransID
    LEFT OUTER JOIN mcDCR.FieldworkProdDT prodDT ON fhd.TransID = prodDT.TransID;

    -- Result Set 2: Paginated Data
    SELECT fhd.CompID, fhd.TransID, fhd.HQcode, fhd.PatchName, fhd.Visited, fhd.UID, usr.Name AS UserName,
        fhd.CustID, cust.Name AS CustName, fhd.Remarks, fhd.IsActive, fhd.CreatedOn,
        ISNULL(empDT.SNo, 0) AS EmpSeqNo, ISNULL(empDT.UID, 0) AS Colleague,
        ISNULL((SELECT Name FROM mcMaster.Users WHERE UID = empDT.UID), '') AS ColleagueName,
        ISNULL(prodDt.SNo, 0) AS ProdSeqNo, ISNULL(prodDT.ProdCode, 0) AS ProductCode,
        ISNULL((SELECT proddesc FROM mcMaster.Products WHERE prodcode = prodDt.prodcode), '') AS ProductDesc,
        ISNULL((SELECT SUM(o.TotalAmount) FROM mcDCR.Orders o WHERE o.TransID = fhd.TransID AND o.IsActive = 1), 0) AS POBAmount
    FROM mcDCR.FieldworkHD fhd
    INNER JOIN mcMaster.Customers cust ON fhd.CustID = cust.CustID
        AND fhd.CompID = @compID AND fhd.UID = @uid
        AND MONTH(fhd.CreatedOn) = @month AND YEAR(fhd.CreatedOn) = @yearOf
    INNER JOIN mcMaster.Users usr ON fhd.CreatedBy = usr.UID
    LEFT OUTER JOIN mcDCR.FieldworkEmpDT empDT ON fhd.TransID = empDt.TransID
    LEFT OUTER JOIN mcDCR.FieldworkProdDT prodDT ON fhd.TransID = prodDT.TransID
    ORDER BY usr.Name, fhd.CreatedOn, fhd.HQcode, fhd.PatchName
    OFFSET (@page - 1) * @pageSize ROWS
    FETCH NEXT @pageSize ROWS ONLY;

END
GO


-- ========================================================================================================================================
-- 2. [mcDCR].[usp_FWHDMonthlyDataList] - All Employees Monthly Report Data (Paginated)
-- ========================================================================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[mcDCR].[usp_FWHDMonthlyDataList]') AND type = N'P')
    DROP PROCEDURE [mcDCR].[usp_FWHDMonthlyDataList]
GO

CREATE PROC [mcDCR].[usp_FWHDMonthlyDataList]
    @compID INT,
    @monthOf INT,
    @yearOf INT,
    @page INT = 1,
    @pageSize INT = 50

AS

-- *********************************************************
-- EXEC [mcDCR].[usp_FWHDMonthlyDataList] 2000, 11, 2024, 1, 50
-- *********************************************************

BEGIN
    SET NOCOUNT ON;

    -- Result Set 1: Total Count
    SELECT COUNT(*) AS TotalCount
    FROM mcDCR.FieldworkHD fhd
    INNER JOIN mcMaster.Customers cust ON fhd.CustID = cust.CustID
        AND fhd.CompID = @compID
        AND MONTH(fhd.CreatedOn) = @monthOf AND YEAR(fhd.CreatedOn) = @yearOf
        AND fhd.UID != 100
    INNER JOIN mcMaster.Users usr ON fhd.CreatedBy = usr.UID
    LEFT OUTER JOIN mcDCR.FieldworkEmpDT empDT ON fhd.TransID = empDt.TransID
    LEFT OUTER JOIN mcDCR.FieldworkProdDT prodDT ON fhd.TransID = prodDT.TransID;

    -- Result Set 2: Paginated Data
    SELECT fhd.CompID, fhd.TransID, fhd.HQcode, fhd.PatchName, fhd.Visited, fhd.UID, usr.Name AS UserName,
        fhd.CustID, cust.Name AS CustName, fhd.Remarks, fhd.IsActive, fhd.CreatedOn,
        ISNULL(empDT.SNo, 0) AS EmpSeqNo, ISNULL(empDT.UID, 0) AS Colleague,
        ISNULL((SELECT Name FROM mcMaster.Users WHERE UID = empDT.UID), '') AS ColleagueName,
        ISNULL(prodDt.SNo, 0) AS ProdSeqNo, ISNULL(prodDT.ProdCode, 0) AS ProductCode,
        ISNULL((SELECT proddesc FROM mcMaster.Products WHERE prodcode = prodDt.prodcode), '') AS ProductDesc,
        ISNULL((SELECT SUM(o.TotalAmount) FROM mcDCR.Orders o WHERE o.TransID = fhd.TransID AND o.IsActive = 1), 0) AS POBAmount
    FROM mcDCR.FieldworkHD fhd
    INNER JOIN mcMaster.Customers cust ON fhd.CustID = cust.CustID
        AND fhd.CompID = @compID
        AND MONTH(fhd.CreatedOn) = @monthOf AND YEAR(fhd.CreatedOn) = @yearOf
        AND fhd.UID != 100
    INNER JOIN mcMaster.Users usr ON fhd.CreatedBy = usr.UID
    LEFT OUTER JOIN mcDCR.FieldworkEmpDT empDT ON fhd.TransID = empDt.TransID
    LEFT OUTER JOIN mcDCR.FieldworkProdDT prodDT ON fhd.TransID = prodDT.TransID
    ORDER BY usr.Name, fhd.CreatedOn, fhd.HQcode, fhd.PatchName
    OFFSET (@page - 1) * @pageSize ROWS
    FETCH NEXT @pageSize ROWS ONLY;

END
GO


-- ========================================================================================================================================
-- 3. [mcDCR].[usp_FWHDEmpYearlyDataList] - Employee Yearly Report Data (Paginated)
-- ========================================================================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[mcDCR].[usp_FWHDEmpYearlyDataList]') AND type = N'P')
    DROP PROCEDURE [mcDCR].[usp_FWHDEmpYearlyDataList]
GO

CREATE PROC [mcDCR].[usp_FWHDEmpYearlyDataList]
    @compID INT,
    @uid INT,
    @yearOf INT,
    @page INT = 1,
    @pageSize INT = 50

AS

-- *********************************************************
-- EXEC [mcDCR].[usp_FWHDEmpYearlyDataList] 2000, 104, 2024, 1, 50
-- *********************************************************

BEGIN
    SET NOCOUNT ON;

    -- Result Set 1: Total Count
    SELECT COUNT(*) AS TotalCount
    FROM mcDCR.FieldworkHD fhd
    INNER JOIN mcMaster.Customers cust ON fhd.CustID = cust.CustID
        AND fhd.CompID = @compID AND fhd.UID = @uid
        AND YEAR(fhd.CreatedOn) = @yearOf
    INNER JOIN mcMaster.Users usr ON fhd.CreatedBy = usr.UID
    LEFT OUTER JOIN mcDCR.FieldworkEmpDT empDT ON fhd.TransID = empDt.TransID
    LEFT OUTER JOIN mcDCR.FieldworkProdDT prodDT ON fhd.TransID = prodDT.TransID;

    -- Result Set 2: Paginated Data
    SELECT fhd.CompID, fhd.TransID, fhd.HQcode, fhd.PatchName, fhd.Visited, fhd.UID, usr.Name AS UserName,
        fhd.CustID, cust.Name AS CustName, fhd.Remarks, fhd.IsActive, fhd.CreatedOn,
        ISNULL(empDT.SNo, 0) AS EmpSeqNo, ISNULL(empDT.UID, 0) AS Colleague,
        ISNULL((SELECT Name FROM mcMaster.Users WHERE UID = empDT.UID), '') AS ColleagueName,
        ISNULL(prodDt.SNo, 0) AS ProdSeqNo, ISNULL(prodDT.ProdCode, 0) AS ProductCode,
        ISNULL((SELECT proddesc FROM mcMaster.Products WHERE prodcode = prodDt.prodcode), '') AS ProductDesc,
        ISNULL((SELECT SUM(o.TotalAmount) FROM mcDCR.Orders o WHERE o.TransID = fhd.TransID AND o.IsActive = 1), 0) AS POBAmount
    FROM mcDCR.FieldworkHD fhd
    INNER JOIN mcMaster.Customers cust ON fhd.CustID = cust.CustID
        AND fhd.CompID = @compID AND fhd.UID = @uid
        AND YEAR(fhd.CreatedOn) = @yearOf
    INNER JOIN mcMaster.Users usr ON fhd.CreatedBy = usr.UID
    LEFT OUTER JOIN mcDCR.FieldworkEmpDT empDT ON fhd.TransID = empDt.TransID
    LEFT OUTER JOIN mcDCR.FieldworkProdDT prodDT ON fhd.TransID = prodDT.TransID
    ORDER BY usr.Name, fhd.CreatedOn, fhd.HQcode, fhd.PatchName
    OFFSET (@page - 1) * @pageSize ROWS
    FETCH NEXT @pageSize ROWS ONLY;

END
GO


-- ========================================================================================================================================
-- 4. [mcDCR].[usp_FWHDYearlyDataList] - All Employees Yearly Report Data (Paginated)
-- ========================================================================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[mcDCR].[usp_FWHDYearlyDataList]') AND type = N'P')
    DROP PROCEDURE [mcDCR].[usp_FWHDYearlyDataList]
GO

CREATE PROC [mcDCR].[usp_FWHDYearlyDataList]
    @compID INT,
    @yearOf INT,
    @page INT = 1,
    @pageSize INT = 50

AS

-- *********************************************************
-- EXEC [mcDCR].[usp_FWHDYearlyDataList] 2000, 2024, 1, 50
-- *********************************************************

BEGIN
    SET NOCOUNT ON;

    -- Result Set 1: Total Count
    SELECT COUNT(*) AS TotalCount
    FROM mcDCR.FieldworkHD fhd
    INNER JOIN mcMaster.Customers cust ON fhd.CustID = cust.CustID
        AND fhd.CompID = @compID
        AND YEAR(fhd.CreatedOn) = @yearOf
        AND fhd.UID != 100
    INNER JOIN mcMaster.Users usr ON fhd.CreatedBy = usr.UID
    LEFT OUTER JOIN mcDCR.FieldworkEmpDT empDT ON fhd.TransID = empDt.TransID
    LEFT OUTER JOIN mcDCR.FieldworkProdDT prodDT ON fhd.TransID = prodDT.TransID;

    -- Result Set 2: Paginated Data
    SELECT fhd.CompID, fhd.TransID, fhd.HQcode, fhd.PatchName, fhd.Visited, fhd.UID, usr.Name AS UserName,
        fhd.CustID, cust.Name AS CustName, fhd.Remarks, fhd.IsActive, fhd.CreatedOn,
        ISNULL(empDT.SNo, 0) AS EmpSeqNo, ISNULL(empDT.UID, 0) AS Colleague,
        ISNULL((SELECT Name FROM mcMaster.Users WHERE UID = empDT.UID), '') AS ColleagueName,
        ISNULL(prodDt.SNo, 0) AS ProdSeqNo, ISNULL(prodDT.ProdCode, 0) AS ProductCode,
        ISNULL((SELECT proddesc FROM mcMaster.Products WHERE prodcode = prodDt.prodcode), '') AS ProductDesc,
        ISNULL((SELECT SUM(o.TotalAmount) FROM mcDCR.Orders o WHERE o.TransID = fhd.TransID AND o.IsActive = 1), 0) AS POBAmount
    FROM mcDCR.FieldworkHD fhd
    INNER JOIN mcMaster.Customers cust ON fhd.CustID = cust.CustID
        AND fhd.CompID = @compID
        AND YEAR(fhd.CreatedOn) = @yearOf
        AND fhd.UID != 100
    INNER JOIN mcMaster.Users usr ON fhd.CreatedBy = usr.UID
    LEFT OUTER JOIN mcDCR.FieldworkEmpDT empDT ON fhd.TransID = empDt.TransID
    LEFT OUTER JOIN mcDCR.FieldworkProdDT prodDT ON fhd.TransID = prodDT.TransID
    ORDER BY usr.Name, fhd.CreatedOn, fhd.HQcode, fhd.PatchName
    OFFSET (@page - 1) * @pageSize ROWS
    FETCH NEXT @pageSize ROWS ONLY;

END
GO


-- ========================================================================================================================================
-- 5. [mcDCR].[usp_FWHDFinancialYearDataList] - Financial/Custom Date Report Data (Paginated)
-- ========================================================================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[mcDCR].[usp_FWHDFinancialYearDataList]') AND type = N'P')
    DROP PROCEDURE [mcDCR].[usp_FWHDFinancialYearDataList]
GO

CREATE PROC [mcDCR].[usp_FWHDFinancialYearDataList]
    @compID INT,
    @fromMonth INT,
    @fromYear INT,
    @toMonth INT,
    @toYear INT,
    @page INT = 1,
    @pageSize INT = 50

AS

-- *********************************************************
-- EXEC [mcDCR].[usp_FWHDFinancialYearDataList] 2000, 04, 2023, 03, 2024, 1, 50
-- *********************************************************

BEGIN
    SET NOCOUNT ON;

    -- Result Set 1: Total Count
    SELECT COUNT(*) AS TotalCount
    FROM mcDCR.FieldworkHD fhd
    INNER JOIN mcMaster.Customers cust ON fhd.CustID = cust.CustID
        AND fhd.CompID = @compID
        AND fhd.CreatedOn BETWEEN DATEFROMPARTS(@fromYear, @fromMonth, 1) AND EOMONTH(DATEFROMPARTS(@toYear, @toMonth, 1))
    INNER JOIN mcMaster.Users usr ON fhd.CreatedBy = usr.UID
    LEFT OUTER JOIN mcDCR.FieldworkEmpDT empDT ON fhd.TransID = empDt.TransID
    LEFT OUTER JOIN mcDCR.FieldworkProdDT prodDT ON fhd.TransID = prodDT.TransID;

    -- Result Set 2: Paginated Data
    SELECT fhd.CompID, fhd.TransID, fhd.HQcode, fhd.PatchName, fhd.Visited, fhd.UID, usr.Name AS UserName,
        fhd.CustID, cust.Name AS CustName, fhd.Remarks, fhd.IsActive, fhd.CreatedOn,
        ISNULL(empDT.SNo, 0) AS EmpSeqNo, ISNULL(empDT.UID, 0) AS Colleague,
        ISNULL((SELECT Name FROM mcMaster.Users WHERE UID = empDT.UID), '') AS ColleagueName,
        ISNULL(prodDt.SNo, 0) AS ProdSeqNo, ISNULL(prodDT.ProdCode, 0) AS ProductCode,
        ISNULL((SELECT proddesc FROM mcMaster.Products WHERE prodcode = prodDt.prodcode), '') AS ProductDesc,
        ISNULL((SELECT SUM(o.TotalAmount) FROM mcDCR.Orders o WHERE o.TransID = fhd.TransID AND o.IsActive = 1), 0) AS POBAmount
    FROM mcDCR.FieldworkHD fhd
    INNER JOIN mcMaster.Customers cust ON fhd.CustID = cust.CustID
        AND fhd.CompID = @compID
        AND fhd.CreatedOn BETWEEN DATEFROMPARTS(@fromYear, @fromMonth, 1) AND EOMONTH(DATEFROMPARTS(@toYear, @toMonth, 1))
    INNER JOIN mcMaster.Users usr ON fhd.CreatedBy = usr.UID
    LEFT OUTER JOIN mcDCR.FieldworkEmpDT empDT ON fhd.TransID = empDt.TransID
    LEFT OUTER JOIN mcDCR.FieldworkProdDT prodDT ON fhd.TransID = prodDT.TransID
    ORDER BY usr.Name, fhd.CreatedOn, fhd.HQcode, fhd.PatchName
    OFFSET (@page - 1) * @pageSize ROWS
    FETCH NEXT @pageSize ROWS ONLY;

END
GO


-- ========================================================================================================================================
-- VERIFICATION: List all new SPs
-- ========================================================================================================================================
-- SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES
-- WHERE ROUTINE_SCHEMA = 'mcDCR' AND ROUTINE_NAME LIKE '%DataList%'
-- ORDER BY ROUTINE_NAME
-- ========================================================================================================================================
