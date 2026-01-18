# PracticeAPI - Employee & Department Management System

A production-ready ASP.NET Core Web API (.NET 10) for managing Employees and Departments with PostgreSQL database.

## ??? Architecture

This project follows **Clean Architecture** principles with clear separation of concerns:

```
Controller ? Service ? Repository ? Database
```

### Layer Structure:
- **Controllers**: Handle HTTP requests/responses and routing
- **Services**: Business logic and data transformation (DTOs)
- **Repositories**: Data access layer with Entity Framework Core
- **Models**: Domain entities
- **DTOs**: Data Transfer Objects for API requests/responses
- **Middleware**: Global exception handling
- **Extensions**: Custom extension methods for clean service configuration

## ?? Features

- ? RESTful API design
- ? Clean Architecture (Controller ? Service ? Repository)
- ? Entity Framework Core with Code First approach
- ? PostgreSQL database with retry logic
- ? Global exception handling middleware
- ? **Custom extension methods for organized configuration**
- ? DTOs for all requests/responses
- ? Async/await pattern throughout
- ? Dependency Injection
- ? Swagger/OpenAPI documentation
- ? Data validation with Data Annotations
- ? Proper HTTP status codes
- ? Filtering and querying support
- ? Referential integrity enforcement
- ? Structured logging
- ? JWT Authentication ready (commented out)

## ?? Prerequisites

- .NET 10 SDK
- PostgreSQL 12 or higher
- Visual Studio 2022 or VS Code
- EF Core CLI tools

## ??? Setup Instructions

### 1. Clone and Navigate
```bash
cd C:\Personal\PracticeAPI
```

### 2. Install EF Core Tools (if not already installed)
```bash
dotnet tool install --global dotnet-ef
```

### 3. Configure Database Connection

Update the connection string in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=EmployeeManagementDB;Username=postgres;Password=yourpassword"
}
```

### 4. Create Database and Run Migrations

```bash
# Navigate to the project directory
cd PracticeAPI

# Create initial migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

### 5. Run the Application

```bash
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7xxx`
- HTTP: `http://localhost:5xxx`
- Swagger UI: `https://localhost:7xxx` (root)

## ?? API Endpoints

### Departments

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/departments` | Get all departments |
| GET | `/api/departments/{id}` | Get department by ID |
| POST | `/api/departments` | Create new department |
| PUT | `/api/departments/{id}` | Update department |
| DELETE | `/api/departments/{id}` | Delete department |

### Employees

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/employees` | Get all employees |
| GET | `/api/employees?departmentId={id}` | Filter employees by department |
| GET | `/api/employees?minSalary={min}&maxSalary={max}` | Filter by salary range |
| GET | `/api/employees/{id}` | Get employee by ID |
| POST | `/api/employees` | Create new employee |
| PUT | `/api/employees/{id}` | Update employee |
| DELETE | `/api/employees/{id}` | Delete employee |

## ?? Sample API Requests

### Create Department
```json
POST /api/departments
{
  "name": "Engineering"
}
```

### Create Employee
```json
POST /api/employees
{
  "name": "John Doe",
  "gender": "Male",
  "salary": 75000,
  "departmentId": 1
}
```

### Update Employee
```json
PUT /api/employees/1
{
  "name": "John Doe",
  "gender": "Male",
  "salary": 80000,
  "departmentId": 2
}
```

## ?? Error Handling

The API uses global exception handling middleware that returns consistent error responses:

```json
{
  "statusCode": 400,
  "message": "Error description",
  "details": "Additional details (development only)"
}
```

### Status Codes:
- `200 OK` - Successful GET/PUT
- `201 Created` - Successful POST
- `204 No Content` - Successful DELETE
- `400 Bad Request` - Validation errors, business rule violations
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Unhandled exceptions

## ?? Testing with Swagger

1. Run the application
2. Navigate to `https://localhost:7xxx` (Swagger UI opens at root)
3. Expand any endpoint
4. Click "Try it out"
5. Enter request data
6. Click "Execute"

## ?? Project Structure

