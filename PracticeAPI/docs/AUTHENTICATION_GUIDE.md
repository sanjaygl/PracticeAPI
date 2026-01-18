# JWT Authentication & Authorization Implementation Guide

## ?? Overview

This API now includes **comprehensive JWT-based authentication** with refresh tokens and **role-based authorization** using PostgreSQL for user management.

---

## ?? Features Implemented

? **JWT Access Tokens** (Short-lived - 15 minutes)
? **Refresh Tokens** (Long-lived - 30 days)
? **Token Rotation** on every refresh
? **Role-Based Authorization** (Super Admin, Admin, Manager, User)
? **Password Hashing** using BCrypt
? **Database Seeding** with default users and roles
? **Centralized Authentication Extension**
? **Swagger Integration** (Ready for JWT Bearer)
? **IP Tracking** for refresh tokens
? **Token Revocation** on logout

---

## ??? Architecture

### Domain Models

1. **Role**
   - Id (PK)
   - Name (Unique: SuperAdmin, Admin, Manager, User)

2. **User**
   - Id (PK)
   - Username (Unique)
   - PasswordHash (BCrypt)
   - RoleId (FK ? Role)
   - IsActive
   - CreatedAt

3. **RefreshToken**
   - Id (PK)
   - Token (Unique, 64-byte random string)
   - UserId (FK ? User)
   - ExpiresAt
   - IsRevoked
   - CreatedAt
   - CreatedByIp

---

## ?? Configuration

### appsettings.json

```json
{
  "Jwt": {
    "SecretKey": "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLongForHS256",
    "Issuer": "PracticeAPI",
    "Audience": "PracticeAPIClient",
    "AccessTokenExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 30
  }
}
```

?? **IMPORTANT**: Change the `SecretKey` to a strong, random value in production!

---

## ?? Database Migration

### Step 1: Create Migration

```bash
cd PracticeAPI
dotnet ef migrations add AddAuthenticationTables
```

### Step 2: Update Database

```bash
dotnet ef database update
```

### Step 3: Verify Seeding

The application will automatically seed:
- 4 Roles: SuperAdmin, Admin, Manager, User
- Default SuperAdmin user
- (Development only) Test users for each role

---

## ?? Default Users (Seeded)

| Username | Password | Role | Access Level |
|----------|----------|------|--------------|
| superadmin | SuperAdmin@123 | SuperAdmin | Full system access |
| admin | Admin@123 | Admin | Manage Employees & Departments |
| manager | Manager@123 | Manager | Manage Employees only |
| user | User@123 | User | Read-only access |

?? **Security**: Change all default passwords immediately after first login!

---

## ?? API Endpoints

### Authentication Endpoints

#### 1. Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "superadmin",
  "password": "SuperAdmin@123"
}
```

**Response (200 OK)**:
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64-encoded-random-token",
  "expiresIn": 900,
  "username": "superadmin",
  "role": "SuperAdmin"
}
```

**Error Responses**:
- `400 Bad Request`: Validation errors
- `401 Unauthorized`: Invalid credentials

---

#### 2. Refresh Token
```http
POST /api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "previous-refresh-token"
}
```

**Response (200 OK)**:
```json
{
  "accessToken": "new-jwt-token",
  "refreshToken": "new-refresh-token",
  "expiresIn": 900
}
```

**Behavior**:
- Old refresh token is immediately revoked
- New token pair is issued
- Previous refresh token cannot be reused

---

#### 3. Logout
```http
POST /api/auth/logout
Authorization: Bearer {access-token}
Content-Type: application/json

{
  "refreshToken": "current-refresh-token"
}
```

**Response (200 OK)**:
```json
{
  "message": "Logged out successfully"
}
```

---

#### 4. Get Current User (Test Endpoint)
```http
GET /api/auth/me
Authorization: Bearer {access-token}
```

**Response (200 OK)**:
```json
{
  "userId": "1",
  "username": "superadmin",
  "role": "SuperAdmin",
  "claims": [
    { "type": "nameid", "value": "1" },
    { "type": "unique_name", "value": "superadmin" },
    { "type": "role", "value": "SuperAdmin" }
  ]
}
```

