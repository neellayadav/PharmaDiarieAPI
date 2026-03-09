-- ========================================================================================================================================
-- UPDATE OLD SPs TO INCLUDE POBAmount FIELD
-- Created: 2026-02-18
-- Purpose: Add POBAmount (sum of order TotalAmount per TransID) to existing download/Excel SPs
-- ========================================================================================================================================


-- ========================================================================================================================================
-- 1. [mcDCR].[usp_FWHDEmpMonthlyList] - Employee Monthly Report (with POBAmount)
-- ========================================================================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[mcDCR].[usp_FWHDEmpMonthlyList]') AND type = N'P')
    DROP PROCEDURE [mcDCR].[usp_FWHDEmpMonthlyList]
GO

CREATE PROC [mcDCR].[usp_FWHDEmpMonthlyList]
    @compID INT,
    @uid INT,
    @month INT,
    @yearOf INT

AS

-- *********************************************************
-- EXEC [mcDCR].[usp_FWHDEmpMonthlyList] 2000, 104, 11, 2024
-- *********************************************************

BEGIN

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

END
GO


-- ========================================================================================================================================
-- 2. [mcDCR].[usp_FWHDMonthlyList] - All Employees Monthly Report (with POBAmount)
-- ========================================================================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[mcDCR].[usp_FWHDMonthlyList]') AND type = N'P')
    DROP PROCEDURE [mcDCR].[usp_FWHDMonthlyList]
GO

CREATE PROC [mcDCR].[usp_FWHDMonthlyList]
    @compID INT,
    @monthOf INT,
    @yearOf INT

AS

-- *********************************************************
-- EXEC [mcDCR].[usp_FWHDMonthlyList] 2000, 11, 2024
-- *********************************************************

BEGIN

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

END
GO


-- ========================================================================================================================================
-- 3. [mcDCR].[usp_FWHDEmpYearlyList] - Employee Yearly Report (with POBAmount)
-- ========================================================================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[mcDCR].[usp_FWHDEmpYearlyList]') AND type = N'P')
    DROP PROCEDURE [mcDCR].[usp_FWHDEmpYearlyList]
GO

CREATE PROC [mcDCR].[usp_FWHDEmpYearlyList]
    @compID INT,
    @uid INT,
    @yearOf INT

AS

-- *********************************************************
-- EXEC [mcDCR].[usp_FWHDEmpYearlyList] 2000, 104, 2024
-- *********************************************************

BEGIN

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

END
GO


-- ========================================================================================================================================
-- 4. [mcDCR].[usp_FWHDYearlyList] - All Employees Yearly Report (with POBAmount)
-- ========================================================================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[mcDCR].[usp_FWHDYearlyList]') AND type = N'P')
    DROP PROCEDURE [mcDCR].[usp_FWHDYearlyList]
GO

CREATE PROC [mcDCR].[usp_FWHDYearlyList]
    @compID INT,
    @yearOf INT

AS

-- *********************************************************
-- EXEC [mcDCR].[usp_FWHDYearlyList] 2000, 2024
-- *********************************************************

BEGIN

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

END
GO


-- ========================================================================================================================================
-- 5. [mcDCR].[usp_FWHDFinancialYearList] - Financial/Custom Date Report (with POBAmount)
-- ========================================================================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[mcDCR].[usp_FWHDFinancialYearList]') AND type = N'P')
    DROP PROCEDURE [mcDCR].[usp_FWHDFinancialYearList]
GO

CREATE PROC [mcDCR].[usp_FWHDFinancialYearList]
    @compID INT,
    @fromMonth INT,
    @fromYear INT,
    @toMonth INT,
    @toYear INT

AS

-- *********************************************************
-- EXEC [mcDCR].[usp_FWHDFinancialYearList] 2000, 04, 2023, 03, 2024
-- *********************************************************

BEGIN

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

END
GO