```
PracticeAPI/
??? Controllers/
?   ??? DepartmentsController.cs
?   ??? EmployeesController.cs
??? Services/
?   ??? Interfaces/
?   ?   ??? IDepartmentService.cs
?   ?   ??? IEmployeeService.cs
?   ??? DepartmentService.cs
?   ??? EmployeeService.cs
??? Repositories/
?   ??? Interfaces/
?   ?   ??? IRepository.cs
?   ?   ??? IDepartmentRepository.cs
?   ?   ??? IEmployeeRepository.cs
?   ??? DepartmentRepository.cs
?   ??? EmployeeRepository.cs
??? Models/
?   ??? Department.cs
?   ??? Employee.cs
??? DTOs/
?   ??? DepartmentDto.cs
?   ??? EmployeeDto.cs
??? Data/
?   ??? ApplicationDbContext.cs
??? Middleware/
?   ??? ErrorResponse.cs
?   ??? GlobalExceptionHandlerMiddleware.cs
??? Extensions/                              ? NEW
?   ??? ApplicationServiceExtensions.cs      ? Comprehensive service registration
?   ??? DatabaseExtensions.cs                ? Database configuration
?   ??? RepositoryExtensions.cs              ? Repository DI registration
?   ??? ServiceExtensions.cs                 ? Business service registration
?   ??? SwaggerExtensions.cs                 ? Swagger configuration
?   ??? MiddlewareExtensions.cs              ? Middleware configuration
??? Program.cs
??? appsettings.json
??? appsettings.Development.json
```

## ?? Extension Methods Architecture

The project uses custom extension methods to keep `Program.cs` clean and organized:

### **ApplicationServiceExtensions**
Comprehensive registration of all application services:
```csharp
builder.Services.AddApplicationConfiguration(builder.Configuration);
```

### **DatabaseExtensions**
Database configuration with retry logic:
```csharp
builder.Services.AddDatabaseConfiguration(builder.Configuration);
```

### **RepositoryExtensions**
Repository pattern DI registration:
```csharp
builder.Services.AddRepositories();
```

### **ServiceExtensions**
Business service layer registration:
```csharp
builder.Services.AddApplicationServices();
```

### **SwaggerExtensions**
Swagger/OpenAPI configuration:
```csharp
builder.Services.AddSwaggerConfiguration();
app.UseSwaggerConfiguration(app.Environment);
```

### **MiddlewareExtensions**
Custom middleware pipeline configuration:
```csharp
app.UseCustomMiddlewares();
```

### Benefits:
- ? **Separation of Concerns**: Each extension handles specific configuration
- ? **Reusability**: Extensions can be reused across projects
- ? **Testability**: Easier to test individual configurations
- ? **Maintainability**: Changes are isolated to specific extension files
- ? **Readability**: `Program.cs` remains clean and focused

## ?? JWT Authentication (Future Enhancement)

The project is prepared for JWT authentication. To enable:

1. Install required packages:
```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

2. Uncomment JWT configuration in `SwaggerExtensions.cs`

3. Add JWT configuration to `appsettings.json`

4. Create a new `AuthenticationExtensions.cs` for JWT setup

5. Apply `[Authorize]` attributes to controllers

## ?? Business Rules

1. **Department Deletion**: Cannot delete a department if it has associated employees
2. **Employee Creation**: DepartmentId must reference a valid department (if provided)
3. **Referential Integrity**: Deleting a department sets employee DepartmentId to null
4. **Validation**: All required fields must be provided and within size limits

## ?? Common Issues

### Migration Issues
```bash
# Reset migrations
dotnet ef database drop
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Port Conflicts
Update `launchSettings.json` if ports are already in use.

### PostgreSQL Connection
Ensure PostgreSQL service is running and credentials are correct.

## ?? Additional Resources

- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core/)
- [ASP.NET Core Web API](https://docs.microsoft.com/aspnet/core/web-api/)
- [Npgsql PostgreSQL Provider](https://www.npgsql.org/efcore/)
- [Extension Methods (C#)](https://docs.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/extension-methods)

## ?? Contributing

This is a practice project. Feel free to use and modify as needed.

## ?? License

This project is for educational purposes.
