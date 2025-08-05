# FCG_MS_Payments - Payment Microservice

A .NET 8 microservice for handling Stripe payments, built with Clean Architecture principles.

## 🏗️ Architecture

This project follows Clean Architecture with the following layers:

- **API Layer** (`FCG_MS_Payments.Api`): Controllers, middleware, and API configuration
- **Application Layer** (`FCG_MS_Payments.Application`): Use cases, DTOs, interfaces, and validation
- **Domain Layer** (`FCG_MS_Payments.Domain`): Business entities, domain logic, and interfaces
- **Infrastructure Layer** (`FCG_MS_Payments.Infrastructure`): External services, repositories, and Stripe integration
- **Tests** (`FCG_MS_Payments.Tests`): Unit and integration tests

## 🚀 Features

- **One-time payments** with Stripe integration
- **JWT Authentication** for microservice communication
- **Health checks** for monitoring
- **Structured logging** with Microsoft.Extensions.Logging
- **Request validation** with FluentValidation
- **Object mapping** with AutoMapper
- **In-memory repository** for development/testing

## 📋 Prerequisites

- .NET 8 SDK
- Stripe account (for test keys)

## 🛠️ Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd FCG_MS_Payments
   ```

2. **Configure Stripe**
   - Get your Stripe test keys from the [Stripe Dashboard](https://dashboard.stripe.com/test/apikeys)
   - Update `src/FCG_MS_Payments.Api/appsettings.json`:
   ```json
   {
     "StripeSettings": {
       "SecretKey": "sk_test_your_actual_stripe_test_key",
       "PublishableKey": "pk_test_your_actual_stripe_publishable_key"
     }
   }
   ```

3. **Configure JWT**
   - Update the JWT secret key in `appsettings.json`:
   ```json
   {
     "JwtSettings": {
       "SecretKey": "your-super-secret-key-with-at-least-32-characters"
     }
   }
   ```

4. **Build and run**
   ```bash
   cd src
   dotnet build
   dotnet run --project FCG_MS_Payments.Api
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
  "customerId": "cus_test123",
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

## 🧪 Testing

Run the tests:
```bash
cd src
dotnet test
```

## 📦 Project Structure

```
src/
├── FCG_MS_Payments.Api/           # Presentation layer
│   ├── Controllers/               # API controllers
│   ├── Models/                    # API response models
│   └── Program.cs                 # Application configuration
├── FCG_MS_Payments.Application/   # Application layer
│   ├── DTOs/                     # Data transfer objects
│   ├── Interfaces/                # Application service interfaces
│   ├── Services/                  # Application services
│   ├── Validators/                # FluentValidation validators
│   └── Mapping/                   # AutoMapper profiles
├── FCG_MS_Payments.Domain/        # Domain layer
│   ├── Entities/                  # Domain entities
│   ├── Enums/                     # Domain enums
│   ├── Exceptions/                # Domain exceptions
│   └── Interfaces/                # Domain service interfaces
├── FCG_MS_Payments.Infrastructure/ # Infrastructure layer
│   ├── Services/                  # External service implementations
│   └── Repositories/              # Repository implementations
└── FCG_MS_Payments.Tests/        # Test projects
    └── PaymentServiceTests.cs     # Unit tests
```

## 🔧 Configuration

### Environment Variables
- `StripeSettings:SecretKey`: Your Stripe secret key
- `StripeSettings:PublishableKey`: Your Stripe publishable key
- `JwtSettings:SecretKey`: JWT signing key (at least 32 characters)

### Development vs Production
- **Development**: Uses in-memory repository and test Stripe keys
- **Production**: Should use a real database and production Stripe keys

## 🚨 Security Considerations

1. **Never commit real API keys** to version control
2. **Use environment variables** for sensitive configuration
3. **Validate all inputs** using FluentValidation
4. **Use HTTPS** in production
5. **Implement proper JWT token management**

## 📝 Development Notes

- This is a study project for learning purposes
- Uses Stripe test environment
- In-memory repository for simplicity
- JWT authentication for microservice communication
- Health checks for monitoring

## 🤝 Contributing

1. Follow Clean Architecture principles
2. Add tests for new features
3. Use meaningful commit messages
4. Update documentation as needed

## 📄 License

This project is for educational purposes. 