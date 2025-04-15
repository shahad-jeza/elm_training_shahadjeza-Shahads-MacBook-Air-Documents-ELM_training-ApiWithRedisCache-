using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ApiWithRedisCache.Services;

public class UserService
{
    private readonly HttpClient _httpClient;
    private readonly IDistributedCache _distributedCache;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<UserService> _logger;
    private readonly MemoryCacheEntryOptions _memoryCacheOptions;

    public UserService(
        HttpClient httpClient,
        IDistributedCache distributedCache,
        IMemoryCache memoryCache,
        ILogger<UserService> logger)
    {
        _httpClient = httpClient;
        _distributedCache = distributedCache;
        _memoryCache = memoryCache;
        _logger = logger;
        
        _httpClient.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
        
        // Configure in-memory cache options (5 minute sliding expiration)
        _memoryCacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    public async Task<List<User>?> GetUsersAsync()
    {
        const string cacheKey = "users_all";
        
        // 1. Try to get from in-memory cache first
        if (_memoryCache.TryGetValue(cacheKey, out List<User>? cachedUsers))
        {
            _logger.LogInformation("Data retrieved from in-memory cache");
            return cachedUsers;
        }

        // 2. If not in memory cache, try Redis
        var redisData = await _distributedCache.GetAsync(cacheKey);
        if (redisData != null)
        {
            _logger.LogInformation("Data retrieved from Redis cache");
            var usersFromRedis = JsonSerializer.Deserialize<List<User>>(Encoding.UTF8.GetString(redisData));
            
            // Store in memory cache for future requests
            _memoryCache.Set(cacheKey, usersFromRedis, _memoryCacheOptions);
            
            return usersFromRedis;
        }

        // 3. If not in any cache, fetch from API
        _logger.LogInformation("Fetching data from external API");
        var usersFromApi = await _httpClient.GetFromJsonAsync<List<User>>("users");
        
        if (usersFromApi == null || usersFromApi.Count == 0)
        {
            return null;
        }

        // Cache in both memory and Redis
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };
        
        // Cache in memory
        _memoryCache.Set(cacheKey, usersFromApi, _memoryCacheOptions);
        
        // Cache in Redis
        await _distributedCache.SetAsync(
            cacheKey, 
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(usersFromApi)), 
            cacheOptions);
        
        return usersFromApi;
    }
}

public record User(
    int Id,
    string Name,
    string Username,
    string Email,
    Address Address,
    string Phone,
    string Website,
    Company Company);

public record Address(
    string Street,
    string Suite,
    string City,
    string Zipcode,
    Geo Geo);

public record Geo(
    string Lat,
    string Lng);

public record Company(
    string Name,
    string CatchPhrase,
    string Bs);