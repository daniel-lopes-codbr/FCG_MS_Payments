# API Testing Guide

## Prerequisites

1. Make sure the API is running: `dotnet run` in the `FCG.MS.Payments.API` directory
2. Update the Stripe API keys in `appsettings.json` with your test keys
3. Update the API key in `appsettings.json` with your secure API key
4. Update the admin secret in `appsettings.json` with your secure admin secret
5. The API will be available at: `http://localhost:5043`

## Authentication

All API endpoints (except Swagger and Admin endpoints) require API key authentication. Include the API key in the request header:

```
X-API-Key: your-secure-api-key-here
```

## API Key Management

### Generate a New API Key

Before testing other endpoints, you need to generate an API key using the admin endpoint:

```bash
curl -X POST "http://localhost:5043/api/admin/generate-api-key" \
  -H "Content-Type: application/json" \
  -d '{
    "serviceName": "MyService",
    "adminSecret": "your-super-secure-admin-secret-here"
  }'
```

Expected response:
```json
{
  "apiKey": "fcg_AbCdEfGhIjKlMnOpQrStUvWxYz123456",
  "serviceName": "MyService",
  "generatedAt": "2024-01-01T00:00:00.000Z",
  "expiresAt": "2025-01-01T00:00:00.000Z"
}
```

**Important**: 
- Use the generated `apiKey` in the `X-API-Key` header for all other API calls
- The admin endpoint is exempt from API key authentication
- Store the admin secret securely and never expose it in client applications

## Test Scenarios

### 1. Create a Product

```bash
curl -X POST "http://localhost:5043/api/products" \
  -H "Content-Type: application/json" \
  -H "X-API-Key: fcg_AbCdEfGhIjKlMnOpQrStUvWxYz123456" \
  -d '{
    "name": "Test Product",
    "description": "A test product for payment processing",
    "price": 29.99,
    "currency": "usd",
    "externalProductId": "PROD-001"
  }'
```

Expected response:
```json
{
  "id": "prod_1234567890",
  "name": "Test Product",
  "description": "A test product for payment processing",
  "price": 29.99,
  "currency": "usd",
  "createdAt": "2024-01-01T00:00:00.000Z"
}
```

**Important**: The `externalProductId` field is **required** and will be stored in Stripe's product metadata for future lookups.

### 2. Create a Payment Intent

```bash
curl -X POST "http://localhost:5043/api/payments/create" \
  -H "Content-Type: application/json" \
  -H "X-API-Key: fcg_AbCdEfGhIjKlMnOpQrStUvWxYz123456" \
  -d '{
    "productId": "prod_1234567890"
  }'
```

Expected response:
```json
{
  "id": "pi_1234567890",
  "clientSecret": "pi_1234567890_secret_abc123",
  "amount": 29.99,
  "currency": "usd",
  "status": "requires_payment_method"
}
```

### 3. Check Payment Status

```bash
curl -X GET "http://localhost:5043/api/payments/pi_1234567890" \
  -H "X-API-Key: fcg_AbCdEfGhIjKlMnOpQrStUvWxYz123456"
```

Expected response:
```json
{
  "id": "pi_1234567890",
  "status": "requires_payment_method",
  "amount": 29.99,
  "currency": "usd",
  "createdAt": "2024-01-01T00:00:00.000Z",
  "updatedAt": null
}
```

## Using Swagger UI

1. Open your browser and navigate to: `http://localhost:5043/swagger`
2. You'll see the interactive API documentation
3. **Note**: Swagger UI is exempt from API key authentication for testing purposes
4. Click on any endpoint to expand it
5. Click "Try it out" to test the endpoint directly from the browser
6. Fill in the required parameters and click "Execute"

## Validation Rules

### API Key Generation
- `serviceName` is **required** and cannot be empty
- `adminSecret` is **required** and must match the configured admin secret

### Product Creation
- `externalProductId` is **required** and cannot be empty
- `name` is **required** and cannot be empty
- `price` must be greater than zero
- `currency` defaults to "usd" if not provided

## Error Responses

### Missing API Key
```json
{
  "error": "API key is required"
}
```

### Invalid API Key
```json
{
  "error": "Invalid API key"
}
```

### Invalid Admin Secret
```json
{
  "error": "Invalid admin secret"
}
```

### Validation Errors
```json
{
  "error": "ExternalProductId is required"
}
```

## Professional API Key Workflow

1. **Initial Setup**: Configure admin secret in `appsettings.json`
2. **Generate API Key**: Use admin endpoint to generate API key for your service
3. **Store Securely**: Store the generated API key securely in your service configuration
4. **Use API Key**: Include the API key in all API requests via `X-API-Key` header
5. **Key Rotation**: Generate new keys periodically for security

## Important Notes

- This API uses Stripe's test environment
- Use test card numbers provided by Stripe for testing payments
- The `clientSecret` returned from the payment intent creation should be used on the frontend to complete the payment
- All amounts are handled in the smallest currency unit (cents for USD)
- Error responses will include a descriptive error message
- The `externalProductId` is stored in Stripe's product metadata for professional binding between systems
- API keys are generated with a 1-year expiration (configurable)
- In production, implement proper key storage and rotation mechanisms
