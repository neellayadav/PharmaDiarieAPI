# API Test Results - Wed Nov 19 14:46:58 IST 2025

**Environment:** Staging
**API Base URL:** http://localhost:7085

---

## Test: Get User List By Company (compid=0)

**Method:** GET
**Endpoint:** /api/User/GetUserListByComp?compid=0

**Response:**
**Status Code:** 200

```json
[
    {
        "compID": 0,
        "uid": 216,
        "userID": "USRNCRY",
        "password": "12345678",
        "name": "string",
        "headQuater": "string",
        "address1": null,
        "locality": null,
        "cityOrTown": null,
        "pincode": null,
        "district": null,
        "state": null,
        "country": null,
        "mobile": "string",
        "telephone": null,
        "isActive": true,
        "isCompAdmin": false,
        "createdBy": null,
        "createdOn": null,
        "modifiedBy": null,
        "modifiedOn": null
    }
]
```

**Result:** ✅ SUCCESS

---

## Test: Get User List By Company (compid=2000)

**Method:** GET
**Endpoint:** /api/User/GetUserListByComp?compid=2000

**Response:**
**Status Code:** 200

```json
[
    {
        "compID": 2000,
        "uid": 101,
        "userID": "McM003",
        "password": "0712",
        "name": "RAVINDER SAMA",
        "headQuater": "SECUNDERABAD",
        "address1": "BORABANDA",
        "locality": "",
        "cityOrTown": "",
        "pincode": 500047,
        "district": "",
        "state": "",
        "country": "",
        "mobile": "9090909099",
        "telephone": "",
        "isActive": true,
        "isCompAdmin": true,
        "createdBy": null,
        "createdOn": null,
        "modifiedBy": null,
        "modifiedOn": null
    },
    {
        "compID": 2000,
        "uid": 102,
        "userID": "McM001",
        "password": "McM001",
        "name": "N T RAO",
        "headQuater": "SECUNDERABAD",
        "address1": "BORABANDA",
        "locality": "",
        "cityOrTown": "",
        "pincode": 500047,
        "district": "",
        "state": "",
        "country": "",
        "mobile": "9848129079",
        "telephone": "",
        "isActive": true,
        "isCompAdmin": true,
        "createdBy": null,
        "createdOn": null,
        "modifiedBy": null,
        "modifiedOn": null
    },
    {
        "compID": 2000,
        "uid": 103,
        "userID": "McM037",
        "password": "McM037",
        "name": "Vanga Venkateswarlu",
        "headQuater": "NELLORE",
        "address1": null,
        "locality": null,
        "cityOrTown": null,
        "pincode": null,
        "district": null,
        "state": null,
        "country": null,
        "mobile": "9177887991",
        "telephone": null,
        "isActive": true,
        "isCompAdmin": true,
        "createdBy": null,
        "createdOn": null,
        "modifiedBy": null,
        "modifiedOn": null
    },
    {
        "compID": 2000,
        "uid": 104,
        "userID": "McM010",
        "password": "McM010",
        "name": "SriKanth Gaja",
        "headQuater": "KURNOOL I",
        "address1": null,
        "locality": null,
        "cityOrTown": null,
        "pincode": null,
        "district": null,
        "state": null,
        "country": null,
        "mobile": "7207911696",
        "telephone": null,
        "isActive": true,
        "isCompAdmin": true,
        "createdBy": null,
        "createdOn": null,
        "modifiedBy": null,
        "modifiedOn": null
    },
    {
        "compID": 2000,
        "uid": 199,
        "userID": "McM041",
        "password": "McM041",
        "name": "Kuruva Mahesh",
        "headQuater": "KURNOOL I",
        "address1": null,
        "locality": null,
        "cityOrTown": null,
        "pincode": null,
        "district": null,
        "state": null,
        "country": null,
        "mobile": "7207905701",
        "telephone": null,
        "isActive": true,
        "isCompAdmin": false,
        "createdBy": null,
        "createdOn": null,
        "modifiedBy": null,
        "modifiedOn": null
    },
    {
        "compID": 2000,
        "uid": 200,
        "userID": "McM031",
        "password": "lyca",
        "name": "Emmadi Kishore Kumar",
        "headQuater": "NIZAMABAD",
        "address1": null,
        "locality": null,
        "cityOrTown": null,
        "pincode": null,
        "district": null,
        "state": null,
        "country": null,
        "mobile": "7207911695",
        "telephone": null,
        "isActive": true,
        "isCompAdmin": false,
        "createdBy": null,
        "createdOn": null,
        "modifiedBy": null,
        "modifiedOn": null
    },
    {
        "compID": 2000,
        "uid": 206,
        "userID": "McM046",
        "password": "McM046",
        "name": "Ponnam Lakshman",
        "headQuater": "DILSUKHNAGAR",
        "address1": null,
        "locality": null,
        "cityOrTown": null,
        "pincode": null,
        "district": null,
        "state": null,
        "country": null,
        "mobile": "7207911700",
        "telephone": null,
        "isActive": true,
        "isCompAdmin": false,
        "createdBy": null,
        "createdOn": null,
        "modifiedBy": null,
        "modifiedOn": null
    },
    {
        "compID": 2000,
        "uid": 207,
        "userID": "McM033",
        "password": "McM033",
        "name": "Sama Pravalika",
        "headQuater": "SECUNDERABAD",
        "address1": null,
        "locality": null,
        "cityOrTown": null,
        "pincode": null,
        "district": null,
        "state": null,
        "country": null,
        "mobile": "8978426466",
        "telephone": null,
        "isActive": true,
        "isCompAdmin": false,
        "createdBy": null,
        "createdOn": null,
        "modifiedBy": null,
        "modifiedOn": null
    },
    {
        "compID": 2000,
        "uid": 209,
        "userID": "McM050",
        "password": "1997",
        "name": "Raju Jorka",
        "headQuater": "NALGONDA",
        "address1": "NALGONDA",
        "locality": null,
        "cityOrTown": null,
        "pincode": null,
        "district": "NALGONDA",
        "state": "TELANGANA",
        "country": "INDIA",
        "mobile": "7207911692",
        "telephone": null,
        "isActive": true,
        "isCompAdmin": false,
        "createdBy": null,
        "createdOn": null,
        "modifiedBy": null,
        "modifiedOn": null
    },
    {
        "compID": 2000,
        "uid": 215,
        "userID": "McM051",
        "password": "venky123",
        "name": "Venkatesh",
        "headQuater": "TIRUPATHI",
        "address1": "1-57, Akkurthy, ",
        "locality": "Srikalahasthi ",
        "cityOrTown": "",
        "pincode": 517536,
        "district": "Chittoor ",
        "state": "Andhra Pradesh",
        "country": "India",
        "mobile": "7207181679",
        "telephone": null,
        "isActive": true,
        "isCompAdmin": false,
        "createdBy": null,
        "createdOn": null,
        "modifiedBy": null,
        "modifiedOn": null
    }
]
```

