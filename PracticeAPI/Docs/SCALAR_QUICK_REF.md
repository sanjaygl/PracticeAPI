# ?? Quick Reference - New API Documentation

## ?? URLs Changed

| What | Old (Swagger) | New (Scalar) |
|------|---------------|--------------|
| **API Docs UI** | `/` or `/swagger` | `/scalar/v1` |
| **OpenAPI JSON** | `/swagger/v1/swagger.json` | `/openapi/v1.json` |

## ?? Quick Start

### 1. Run Application
```bash
dotnet run
```

### 2. Open Browser
```
https://localhost:7xxx/scalar/v1
```

### 3. Test Login
```http
POST /api/auth/login

{
  "username": "superadmin",
  "password": "SuperAdmin@123"
}
```

### 4. Authorize
1. Copy `accessToken` from response
2. Click **?? Authorize** button in Scalar
3. Enter: `Bearer {your-token}`
4. Click **Authenticate**

### 5. Test Protected Endpoints
All requests now include your JWT token automatically! ?

---

## ? What's New

### Scalar UI Features
- ? Modern, fast interface
- ? Better JWT authentication UX
- ? Code generation (C#, JS, Python, etc.)
- ? Dark mode support
- ? Copy/paste friendly

### Why Changed?
- ? Swashbuckle 7.2.0 was incompatible with .NET 10
- ? Scalar is built specifically for .NET 10
- ? Better performance and features

---

## ?? For Developers

### Import OpenAPI to Tools

**Postman/Insomnia/Thunder Client**:
```
https://localhost:7xxx/openapi/v1.json
```

### Customize Scalar Theme

Edit `SwaggerExtensions.cs`:
```csharp
app.MapScalarApiReference(options =>
{
    options.WithTheme(ScalarTheme.Purple); // or Blue, Mars, Saturn
});
```

---

## ?? Documentation Files

- `SWAGGER_MIGRATION.md` - Full migration details
- `AUTHENTICATION_GUIDE.md` - Authentication docs
- `QUICK_START_AUTH.md` - Testing guide

---

## ? Build Status

**Current**: ? **Build Successful**
**Runtime**: ? **No Errors**
**UI**: ? **Scalar Working**

---

**Need help?** Check `SWAGGER_MIGRATION.md` for detailed information.