---

## ?? Authorization Rules

### Departments API

| Endpoint | Method | Roles Allowed |
|----------|--------|---------------|
| GET /api/departments | GET | All authenticated users |
| GET /api/departments/{id} | GET | All authenticated users |
| POST /api/departments | POST | SuperAdmin, Admin |
| PUT /api/departments/{id} | PUT | SuperAdmin, Admin |
| DELETE /api/departments/{id} | DELETE | **SuperAdmin only** |

### Employees API

| Endpoint | Method | Roles Allowed |
|----------|--------|---------------|
| GET /api/employees | GET | All authenticated users |
| GET /api/employees/{id} | GET | All authenticated users |
| POST /api/employees | POST | SuperAdmin, Admin, Manager |
| PUT /api/employees/{id} | PUT | SuperAdmin, Admin, Manager |
| DELETE /api/employees/{id} | DELETE | SuperAdmin, Admin |

---

## ?? Testing with Swagger UI

### Step 1: Login
1. Navigate to `https://localhost:xxxx` (Swagger UI)
2. Expand `/api/auth/login`
3. Click "Try it out"
4. Enter credentials (e.g., superadmin/SuperAdmin@123)
5. Click "Execute"
6. Copy the `accessToken` from the response

### Step 2: Authorize
1. Click the "Authorize" button at the top right (lock icon)
2. Enter: `Bearer {your-access-token}`
3. Click "Authorize"
4. Click "Close"

### Step 3: Test Protected Endpoints
- All subsequent requests will include the Bearer token
- Try different endpoints based on your role
- Observe `403 Forbidden` for unauthorized actions

---

## ??? Implementation Details

### JWT Token Claims

Access tokens include:
- `nameid` (NameIdentifier): User ID
- `unique_name` (Name): Username
- `role` (Role): User's role
- Custom claims: `userId`, `username`, `role`

### Token Validation

- **Issuer Validation**: Ensures token is from PracticeAPI
- **Audience Validation**: Ensures token is for PracticeAPIClient
- **Lifetime Validation**: Checks expiration with zero clock skew
- **Signature Validation**: Verifies HMACSHA256 signature

### Refresh Token Security

- **Random Generation**: 64-byte cryptographically secure random tokens
- **One-Time Use**: Revoked immediately after use
- **IP Tracking**: Logs the IP address that created the token
- **Expiration**: 30-day validity (configurable)
- **Revocation**: Can be manually revoked or auto-revoked on logout

---

## ?? Security Best Practices

### ? Implemented

1. **Password Hashing**: BCrypt with automatic salt
2. **Token Rotation**: New refresh token on every refresh
3. **Short-Lived Access Tokens**: 15 minutes (mitigates token theft)
4. **HTTPS Only** (in production - set `RequireHttpsMetadata = true`)
5. **No Plain-Text Passwords**: Never stored or logged
6. **Failed Login Logging**: All attempts are logged
7. **IP Tracking**: Refresh tokens track creator IP

### ?? Recommended Enhancements

1. **Rate Limiting**: Implement on login endpoint
2. **Account Lockout**: After N failed attempts
3. **Password Complexity**: Enforce strong passwords
4. **Refresh Token Limit**: Max N active tokens per user
5. **Refresh Token Reuse Detection**: Revoke all user tokens on reuse
6. **Two-Factor Authentication** (2FA)
7. **Audit Logging**: Track all authentication events
8. **Token Blacklisting**: For immediate revocation

---

## ?? Extension Architecture

### AuthenticationExtensions.cs

Provides:
- JWT Bearer configuration
- Token validation parameters
- Role-based authorization policies
- Event handlers for logging

**Usage**:
```csharp
services.AddJwtAuthentication(configuration);
```

**Policies Defined**:
- `SuperAdminOnly`: SuperAdmin role only
- `AdminOnly`: SuperAdmin, Admin
- `ManagerOnly`: SuperAdmin, Admin, Manager
- `UserAccess`: All authenticated users

---

## ?? Database Seeding

### Automatic Seeding on Startup

```csharp
// In Program.cs
await DatabaseSeeder.SeedAsync(context, logger);

// In Development only
await DatabaseSeeder.SeedTestUsersAsync(context, logger);
```

