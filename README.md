# API with Redis Cache & Optimized Frontend

A .NET Web API with Redis caching, in-memory caching, and an optimized frontend with lazy-loaded images.

## Features

- **Caching Layers**
  - Redis distributed caching
  - In-memory caching with `IMemoryCache`
- **Frontend**
  - Dynamic UI updates with Fetch API
  - Image lazy loading
  - WebP support where available
- **Image Optimization**
  - Native lazy loading (`loading="lazy"`)

## Setup

### Prerequisites
- .NET 6+ SDK
- Redis server (local or cloud)
- Node.js (for optional frontend tooling)

### Installation
1. Clone the repo:

2. Configure Redis:
   - Update connection string in `appsettings.json`:
     ```json
     "Redis": {
       "Configuration": "localhost:6379",
       "InstanceName": "SampleInstance_"
     }
     ```

3. Run the API:
   ```bash
   dotnet run
   ```

4. Access the frontend:
   ```
   https://localhost:7181
   ```

## API Endpoints
| Endpoint | Description |
|----------|-------------|
| `GET /users` | Get all users (cached) |

## Frontend Structure
```
wwwroot/
├── index.html       # Main UI
├── app.js          # Dynamic data loading
└── styles/         # CSS (inline in index.html)
```



## License
MIT
