using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using ApiWithRedisCache.Services;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add HttpClient and UserService
builder.Services.AddHttpClient();
builder.Services.AddScoped<UserService>();

// Add Memory Cache
builder.Services.AddMemoryCache();
// Add Redis distributed cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379"; // Your Redis server address
    options.InstanceName = "SampleInstance_"; // Optional prefix for keys
});

var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles(); 
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html"); 

app.Run();