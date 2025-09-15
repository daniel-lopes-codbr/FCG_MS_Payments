# Image URL Feature for Product Creation API

## Overview
The Products API now supports adding image URLs to products. This feature allows users to associate visual representations with their products, making the API more suitable for e-commerce and gaming platforms.

## New Fields

### CreateProductRequest
- **ImageUrl** (optional): A string containing a valid absolute URL to an image
  - Must be a valid absolute URL if provided
  - Can be left empty (product will be created without an image)
  - Supports any image format (JPG, PNG, GIF, etc.)

### CreateProductResponse
- **ImageUrl**: Returns the image URL that was provided during creation

## Implementation Details

### Validation
- Image URL is validated to ensure it's a valid absolute URL format
- Uses `Uri.TryCreate()` for robust URL validation
- Validation only occurs if an ImageUrl is provided (field is optional)

### Stripe Integration
- Image URL is stored in Stripe product metadata under the key `imageUrl`
- This allows for easy retrieval and management of product images
- No changes to Stripe's core product structure

## Usage Examples

### Creating a Product with Image
```json
POST /api/products
{
  "name": "Cyberpunk 2077",
  "description": "An open-world action-adventure story",
  "price": 59.99,
  "currency": "usd",
  "externalProductId": "game_cp2077",
  "imageUrl": "https://images.unsplash.com/photo-1542751371-adc38448a05e?w=800&h=600&fit=crop"
}
```

### Creating a Product without Image
```json
POST /api/products
{
  "name": "Game Without Image",
  "description": "Testing product creation without image",
  "price": 19.99,
  "currency": "usd",
  "externalProductId": "game_no_image"
}
```

## Testing

### Test Files
- `TestProductsAPI.http`: Contains 20 test requests with various gaming images
- `GamingImagesTestData.cs`: C# class with test data and helper methods

### Test Scenarios
1. **Valid Image URLs**: 20 different gaming-related images from free sources
2. **No Image**: Testing optional field behavior
3. **Invalid URLs**: Testing validation error handling
4. **Edge Cases**: Long descriptions, special characters, different currencies

### Manual Testing
Use the HTTP test file in your IDE (Rider, VS Code, etc.) to test the API:
1. Set your `baseUrl` and `apiKey` environment variables
2. Run individual test requests
3. Verify responses and error handling

## Image Sources
The test data includes images from:
- **Unsplash**: High-quality, free gaming photos
- **Pexels**: Free stock photos
- **Other free sources**: Suitable for academic projects

## Future Enhancements
- Image file upload support
- Image resizing and optimization
- Multiple image support per product
- Image validation (format, size, content)

## Backward Compatibility
- Existing API calls without ImageUrl will continue to work
- No breaking changes to existing functionality
- ImageUrl field is optional and defaults to empty string
