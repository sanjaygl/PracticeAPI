# ?? Quick Start Guide - Authentication Setup

## Prerequisites Checklist
- ? PostgreSQL running on localhost:5432
- ? .NET 10 SDK installed
- ? Connection string updated in appsettings.json

## Step-by-Step Setup

### 1. Create Database Migration
```bash
cd C:\Personal\PracticeAPI\PracticeAPI
dotnet ef migrations add AddAuthenticationTables
```

### 2. Apply Migration & Seed Database
```bash
dotnet ef database update
```

### 3. Run the Application
```bash
dotnet run
```

The application will automatically:
- Create database tables
- Seed 4 roles (SuperAdmin, Admin, Manager, User)
- Create default SuperAdmin user
- Seed test users (in Development mode)

### 4. Access Swagger UI
Navigate to: `https://localhost:7xxx` (check console output for exact port)

---

## ?? Test Authentication Flow

### Test 1: Login as SuperAdmin

**Request**:
```http
POST /api/auth/login
```
```json
{
  "username": "superadmin",
  "password": "SuperAdmin@123"
}
```

**Expected Response (200)**:
```json
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "random-base64-string",
  "expiresIn": 900,
  "username": "superadmin",
  "role": "SuperAdmin"
}
```

? **Success**: Copy the `accessToken`

---

### Test 2: Authorize in Swagger

1. Click the ?? **Authorize** button (top right)
2. Enter: `Bearer {your-access-token}`
3. Click **Authorize**
4. Click **Close**

---

### Test 3: Test Protected Endpoint

**Request**:
```http
GET /api/auth/me
```

**Expected Response (200)**:
```json
{
  "userId": "1",
  "username": "superadmin",
  "role": "SuperAdmin",
  "claims": [...]
}
```

? **Success**: You're authenticated!

---

### Test 4: Test Role-Based Access

**As SuperAdmin - Create Department** (Should succeed):
```http
POST /api/departments
```
```json
{
  "name": "Engineering"
}
```

**Expected**: `201 Created`

---

**Login as User** (read-only):
```http
POST /api/auth/login
```
```json
{
  "username": "user",
  "password": "User@123"
}
```

**Try to Create Department**:
```http
POST /api/departments
```

**Expected**: `403 Forbidden` ?

? **Success**: Role-based authorization is working!

---

### Test 5: Refresh Token Flow

1. Wait until access token expires (15 minutes) OR
2. Test refresh immediately

**Request**:
```http
POST /api/auth/refresh
```
```json
{
  "refreshToken": "your-refresh-token-from-login"
}
```

**Expected Response (200)**:
```json
{
  "accessToken": "new-token",
  "refreshToken": "new-refresh-token",
  "expiresIn": 900
}
```

? **Success**: Token rotation is working!

?? **Note**: Old refresh token is now invalid (try using it again ? 401)

---

### Test 6: Logout

**Request**:
```http
POST /api/auth/logout
Authorization: Bearer {your-access-token}
```
```json
{
  "refreshToken": "your-current-refresh-token"
}
```

**Expected Response (200)**:
```json
{
  "message": "Logged out successfully"
}
```

? **Success**: Refresh token is revoked!

?? **Try to use the same refresh token** ? `401 Unauthorized`

---

## ?? All Default Test Users

| Username | Password | Role | Test Scenario |
|----------|----------|------|---------------|
| superadmin | SuperAdmin@123 | SuperAdmin | Full access - can delete departments |
| admin | Admin@123 | Admin | Can manage employees & departments |
| manager | Manager@123 | Manager | Can manage employees only |
| user | User@123 | User | Read-only access |

---

## ? Success Checklist

- [ ] Database created successfully
- [ ] Migrations applied without errors
- [ ] Application starts without errors
- [ ] Can login with superadmin
- [ ] Token is returned in login response
- [ ] Can access `/api/auth/me` with token
- [ ] Swagger Authorization button works
- [ ] Can create department as SuperAdmin
- [ ] Get 403 when user tries to create department
- [ ] Refresh token works
- [ ] Old refresh token is invalid after refresh
- [ ] Logout revokes refresh token

---

## ?? Common Issues & Solutions

### Issue: Migration fails with "table already exists"

**Solution**:
```bash
dotnet ef database drop --force
dotnet ef migrations remove
dotnet ef migrations add AddAuthenticationTables
dotnet ef database update
```

---

### Issue: Login returns 401 even with correct password

**Check**:
1. Database was seeded (check Roles and Users tables)
2. Password hashing is working (BCrypt package installed)
3. Check application logs for errors

**Solution**:
```bash
# Re-run seeding manually if needed
# Or drop database and recreate:
dotnet ef database drop --force
dotnet ef database update
```

---

### Issue: Token not accepted (401 Unauthorized)

**Check**:
1. Token format: `Authorization: Bearer {token}` (note the space!)
2. Token hasn't expired (15 min lifetime)
3. SecretKey in appsettings.json is correct
4. No extra spaces or line breaks in token

---

### Issue: 403 Forbidden on endpoint

**Check**:
1. User has correct role for the endpoint
2. Token contains role claim (check `/api/auth/me`)
3. Authorization attribute on controller

---

## ?? Verify Database

### Check Seeded Data

```sql
-- Connect to PostgreSQL
psql -U postgres -d EmployeeManagementDB

-- Check roles
SELECT * FROM "Roles";

-- Expected output:
-- Id | Name
-- 1  | SuperAdmin
-- 2  | Admin
-- 3  | Manager
-- 4  | User

-- Check users
SELECT "Id", "Username", "IsActive", "RoleId" FROM "Users";

-- Expected output (at minimum):
-- Id | Username   | IsActive | RoleId
-- 1  | superadmin | true     | 1

-- Check if password is hashed
SELECT "Username", LENGTH("PasswordHash") as "PasswordHashLength" 
FROM "Users" 
WHERE "Username" = 'superadmin';

-- Expected: PasswordHashLength should be 60 (BCrypt hash length)
```

---

## ?? Next Steps

Once authentication is working:

1. **Change Default Passwords**
   - Create password change endpoint
   - Update all default passwords

2. **Test All Endpoints**
   - Try each role with each endpoint
   - Verify authorization rules

3. **Production Deployment**
   - Change JWT SecretKey
   - Enable HTTPS only (`RequireHttpsMetadata = true`)
   - Set longer refresh token expiry
   - Configure logging
   - Set up rate limiting

4. **Monitoring**
   - Check logs for failed login attempts
   - Monitor refresh token usage
   - Track unauthorized access attempts

---

## ?? Need Help?

1. Check `AUTHENTICATION_GUIDE.md` for detailed documentation
2. Review application logs in console
3. Check EF Core logs for database issues
4. Verify PostgreSQL is running: `pg_isready`

---

**Happy Coding! ??**
