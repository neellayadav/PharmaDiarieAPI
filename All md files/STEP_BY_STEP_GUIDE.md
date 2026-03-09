# Step-by-Step Guide to Fix API Issues

## Prerequisites
- SQL Server Management Studio (SSMS) or Azure Data Studio installed
- Access to your database server
- API currently running on `http://localhost:7085`

---

## Step 1: Execute SQL Script

### Option A: Using SQL Server Management Studio (SSMS)

1. **Open SSMS**
   - Connect to your SQL Server instance

2. **Open the Script File**
   - File → Open → File
   - Navigate to: `/Users/sharma/Desktop/chinna/Pharma/Develop/API/Pharmadiarie/URGENT_FIX_SCRIPT.sql`
   - Click Open

3. **Select Database**
   - In the toolbar, select your database from the dropdown
   - (Should be the same database where McMaster.Users table exists)

4. **Execute Script**
   - Click "Execute" button (or press F5)
   - Wait for completion message

5. **Verify Results**
   - You should see: "Stored procedures created successfully!"
   - Check Messages tab for any errors

### Option B: Using Azure Data Studio

1. **Open Azure Data Studio**
   - Connect to your SQL Server instance

2. **Open the Script File**
   - File → Open File
   - Navigate to: `/Users/sharma/Desktop/chinna/Pharma/Develop/API/Pharmadiarie/URGENT_FIX_SCRIPT.sql`
   - Click Open

3. **Select Connection**
   - Ensure you're connected to the correct database

4. **Execute Script**
   - Click "Run" button (or press F5)
   - Wait for completion

5. **Verify Results**
   - Check output panel for success messages

### Option C: Using sqlcmd (Command Line)

```bash
# Replace <server>, <database>, <username>, <password> with your values
sqlcmd -S <server> -d <database> -U <username> -P <password> -i "/Users/sharma/Desktop/chinna/Pharma/Develop/API/Pharmadiarie/URGENT_FIX_SCRIPT.sql"
```

---

## Step 2: Verify Database Changes

After executing the script, run these verification queries:

### Verify usp_UserListByCompany includes isCompAdmin
```sql
-- Check the procedure definition
SELECT OBJECT_DEFINITION(OBJECT_ID('[mcDCR].[usp_UserListByCompany]'))
```
**Expected:** Should contain `[isCompAdmin]` in the SELECT statement

### Verify Screen Stored Procedures Exist
```sql
-- List all screen-related procedures
SELECT
    ROUTINE_SCHEMA,
    ROUTINE_NAME,
    CREATED
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_TYPE = 'PROCEDURE'
  AND ROUTINE_NAME IN (
      'usp_ScreenSync',
      'usp_GetActiveScreens',
      'usp_SaveUserScreenPermissions',
      'usp_GetUserScreenPermissions'
  )
ORDER BY ROUTINE_NAME
```
**Expected:** Should return 4 rows

### Test Stored Procedures Directly
```sql
-- Test 1: Get active screens (should return empty or existing screens)
EXEC [McMaster].[usp_GetActiveScreens]

-- Test 2: Test usp_UserListByCompany with your compid
EXEC [mcDCR].[usp_UserListByCompany] @compid = 2000
-- Verify isCompAdmin column appears in results
```

---

## Step 3: Restart API

### If running in Visual Studio:
1. Stop debugging (Shift + F5)
2. Start debugging again (F5)

### If running via dotnet CLI:
```bash
# Stop the process (Ctrl+C)
cd "/Users/sharma/Desktop/chinna/Pharma/Develop/API/Pharmadiarie/PharmaDiariesAPI"
dotnet run
```

### If running as a service:
```bash
# Restart your API service
sudo systemctl restart pharmadiaries-api
# (or however you've configured it)
```

---

## Step 4: Test API Endpoints

### Test 1: Verify isCompAdmin Fix

**Using Browser:**
```
http://localhost:7085/api/User/GetUserListByComp?compid=2000
```

**Using curl:**
```bash
curl -X GET "http://localhost:7085/api/User/GetUserListByComp?compid=2000"
```

**Expected Response:**
```json
[
  {
    "compID": 2000,
    "uid": 101,
    "userID": "user001",
    "name": "John Doe",
    "isCompAdmin": true,  // ← Should be true or false, NOT null
    ...
  }
]
```