### Idempotent Seeding

- Roles and users are only created if they don't exist
- Safe to run multiple times
- No duplicate data

### Customization

Edit `DatabaseSeeder.cs` to:
- Change default passwords
- Add more roles
- Seed additional test users
- Customize user properties

---

## ?? Troubleshooting

### Issue: "401 Unauthorized" on protected endpoints

**Solution**:
1. Verify token is included in Authorization header
2. Format: `Authorization: Bearer {token}`
3. Check token hasn't expired (15 min lifetime)
4. Verify token signature matches SecretKey

### Issue: "403 Forbidden" on endpoint

**Solution**:
1. Check user's role matches endpoint requirements
2. Verify role claim is present in token
3. Review authorization attribute on controller

### Issue: Refresh token "Invalid or expired"

**Solution**:
1. Ensure refresh token hasn't been used before
2. Check if token is expired (30 days)
3. Verify token wasn't revoked on logout
4. Confirm user account is still active

### Issue: Cannot login with seeded users

**Solution**:
1. Verify database migration ran successfully
2. Check seeding logs for errors
3. Confirm PostgreSQL connection is working
4. Ensure BCrypt package is installed

---

## ?? Example Postman Collection

### Environment Variables
```json
{
  "baseUrl": "https://localhost:7xxx",
  "accessToken": "",
  "refreshToken": ""
}
```

### Test Script (Save tokens after login)
```javascript
if (pm.response.code === 200) {
    var jsonData = pm.response.json();
    pm.environment.set("accessToken", jsonData.accessToken);
    pm.environment.set("refreshToken", jsonData.refreshToken);
}
```

### Authorization Header
```
Authorization: Bearer {{accessToken}}
```

---

## ?? Next Steps

### Phase 1: Current Implementation ?
- [x] JWT Authentication
- [x] Refresh Token system
- [x] Role-based authorization
- [x] Database seeding
- [x] Password hashing

### Phase 2: Enhancements ??
- [ ] Password change endpoint
- [ ] Password reset (email-based)
- [ ] User registration endpoint
- [ ] Account activation
- [ ] Email verification

### Phase 3: Advanced Features ??
- [ ] Two-Factor Authentication (2FA)
- [ ] OAuth2/OpenID Connect
- [ ] Social login (Google, Facebook)
- [ ] Refresh token rotation detection
- [ ] Session management
- [ ] Activity tracking

---

## ?? Code Examples

### Manual Authorization Check in Code

```csharp
[HttpPost("special-action")]
[Authorize]
public IActionResult SpecialAction()
{
    var userId = User.FindFirst("userId")?.Value;
    var role = User.FindFirst("role")?.Value;
    
    if (role == "SuperAdmin")
    {
        // SuperAdmin-only logic
    }
    
    return Ok();
}
```

### Policy-Based Authorization

```csharp
[HttpDelete("{id}")]
[Authorize(Policy = "SuperAdminOnly")]
public async Task<IActionResult> DeleteDepartment(int id)
{
    // Only SuperAdmin can access this
}
```

### Multiple Roles

```csharp
[HttpPost]
[Authorize(Roles = "SuperAdmin,Admin,Manager")]
public async Task<IActionResult> CreateEmployee(...)
{
    // SuperAdmin, Admin, or Manager can access
}
```

---

## ?? Monitoring & Logging

### Authentication Events Logged

- ? Successful logins
- ? Failed login attempts
- ? Token refresh operations
- ? Token validation failures
- ? Logout operations
- ? Authorization challenges

### Log Example

```
[INF] User logged in successfully - superadmin
[WRN] Login failed: User not found - hacker123
[INF] Token refreshed successfully for user 1
[ERR] Authentication failed: Token has expired
```

---

## ?? Support & Contributing

For issues or questions:
1. Check this documentation
2. Review the code comments
3. Check application logs
4. Refer to Microsoft JWT documentation

---

## ?? License

This authentication system is part of the PracticeAPI project and follows the same licensing terms.

---

**Last Updated**: [Current Date]
**API Version**: v1
**Authentication Version**: JWT + Refresh Token v1.0
