# Azure Data Studio - Quick Guide to Fix API Issues

## Step-by-Step Instructions

### Step 1: Open Azure Data Studio

1. **Launch Azure Data Studio**
   - Open the application from your Applications folder or Launchpad

2. **Connect to Your Database** (if not already connected)
   - Click "New Connection" or use existing connection
   - Server: Your SQL Server address
   - Authentication: SQL Login or Windows Authentication
   - Username & Password: Your credentials
   - Database: Select your PharmaDiaries database
   - Click "Connect"

---

### Step 2: Open the Fix Script

1. **Open File**
   - Click: **File** → **Open File** (or press `Cmd+O` on Mac)
   - Navigate to: `/Users/sharma/Desktop/chinna/Pharma/Develop/API/Pharmadiarie/`
   - Select: **URGENT_FIX_SCRIPT.sql**
   - Click **Open**

2. **Verify Connection**
   - Look at the bottom-right corner of Azure Data Studio
   - Should show: Connected to your database
   - If not connected, click the connection button and select your database

3. **Select Correct Database**
   - At the top toolbar, you'll see a database dropdown
   - Make sure it's set to your PharmaDiaries database (the one with McMaster schema)

---

### Step 3: Execute the Script

1. **Review the Script** (Optional)
   - Scroll through to see what it will do
   - It will:
     - Fix `usp_UserListByCompany`
     - Create 4 new stored procedures

2. **Run the Script**
   - Click the **"Run"** button (green play icon)
   - Or press **F5**
   - Or right-click and select "Run"

3. **Wait for Completion**
   - You'll see messages in the "Messages" tab at the bottom
   - Should complete in a few seconds

4. **Check Results**
   - Look for: **"Stored procedures created successfully!"**
   - The "Messages" tab will show execution details
   - Should see verification query results

**Expected Output:**
```
Stored procedures created successfully!

Verification:
ROUTINE_SCHEMA | ROUTINE_NAME
McMaster       | usp_GetActiveScreens
McMaster       | usp_GetUserScreenPermissions
McMaster       | usp_SaveUserScreenPermissions
McMaster       | usp_ScreenSync
mcDCR          | usp_UserListByCompany
```

---

### Step 4: Verify Database Changes (Recommended)

1. **Open Verification Script**
   - File → Open File
   - Select: **VERIFY_DATABASE.sql**
   - Click **Open**

2. **Run Verification**
   - Click **"Run"** button or press **F5**
   - This will check that everything was created correctly

3. **Review Results**
   - Should see ✅ success messages for:
     - Tables exist
     - isCompAdmin column exists
     - All 6 stored procedures exist
     - No NULL values issues

---

### Step 5: Quick Test from Azure Data Studio

Before testing the API, you can test the stored procedures directly:

**Test 1: Test usp_UserListByCompany**
```sql
-- Open a new query window (File → New Query)
-- Run this query:
EXEC [mcDCR].[usp_UserListByCompany] @compid = 2000
```

**Expected Result:**
- You should see a result set with user data
- **IMPORTANT:** Check that `isCompAdmin` column appears in the results
- Values should be 0 or 1 (not NULL)

**Test 2: Test Screen Procedures**
```sql
-- Test getting active screens (will be empty initially)
EXEC [McMaster].[usp_GetActiveScreens]
```

**Expected Result:**
- Empty result set (no error)
- This confirms the procedure exists and works

---

## Troubleshooting in Azure Data Studio

### Issue: "Cannot open database requested by the login"
**Solution:**
1. Click on the connection name at bottom-right
2. Select "Change Connection"
3. Choose the correct database from dropdown
4. Click "Connect"

### Issue: "Invalid object name"
**Solution:**
1. Check you're connected to the right database server
2. Verify database name matches where your tables exist
3. Check connection dropdown at top of query window

### Issue: Script doesn't run / No output
**Solution:**
1. Make sure the query window is active (click in it)
2. Check connection status at bottom-right (should show green dot)
3. Try right-click → "Run" instead of using F5

### Issue: "Stored procedure already exists"
**Solution:**
This is fine! The script handles this with DROP IF EXISTS
If you see errors, they're likely warnings, not failures
Check the final verification query results

---

## After Successful Execution

Once you see "Stored procedures created successfully!" message:

### Next: Restart Your API

**If API is running in Terminal:**
1. Go to the terminal where API is running
2. Press `Ctrl+C` to stop
3. Restart:
   ```bash
   cd "/Users/sharma/Desktop/chinna/Pharma/Develop/API/Pharmadiarie/PharmaDiariesAPI"
   dotnet run
   ```

**If API is running in Visual Studio:**
1. Click "Stop" button or press Shift+F5
2. Click "Start" button or press F5

**If API is running as a service:**
```bash
sudo systemctl restart your-api-service
```

### Then: Test the API Endpoints

**Automated Testing (Recommended):**
```bash
# Open new terminal
cd "/Users/sharma/Desktop/chinna/Pharma/Develop/API/Pharmadiarie"
./test_endpoints.sh
```

**Manual Testing:**

1. **Test GetUserListByComp:**
   - Open browser or use curl
   - URL: `http://localhost:7085/api/User/GetUserListByComp?compid=2000`
   - Check response for `"isCompAdmin": true` or `false` (NOT null)

2. **Test Screen API:**
   - URL: `http://localhost:7085/api/Screen`
   - Should return: `[]` (empty array, not error)

---

## Visual Guide - What to Look For

### ✅ Success Indicators in Azure Data Studio:

**Messages Tab:**
```
Command(s) completed successfully.
(4 rows affected)
Stored procedures created successfully!
```

**Results Tab:**
```
ROUTINE_SCHEMA    ROUTINE_NAME
McMaster          usp_GetActiveScreens
McMaster          usp_GetUserScreenPermissions
...
```

### ❌ Error Indicators:

**Red text in Messages:**
```
Msg 207, Level 16, State 1
Invalid column name 'isCompAdmin'
```
→ This means the script didn't run completely. Try again.

**No results in verification:**
```
(0 rows affected)
```
→ Stored procedures weren't created. Check connection.

---

## Quick Reference - Azure Data Studio Shortcuts

| Action | Mac Shortcut | Windows Shortcut |
|--------|-------------|------------------|
| Open File | Cmd+O | Ctrl+O |
| New Query | Cmd+N | Ctrl+N |
| Run Query | F5 or Cmd+Shift+E | F5 or Ctrl+Shift+E |
| Save File | Cmd+S | Ctrl+S |
| Find | Cmd+F | Ctrl+F |

---

## Need Help?

If you see any errors during execution:

1. **Take a screenshot** of the error in Azure Data Studio
2. **Copy the error text** from the Messages tab
3. **Note which step** you're on
4. **Tell me** and I'll help you resolve it immediately

---

## You're Ready!

**Current Status:**
- ✅ Azure Data Studio is your SQL tool
- ✅ Fix script is ready to execute
- ✅ Verification script is ready
- ✅ Test script is ready

**Next Action:**
1. Open Azure Data Studio
2. Connect to your database
3. Open URGENT_FIX_SCRIPT.sql
4. Click Run (F5)
5. Look for success message
6. Tell me the result!

Let me know once you've executed the script and what messages you see!
