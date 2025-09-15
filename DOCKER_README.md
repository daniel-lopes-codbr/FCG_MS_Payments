# FCG MS Payments - Docker Setup

## Prerequisites

1. Docker and Docker Compose installed
2. Stripe account with API keys

## Environment Variables

Before running the application, you need to set up the Stripe configuration. Create a `.env` file in the same directory as the `docker-compose.yml` file with the following content:

```env
# Stripe Configuration
STRIPE_PUBLISHABLE_KEY=pk_test_your_publishable_key_here
STRIPE_SECRET_KEY=sk_test_your_secret_key_here
```

Replace the placeholder values with your actual Stripe keys:
- `STRIPE_PUBLISHABLE_KEY`: Your Stripe publishable key (starts with `pk_test_` for test mode or `pk_live_` for live mode)
- `STRIPE_SECRET_KEY`: Your Stripe secret key (starts with `sk_test_` for test mode or `sk_live_` for live mode)

## Running the Application

1. Make sure you have created the `.env` file with your Stripe keys
2. Run the following command in the project root directory:

```bash
docker-compose up --build
```

The API will be available at `http://localhost:3003`

## Stopping the Application

```bash
docker-compose down
```

## Notes

- The Payments API runs on port 3003 (different from the other APIs)
- No database is required for this service
- The application uses the Production environment by default
- The container will automatically restart unless stopped manually
