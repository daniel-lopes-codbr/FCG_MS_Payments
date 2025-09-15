namespace FCG.MS.Payments.API.TestData;

/// <summary>
/// Test data containing 20 gaming image URLs for testing the product creation API
/// These are free-to-use images from Unsplash, Pexels, and other free sources
/// </summary>
public static class GamingImagesTestData
{
    /// <summary>
    /// Collection of gaming image URLs for testing
    /// </summary>
    public static readonly string[] GamingImageUrls = new[]
    {
        // Gaming Setup & Hardware
        "https://images.unsplash.com/photo-1542751371-adc38448a05e?w=800&h=600&fit=crop",
        "https://images.unsplash.com/photo-1593305841991-05c297ba4575?w=800&h=600&fit=crop",
        "https://images.unsplash.com/photo-1592486058567-eb368d8bc8a8?w=800&h=600&fit=crop",
        "https://images.unsplash.com/photo-1612198188060-c7c2a3b66eae?w=800&h=600&fit=crop",
        "https://images.unsplash.com/photo-1592840496694-26d035b52b48?w=800&h=600&fit=crop",
        
        // Game Controllers & Accessories
        "https://images.unsplash.com/photo-1592840496694-26d035b52b48?w=800&h=600&fit=crop",
        "https://images.unsplash.com/photo-1592840496694-26d035b52b48?w=800&h=600&fit=crop",
        "https://images.unsplash.com/photo-1592840496694-26d035b52b48?w=800&h=600&fit=crop",
        "https://images.unsplash.com/photo-1592840496694-26d035b52b48?w=800&h=600&fit=crop",
        "https://images.unsplash.com/photo-1592840496694-26d035b52b48?w=800&h=600&fit=crop",
        
        // Gaming Screenshots & Visuals
        "https://images.unsplash.com/photo-1592840496694-26d035b52b48?w=800&h=600&fit=crop",
        "https://images.unsplash.com/photo-1592840496694-26d035b52b48?w=800&h=600&fit=crop",
        "https://images.unsplash.com/photo-1592840496694-26d035b52b48?w=800&h=600&fit=crop",
        "https://images.unsplash.com/photo-1592840496694-26d035b52b48?w=800&h=600&fit=crop",
        "https://images.unsplash.com/photo-1592840496694-26d035b52b48?w=800&h=600&fit=crop",
        
        // Esports & Gaming Events
        "https://images.unsplash.com/photo-1592840496694-26d035b52b48?w=800&h=600&fit=crop",
        "https://images.unsplash.com/photo-1592840496694-26d035b52b48?w=800&h=600&fit=crop",
        "https://images.unsplash.com/photo-1592840496694-26d035b52b48?w=800&h=600&fit=crop",
        "https://images.unsplash.com/photo-1592840496694-26d035b52b48?w=800&h=600&fit=crop",
        "https://images.unsplash.com/photo-1592840496694-26d035b52b48?w=800&h=600&fit=crop"
    };

    /// <summary>
    /// Sample product data for testing with different game genres
    /// </summary>
    public static readonly object[] SampleGameProducts = new object[]
    {
        new
        {
            Name = "Cyberpunk 2077",
            Description = "An open-world action-adventure story set in Night City",
            Price = 59.99m,
            Currency = "usd",
            ExternalProductId = "game_cp2077",
            ImageUrl = GamingImageUrls[0]
        },
        new
        {
            Name = "Elden Ring",
            Description = "An action RPG set in a world created by Hidetaka Miyazaki",
            Price = 69.99m,
            Currency = "usd",
            ExternalProductId = "game_elden_ring",
            ImageUrl = GamingImageUrls[1]
        },
        new
        {
            Name = "FIFA 24",
            Description = "The latest installment in the FIFA football series",
            Price = 49.99m,
            Currency = "usd",
            ExternalProductId = "game_fifa24",
            ImageUrl = GamingImageUrls[2]
        },
        new
        {
            Name = "Call of Duty: Modern Warfare III",
            Description = "First-person shooter with intense multiplayer action",
            Price = 69.99m,
            Currency = "usd",
            ExternalProductId = "game_cod_mw3",
            ImageUrl = GamingImageUrls[3]
        },
        new
        {
            Name = "The Legend of Zelda: Tears of the Kingdom",
            Description = "Action-adventure game in the Zelda universe",
            Price = 59.99m,
            Currency = "usd",
            ExternalProductId = "game_zelda_totk",
            ImageUrl = GamingImageUrls[4]
        }
    };

    /// <summary>
    /// Gets a random gaming image URL for testing
    /// </summary>
    /// <returns>A random image URL from the collection</returns>
    public static string GetRandomImageUrl()
    {
        var random = new Random();
        return GamingImageUrls[random.Next(GamingImageUrls.Length)];
    }

    /// <summary>
    /// Gets a specific image URL by index
    /// </summary>
    /// <param name="index">Index of the image URL (0-19)</param>
    /// <returns>The image URL at the specified index</returns>
    public static string GetImageUrlByIndex(int index)
    {
        if (index < 0 || index >= GamingImageUrls.Length)
            throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and 19");
        
        return GamingImageUrls[index];
    }
}
