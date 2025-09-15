# FCG MS Payments API

A .NET 8 API for handling Stripe payments following Clean Architecture principles. 

## Project Structure

This project follows Clean Architecture with the following layers:

- **Domain**: Contains core business entities (Product, Payment)
- **Application**: Contains business logic, DTOs, and interfaces
- **Infrastructure**: Contains external dependencies (Stripe SDK implementation)
- **API**: Contains controllers and API endpoints

## Features

- Create products in Stripe
- Create payment intents for products
- Check payment status
- Full Swagger/OpenAPI documentation

## Prerequisites

- .NET 8 SDK
- Stripe account with test API keys

## Configuration

1. Update the `appsettings.json` file with your Stripe API keys:

```json
{
  "Stripe": {
    "PublishableKey": "pk_test_your_publishable_key_here",
    "SecretKey": "sk_test_your_secret_key_here"
  }
}
```

2. For production, use environment variables or secure configuration management.

## API Endpoints

### Products

- `POST /api/products` - Create a new product

Request body:
```json
{
  "name": "Product Name",
  "description": "Product Description",
  "price": 29.99,
  "currency": "usd"
}
```

### Payments

- `POST /api/payments/create` - Create a payment intent

Request body:
```json
{
  "productId": "prod_1234567890"
}
```

- `GET /api/payments/{id}` - Get payment status

## Running the Application

1. Navigate to the API project:
```bash
cd FCG.MS.Payments.API
```

2. Run the application:
```bash
dotnet run
```

3. Access Swagger documentation at: `https://localhost:7001/swagger`

## Testing

The API is configured to work with Stripe's test environment. Use test card numbers provided by Stripe for testing payments.

## Architecture

This project implements Clean Architecture principles:

- **Domain Layer**: Pure business logic and entities
- **Application Layer**: Use cases and business rules
- **Infrastructure Layer**: External services and data access
- **Presentation Layer**: API controllers and DTOs

The separation of concerns allows for easy testing and future modifications without affecting the core business logic.
