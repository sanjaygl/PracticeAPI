# Extension Methods Architecture - Before & After

## ? Benefits of Using Extension Methods

### Before (Monolithic Program.cs)
```csharp
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllers();

    // Configure PostgreSQL Database
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Register Repositories
    builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
    builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

    // Register Services
    builder.Services.AddScoped<IDepartmentService, DepartmentService>();
    builder.Services.AddScoped<IEmployeeService, EmployeeService>();

    // Configure Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Global Exception Handler Middleware
    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "PracticeAPI v1");
            c.RoutePrefix = string.Empty;
            c.DocumentTitle = "PracticeAPI Documentation";
        });
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
```

### After (Clean with Extensions)
```csharp
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    // Register all application services (Database, Repositories, Services, Swagger)
    builder.Services.AddApplicationConfiguration(builder.Configuration);

    var app = builder.Build();

    // Configure the HTTP request pipeline
    app.UseCustomMiddlewares();
    app.UseSwaggerConfiguration(app.Environment);
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
```

## ?? Extension Methods Structure

### 1. ApplicationServiceExtensions.cs
**Purpose**: One-stop registration for all services
```csharp
public static IServiceCollection AddApplicationConfiguration(
    this IServiceCollection services,
    IConfiguration configuration)
{
    services.AddControllers();
    services.AddDatabaseConfiguration(configuration);
    services.AddRepositories();
    services.AddApplicationServices();
    services.AddSwaggerConfiguration();
    return services;
}
```

### 2. DatabaseExtensions.cs
**Purpose**: Database configuration with best practices
```csharp
public static IServiceCollection AddDatabaseConfiguration(
    this IServiceCollection services,
    IConfiguration configuration)
{
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(
            configuration.GetConnectionString("DefaultConnection"),
            npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorCodesToAdd: null)));
    return services;
}
```

**Features**:
- ? Retry logic for database connections
- ? Configurable retry parameters
- ? Automatic migration support (optional)

### 3. RepositoryExtensions.cs
**Purpose**: Centralized repository registration
```csharp
public static IServiceCollection AddRepositories(this IServiceCollection services)
{
    services.AddScoped<IDepartmentRepository, DepartmentRepository>();
    services.AddScoped<IEmployeeRepository, EmployeeRepository>();
    return services;
}
```

**Benefits**:
- ? Easy to add new repositories
- ? Single source of truth for repository DI
- ? Consistent lifetime management (Scoped)

### 4. ServiceExtensions.cs
**Purpose**: Business service layer registration
```csharp
public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
    services.AddScoped<IDepartmentService, DepartmentService>();
    services.AddScoped<IEmployeeService, EmployeeService>();
    return services;
}
```

### 5. SwaggerExtensions.cs
**Purpose**: Swagger/OpenAPI configuration
```csharp
public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(options => { /* config */ });
    return services;
}

public static IApplicationBuilder UseSwaggerConfiguration(
    this IApplicationBuilder app, 
    IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options => { /* config */ });
    }
    return app;
}
```

**Features**:
- ? Environment-specific configuration
- ? Enhanced Swagger UI options
- ? JWT preparation (commented)

### 6. MiddlewareExtensions.cs
**Purpose**: Custom middleware pipeline
```csharp
public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
{
    return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
}

public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
{
    app.UseGlobalExceptionHandler();
    // Add more middlewares here
    return app;
}
```

**Benefits**:
- ? Chainable middleware registration
- ? Easy to add request logging, performance monitoring, etc.
- ? Centralized middleware management

## ?? Key Advantages

### 1. **Separation of Concerns**
Each extension focuses on a specific aspect of configuration:
- Database setup ? `DatabaseExtensions`
- Repository DI ? `RepositoryExtensions`
- Business services ? `ServiceExtensions`
- API documentation ? `SwaggerExtensions`
- Middleware pipeline ? `MiddlewareExtensions`

### 2. **Maintainability**
- New repositories? Update only `RepositoryExtensions.cs`
- New service? Update only `ServiceExtensions.cs`
- Change database provider? Update only `DatabaseExtensions.cs`

### 3. **Testability**
Each extension can be tested independently:
```csharp
[Fact]
public void AddRepositories_RegistersAllRepositories()
{
    var services = new ServiceCollection();
    services.AddRepositories();
    
    var serviceProvider = services.BuildServiceProvider();
    var repo = serviceProvider.GetService<IDepartmentRepository>();
    
    Assert.NotNull(repo);
}
```

### 4. **Reusability**
Extension methods can be:
- Copied to other projects
- Packaged as NuGet packages
- Shared across microservices

### 5. **Readability**
`Program.cs` becomes a high-level blueprint:
```csharp
// Setup
builder.Services.AddApplicationConfiguration(builder.Configuration);

// Pipeline
app.UseCustomMiddlewares();
app.UseSwaggerConfiguration(app.Environment);
```

## ?? Adding New Features

### Example: Adding a New Repository

**Step 1**: Create the repository
```csharp
public interface IProductRepository : IRepository<Product> { }
public class ProductRepository : IProductRepository { }
```

**Step 2**: Register in `RepositoryExtensions.cs`
```csharp
public static IServiceCollection AddRepositories(this IServiceCollection services)
{
    services.AddScoped<IDepartmentRepository, DepartmentRepository>();
    services.AddScoped<IEmployeeRepository, EmployeeRepository>();
    services.AddScoped<IProductRepository, ProductRepository>(); // ? Added
    return services;
}
```

**No changes needed in `Program.cs`!**

### Example: Adding Request Logging Middleware

**Step 1**: Create the middleware
```csharp
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    public RequestLoggingMiddleware(RequestDelegate next) => _next = next;
    
    public async Task InvokeAsync(HttpContext context)
    {
        // Log request
        await _next(context);
        // Log response
    }
}
```

**Step 2**: Add to `MiddlewareExtensions.cs`
```csharp
public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
{
    app.UseGlobalExceptionHandler();
    app.UseMiddleware<RequestLoggingMiddleware>(); // ? Added
    return app;
}
```

**No changes needed in `Program.cs`!**

## ?? Comparison Summary

| Aspect | Before | After |
|--------|--------|-------|
| **Program.cs Lines** | ~60 lines | ~15 lines |
| **Readability** | Mixed concerns | Clear intent |
| **Testability** | Hard to test setup | Easy to test extensions |
| **Maintainability** | All in one place | Organized by concern |
| **Reusability** | None | High |
| **Scalability** | Gets messy | Stays clean |

## ?? Best Practices

1. **Group Related Configurations**: Keep related services in the same extension
2. **Use Descriptive Names**: `AddDatabaseConfiguration` is better than `AddDb`
3. **Return Services/App**: Enable method chaining
4. **Add XML Comments**: Document what each extension does
5. **Keep It Simple**: Don't over-engineer small projects
6. **Follow Conventions**: Use `Add*` for services, `Use*` for middleware

## ?? Future Enhancements

Easily add more extensions:
- `AuthenticationExtensions` - JWT, OAuth, etc.
- `CachingExtensions` - Redis, Memory cache
- `HealthCheckExtensions` - Database, API health checks
- `CorsExtensions` - CORS policies
- `ApiVersioningExtensions` - API versioning
- `RateLimitingExtensions` - Rate limiting policies

## ?? Conclusion

Extension methods transform `Program.cs` from a configuration dump into a clean, readable blueprint of your application's architecture. This approach scales beautifully as your application grows!
