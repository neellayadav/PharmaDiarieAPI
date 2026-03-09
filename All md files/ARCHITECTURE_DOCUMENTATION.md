# PharmaDiaries - Solution Architecture Documentation

**Project:** PharmaDiaries API
**Framework:** ASP.NET Core 7.0
**Database:** SQL Server (Azure)
**Document Version:** 1.0
**Generated:** February 2026

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [Solution Structure](#2-solution-structure)
3. [Architecture Diagram](#3-architecture-diagram)
4. [Project Dependencies](#4-project-dependencies)
5. [Layer-wise Architecture](#5-layer-wise-architecture)
6. [Design Patterns](#6-design-patterns)
7. [API Endpoints](#7-api-endpoints)
8. [Data Models](#8-data-models)
9. [Database Architecture](#9-database-architecture)
10. [External Integrations](#10-external-integrations)
11. [Security Architecture](#11-security-architecture)
12. [Configuration Management](#12-configuration-management)
13. [Technical Debt & Recommendations](#13-technical-debt--recommendations)

---

## 1. Executive Summary

**PharmaDiaries** is a multi-tenant pharmaceutical field diary management system that enables pharmaceutical companies to track field work activities, customer visits, product orders, and generate business reports.

### Key Capabilities

| Feature | Description |
|---------|-------------|
| **User Management** | Multi-role user system with company isolation |
| **Field Work Tracking** | GPS-enabled visit logging with location accuracy |
| **Customer Management** | Doctor/Chemist/Stockist tracking with geo-location |
| **Product Catalog** | Product management with image variants |
| **Order Management** | Personal Order Booking (POB) system |
| **Reporting** | Excel-based monthly/yearly reports |
| **Cloud Storage** | Cloudflare R2 for media assets |

### Technology Stack

| Component | Technology |
|-----------|------------|
| Backend Framework | ASP.NET Core 7.0 |
| Database | SQL Server (Azure SQL) |
| Data Access | ADO.NET + Stored Procedures |
| Cloud Storage | Cloudflare R2 (S3 Compatible) |
| Image Processing | SixLabors.ImageSharp |
| Report Generation | ClosedXML, SpreadsheetLight |
| API Documentation | Swagger/OpenAPI |

---

## 2. Solution Structure

```
PharmaDiaries.sln
│
├── PharmaDiariesAPI/              # Web API Project (Entry Point)
│   ├── Controllers/               # API Controllers
│   ├── Services/                  # Cloud Services (R2)
│   ├── Program.cs                 # DI Configuration
│   └── appsettings.json           # Configuration
│
├── PharmaDiaries.Bussiness/       # Business Logic Implementation
│
├── PharmaDiaries.BusinessContract/ # Business Layer Interfaces
│
├── PharmaDiaries.DataAccess/      # Repository Implementations
│
├── PharmaDiaries.DataAccessContract/ # Repository Interfaces
│
├── PharmaDiaries.Models/          # Data Models & DTOs
│
├── PharmaDiaries.platforms/       # Helper Utilities
│   └── Helpers/
│       ├── SqlHelper.cs
│       └── DataTableHelper.cs
│
└── PharmaDiaries.Utils/           # Report Generation
    └── Reports.cs
```

---

## 3. Architecture Diagram

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              CLIENT LAYER                                    │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │ Mobile App  │  │  Web App    │  │   Swagger   │  │  3rd Party  │        │
│  │  (Android)  │  │  (Future)   │  │     UI      │  │   Clients   │        │
│  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘        │
└─────────┼────────────────┼────────────────┼────────────────┼────────────────┘
          │                │                │                │
          └────────────────┴────────────────┴────────────────┘
                                    │
                              HTTPS/REST
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                           API LAYER (PharmaDiariesAPI)                       │
│  ┌────────────────────────────────────────────────────────────────────┐     │
│  │                         ASP.NET Core 7.0                            │     │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐ │     │
│  │  │  Login   │ │   User   │ │ Customer │ │ FieldWork│ │  Orders  │ │     │
│  │  │Controller│ │Controller│ │Controller│ │Controller│ │Controller│ │     │
│  │  └────┬─────┘ └────┬─────┘ └────┬─────┘ └────┬─────┘ └────┬─────┘ │     │
│  │       │            │            │            │            │        │     │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐ │     │
│  │  │ Product  │ │  Image   │ │ Reports  │ │  Lookup  │ │  Areas   │ │     │
│  │  │Controller│ │Controller│ │Controller│ │Controller│ │Controller│ │     │
│  │  └────┬─────┘ └────┬─────┘ └────┬─────┘ └────┬─────┘ └────┬─────┘ │     │
│  └───────┼────────────┼────────────┼────────────┼────────────┼────────┘     │
│          │            │            │            │            │              │
│          └────────────┴────────────┴────────────┴────────────┘              │
│                                    │                                         │
│                         Dependency Injection                                 │
│                                    │                                         │
└────────────────────────────────────┼─────────────────────────────────────────┘
                                     │
                                     ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                    BUSINESS LAYER (PharmaDiaries.Business)                   │
│  ┌────────────────────────────────────────────────────────────────────┐     │
│  │                    Business Contract (Interfaces)                   │     │
│  │  ILoginBusiness │ IUserBusiness │ ICustomerBusiness │ IFWHDBusiness│     │
│  │  IProductBusiness │ IReportBusiness │ IAreasBusiness │ etc.        │     │
│  └────────────────────────────────────────────────────────────────────┘     │
│                                    │                                         │
│                              Implements                                      │
│                                    │                                         │
│  ┌────────────────────────────────────────────────────────────────────┐     │
│  │                    Business Implementation                          │     │
│  │  LoginBusiness │ UserBusiness │ CustomerBusiness │ FWHDBusiness    │     │
│  │  ProductBusiness │ ReportBusiness │ AreasBusiness │ etc.           │     │
│  └────────────────────────────────────────────────────────────────────┘     │
│                                    │                                         │
└────────────────────────────────────┼─────────────────────────────────────────┘
                                     │
                                     ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                  DATA ACCESS LAYER (PharmaDiaries.DataAccess)                │
│  ┌────────────────────────────────────────────────────────────────────┐     │
│  │                  Data Access Contract (Interfaces)                  │     │
│  │  ILoginRepository │ IUserRepository │ ICustomerRepository          │     │
│  │  IProductRepository │ IFWHDRepository │ IReportRepository │ etc.   │     │
│  └────────────────────────────────────────────────────────────────────┘     │
│                                    │                                         │
│                              Implements                                      │
│                                    │                                         │
│  ┌────────────────────────────────────────────────────────────────────┐     │
│  │                    Repository Implementation                        │     │
│  │  LoginRepository │ UserRepository │ CustomerRepository              │     │
│  │  ProductRepository │ FWHDRepository │ ReportRepository │ etc.       │     │
│  └────────────────────────────────────────────────────────────────────┘     │
│                                    │                                         │
│  ┌────────────────────────────────────────────────────────────────────┐     │
│  │                         Helper Utilities                            │     │
│  │           SqlHelper (SP Execution) │ DataTableHelper (Mapping)      │     │
│  └────────────────────────────────────────────────────────────────────┘     │
│                                    │                                         │
└────────────────────────────────────┼─────────────────────────────────────────┘
                                     │
                                     ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                            DATA LAYER                                        │
│                                                                              │
│  ┌─────────────────────────┐          ┌─────────────────────────┐           │
│  │      SQL Server         │          │    Cloudflare R2        │           │
│  │    (Azure SQL DB)       │          │   (Media Storage)       │           │
│  │                         │          │                         │           │
│  │  Schema: [mcDCR]        │          │  Bucket: pdimages       │           │
│  │  50+ Stored Procedures  │          │  - Product Images       │           │
│  │                         │          │  - Company Logos        │           │
│  │  Tables:                │          │  - User Profiles        │           │
│  │  - Users                │          │  - Icons                │           │
│  │  - Customers            │          │                         │           │
│  │  - Products             │          │  Variants:              │           │
│  │  - FieldWorkHeader      │          │  - Original (2500x2500) │           │
│  │  - FieldWorkDetails     │          │  - Large (1200x1200)    │           │
│  │  - Orders               │          │  - Medium (800x800)     │           │
│  │  - Lookups              │          │  - Small (400x400)      │           │
│  │  - Areas                │          │                         │           │
│  │  - Roles                │          │                         │           │
│  └─────────────────────────┘          └─────────────────────────┘           │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```

### Request Flow Diagram

```
┌──────────┐     ┌────────────┐     ┌────────────┐     ┌────────────┐     ┌──────────┐
│  Client  │────▶│ Controller │────▶│  Business  │────▶│ Repository │────▶│ Database │
│          │     │            │     │   Layer    │     │            │     │          │
└──────────┘     └────────────┘     └────────────┘     └────────────┘     └──────────┘
                       │                   │                  │
                       │                   │                  │
                       ▼                   ▼                  ▼
                 ┌──────────┐        ┌──────────┐       ┌──────────┐
                 │ Validate │        │ Delegate │       │ Execute  │
                 │ Request  │        │ to Repo  │       │ SP + Map │
                 └──────────┘        └──────────┘       └──────────┘
```

---

## 4. Project Dependencies

### Dependency Graph

```
                    ┌─────────────────────┐
                    │   PharmaDiariesAPI  │
                    │    (Entry Point)    │
                    └──────────┬──────────┘
                               │
              ┌────────────────┼────────────────┐
              │                │                │
              ▼                ▼                ▼
┌─────────────────────┐ ┌─────────────┐ ┌─────────────────────┐
│PharmaDiaries.Business│ │PharmaDiaries│ │     External        │
│                     │ │ .DataAccess │ │     Packages         │
└──────────┬──────────┘ └──────┬──────┘ │  - AWSSDK.S3        │
           │                   │         │  - ImageSharp       │
           │                   │         │  - Swashbuckle      │
           ▼                   ▼         └─────────────────────┘
┌─────────────────────┐ ┌─────────────────────┐
│PharmaDiaries        │ │PharmaDiaries        │
│.BusinessContract    │ │.DataAccessContract  │
└──────────┬──────────┘ └──────────┬──────────┘
           │                       │
           │                       ├─────────────────────┐
           ▼                       ▼                     ▼
    ┌─────────────────────┐ ┌─────────────────────┐ ┌─────────────────────┐
    │ PharmaDiaries.Models│ │PharmaDiaries.platforms│ │ PharmaDiaries.Utils │
    │                     │ │   (SqlHelper etc.)   │ │   (Reports)        │
    └─────────────────────┘ └─────────────────────┘ └─────────────────────┘
```

### NuGet Packages

| Package | Version | Purpose |
|---------|---------|---------|
| AWSSDK.S3 | 3.7.305.23 | Cloudflare R2 Storage |
| SixLabors.ImageSharp | 3.1.12 | Image Processing |
| Swashbuckle.AspNetCore | 6.5.0 | Swagger/OpenAPI |
| Newtonsoft.Json | 13.0.4 | JSON Serialization |
| System.Data.SqlClient | 4.9.0 | SQL Server Connectivity |
| ClosedXML | 0.105.0 | Excel Generation |
| SpreadsheetLight | 3.5.0 | Excel Handling |

---

## 5. Layer-wise Architecture

### 5.1 Presentation Layer (Controllers)

**Location:** `/PharmaDiariesAPI/Controllers/worktype/`

Controllers handle HTTP requests, validate input, and return responses.

```csharp
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserBusiness _userBusiness;

    public UserController(IUserBusiness userBusiness)
    {
        _userBusiness = userBusiness;
    }

    [HttpGet("GetUserList")]
    public IActionResult GetUserList()
    {
        var result = _userBusiness.GetUserList();
        return Ok(result);
    }
}
```

### 5.2 Business Layer

**Location:** `/PharmaDiaries.Bussiness/`

Acts as a facade, delegating operations to repositories.

```csharp
public class UserBusiness : IUserBusiness
{
    private readonly IUserRepository _repository;

    public UserBusiness(IUserRepository repository)
    {
        _repository = repository;
    }

    public List<UserModel> GetUserList()
    {
        return _repository.GetUserList();
    }
}
```

### 5.3 Data Access Layer

**Location:** `/PharmaDiaries.DataAccess/`

Executes stored procedures and maps results to models.

```csharp
public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("APIconnectionString");
    }

    public List<UserModel> GetUserList()
    {
        DataSet ds = SqlHelper.ExecuteDataset(
            _connectionString,
            "[mcDCR].[usp_UserList]"
        );
        return DataTableHelper.ConvertDataTable<UserModel>(ds.Tables[0]);
    }
}
```

---

## 6. Design Patterns

### 6.1 Repository Pattern

```
┌─────────────────┐         ┌─────────────────┐
│   IRepository   │◀────────│   Repository    │
│   (Interface)   │         │(Implementation) │
└─────────────────┘         └────────┬────────┘
                                     │
                                     ▼
                            ┌─────────────────┐
                            │    Database     │
                            │ (SQL Server)    │
                            └─────────────────┘
```

**Purpose:** Abstracts data access logic from business logic.

### 6.2 Dependency Injection

```
┌─────────────────────────────────────────────────────────────┐
│                      Program.cs (DI Container)               │
│                                                              │
│  builder.Services.AddSingleton<IUserRepository,             │
│                                 UserRepository>();           │
│  builder.Services.AddSingleton<IUserBusiness,               │
│                                 UserBusiness>();             │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                     Controller Constructor                   │
│                                                              │
│  public UserController(IUserBusiness userBusiness)          │
│  {                                                          │
│      _userBusiness = userBusiness; // Injected              │
│  }                                                          │
└─────────────────────────────────────────────────────────────┘
```

### 6.3 Service Facade Pattern

```
┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│  Controller  │────▶│   Business   │────▶│  Repository  │
│              │     │   (Facade)   │     │              │
└──────────────┘     └──────────────┘     └──────────────┘
```

**Purpose:** Provides a simplified interface to the complex subsystem.

### 6.4 Patterns Summary

| Pattern | Implementation | Location |
|---------|----------------|----------|
| Repository | IRepository + Repository | DataAccess, DataAccessContract |
| Dependency Injection | Constructor Injection | Program.cs, All Controllers |
| Service Facade | Business Layer | Business, BusinessContract |
| Factory (Implicit) | DataTableHelper | platforms/Helpers |
| Command | SqlCommand with SP | DataAccess |

---

## 7. API Endpoints

### 7.1 Authentication

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/login/Validate` | User login |
| POST | `/api/login/SignUp` | User registration |

### 7.2 User Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/user/GetUserList` | List all users |
| GET | `/api/user/GetUserListByComp` | Users by company |
| POST | `/api/user/Save` | Create user |
| POST | `/api/user/Update` | Update user |
| POST | `/api/user/Delete` | Delete user |
| POST | `/api/user/ResetPassword` | Reset password |

### 7.3 Customer Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/customer/CustomersListByCompType` | Filtered customers |
| POST | `/api/customer/Save` | Create customer |
| POST | `/api/customer/Update` | Update customer |
| POST | `/api/customer/Delete` | Delete customer |
| GET | `/api/customer/GetLocation` | Get GPS location |
| POST | `/api/customer/UpdateLocation` | Update GPS location |

### 7.4 Product Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/product/GetProductList` | List products |
| POST | `/api/product/Save` | Create product |
| POST | `/api/product/Update` | Update product |
| POST | `/api/product/Delete` | Delete product |

### 7.5 Field Work

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/fieldworkheader/FWHeaderList` | List field work |
| POST | `/api/fieldworkheader/Save` | Create field work entry |
| POST | `/api/fieldworkheader/Delete` | Delete entry |
| GET | `/api/fieldworkheader/GetFieldWorkSummary` | Summary statistics |

### 7.6 Orders (POB)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/orders/Save` | Create order |
| POST | `/api/orders/SaveMultiple` | Batch orders |
| POST | `/api/orders/Update` | Update order |
| POST | `/api/orders/Delete` | Delete order |
| GET | `/api/orders/GetByTransID` | Orders by transaction |
| GET | `/api/orders/GetByCompany` | Orders by company |

### 7.7 Reports

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/reports/MonthlyReport` | Monthly report |
| POST | `/api/reports/FWEmpMonthly` | Employee monthly |
| POST | `/api/reports/FWYearly` | Yearly report |
| GET | `/api/reports/Download/{compId}/{fileName}` | Download Excel |

### 7.8 Media

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/image/UploadProductImage` | Upload product image |
| POST | `/api/image/UploadLogo` | Upload company logo |
| POST | `/api/image/UploadProfileImage` | Upload profile |
| DELETE | `/api/image/Delete` | Delete image |

---

## 8. Data Models

### 8.1 Class Diagram - Core Models

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              USER DOMAIN                                     │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ┌─────────────────────┐       ┌─────────────────────┐                      │
│  │      UserModel      │       │    LoginUserModel   │                      │
│  ├─────────────────────┤       ├─────────────────────┤                      │
│  │ + CompID: int       │       │ + CompID: string    │                      │
│  │ + UID: int          │       │ + UserID: string    │                      │
│  │ + UserID: string    │       │ + Password: string  │                      │
│  │ + Password: string  │       └─────────────────────┘                      │
│  │ + Name: string      │                                                     │
│  │ + Mobile: string    │       ┌─────────────────────┐                      │
│  │ + Address: string   │       │     SignUpModel     │                      │
│  │ + ProfileImageURL   │       ├─────────────────────┤                      │
│  │ + IsCompAdmin: bool │       │ + UserID: string    │                      │
│  │ + RoleID: int       │       │ + Password: string  │                      │
│  │ + ReportingMgrID    │       │ + Mobile: string    │                      │
│  │ + CreatedBy: int    │       └─────────────────────┘                      │
│  │ + CreatedOn: DateTime│                                                    │
│  │ + ModifiedBy: int   │                                                     │
│  │ + ModifiedOn: DateTime│                                                   │
│  └─────────────────────┘                                                     │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────┐
│                            CUSTOMER DOMAIN                                   │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ┌─────────────────────┐       ┌─────────────────────────────┐              │
│  │   CustomerModel     │       │ CustomerLocationUpdateRequest│              │
│  ├─────────────────────┤       ├─────────────────────────────┤              │
│  │ + CompID: int       │       │ + CompID: int               │              │
│  │ + CustID: int       │       │ + CustID: int               │              │
│  │ + Name: string      │       │ + Latitude: decimal         │              │
│  │ + Type: string      │       │ + Longitude: decimal        │              │
│  │ + QUALIFICATION     │       └─────────────────────────────┘              │
│  │ + Speciality        │                                                     │
│  │ + Address: string   │       ┌─────────────────────────────┐              │
│  │ + Latitude: decimal │       │  CustomerLocationResponse   │              │
│  │ + Longitude: decimal│       ├─────────────────────────────┤              │
│  │ + IsActive: bool    │       │ + CustID: int               │              │
│  │ + Audit Fields...   │       │ + Name: string              │              │
│  └─────────────────────┘       │ + Latitude: decimal         │              │
│                                │ + Longitude: decimal        │              │
│                                └─────────────────────────────┘              │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────┐
│                            PRODUCT DOMAIN                                    │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ┌─────────────────────┐                                                     │
│  │    ProductModel     │                                                     │
│  ├─────────────────────┤                                                     │
│  │ + CompID: int       │                                                     │
│  │ + prodcode: string  │                                                     │
│  │ + proddesc: string  │                                                     │
│  │ + prodtype: string  │                                                     │
│  │ + prodpack: string  │                                                     │
│  │ + prodprice: decimal│  (Stockist Price)                                   │
│  │ + MRP: decimal      │  (Maximum Retail Price)                             │
│  │ + ImageURL: string  │  (R2 Storage URL)                                   │
│  │ + isActive: bool    │                                                     │
│  │ + Audit Fields...   │                                                     │
│  └─────────────────────┘                                                     │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────┐
│                          FIELD WORK DOMAIN                                   │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ┌─────────────────────┐       ┌─────────────────────┐                      │
│  │  FieldWorkHeader    │       │   FieldworkEmpDT    │                      │
│  ├─────────────────────┤       ├─────────────────────┤                      │
│  │ + CompID: int       │       │ + TransID: string   │                      │
│  │ + TransID: string   │◀──────│ + SNo: int          │                      │
│  │ + UID: int          │  1:N  │ + UID: int          │                      │
│  │ + HQcode: string    │       │ + IsActive: bool    │                      │
│  │ + PatchName: string │       └─────────────────────┘                      │
│  │ + CustID: int       │                                                     │
│  │ + Visited: bool     │       ┌─────────────────────┐                      │
│  │ + Remarks: string   │       │   FieldworkProdDT   │                      │
│  │ + Latitude: decimal │       ├─────────────────────┤                      │
│  │ + Longitude: decimal│       │ + TransID: string   │                      │
│  │ + LocationAccuracy  │◀──────│ + SNo: int          │                      │
│  │ + EmpDTs: List<>    │  1:N  │ + Prodcode: string  │                      │
│  │ + ProdDTs: List<>   │       │ + IsActive: bool    │                      │
│  │ + Orders: List<>    │       └─────────────────────┘                      │
│  │ + Audit Fields...   │                                                     │
│  └─────────────────────┘       ┌─────────────────────┐                      │
│           │                    │     OrderModel      │                      │
│           │                    ├─────────────────────┤                      │
│           │               1:N  │ + CompID: int       │                      │
│           └───────────────────▶│ + OrderID: int      │                      │
│                                │ + TransID: string   │                      │
│                                │ + ProductID: int    │                      │
│                                │ + Quantity: int     │                      │
│                                │ + UnitPrice: decimal│                      │
│                                │ + TotalAmount: decimal│                    │
│                                │ + Audit Fields...   │                      │
│                                └─────────────────────┘                      │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────┐
│                           COMMON MODELS                                      │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ┌─────────────────────┐       ┌─────────────────────┐                      │
│  │    ApiResponse<T>   │       │       Lookup        │                      │
│  ├─────────────────────┤       ├─────────────────────┤                      │
│  │ + Success: bool     │       │ + CompID: int       │                      │
│  │ + Message: string   │       │ + code: string      │                      │
│  │ + Data: T           │       │ + description: string│                     │
│  └─────────────────────┘       │ + type: string      │                      │
│                                │ + IsActive: bool    │                      │
│                                └─────────────────────┘                      │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## 9. Database Architecture

### 9.1 Database Details

| Property | Value |
|----------|-------|
| Server | SQL8020.site4now.net |
| Database | db_9f30c5_mcmewastaging |
| Schema | [mcDCR] |
| Access Method | Stored Procedures |

### 9.2 Stored Procedures

| Category | Stored Procedure | Description |
|----------|-----------------|-------------|
| **Auth** | usp_UserLogin | Validate login |
| | usp_UserSignUp | Register user |
| **User** | usp_UserList | Get all users |
| | usp_UserListByCompany | Users by company |
| | usp_UserInsert | Create user |
| | usp_UserUpdate | Update user |
| | usp_UserDelete | Delete user |
| | usp_UpdateUserPassword | Reset password |
| **Customer** | usp_CustomerListByComp_Type | Get customers |
| | usp_CustomerInsert | Create customer |
| | usp_CustomerUpdate | Update customer |
| | usp_CustomerDelete | Delete customer |
| **Product** | usp_ProductListByCompany | Get products |
| | usp_ProductInsert | Create product |
| | usp_ProductUpdate | Update product |
| | usp_ProductDelete | Delete product |
| **Orders** | usp_OrderInsert | Create order |
| | usp_OrderUpdate | Update order |
| | usp_OrderDelete | Delete order |
| | usp_OrderListByTransID | Orders by transaction |
| | usp_OrderListByCompany | Orders by company |
| **Reports** | usp_FWHDMonthlyList | Monthly FW report |
| | usp_FWHDEmpMonthlyList | Employee monthly |
| | usp_FWHDEmpYearlyList | Employee yearly |

---

## 10. External Integrations

### 10.1 Cloudflare R2 Storage

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        R2 Storage Integration                                │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐        │
│  │  ImageController│────▶│ R2StorageService│────▶│  Cloudflare R2  │        │
│  └─────────────────┘     └─────────────────┘     └─────────────────┘        │
│                                   │                                          │
│                                   ▼                                          │
│                          ┌─────────────────┐                                 │
│                          │  ImageSharp     │                                 │
│                          │  (Processing)   │                                 │
│                          └─────────────────┘                                 │
│                                                                              │
│  Image Variants Generated:                                                   │
│  ┌──────────────────────────────────────────────────────────────────┐       │
│  │ Products: Original(2500x2500), Large(1200), Medium(800), Small(400)│     │
│  │ Logos: PNG (1000x1000)                                            │       │
│  │ Icons: PNG (128x128, 64x64, 32x32, 16x16)                        │       │
│  │ Profiles: JPEG (500x500)                                          │       │
│  └──────────────────────────────────────────────────────────────────┘       │
│                                                                              │
│  Bucket: pdimages                                                            │
│  Public URL: https://pub-55994abda41848bca7f90c272fcacbc2.r2.dev            │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 10.2 Excel Report Generation

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        Report Generation Flow                                │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐        │
│  │ReportsController│────▶│ ReportBusiness  │────▶│ReportRepository │        │
│  └─────────────────┘     └─────────────────┘     └─────────────────┘        │
│          │                                                │                  │
│          │                                                ▼                  │
│          │                                       ┌─────────────────┐        │
│          │                                       │   SQL Server    │        │
│          │                                       │ (Data Retrieval)│        │
│          │                                       └─────────────────┘        │
│          │                                                                   │
│          ▼                                                                   │
│  ┌─────────────────┐     ┌─────────────────┐                                │
│  │  Reports.cs     │────▶│   ClosedXML     │                                │
│  │ (PharmaDiaries  │     │ SpreadsheetLight│                                │
│  │  .Utils)        │     └─────────────────┘                                │
│  └─────────────────┘              │                                          │
│                                   ▼                                          │
│                          ┌─────────────────┐                                 │
│                          │  Local Storage  │                                 │
│                          │Reports/{compId}/│                                 │
│                          │   reports/      │                                 │
│                          └─────────────────┘                                 │
│                                   │                                          │
│                                   ▼                                          │
│                          Download via API:                                   │
│                   GET /api/reports/Download/{compId}/{fileName}              │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## 11. Security Architecture

### 11.1 Authentication Flow

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                         Authentication Flow                                  │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ┌──────────┐     ┌──────────┐     ┌──────────┐     ┌──────────┐           │
│  │  Client  │────▶│  Login   │────▶│  Login   │────▶│  Login   │           │
│  │  (App)   │     │Controller│     │ Business │     │Repository│           │
│  └──────────┘     └──────────┘     └──────────┘     └──────────┘           │
│       │                                                   │                  │
│       │ LoginUserModel                                    │                  │
│       │ (CompID, UserID, Password)                       │                  │
│       │                                                   ▼                  │
│       │                                          ┌──────────────┐           │
│       │                                          │  SQL Server  │           │
│       │                                          │usp_UserLogin │           │
│       │                                          └──────────────┘           │
│       │                                                   │                  │
│       │◀──────────────────────────────────────────────────┘                  │
│       │ UserModel (Full user details + role)                                │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 11.2 Multi-Tenancy

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                    Multi-Tenant Data Isolation                               │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  All database queries are filtered by CompID (Company ID)                   │
│                                                                              │
│  ┌───────────────────────────────────────────────────────────────┐          │
│  │                     Single Database                            │          │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐            │          │
│  │  │  Company 1  │  │  Company 2  │  │  Company 3  │            │          │
│  │  │  CompID: 1  │  │  CompID: 2  │  │  CompID: 3  │            │          │
│  │  │             │  │             │  │             │            │          │
│  │  │ - Users     │  │ - Users     │  │ - Users     │            │          │
│  │  │ - Customers │  │ - Customers │  │ - Customers │            │          │
│  │  │ - Products  │  │ - Products  │  │ - Products  │            │          │
│  │  │ - FieldWork │  │ - FieldWork │  │ - FieldWork │            │          │
│  │  └─────────────┘  └─────────────┘  └─────────────┘            │          │
│  └───────────────────────────────────────────────────────────────┘          │
│                                                                              │
│  WHERE CompID = @CompID  (Applied to all queries)                           │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 11.3 Role-Based Access Control

| Component | Purpose |
|-----------|---------|
| RoleHierarchy | Define role levels |
| ScreenModel | Define application screens |
| UserScreenPermission | User-screen access mapping |
| RoleID on UserModel | User's role assignment |

---

## 12. Configuration Management

### 12.1 Application Settings

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ReportSettings": {
    "BaseUrl": "https://pharmaapi-xxx.azurewebsites.net"
  },
  "ConnectionStrings": {
    "APIconnectionString": "..."
  },
  "CloudflareR2": {
    "AccountId": "...",
    "AccessKeyId": "...",
    "SecretAccessKey": "...",
    "BucketName": "pdimages",
    "PublicUrl": "https://pub-xxx.r2.dev"
  }
}
```

### 12.2 Dependency Injection Registration

All services are registered as **Singleton** in Program.cs:

```csharp
// Repository Layer
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
builder.Services.AddSingleton<IProductRepository, ProductRepository>();
// ... more repositories

// Business Layer
builder.Services.AddSingleton<IUserBusiness, UserBusiness>();
builder.Services.AddSingleton<ICustomerBusiness, CustomerBusiness>();
builder.Services.AddSingleton<IProductBusiness, ProductBusiness>();
// ... more business services

// External Services
builder.Services.AddSingleton<IR2StorageService, R2StorageService>();
```

---

## 13. Technical Debt & Recommendations

### 13.1 Current Issues

| Issue | Risk Level | Description |
|-------|------------|-------------|
| Singleton Services | Medium | All services as Singleton can cause thread-safety issues |
| Hardcoded Credentials | High | Connection strings in appsettings.json |
| No Async/Await | Medium | Synchronous operations block threads |
| CORS AllowAnyOrigin | High | Security risk in production |
| No Input Validation | Medium | Missing model validation |
| No Unit Tests | Medium | No test coverage |
| Thin Business Layer | Low | Minimal business logic |

### 13.2 Recommendations

| Priority | Recommendation | Benefit |
|----------|---------------|---------|
| P1 | Move credentials to Azure Key Vault | Security |
| P1 | Restrict CORS origins | Security |
| P2 | Change to Scoped lifetime | Thread safety |
| P2 | Add FluentValidation | Data integrity |
| P2 | Implement async/await | Performance |
| P3 | Add xUnit test project | Quality |
| P3 | Add Serilog logging | Observability |
| P3 | Consider Entity Framework | Maintainability |

---

## Appendix A: File Structure

```
PharmaDiaries/
├── PharmaDiaries.sln
├── PharmaDiariesAPI/
│   ├── Controllers/worktype/
│   │   ├── LoginController.cs
│   │   ├── UserController.cs
│   │   ├── CustomerController.cs
│   │   ├── ProductController.cs
│   │   ├── CompanyController.cs
│   │   ├── FieldWorkEmployeeController.cs
│   │   ├── FieldworkHeaderController.cs
│   │   ├── OrdersController.cs
│   │   ├── ImageController.cs
│   │   ├── ReportsController.cs
│   │   ├── LookupController.cs
│   │   ├── AreasController.cs
│   │   ├── ScreenController.cs
│   │   ├── RoleHierarchyController.cs
│   │   └── DCRDateRequestController.cs
│   ├── Services/
│   │   └── R2StorageService.cs
│   ├── Program.cs
│   └── appsettings.json
├── PharmaDiaries.BusinessContract/
│   └── [All Business Interfaces]
├── PharmaDiaries.Bussiness/
│   └── [All Business Implementations]
├── PharmaDiaries.DataAccessContract/
│   └── Repository/[All Repository Interfaces]
├── PharmaDiaries.DataAccess/
│   └── [All Repository Implementations]
├── PharmaDiaries.Models/
│   └── [All Data Models]
├── PharmaDiaries.platforms/
│   └── Helpers/
│       ├── SqlHelper.cs
│       └── DataTableHelper.cs
└── PharmaDiaries.Utils/
    └── Reports.cs
```

---

**Document Generated:** February 2026
**Solution Version:** .NET 7.0
**Author:** Architecture Analysis Tool