**Result:** ✅ SUCCESS

---

## Test: Get All Active Screens

**Method:** GET
**Endpoint:** /api/Screen

**Response:**
**Status Code:** 200

```json
[]
```

**Result:** ✅ SUCCESS

---

## Test: Sync Screens to Database

**Method:** POST
**Endpoint:** /api/screen/sync
**Request Body:**
```json
[
    {
        "screenName": "Dashboard",
        "screenRoute": "/dashboard",
        "screenDescription": "Main dashboard screen",
        "isActive": true
    },
    {
        "screenName": "Employee List",
        "screenRoute": "/employees",
        "screenDescription": "View and manage employees",
        "isActive": true
    },
    {
        "screenName": "Daily Call Report",
        "screenRoute": "/dcr",
        "screenDescription": "Daily call reporting",
        "isActive": true
    },
    {
        "screenName": "Products",
        "screenRoute": "/products",
        "screenDescription": "Product management",
        "isActive": true
    },
    {
        "screenName": "Customers",
        "screenRoute": "/customers",
        "screenDescription": "Customer management",
        "isActive": true
    }
]
```

**Response:**
**Status Code:** 200

```json
true
```

**Result:** ✅ SUCCESS

---

## Test: Get All Active Screens (After Sync)

**Method:** GET
**Endpoint:** /api/Screen

**Response:**
**Status Code:** 200

```json
[
    {
        "screenID": 1,
        "screenName": "Dashboard",
        "screenRoute": "/dashboard",
        "screenDescription": "Main dashboard screen",
        "isActive": true,
        "createdDate": "2025-11-19T01:17:04.64"
    },
    {
        "screenID": 2,
        "screenName": "Employee List",
        "screenRoute": "/employees",
        "screenDescription": "View and manage employees",
        "isActive": true,
        "createdDate": "2025-11-19T01:17:05.373"
    },
    {
        "screenID": 3,
        "screenName": "Daily Call Report",
        "screenRoute": "/dcr",
        "screenDescription": "Daily call reporting",
        "isActive": true,
        "createdDate": "2025-11-19T01:17:05.667"
    },
    {
        "screenID": 4,
        "screenName": "Products",
        "screenRoute": "/products",
        "screenDescription": "Product management",
        "isActive": true,
        "createdDate": "2025-11-19T01:17:05.96"
    },
    {
        "screenID": 5,
        "screenName": "Customers",
        "screenRoute": "/customers",
        "screenDescription": "Customer management",
        "isActive": true,
        "createdDate": "2025-11-19T01:17:06.25"
    }
]
```

**Result:** ✅ SUCCESS

---

## Test: Save User Screen Permissions (UserID=216)

**Method:** POST
**Endpoint:** /api/screen/users/216/permissions
**Request Body:**
```json
{
    "userID": 216,
    "screenIDs": [
        1,
        2,
        3
    ],
    "createdBy": 1
}
```

**Response:**
**Status Code:** 200

```json
true
```

**Result:** ✅ SUCCESS

---

## Test: Get User Screen Permissions (UserID=216)

**Method:** GET
**Endpoint:** /api/screen/users/216/permissions

**Response:**
**Status Code:** 200

```json
[
    {
        "permissionID": 1,
        "userID": 216,
        "screenID": 1,
        "hasAccess": true,
        "createdBy": null,
        "createdOn": null,
        "modifiedBy": null,
        "modifiedOn": null
    },
    {
        "permissionID": 2,
        "userID": 216,
        "screenID": 2,
        "hasAccess": true,
        "createdBy": null,
        "createdOn": null,
        "modifiedBy": null,
        "modifiedOn": null
    },
    {
        "permissionID": 3,
        "userID": 216,
        "screenID": 3,
        "hasAccess": true,
        "createdBy": null,
        "createdOn": null,
        "modifiedBy": null,
        "modifiedOn": null
    }
]
```

**Result:** ✅ SUCCESS

---


---

## Summary

All API tests completed. Review results above.

**Test Date:** Wed Nov 19 14:47:08 IST 2025
**Total Tests:** 7