**✅ Success Criteria:** `isCompAdmin` shows `true` or `false` (NOT `null`)

---

### Test 2: Verify Screen API Fix

**Using Browser:**
```
http://localhost:7085/api/Screen
```

**Using curl:**
```bash
curl -X GET "http://localhost:7085/api/Screen"
```

**Expected Response:**
```json
[]
```
(Empty array is fine - means no screens synced yet)

**✅ Success Criteria:** Returns `[]` without errors (not 500 error)

---

### Test 3: Sync Screens (Optional)

**Using curl:**
```bash
curl -X POST "http://localhost:7085/api/screen/sync" \
  -H "Content-Type: application/json" \
  -d '[
    {
      "screenName": "Dashboard",
      "screenRoute": "/dashboard",
      "screenDescription": "Main dashboard screen",
      "isActive": true
    },
    {
      "screenName": "Employee List",
      "screenRoute": "/employees",
      "screenDescription": "View all employees",
      "isActive": true
    }
  ]'
```

**Expected Response:**
```json
{
  "success": true,
  "message": "Screens synced successfully"
}
```

Then verify:
```bash
curl -X GET "http://localhost:7085/api/Screen"
```

Should return the 2 screens you just synced.

---

### Test 4: Save User Permissions (Optional)

**Using curl:**
```bash
# First, get screen IDs from Test 3 above
# Then save permissions for a user (replace userID 101 with actual user)

curl -X POST "http://localhost:7085/api/screen/users/101/permissions" \
  -H "Content-Type: application/json" \
  -d '{
    "userID": 101,
    "screenIDs": [1, 2],
    "createdBy": 1
  }'
```

**Expected Response:**
```json
{
  "success": true,
  "message": "Permissions saved successfully"
}
```

Then verify:
```bash
curl -X GET "http://localhost:7085/api/screen/users/101/permissions"
```

Should return the screens assigned to user 101.

---

## Troubleshooting

### Issue: "Invalid object name 'McMaster.ScreenMaster'"
**Solution:** Make sure you ran the table creation scripts first. Check if tables exist:
```sql
SELECT * FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME IN ('ScreenMaster', 'UserScreenPermissions')
```

### Issue: "Could not find stored procedure"
**Solution:**
1. Verify you executed URGENT_FIX_SCRIPT.sql
2. Check you're connected to the correct database
3. Run verification query from Step 2

### Issue: Still getting NULL for isCompAdmin
**Solution:**
1. Verify the ALTER PROC executed successfully
2. Check if data in table has NULL values:
```sql
SELECT UID, Name, isCompAdmin FROM McMaster.Users WHERE CompID = 2000
```
3. Update NULL values if needed:
```sql
UPDATE McMaster.Users SET isCompAdmin = 0 WHERE isCompAdmin IS NULL
```

### Issue: Screen API still returns error
**Solution:**
1. Check API logs for specific error
2. Verify all 4 stored procedures exist
3. Restart API again
4. Check database connection string in appsettings.json

---

## Success Checklist

After completing all steps, verify:

- [ ] SQL script executed without errors
- [ ] 4 new stored procedures created (verified in Step 2)
- [ ] usp_UserListByCompany updated with isCompAdmin
- [ ] API restarted successfully
- [ ] GET /api/User/GetUserListByComp returns isCompAdmin as true/false
- [ ] GET /api/Screen returns [] without error
- [ ] (Optional) POST /api/screen/sync works
- [ ] (Optional) POST /api/screen/users/{id}/permissions works

---

## What to Do After Success

1. **Update Production Database**
   - Save URGENT_FIX_SCRIPT.sql for production deployment
   - Execute on production server when ready

2. **Test Flutter App**
   - The Flutter app can now receive isCompAdmin correctly
   - Screen permission features can be tested

3. **Monitor API Logs**
   - Check for any other issues
   - Verify all endpoints working correctly

---

## Need Help?

If you encounter any errors during execution:
1. Copy the exact error message
2. Note which step you're on
3. Check the SQL output/messages
4. Share with me and I'll help resolve it

---

**Ready? Start with Step 1! Let me know when you've completed the SQL script execution and I'll help with the testing.**
