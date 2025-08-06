# FCG_MS_Payments - Payment Microservice

A .NET 8 microservice for handling Stripe payments, built with Clean Architecture principles and support for stripe-mock for development.

## 🏗️ Architecture

This project follows Clean Architecture with the following layers:

- **API Layer** (`FCG_MS_Payments.Api`): Controllers, middleware, and API configuration
- **Application Layer** (`FCG_MS_Payments.Application`): Use cases, DTOs, interfaces, and validation
- **Domain Layer** (`FCG_MS_Payments.Domain`): Business entities, domain logic, and interfaces
- **Infrastructure Layer** (`FCG_MS_Payments.Infrastructure`): External services, repositories, and Stripe integration
- **Tests** (`FCG_MS_Payments.Tests`): Unit and integration tests

## 🚀 Features

- **One-time payments** with Stripe integration
- **Stripe Mock Server** for development and testing
- **JWT Authentication** for microservice communication
- **Health checks** for monitoring
- **Structured logging** with Microsoft.Extensions.Logging
- **Request validation** with FluentValidation
- **Object mapping** with AutoMapper
- **In-memory repository** for development/testing
- **Docker support** with separate containers for API and stripe-mock

## 📋 Prerequisites

- .NET 8 SDK
- Docker and Docker Compose
- (Optional) Stripe account for production

## 🛠️ Setup

### Option 1: Using Stripe Mock Server (Recommended for Development)

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd FCG_MS_Payments
   ```

2. **Start stripe-mock server**
   ```bash
   docker-compose -f docker-compose.stripe-mock.yml up -d
   ```

3. **Start the API with Docker**
   ```bash
   cd src
   docker-compose -f docker-compose.api.yml up --build
   ```

4. **Or run locally with mock server**
   ```bash
   cd src
   dotnet run --project FCG_MS_Payments.Api
   ```

### Option 2: Using Real Stripe Account

1. **Get your Stripe test keys** from the [Stripe Dashboard](https://dashboard.stripe.com/test/apikeys)
2. **Update configuration** in `src/FCG_MS_Payments.Api/appsettings.json`:
   ```json
   {
     "StripeSettings": {
       "SecretKey": "sk_test_your_actual_stripe_test_key",
       "PublishableKey": "pk_test_your_actual_stripe_publishable_key",
       "UseMockServer": false
     }
   }
   ```

## 🧪 Testing with Mock Scenarios

The stripe-mock server includes predefined scenarios for testing:

### Available Test Scenarios

1. **Success Scenario** (`cus_test_success`)
   - Customer ID: `cus_test_success`
   - Expected status: `succeeded`

2. **Failure Scenario** (`cus_test_failure`)
   - Customer ID: `cus_test_failure`
   - Expected status: `canceled`

3. **Processing Scenario** (`cus_test_processing`)
   - Customer ID: `cus_test_processing`
   - Expected status: `processing`

4. **Requires Payment Method** (`cus_test_requires_payment_method`)
   - Customer ID: `cus_test_requires_payment_method`
   - Expected status: `requires_payment_method`

### Running Test Scenarios

Use the provided test script:
```bash
chmod +x test-mock-scenarios.sh
./test-mock-scenarios.sh
```

Or test manually with curl:
```bash
# Test successful payment
curl -X POST "http://localhost:5000/api/payments/create" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer your-jwt-token" \
  -d '{
    "amount": 10.00,
    "currency": "usd",
    "customerId": "cus_test_success",
    "description": "Test successful payment"
  }'
```

## 📚 API Endpoints

### Authentication
All endpoints require JWT authentication. Include the token in the Authorization header:
```
Authorization: Bearer <your-jwt-token>
```

### Payment Endpoints

#### Create Payment
```http
POST /api/payments/create
Content-Type: application/json

{
  "amount": 100.00,
  "currency": "usd",
  "customerId": "cus_test_success",
  "description": "Payment for product",
  "metadata": {
    "productId": "prod_123",
    "orderId": "order_456"
  }
}
```

#### Confirm Payment
```http
POST /api/payments/confirm
Content-Type: application/json

