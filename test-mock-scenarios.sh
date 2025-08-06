#!/bin/bash

# Test script for stripe-mock scenarios
# Make sure stripe-mock is running: docker-compose -f docker-compose.stripe-mock.yml up -d

BASE_URL="http://localhost:5000/api/payments"
JWT_TOKEN="your-jwt-token-here"  # Replace with actual JWT token

echo "🧪 Testing Stripe Mock Scenarios"
echo "=================================="

# Test 1: Create a successful payment
echo "1. Testing successful payment scenario..."
curl -X POST "$BASE_URL/create" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -d '{
    "amount": 10.00,
    "currency": "usd",
    "customerId": "cus_test_success",
    "description": "Test successful payment",
    "metadata": {
      "test_scenario": "success"
    }
  }' | jq '.'

echo -e "\n"

# Test 2: Create a payment that will fail
echo "2. Testing failed payment scenario..."
curl -X POST "$BASE_URL/create" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -d '{
    "amount": 20.00,
    "currency": "usd",
    "customerId": "cus_test_failure",
    "description": "Test failed payment",
    "metadata": {
      "test_scenario": "failure"
    }
  }' | jq '.'

echo -e "\n"

# Test 3: Create a processing payment
echo "3. Testing processing payment scenario..."
curl -X POST "$BASE_URL/create" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -d '{
    "amount": 15.00,
    "currency": "usd",
    "customerId": "cus_test_processing",
    "description": "Test processing payment",
    "metadata": {
      "test_scenario": "processing"
    }
  }' | jq '.'

echo -e "\n"

# Test 4: Confirm a payment
echo "4. Testing payment confirmation..."
curl -X POST "$BASE_URL/confirm" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -d '{
    "stripePaymentIntentId": "pi_success_123"
  }' | jq '.'

echo -e "\n"

# Test 5: Get payment by ID
echo "5. Testing get payment by ID..."
curl -X GET "$BASE_URL/$(curl -s "$BASE_URL/create" -H "Content-Type: application/json" -H "Authorization: Bearer $JWT_TOKEN" -d '{"amount": 5.00, "currency": "usd", "customerId": "cus_test", "description": "Test payment"}' | jq -r '.data.id')" \
  -H "Authorization: Bearer $JWT_TOKEN" | jq '.'

echo -e "\n"

# Test 6: Get payment by Stripe ID
echo "6. Testing get payment by Stripe ID..."
curl -X GET "$BASE_URL/stripe/pi_success_123" \
  -H "Authorization: Bearer $JWT_TOKEN" | jq '.'

echo -e "\n"

# Test 7: Get customer payments
echo "7. Testing get customer payments..."
curl -X GET "$BASE_URL/customer/cus_test_success" \
  -H "Authorization: Bearer $JWT_TOKEN" | jq '.'

echo -e "\n"

# Test 8: Health check
echo "8. Testing health check..."
curl -X GET "http://localhost:5000/health" | jq '.'

echo -e "\n"
echo "✅ All tests completed!" 