# Pharmadiarie API — Documentation

> **Last Updated:** 2026-03-09 (Phase 2: Expense, Sales, LoginLog, App modules + Report pagination + Model enhancements)
>
> **Standing Rule:** This file MUST be updated whenever changes are made to the API project.

---

## Table of Contents

1. [Overview](#1-overview)
2. [Project Structure](#2-project-structure)
3. [Technology Stack](#3-technology-stack)
4. [Configuration](#4-configuration)
5. [Controllers & Endpoints](#5-controllers--endpoints)
6. [Business Layer](#6-business-layer)
7. [Data Access Layer](#7-data-access-layer)
8. [Data Models](#8-data-models)
9. [Database](#9-database)
10. [Special Patterns](#10-special-patterns)
11. [Modules & Features](#11-modules--features)
12. [Deployment](#12-deployment)

---

## 1. Overview

ASP.NET Core 7.0 REST API serving both the Next.js web dashboard and Flutter mobile app. All business logic lives in SQL Server stored procedures — the API is a thin wrapper that routes HTTP requests to SPs via ADO.NET.

- **Staging URL:** `https://pharmaapi-fbgjfgc2gddfevfx.eastasia-01.azurewebsites.net`
- **Production URL:** `https://pharmaprodapi-efhcduete6ejb0dq.eastasia-01.azurewebsites.net`
- **Swagger:** `/swagger/` (development only)

---

## 2. Project Structure

```
PharmaDiaries.sln
├── PharmaDiariesAPI/                    # Web API entry point
│   ├── Controllers/                     # 22 API controllers (140+ endpoints)
│   ├── Program.cs                       # DI setup, CORS, middleware
│   ├── appsettings.json                 # DB, R2, base URL config
│   └── Services/                        # R2StorageService
├── PharmaDiaries.Bussiness/             # Business logic implementations
├── PharmaDiaries.BusinessContract/      # Business interfaces (IBusiness)
├── PharmaDiaries.DataAccess/            # Repository implementations (ADO.NET)
├── PharmaDiaries.DataAccessContract/    # Repository interfaces
│   └── Repository/                      # IRepository interfaces
├── PharmaDiaries.Models/                # Request/response DTOs
├── PharmaDiaries.Utils/                 # Report generation (SpreadsheetLight)
├── PharmaDiaries.platforms/             # SqlHelper, DataTableHelper
└── DatabaseMigrations/                  # SQL migration scripts
```

---

## 3. Technology Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| Framework | ASP.NET Core | 7.0 |
| Language | C# | 11 |
| Data Access | ADO.NET (no ORM) | — |
| Database | SQL Server (Azure SQL) | — |
| Image Storage | Cloudflare R2 (S3 API) | AWSSDK.S3 3.7.305 |
| Image Processing | SixLabors.ImageSharp | 3.1.12 |
| Reports | SpreadsheetLight / ClosedXML | — |
| JSON | Newtonsoft.Json | 13.0.4 |
| API Docs | Swashbuckle (Swagger) | 6.5.0 |

---

## 4. Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "APIconnectionString": "Data Source=SQL8020.site4now.net;Initial Catalog=db_9f30c5_mcmewastaging;..."
  },
  "ReportSettings": {
    "BaseUrl": "https://pharmaapi-fbgjfgc2gddfevfx.eastasia-01.azurewebsites.net"
  },
  "CloudflareR2": {
    "AccountId": "...",
    "AccessKeyId": "...",
    "SecretAccessKey": "...",
    "BucketName": "pdimages",
    "PublicUrl": "..."
  }
}
```

### Dependency Injection (Program.cs)

- All Business + Repository services registered as **Singleton**
- `IR2StorageService` → `R2StorageService` for image uploads
- CORS: `AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()` (restrict in production)

**Phase 2 DI Registrations:**
- `ILoginLogRepository` → `LoginLogRepository`, `ILoginLogBusiness` → `LoginLogBusiness`
- `IExpenseRepository` → `ExpenseRepository`, `IExpenseBusiness` → `ExpenseBusiness`
- `IExpensePolicyRepository` → `ExpensePolicyRepository`, `IExpensePolicyBusiness` → `ExpensePolicyBusiness`
- `IAppRepository` → `AppRepository`, `IAppBusiness` → `AppBusiness`
- `ISalesRepository` → `SalesRepository`, `ISalesBusiness` → `SalesBusiness`

---

## 5. Controllers & Endpoints

### Summary (22 Controllers, 140+ Endpoints)

| Controller | Route | Endpoints | Purpose |
|-----------|-------|-----------|---------|
| LoginController | `api/login` | 2 | Authentication |
| UserController | `api/user` | 8 | User management |
| CompanyController | `api/company` | 16 | Company management & settings (incl. dedicated settings update) |
| ExpenseController | `api/expense` | 13 | Expense claims (core) |
| ExpensePolicyController | `api/expensepolicy` | 14 | Policies & rates |
| ExpenseApprovalController | `api/expenseapproval` | 6 | Claim approvals |
| ExpenseReportController | `api/expensereport` | 3 | Reports & settlement |
| ReportsController | `api/reports` | 18 | Business reports (Excel/JSON + paginated data endpoints) |
| AreasController | `api/areas` | 8 | Geographic areas |
| CustomerController | `api/customer` | 6 | Customer management |
| ProductController | `api/product` | 4 | Product catalog |
| ImageController | `api/image` | 5 | Image upload/delete |
| LoginLogController | `api/loginlog` | 8 | Login tracking |
| RoleHierarchyController | `api/rolehierarchy` | 4 | Role rankings |
| OrdersController | `api/orders` | 6 | Personal Order Booking |
| SalesController | `api/sales` | 11 | Primary/Secondary Sales (Header + Detail) |
| DCRDateRequestController | `api/dcrdaterequest` | 7 | Past date approval |
| AppController | `api/app` | 1 | Version check |
| FieldworkHeaderController | `api/fieldworkheader` | 7 | Field work entries |
| FieldWorkEmployeeController | `api/fieldworkemployee` | 1 | FW employee data |
| ScreenController | `api/screen` | 4 | Screen/permission management |
| LookupController | `api/lookup` | 2 | Lookup values |

---

### Detailed Endpoints

#### LoginController — `api/login`
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/Validate` | Authenticate user credentials |
| POST | `/SignUp` | Register new user |

#### UserController — `api/user`
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/GetUserList` | All users (system-wide) |
| GET | `/GetUserListByComp` | Users by CompID |
| POST | `/Save` | Create user |
| POST | `/Update` | Update user |
| POST | `/Delete` | Soft delete user |
| POST | `/ResetPassword` | Reset password |
| GET | `/GetPotentialManagers` | Eligible managers (query: compid, currentuid, currentroleid) |
| POST | `/DeleteUserByUserID` | Delete by credentials |

#### CompanyController — `api/company`
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/Insert` | Create company |
| GET | `/GetCompanyById` | Get company (query: id) |
| GET | `/GetAllCompanies` | All companies |
| GET | `/active` | Active companies only |
| GET | `/search/{searchTerm}` | Search by name |
| PUT | `/Update` | Update company |
| PATCH | `/Status` | Toggle active/inactive |
| DELETE | `/Delete` | Delete company (query: id, modifiedBy) |
| POST | `/CheckName` | Check name uniqueness |
| GET | `/Count` | Company statistics (total, active, inactive) |
| GET | `/GetLocationTrackerSetting` | GPS tracking enabled? (query: compId) |
| POST | `/UpdateLocationTrackerSetting` | Toggle GPS (query: compId, isEnabled, modifiedBy) |
| GET | `/GetGeoFenceSettings` | Geofence config (query: compId) |
| POST | `/UpdateGeoFenceSettings` | Update geofence |
| POST | `/UpdateCompanySettings` | Update all company settings (LocationTracker, GeoFence, GPSAccuracy, ShowOnDuty) via dedicated SP |

#### ExpenseController — `api/expense`
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/Calculate` | Calculate DA/TA with validations |
| POST | `/CreateClaim` | Create new expense claim |
| POST | `/UpdateClaim` | Update claim before submission |
| GET | `/GetMyClaims` | User's claims (query: compID, uid, month, year, status) |
| GET | `/GetClaimDetail` | Claim + lines + approval history (query: compID, claimID) |
| POST | `/AddLine` | Add expense line to claim |
| POST | `/UpdateLine` | Update expense line |
| POST | `/DeleteLine` | Delete expense line (query: compID, lineID, modifiedBy) |
| POST | `/SubmitClaim` | Submit for approval |
| POST | `/ResubmitClaim` | Resubmit returned claim |
| POST | `/UploadReceipt` | Upload receipt image |
| GET | `/GetAttachments` | Get receipts by line (query: compID, lineID) |
| GET | `/GetTravelModes` | All travel modes (convenience endpoint) |

#### ExpensePolicyController — `api/expensepolicy`
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/CreateTravelMode` | Create travel mode (Car, Train, etc.) |
| POST | `/UpdateTravelMode` | Update travel mode |
| GET | `/GetTravelModes` | All travel modes (global, CompID=0) |
| POST | `/CreatePolicy` | Create allowance policy |
| POST | `/UpdatePolicy` | Update policy |
| GET | `/GetPolicies` | Company policies (query: compID) |
| POST | `/SaveDARates` | Save DA rates bulk |
| GET | `/GetDARates` | DA rates by policy (query: compID, policyID) |
| POST | `/SaveTARates` | Save TA rates bulk |
| GET | `/GetTARates` | TA rates by policy (query: compID, policyID) |
| POST | `/SaveEmployeeDARates` | Employee-specific DA rates |
| GET | `/GetEmployeeDARates` | Get employee DA rates (query: compID, policyID, uid) |
| POST | `/SaveEmployeeTARates` | Employee-specific TA rates |
| GET | `/GetEmployeeTARates` | Get employee TA rates |

#### ExpenseApprovalController — `api/expenseapproval`
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/GetPending` | Pending approvals (query: compID, approverUID) |
| POST | `/Approve` | Approve claim |
| POST | `/Reject` | Reject claim |
| POST | `/Return` | Return to employee |
| POST | `/BulkApprove` | Bulk approve multiple claims |
| GET | `/GetHistory` | Approval action history (query: compID, claimID) |

#### ExpenseReportController — `api/expensereport`
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/GetAllClaims` | All claims with filters (compID, month, year, uid, status) |
| POST | `/SettleClaim` | Mark claim as settled |
| GET | `/GetSettlements` | Settlements by month (query: compID, month, year) |

#### ReportsController — `api/reports`
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/MonthlyReport` | Monthly FW report (Excel) |
| POST | `/FWEmpMonthly` | Employee monthly (Excel) |
| POST | `/FWMonthly` | Company monthly (Excel) |
| POST | `/FWEmpYearly` | Employee yearly (Excel) |
| POST | `/FWYearly` | Company yearly (Excel) |
| POST | `/FinancialYearly` | Financial year (Excel) |
| POST | `/FWCustomerCallSummary` | Customer call summary (JSON) |
| POST | `/FWEmpMonthlyData` | Employee monthly (paginated JSON, includes POBAmount/SecondaryAmount) |
| POST | `/FWMonthlyData` | Company monthly (paginated JSON, includes POBAmount/SecondaryAmount) |
| POST | `/FWEmpYearlyData` | Employee yearly (paginated JSON, includes POBAmount/SecondaryAmount) |
| POST | `/FWYearlyData` | Company yearly (paginated JSON, includes POBAmount/SecondaryAmount) |
| POST | `/FinancialYearlyData` | Financial year (paginated JSON, includes POBAmount/SecondaryAmount) |
| GET | `/Download/{compId}/{fileName}` | Download generated report file |

**Paginated Data Response Format:**
```json
{
  "message": "Report data retrieved successfully",
  "data": {
    "totalCount": 150,
    "page": 1,
    "pageSize": 50,
    "data": [
      {
        "compID": 2000, "transID": "T001", "hQcode": "HQ1",
        "patchName": "P1", "visited": "2026-03-01", "uid": 100,
        "userName": "John", "custID": 5, "custName": "Dr. Sharma",
        "remarks": "", "pobAmount": 500.00, "secondaryAmount": 1200.00,
        "productCode": 23, "productDesc": "Paracetamol 500mg"
      }
    ]
  }
}
```

#### AreasController — `api/areas`
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/GetRegionList` | Regions (query: compid) |
| GET | `/GetHeadQuaterList` | Headquarters (query: compid) |
| GET | `/GetHeadQuaterListByRegion` | HQ by region (query: compid, region) |
| GET | `/GetPatchListByHeadQuater` | Patches by HQ (query: compid, hQuater) |
| GET | `/GetAreaList` | All areas (query: compid) |
| POST | `/Save` | Create area |
| POST | `/Update` | Update area |
| POST | `/Delete` | Delete area |

#### CustomerController — `api/customer`
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/CustomersListByCompType` | Customers by type (query: compid, custType) |
| POST | `/Save` | Create customer |
| POST | `/Update` | Update customer |
| POST | `/Delete` | Delete customer |
| GET | `/GetLocation` | Customer GPS location (query: compId, custId) |
| POST | `/UpdateLocation` | Update customer location |

#### ProductController — `api/product`
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/GetProductList` | Products (query: compid) |
| POST | `/Save` | Create product |
| POST | `/Update` | Update product |
| POST | `/Delete` | Delete product |

#### ImageController — `api/image`
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/UploadProductImage` | Upload product image (max 5MB, JPEG/PNG/WebP) |
| POST | `/UploadLogo` | Upload company logo |
| POST | `/UploadIcon` | Upload company icon |
| POST | `/UploadProfileImage` | Upload user profile |
| DELETE | `/Delete` | Delete image (query: key) |

Image storage: Cloudflare R2, 4 variants (Original, 1200px, 800px, 400px).

#### LoginLogController — `api/loginlog`
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/Save` | Log login event |
| PUT | `/Update` | Log logout |
| GET | `/GetByCompUID` | Logs by company + user |
| GET | `/GetByLogId` | Log by ID |
| GET | `/GetByCompUIDMonthYear` | Logs by month |
| GET | `/GetByCompUIDDate` | Logs by date |
| GET | `/GetByCompIdMonthYear` | All users' logs by month |
| GET | `/GetByCompIdDate` | All users' logs by date |

#### RoleHierarchyController — `api/rolehierarchy`
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/DistinctRoles` | All distinct roles from Lookup |
| POST | `/Save` | Save single role ranking |
| GET | `/List` | All ranked roles |
| POST | `/SaveBatch` | Save multiple rankings |

#### OrdersController — `api/orders`
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/Save` | Create order |
| POST | `/SaveMultiple` | Bulk create orders |
| POST | `/Update` | Update order |
| POST | `/Delete` | Delete order (query: orderId, modifiedBy) |
| GET | `/GetByTransID` | Orders by transaction |
| GET | `/GetByCompany` | Company orders (query: compId, fromDate, toDate) |

#### SalesController — `api/sales`

Primary/Secondary Sales module with header-detail pattern. Headers contain sale metadata (customer, user, date range, type), details contain product line items. GET endpoints return nested response (header with embedded details array).

| Method | Route | Description |
|--------|-------|-------------|
| POST | `/CreateSale` | Combined create: header + all detail lines in one transaction. Returns auto-generated SalesID + ItemIDs |
| POST | `/CreateHeader` | Create header only (auto-generates SalesID). Returns SalesID |
| GET | `/GetHeaderById` | Single header with nested details (query: compId, salesId, uid) |
| GET | `/GetHeaderList` | Headers with nested details, all filters optional except compId (query: compId, uid, custId, type, fromDate, toDate). Date range is treated as a pair — both must be provided to filter |
| POST | `/UpdateHeader` | Update header by composite PK (CompID, SalesID, UID) |
| POST | `/DeleteHeader` | Soft delete header (IsActive = 0) |
| POST | `/CreateDetail` | Add single detail line (requires existing SalesID). Returns ItemID |
| GET | `/GetDetailById` | Single detail with product info (query: itemId) |
| GET | `/GetDetailList` | All details for a sale with product info (query: salesId) |
| POST | `/UpdateDetail` | Update detail by ItemID |
| POST | `/DeleteDetail` | Hard delete detail by ItemID |

**SalesID Auto-Generation:** Format `S{CompID}{UID}{4-digit seq}` — e.g., `S20001000001`, `S20001000002`

**Nested Response Example (GetHeaderById / GetHeaderList):**
```json
{
  "success": true,
  "data": {
    "compID": 2000,
    "salesID": "S20001000001",
    "uid": 100,
    "userName": "ADMIN",
    "custID": 5,
    "customerName": "Dr. Sharma",
    "customerType": "DOCTOR",
    "type": "PRIMARY",
    "isActive": true,
    "details": [
      {
        "itemID": 1,
        "productID": 23,
        "productName": "Paracetamol 500mg",
        "productPrice": 45.00,
        "mrp": 55.00,
        "quantity": 10,
        "unitPrice": 50.00,
        "totalAmount": 500.00
      }
    ]
  }
}
```

**Combined Create Request (CreateSale):**
```json
{
  "compID": 2000,
  "uid": 100,
  "fromDate": "2026-03-01",
  "toDate": "2026-03-31",
  "custID": 5,
  "type": "PRIMARY",
  "createdBy": 100,
  "details": [
    { "productID": 23, "quantity": 10, "unitPrice": 50.00, "totalAmount": 500.00 },
    { "productID": 45, "quantity": 5, "unitPrice": 100.00, "totalAmount": 500.00 }
  ]
}
```

#### DCRDateRequestController — `api/dcrdaterequest`
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/CreateRequest` | Submit past date request |
| POST | `/Approve` | Approve request |
| POST | `/Reject` | Reject request |
| GET | `/GetByEmployee` | User's requests |
| GET | `/GetPendingRequests` | Pending requests |
| GET | `/GetAllRequests` | All with filters (status, date range) |
| GET | `/IsDateApproved` | Check if date approved |

#### AppController — `api/app`
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/VersionCheck` | Check for app update (query: platform, currentVersion) |

#### FieldworkHeaderController — `api/fieldworkheader`
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/FWHeaderList` | Get field work headers |
| POST | `/Save` | Create field work entry |
| POST | `/Delete` | Delete field work |
| POST | `/OtherWorkSave` | Save other work details |
| POST | `/GetEmpDateWiseFW` | Get FW by date |
| GET | `/GetFieldWorkSummary` | FW summary (query: compId, uid) |
| GET | `/GetFWMonthlyReport` | Monthly report (query: compId, monthOf, yearOf) |

#### FieldWorkEmployeeController — `api/fieldworkemployee`
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/GetAllFWEmployees` | FW employees (query: transId) |

#### ScreenController — `api/screen`
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/sync` | Sync screens |
| GET | `/` | Get active screens |
| POST | `/users/{userId}/permissions` | Save user permissions |
| GET | `/users/{userId}/permissions` | Get user permissions |

---

## 6. Business Layer

### Interfaces & Implementations

| Interface | Implementation | Key Methods |
|-----------|----------------|-------------|
| `ILoginBusiness` | `LoginBusiness` | Validate, SignUp |
| `IUserBusiness` | `UserBusiness` | GetUserList, Save, Update, Delete, ResetPassword, GetPotentialManagers |
| `ICompanyBusiness` | `CompanyBusiness` | CreateCompanyAsync, GetCompanyByIdAsync, UpdateCompanyAsync, GetLocationTrackerSettingAsync, UpdateGeoFenceSettingsAsync |
| `IExpenseBusiness` | `ExpenseBusiness` | CalculateExpenseAsync, CreateClaimAsync, GetClaimsByUIDAsync, SubmitClaimAsync, ApproveClaimAsync, SettleClaimAsync |
| `IExpensePolicyBusiness` | `ExpensePolicyBusiness` | CreateTravelModeAsync, CreatePolicyAsync, SaveDARateAsync, SaveEmployeeDARateAsync |
| `IAreasBusiness` | `AreasBusiness` | RegionList, HeadQuarterList, AreaList, Save, Update, Delete |
| `ICustomerBusiness` | `CustomerBusiness` | CustomerListByCompType, Save, Update, Delete, GetCustomerLocation |
| `IProductBusiness` | `ProductBusiness` | GetProductList, Save, Update, Delete |
| `IReportBusiness` | `ReportBusiness` | GetMonthlyReport, GetEmpMonthlyReport, GetCustomerCallSummary |
| `ILoginLogBusiness` | `LoginLogBusiness` | Save, Update, GetByCompUID, GetByLogId |
| `IRoleHierarchyBusiness` | `RoleHierarchyBusiness` | GetDistinctRoles, SaveRoleHierarchy, SaveRoleHierarchyBatch |
| `IFWHDBusiness` | `FWHDBusiness` | FWHeaderList, Save, Delete, OtherworkSave, GetEmpDateWiseFW |
| `IFWEmpDTBusiness` | `FWEmpDTBusiness` | GetAllFieldworkEmp |
| `ILookupBusiness` | `LookupBusiness` | Lookuplist, LookupListByType |
| `IScreenBusiness` | `ScreenBusiness` | SyncScreens, GetActiveScreens, SaveUserScreenPermissions |
| `IAppBusiness` | `AppBusiness` | CheckVersion |
| `ISalesBusiness` | `SalesBusiness` | CreateSale, CreateHeader, GetHeaderById, GetHeaderList, UpdateHeader, DeleteHeader, CreateDetail, GetDetailById, GetDetailList, UpdateDetail, DeleteDetail |

---

## 7. Data Access Layer

All repositories use ADO.NET with stored procedures. No Entity Framework.

| Interface | Implementation | Purpose |
|-----------|----------------|---------|
| `ILoginRepository` | `LoginRepository` | User authentication |
| `IUserRepository` | `UserRepository` | User CRUD |
| `ICompanyRepository` | `CompanyRepository` | Company management + settings |
| `IExpenseRepository` | `ExpenseRepository` | Claims, approvals, settlement |
| `IExpensePolicyRepository` | `ExpensePolicyRepository` | Travel modes, policies, rates |
| `IAreasRepository` | `AreasRepository` | Region → HQ → Patch → Area |
| `ICustomerRepository` | `CustomerRepository` | Customer + location |
| `IProductRepository` | `ProductRepository` | Product catalog |
| `IReportRepository` | `ReportRepository` | Report generation (Excel + JSON) |
| `ILoginLogRepository` | `LoginLogRepository` | Login/logout tracking |
| `IRoleHierarchyRepository` | `RoleHierarchyRepository` | Role ranking |
| `IFWHDRepository` | `FWHDRepository` | Field work headers |
| `IFWEmpDTRepository` | `FWEmpDTRepository` | Field work employees |
| `IOrdersRepository` | `OrdersRepository` | POB orders (replaces deprecated `OrderRepository`, adds `orderType` support) |
| `IDCRDateRequestRepository` | `DCRDateRequestRepository` | Past date approvals |
| `IScreenRepository` | `ScreenRepository` | Screen management |
| `ISalesRepository` | `SalesRepository` | Primary/Secondary Sales (header-detail CRUD, combined create with transaction) |
| `IAppRepository` | `AppRepository` | App version check (force update) |

### SqlHelper Utility

```csharp
// PharmaDiaries.platforms/SqlHelper.cs
SqlHelper.ExecuteDataset(connStr, spName, params)
SqlHelper.ExecuteScalar(connStr, spName, params)
SqlHelper.ExecuteNonQuery(connStr, spName, params)
```

---

## 8. Data Models

### Response Wrapper

```csharp
public class ApiResponse<T> {
    bool Success;
    string Message;
    T? Data;
}

public class SpStatusResponse {
    string Status;   // "OK" or "ERROR"
    string Message;
}
```

### Key Models

**User**: `UserModel` (includes `emailid`), `LoginUserModel`, `ResetPasswordModel`, `SignUpModel`

**Company**: `CompanyModel` (includes `emailid`, `ShowOnDuty`), `CompanyCreateRequest` (includes `emailid`), `CompanyUpdateRequest` (settings fields separated out), `GeoFenceSettingsRequest`, `CompanySettingsUpdateRequest` (dedicated: `IsLocationTrackerEnabled`, `GeoFenceRadiusMeters`, `GPSAccuracyThreshold`, `ShowOnduty`, `ModifiedBy`), `CompanyCountResponse`

**Expense**: `TravelModeModel`, `AllowancePolicyModel`, `DARateModel`, `TARateModel`, `EmployeeDARateModel`, `EmployeeTARateModel`, `ExpenseClaimModel`, `ExpenseClaimLineModel`, `ApprovalActionModel`, `SettlementModel`, `ClaimAttachmentInsertRequest`

**Field Work**: `FieldWorkHeader` (with nested `FieldworkEmpDT`, `FieldworkProdDT`, `OrderModel` — includes `orderType` field, default: "POB")

**Sales**: `SalesHeaderModel` (with nested `List<SalesDetailModel> Details`), `SalesDetailModel`, `SalesHeaderCreateRequest`, `SalesHeaderUpdateRequest`, `SalesHeaderDeleteRequest`, `SalesDetailCreateRequest`, `SalesDetailUpdateRequest`, `SalesDetailDeleteRequest`, `CreateSaleRequest` (with nested `List<CreateSaleDetailItem>`), `CreateSaleResponse`

**Monthly Report**: `MonthlyReportModel` now includes `POBAmount` and `SecondaryAmount` fields (aggregated from Orders by TransID)

**Master Data**: `CustomerModel`, `ProductModel`, `AreasModel`, `LookupModel`

**Reports (Pagination)**: `PaginatedReportResponse` (TotalCount, Page, PageSize, Data[]), `ReportDataItem` (includes `POBAmount`, `SecondaryAmount`), `EmpMonthlyDataRequest`, `MonthlyDataRequest`, `EmpYearlyDataRequest`, `YearlyDataRequest`, `FinancialYearDataRequest` — all pagination requests default to Page=1, PageSize=50

**Login Log**: `LoginLogInsertRequest`, `LoginLogUpdateRequest`, `LoginLogModel`

---

## 9. Database

### Connection

- **Server**: SQL8020.site4now.net
- **Database**: db_9f30c5_mcmewastaging (staging)
- **MARS**: MultipleActiveResultSets = true

### Schemas

| Schema | Purpose |
|--------|---------|
| `mcMaster` | Users, Company, Areas |
| `mcDCR` | Field work, DCR, Customers, Products, Orders, Sales |
| `mcExpense` | Claims, Policies, Rates, Approvals, Settlements |

### Stored Procedures (80+)

**User & Login**: `mcMaster.usp_UserInsert`, `usp_UserUpdate`, `usp_UserDelete`, `usp_UserList`, `usp_UserListByComp`, `usp_UserResetPassword`, `usp_UserLogin`, `usp_GetPotentialManagers`, `usp_DeleteUserByUserID`

**Company**: `mcDCR.usp_CompanyCreate`, `usp_CompanyGetById`, `usp_CompanyGetAll`, `usp_CompanyGetActive`, `usp_CompanySearchByName`, `usp_CompanyUpdate`, `usp_CompanyUpdateStatus`, `usp_CompanyDelete`, `usp_CompanyCheckNameExists`, `usp_CompanyGetCount`, `usp_CompanySettingsUpdate` (dedicated settings: LocationTracker, GeoFence, GPSAccuracy, ShowOnDuty) | `mcMaster.usp_GetCompanyGeoFenceSettings`, `usp_UpdateCompanyGeoFenceSettings`

**Expense**: `mcExpense.usp_TravelModeInsert`, `usp_TravelModeUpdate`, `usp_TravelModeGetAll`, `usp_AllowancePolicyInsert`, `usp_AllowancePolicyUpdate`, `usp_AllowancePolicyGetByComp`, `usp_DARateInsert`, `usp_DARateGetByPolicy`, `usp_TARateInsert`, `usp_EmployeeDARateInsert`, `usp_ExpenseClaimInsert`, `usp_ExpenseClaimUpdate`, `usp_ExpenseClaimGetByUID`, `usp_ClaimLineInsert`, `usp_ClaimLineUpdate`, `usp_ExpenseClaimLineDelete`, `usp_ExpenseClaimSubmit`, `usp_ExpenseClaimApprove`, `usp_ExpenseClaimReject`, `usp_ExpenseClaimSettle`, `usp_ExpenseCalculate`, `usp_ExpenseClaimRecalcTotals`, `usp_ClaimAttachmentInsert`, `usp_ClaimAttachmentGetByLine`

**Areas & Customers**: `mcDCR.usp_RegionList`, `usp_HeadQuaterList`, `usp_HeadQuaterListByRegion`, `usp_PatchListByHeadQuater`, `usp_AreasList`, `usp_CustomerListByComp_Type`, `usp_CustomerInsert`, `usp_CustomerUpdate`, `usp_CustomerDelete` | `mcMaster.usp_Areas_Insert`, `usp_Areas_Update`, `usp_Areas_Delete`, `usp_GetCustomerLocation`, `usp_UpdateCustomerLocation`

**Field Work**: `mcDCR.usp_FWHeaderList`, `usp_FWHeaderInsert`, `usp_FWHeaderDelete`, `usp_FWOthersInsert`, `usp_FWEmpDTList`, `usp_FWHeaderMonthlyReport`

**Report Data (Paginated)**: `mcDCR.usp_FWHDEmpMonthlyDataList`, `usp_FWHDMonthlyDataList`, `usp_FWHDEmpYearlyDataList`, `usp_FWHDYearlyDataList`, `usp_FWHDFinancialYearDataList` — all support `@Page`, `@PageSize` params and return `TotalCount` as output parameter. Existing Excel SPs (`usp_FWHDEmpMonthlyList`, `usp_FWHDMonthlyList`, `usp_FWHDEmpYearlyList`, `usp_FWHDYearlyList`, `usp_FWHDFinancialYearList`) now include POBAmount column

**Sales**: `mcDCR.usp_SalesHeader_Insert` (auto-generates SalesID), `usp_SalesHeader_GetById`, `usp_SalesHeader_GetList`, `usp_SalesHeader_Update`, `usp_SalesHeader_Delete` (soft delete), `usp_SalesDetail_Insert`, `usp_SalesDetail_GetById`, `usp_SalesDetail_GetList`, `usp_SalesDetail_Update`, `usp_SalesDetail_Delete` (hard delete). Tables: `mcDCR.SalesHeader` (PK: CompID, SalesID, UID), `mcDCR.SalesDetail` (PK: ItemID identity). SQL script: `SQL/Version 1-0-1+21/9-SalesCRUD.sql`

**Others**: `mcDCR.usp_ProductList/Insert/Update/Delete`, `usp_OrdersInsert/Update/Delete`, `usp_DCRDateRequestInsert/Approve/Reject`, `usp_LoginLogInsert/Update`, `usp_LookupList/LookupListByType` | `mcMaster.usp_RoleHierarchySave/GetAll` | `dbo.usp_AppVersionCheck`

### Migration Scripts

Located at `./DatabaseMigrations/`:
```
2024-12-15_PharmaDiarie_SP_Migration.sql         — Core SPs
2024-12-17_Fix_User_SPs_Correct_Schema.sql
2024-12-27_Add_ReportingManager_Feature.sql       — Manager hierarchy
2025-01-05_Add_RoleHierarchy_Feature.sql          — Role ranking
2025-01-06_Add_GeoFencing_Feature.sql             — Location tracking
2025-01-06_Fix_IST_Timezone.sql                   — CustomDT setup
SP_FWHDDataList_Paginated.sql
SP_FWHD_AddPOBAmount.sql                          — POB support
```

---

## 10. Special Patterns

### CustomDT()

**CRITICAL:** Always use `dbo.CustomDT()` instead of `GETDATE()` in all stored procedures. This ensures consistent IST timezone handling.

### ApiResponse Wrapper

All controllers return:
```csharp
return Ok(new ApiResponse<T> { Success = true, Message = "...", Data = result });
```

### SP Error Pattern

SPs return `{ Status, Message }` for validation errors. Controllers convert to HTTP 400:
```csharp
if (result.Status == "ERROR")
    return BadRequest(new ApiResponse<T> { Success = false, Message = result.Message, Data = result });
```

### Image Processing

- **Storage**: Cloudflare R2 (S3 API)
- **Service**: `IR2StorageService` / `R2StorageService`
- **Variants**: Original, Large (1200×1200), Medium (800×800), Small (400×400)
- **Constraints**: Max 5MB, JPEG/PNG/WebP only

### Report Generation

- **Format**: Excel (.xlsx) via SpreadsheetLight/ClosedXML, plus paginated JSON
- **Output**: `/Reports/{CompID}/reports/` directory
- **Types**: Monthly, Yearly, Financial Year, Customer Call Summary
- **POB Amount Column**: All Excel reports now include POB Amount (Column 13, before Remarks) — aggregated by TransID from order TotalAmount
- **Secondary Amount**: Added alongside POBAmount for tracking secondary sales data
- **Refactored**: Header and data population extracted into shared helpers (`AddReportHeaders()`, `PopulateReportData()`) for consistency

### Authentication

- Form-based: `POST /api/login/Validate` with `{ CompID, UserID, Password }`
- No JWT in API — session managed by clients
- Roles: Super Admin (UID=100), Company Admin, Staff

### Soft Deletes

All entities use `IsActive` flag. Queries always filter `WHERE IsActive = 1`.

---

## 11. Modules & Features

| Module | Controllers | Key Features |
|--------|------------|--------------|
| **Authentication** | Login | Validate, SignUp |
| **User Management** | User | CRUD, password reset, manager hierarchy |
| **Company** | Company | Profile, settings (dedicated endpoint), geofencing, GPS tracking, ShowOnDuty, emailid |
| **Field Work (DCR)** | FieldworkHeader, FieldWorkEmployee | Save visits, delete, summaries, monthly reports |
| **Expense (DA/TA)** | Expense, ExpensePolicy, ExpenseApproval, ExpenseReport | Full claim lifecycle, policies, rates, approval, settlement |
| **Orders (POB)** | Orders | Save, bulk save, update, delete (with `orderType` support) |
| **Sales** | Sales | Primary/Secondary sales, combined create (header+details in one transaction), nested response, auto-generated SalesID |
| **DCR Date Requests** | DCRDateRequest | Request, approve, reject past dates |
| **Reports** | Reports | Excel + paginated JSON, POB/Secondary amounts, multiple time ranges |
| **Master Data** | Areas, Customer, Product, Lookup | CRUD for all master entities |
| **Location** | Company (settings) | Geofence config, GPS toggle |
| **Roles** | RoleHierarchy | Ranking, batch save |
| **Permissions** | Screen | Sync, user permissions |
| **Images** | Image | Upload/delete with R2 storage |

---

## 12. Deployment

### Build

```bash
dotnet build                    # Debug build
dotnet publish -c Release       # Release → ../../../PublishAPIs
```

### Environments

| Setting | Development | Production |
|---------|------------|------------|
| Swagger | Enabled | Disabled |
| CORS | All origins | Restricted |
| Logging | Full | Information+ |
| HTTPS redirect | Commented out | Enabled |

### HTTP Status Codes

| Code | Usage |
|------|-------|
| 200 | Success |
| 400 | Validation error (SP returned ERROR) |
| 404 | Not found |
| 500 | Server error |

### Security

- All queries filter by CompID — no cross-company data
- UID-based user segregation
- Permission matrix: Super Admin > Company Admin > Staff