{
  "stripePaymentIntentId": "pi_test123"
}
```

#### Get Payment by ID
```http
GET /api/payments/{id}
```

#### Get Payment by Stripe ID
```http
GET /api/payments/stripe/{stripePaymentIntentId}
```

#### Get Customer Payments
```http
GET /api/payments/customer/{customerId}
```

### Health Check
```http
GET /health
```

## 🐳 Docker Commands

### Start stripe-mock only
```bash
docker-compose -f docker-compose.stripe-mock.yml up -d
```

### Start API with stripe-mock
```bash
cd src
docker-compose -f docker-compose.api.yml up --build
```

### View logs
```bash
# API logs
docker-compose -f src/docker-compose.api.yml logs -f fcg-ms-payments-api

# Stripe mock logs
docker-compose -f docker-compose.stripe-mock.yml logs -f stripe-mock
```

### Stop services
```bash
# Stop API
docker-compose -f src/docker-compose.api.yml down

# Stop stripe-mock
docker-compose -f docker-compose.stripe-mock.yml down
```

## 🧪 Testing

Run the tests:
```bash
cd src
dotnet test
```

## 📦 Project Structure

```
FCG_MS_Payments/
├── src/
│   ├── FCG_MS_Payments.Api/           # Presentation layer
│   │   ├── Controllers/               # API controllers
│   │   ├── Models/                    # API response models
│   │   ├── HealthChecks/              # Health check implementations
│   │   └── Program.cs                 # Application configuration
│   ├── FCG_MS_Payments.Application/   # Application layer
│   │   ├── DTOs/                     # Data transfer objects
│   │   ├── Interfaces/                # Application service interfaces
│   │   ├── Services/                  # Application services
│   │   ├── Validators/                # FluentValidation validators
│   │   └── Mapping/                   # AutoMapper profiles
│   ├── FCG_MS_Payments.Domain/        # Domain layer
│   │   ├── Entities/                  # Domain entities
│   │   ├── Enums/                     # Domain enums
│   │   ├── Exceptions/                # Domain exceptions
│   │   └── Interfaces/                # Domain service interfaces
│   ├── FCG_MS_Payments.Infrastructure/ # Infrastructure layer
│   │   ├── Services/                  # External service implementations
│   │   └── Repositories/              # Repository implementations
│   └── FCG_MS_Payments.Tests/        # Test projects
│       └── PaymentServiceTests.cs     # Unit tests
├── stripe-mock/
│   └── fixtures.json                  # Mock scenarios configuration
├── docker-compose.stripe-mock.yml     # Stripe mock server
├── src/docker-compose.api.yml         # API with stripe-mock
├── test-mock-scenarios.sh             # Test script
└── README.md                          # This file
```

## 🔧 Configuration

### Environment Variables
- `StripeSettings:SecretKey`: Your Stripe secret key (or mock key)
- `StripeSettings:PublishableKey`: Your Stripe publishable key (or mock key)
- `StripeSettings:MockServerUrl`: URL of the stripe-mock server
- `StripeSettings:UseMockServer`: Enable/disable mock server
- `JwtSettings:SecretKey`: JWT signing key (at least 32 characters)

### Development vs Production
- **Development**: Uses stripe-mock server and in-memory repository
- **Production**: Should use real Stripe API and a real database

## 🚨 Security Considerations

1. **Never commit real API keys** to version control
2. **Use environment variables** for sensitive configuration
3. **Validate all inputs** using FluentValidation
4. **Use HTTPS** in production
5. **Implement proper JWT token management**

## 📝 Development Notes

- This is a study project for learning purposes
- Uses stripe-mock for development and testing
- In-memory repository for simplicity
- JWT authentication for microservice communication
- Health checks for monitoring
- Docker support for easy deployment

## 🤝 Contributing

1. Follow Clean Architecture principles
2. Add tests for new features
3. Use meaningful commit messages
4. Update documentation as needed

## 📄 License

This project is for educational purposes. 