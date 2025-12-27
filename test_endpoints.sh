#!/bin/bash

# ========================================================================================================================================
# API Endpoint Test Script
# Run this after executing URGENT_FIX_SCRIPT.sql and restarting the API
# ========================================================================================================================================

API_BASE_URL="http://localhost:7085"

echo "========================================="
echo "Testing PharmaDiaries API Endpoints"
echo "========================================="
echo ""

# Test 1: GetUserListByComp - Check if isCompAdmin is not null
echo "Test 1: Testing GetUserListByComp endpoint..."
echo "URL: ${API_BASE_URL}/api/User/GetUserListByComp?compid=2000"
echo ""
response=$(curl -s -w "\nHTTP_STATUS:%{http_code}" "${API_BASE_URL}/api/User/GetUserListByComp?compid=2000")
http_status=$(echo "$response" | grep "HTTP_STATUS" | cut -d: -f2)
body=$(echo "$response" | sed '/HTTP_STATUS/d')

echo "Response:"
echo "$body" | python3 -m json.tool 2>/dev/null || echo "$body"
echo ""

if [ "$http_status" == "200" ]; then
    echo "✅ Status: SUCCESS (200)"

    # Check if isCompAdmin is present and not null
    if echo "$body" | grep -q '"isCompAdmin"'; then
        if echo "$body" | grep -q '"isCompAdmin":null'; then
            echo "❌ FAILED: isCompAdmin is still NULL"
        else
            echo "✅ PASSED: isCompAdmin field present with value"
        fi
    else
        echo "❌ FAILED: isCompAdmin field not found in response"
    fi
else
    echo "❌ Status: FAILED ($http_status)"
fi

echo ""
echo "========================================="
echo ""

# Test 2: Get Screens - Check if it returns without error
echo "Test 2: Testing Screen GET endpoint..."
echo "URL: ${API_BASE_URL}/api/Screen"
echo ""
response=$(curl -s -w "\nHTTP_STATUS:%{http_code}" "${API_BASE_URL}/api/Screen")
http_status=$(echo "$response" | grep "HTTP_STATUS" | cut -d: -f2)
body=$(echo "$response" | sed '/HTTP_STATUS/d')

echo "Response:"
echo "$body" | python3 -m json.tool 2>/dev/null || echo "$body"
echo ""

if [ "$http_status" == "200" ]; then
    echo "✅ Status: SUCCESS (200)"
    echo "✅ PASSED: Screen API is working (no runtime error)"
else
    echo "❌ Status: FAILED ($http_status)"
    echo "❌ FAILED: Screen API returned error"
fi

echo ""
echo "========================================="
echo ""

# Test 3: Sync Screens (Optional - will add test data)
echo "Test 3: Testing Screen Sync endpoint..."
echo "URL: ${API_BASE_URL}/api/screen/sync"
echo ""

read -p "Do you want to test screen sync? This will add test data. (y/n): " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    response=$(curl -s -w "\nHTTP_STATUS:%{http_code}" \
        -X POST "${API_BASE_URL}/api/screen/sync" \
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
        ]')

    http_status=$(echo "$response" | grep "HTTP_STATUS" | cut -d: -f2)
    body=$(echo "$response" | sed '/HTTP_STATUS/d')

    echo "Response:"
    echo "$body" | python3 -m json.tool 2>/dev/null || echo "$body"
    echo ""

    if [ "$http_status" == "200" ]; then
        echo "✅ Status: SUCCESS (200)"
        echo "✅ PASSED: Screens synced successfully"

        # Verify screens were added
        echo ""
        echo "Verifying screens were added..."
        response=$(curl -s "${API_BASE_URL}/api/Screen")
        echo "$response" | python3 -m json.tool 2>/dev/null || echo "$response"
    else
        echo "❌ Status: FAILED ($http_status)"
        echo "❌ FAILED: Screen sync failed"
    fi
else
    echo "⏭️  Skipped screen sync test"
fi

echo ""
echo "========================================="
echo ""

# Summary
echo "Test Summary:"
echo "============="
echo "1. GetUserListByComp: Check results above"
echo "2. Screen GET: Check results above"
echo "3. Screen Sync: Check results above (if tested)"
echo ""
echo "If all tests passed, your API is working correctly!"
echo ""
