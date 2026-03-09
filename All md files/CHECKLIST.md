# API Fix Checklist ✅

Use this checklist to track your progress:

---

## Phase 1: Database Fixes (Azure Data Studio)

### Step 1: Open and Execute Fix Script
- [ ] Open Azure Data Studio
- [ ] Connect to PharmaDiaries database
- [ ] Open file: `URGENT_FIX_SCRIPT.sql`
- [ ] Verify correct database is selected (dropdown at top)
- [ ] Click Run (F5)
- [ ] See message: "Stored procedures created successfully!"

**Status:** ⬜ Not Started | ⏳ In Progress | ✅ Completed | ❌ Error

---

### Step 2: Verify Database Changes
- [ ] Open file: `VERIFY_DATABASE.sql`
- [ ] Run the script (F5)
- [ ] Check results show:
  - [ ] ✅ isCompAdmin column exists
  - [ ] ✅ All 6 stored procedures exist
  - [ ] ✅ No NULL values in isCompAdmin

**Status:** ⬜ Not Started | ⏳ In Progress | ✅ Completed | ❌ Error

---

### Step 3: Test Stored Procedures Directly (Optional)
- [ ] Open new query in Azure Data Studio
- [ ] Run: `EXEC [mcDCR].[usp_UserListByCompany] @compid = 2000`
- [ ] Verify `isCompAdmin` column appears in results
- [ ] Run: `EXEC [McMaster].[usp_GetActiveScreens]`
- [ ] Verify no errors (empty result is fine)

**Status:** ⬜ Not Started | ⏳ In Progress | ✅ Completed | ❌ Error

---

## Phase 2: API Restart

### Step 4: Restart API
Choose your method:

**Terminal:**
- [ ] Stop API (Ctrl+C)
- [ ] Restart: `dotnet run` in PharmaDiariesAPI folder
- [ ] Wait for "Now listening on: http://localhost:7085"

**Visual Studio:**
- [ ] Stop debugging (Shift+F5)
- [ ] Start debugging (F5)
- [ ] Wait for API to start

**Status:** ⬜ Not Started | ⏳ In Progress | ✅ Completed | ❌ Error

---

## Phase 3: API Testing

### Step 5: Automated Testing (Recommended)
- [ ] Open new Terminal window
- [ ] Navigate to: `cd "/Users/sharma/Desktop/chinna/Pharma/Develop/API/Pharmadiarie"`
- [ ] Run: `./test_endpoints.sh`
- [ ] Check results:
  - [ ] Test 1 (GetUserListByComp): ✅ PASSED
  - [ ] Test 2 (Screen API): ✅ PASSED

**Status:** ⬜ Not Started | ⏳ In Progress | ✅ Completed | ❌ Error

---

### Step 6: Manual Testing (Alternative)

**Test 1: GetUserListByComp**
- [ ] Open browser: `http://localhost:7085/api/User/GetUserListByComp?compid=2000`
- [ ] Check response has `"isCompAdmin": true` or `false` (NOT null)
- [ ] Status: ✅ Pass | ❌ Fail

**Test 2: Screen API**
- [ ] Open browser: `http://localhost:7085/api/Screen`
- [ ] Check response is `[]` (empty array, no error)
- [ ] Status: ✅ Pass | ❌ Fail

---

## Phase 4: Optional - Test Screen Sync

### Step 7: Sync Test Screens
- [ ] Use Postman, curl, or test script
- [ ] POST to: `http://localhost:7085/api/screen/sync`
- [ ] Send JSON with test screens
- [ ] Verify: GET `http://localhost:7085/api/Screen` returns synced screens

**Status:** ⬜ Not Started | ⏳ In Progress | ✅ Completed | ❌ Error

---

## Final Verification

### All Systems Check
- [ ] Database: isCompAdmin column exists and returns values
- [ ] Database: All 4 screen stored procedures exist
- [ ] API: GetUserListByComp returns isCompAdmin (not null)
- [ ] API: Screen endpoint works without error
- [ ] Documentation: All fix files saved for production

---

## If Everything Passes ✅

You're ready for:
- [ ] Deploy to staging server
- [ ] Test with Flutter app
- [ ] Schedule production deployment
- [ ] Apply fixes to production database

---

## If You Encounter Errors ❌

**Error in Azure Data Studio:**
1. Copy the exact error message
2. Take screenshot
3. Tell me: "Error in Step X: [error message]"

**Error in API Testing:**
1. Check API logs/console
2. Copy the error
3. Tell me: "API test failed: [error]"

**I'll help you fix it immediately!**

---

## Quick Status Check

Where are you now? Mark your current position:

- [ ] 📍 Haven't started yet
- [ ] 📍 Opened Azure Data Studio
- [ ] 📍 Executed URGENT_FIX_SCRIPT.sql
- [ ] 📍 Verified database changes
- [ ] 📍 Restarted API
- [ ] 📍 Testing endpoints
- [ ] 📍 All tests passed! ✅
- [ ] 📍 Encountered an error (need help)

---

## Time Estimate

If everything goes smoothly:
- Azure Data Studio execution: **2-3 minutes**
- Verification: **1-2 minutes**
- API restart: **1 minute**
- Testing: **2-3 minutes**
- **Total: ~7-10 minutes**

---

## Contact Point

Tell me:
✅ "Step X completed successfully"
❌ "Error at Step X: [describe issue]"
❓ "Question about Step X: [your question]"

Let's fix these API issues together! 🚀
