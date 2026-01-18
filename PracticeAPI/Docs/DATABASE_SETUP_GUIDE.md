# 🚨 Database Setup - Migration Required

## ❌ Current Error

```
PostgresException: 42P01: relation "Roles" does not exist POSITION: 41
```

**Cause**: Database tables haven't been created yet. You need to run EF Core migrations.

---

## ✅ Step-by-Step Solution

### Step 1: Stop the Running Application

Press **Ctrl+C** or stop the debugger in Visual Studio.

- [ ] Application stopped

---

### Step 2: Install EF Core Tools (First Time Only)

**Option A: Using Package Manager Console in Visual Studio**

1. Open Visual Studio
2. Go to **Tools** → **NuGet Package Manager** → **Package Manager Console**
3. Run this command:

```powershell
dotnet tool install --global dotnet-ef
```

**Option B: Using Terminal/Command Prompt**

```bash
dotnet tool install --global dotnet-ef
```

**Expected Output**:
```
You can invoke the tool using the following command: dotnet-ef
Tool 'dotnet-ef' (version 'X.X.X') was successfully installed.
```

**If already installed**, you might see:
```
Tool 'dotnet-ef' is already installed.
```

**To update existing installation**:
```bash
dotnet tool update --global dotnet-ef
```

- [ ] EF Core tools installed successfully
- [ ] Verified with: `dotnet ef --version`

---

### Step 3: Navigate to Project Directory

**In Package Manager Console (Visual Studio):**
```powershell
cd C:\Personal\PracticeAPI\PracticeAPI
```

**Or in Command Prompt/Terminal:**
```bash
cd C:\Personal\PracticeAPI\PracticeAPI
```

**Verify you're in the correct directory** (should contain `PracticeAPI.csproj`):
```powershell
dir *.csproj
# Should show: PracticeAPI.csproj
```

- [ ] Navigated to correct directory
- [ ] `PracticeAPI.csproj` file exists in current directory

---

### Step 4: Create the Migration

Run this command in Package Manager Console or Terminal:

```bash
dotnet ef migrations add InitialCreateWithAuth
```

**Expected Output**:
```
Build started...
Build succeeded.
Done. To undo this action, use 'dotnet ef migrations remove'
```

**What this does**:
- Analyzes your DbContext and entity models
- Creates migration files in `Migrations/` folder
- Generates code to create all tables and relationships

**If you get an error**, check:
- You're in the project directory (where `.csproj` is)
- The project builds successfully: `dotnet build`
- EF Core packages are installed (they are in your `.csproj`)

- [ ] Migration created successfully
- [ ] `Migrations/` folder created with new files
- [ ] No errors in output

---

### Step 5: Verify Migration Files

Check that these files were created:

```
PracticeAPI/
└── Migrations/
    ├── 20240XXX_InitialCreateWithAuth.cs          # The migration file
    └── ApplicationDbContextModelSnapshot.cs        # Current model snapshot
```

**In Package Manager Console**:
```powershell
dir Migrations
```

**In Command Prompt**:
```bash
dir Migrations
```

- [ ] Migration files exist in `Migrations/` folder

---

### Step 6: Apply the Migration to Database

Run this command:

```bash
dotnet ef database update
```

**Expected Output**:
```
Build started...
Build succeeded.
Applying migration '20240XXX_InitialCreateWithAuth'.
Done.
```

**What this does**:
- Connects to PostgreSQL database
- Creates all tables (Departments, Employees, Roles, Users, RefreshTokens)
- Sets up foreign keys and indexes
- Applies all database constraints

**If you get "database does not exist" error**, see Troubleshooting section below.

- [ ] Migration applied successfully
- [ ] No errors in output
- [ ] Database updated

---

### Step 7: Verify Tables Created

**Option A: Using psql (PostgreSQL CLI)**
```bash
psql -U postgres -d EmployeeManagementDB -c "\dt"
```

**Option B: Using psql Interactive**
```bash
psql -U postgres -d EmployeeManagementDB

# List all tables
\dt

# Exit
\q
```

**Expected tables**:
- Departments
- Employees
- Roles
- Users
- RefreshTokens
- __EFMigrationsHistory (tracks applied migrations)

**Option C: Using pgAdmin**
- Open pgAdmin
- Navigate to: Databases → EmployeeManagementDB → Schemas → public → Tables
- Right-click on "Tables" → Refresh
- Verify all 6 tables exist

- [ ] All 6 tables verified (including __EFMigrationsHistory)

---

### Step 8: Run the Application

**In Visual Studio**: Press **F5** or click the green **Run** button

**Or in Terminal**:
```bash
dotnet run
```

**Expected Behavior**:
1. Application starts successfully
2. Database seeding runs automatically
3. Console logs show:
   ```
   [INF] Seeded role: SuperAdmin
   [INF] Seeded role: Admin
   [INF] Seeded role: Manager
   [INF] Seeded role: User
   [INF] Seeded SuperAdmin user with username: superadmin
   [WRN] Default SuperAdmin password is 'SuperAdmin@123'. Please change it immediately!
   [INF] Seeded test user: admin
   [INF] Seeded test user: manager
   [INF] Seeded test user: user
   [INF] Database seeding completed successfully
   ```

- [ ] Application started without errors
- [ ] Seeding logs appeared in console
- [ ] No database-related errors
- [ ] Application running on https://localhost:XXXX

---

### Step 9: Access Scalar UI

Open your browser and navigate to:
```
https://localhost:7xxx/scalar/v1
```
(Replace `7xxx` with the actual port shown in your console)

- [ ] Scalar UI accessible
- [ ] All endpoints visible
- [ ] No JavaScript errors

---

### Step 10: Test Authentication

1. **In Scalar UI**, navigate to `/api/auth/login`
2. Click **"Try it out"**
3. Enter credentials:
   ```json
   {
     "username": "superadmin",
     "password": "SuperAdmin@123"
   }
   ```
4. Click **"Execute"**

**Expected Response (200 OK)**:
```json
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "base64-string...",
  "expiresIn": 900,
  "username": "superadmin",
  "role": "SuperAdmin"
}
```

5. **Copy the `accessToken`**
6. Click the **🔒 Authorize** button (top right in Scalar)
7. Paste: `Bearer {your-access-token}`
8. Click **"Authorize"**

- [ ] Login successful (200 OK)
- [ ] Received access token and refresh token
- [ ] Can authorize in Scalar UI with token
- [ ] Can test protected endpoints

---

## 📋 Setup Completion Checklist

Mark each item as you complete it:

### Prerequisites
- [ ] PostgreSQL is installed and running
- [ ] Connection string in `appsettings.json` is correct
- [ ] .NET 10 SDK installed
- [ ] Visual Studio 2022 or VS Code installed
- [ ] EF Core tools installed (`dotnet tool install --global dotnet-ef`)

### Migration Steps
- [ ] Application stopped
- [ ] Navigated to correct directory (`PracticeAPI.csproj` location)
- [ ] EF Core tools installed/verified
- [ ] Migration created (`dotnet ef migrations add InitialCreateWithAuth`)
- [ ] Migration files exist in `Migrations/` folder
- [ ] Migration applied (`dotnet ef database update`)
- [ ] All 6 tables exist in database

### Verification
- [ ] Application runs without errors
- [ ] Database seeding completed
- [ ] 4 roles seeded (SuperAdmin, Admin, Manager, User)
- [ ] SuperAdmin user created
- [ ] Test users created (dev environment)

### Testing
- [ ] Can access Scalar UI at `/scalar/v1`
- [ ] Can login with `superadmin`/`SuperAdmin@123`
- [ ] Received JWT tokens
- [ ] Can authorize and test protected endpoints

### Post-Setup (Important!)
- [ ] ⚠️ Change SuperAdmin default password
- [ ] ⚠️ Change JWT SecretKey in production
- [ ] ⚠️ Change all test user passwords

---

## 🔧 Troubleshooting

### Issue: "dotnet-ef does not exist"

**Error Message**:
```
Could not execute because the specified command or file was not found.
You intended to run a global tool, but a dotnet-prefixed executable with this name could not be found on the PATH.
```

**Solution**: Install EF Core tools globally:
```bash
dotnet tool install --global dotnet-ef
```

**Verify installation**:
```bash
dotnet ef --version
# Should show: Entity Framework Core .NET Command-line Tools
```

**If still not working**, try:
1. Close and reopen your terminal/Package Manager Console
2. Restart Visual Studio
3. Check PATH environment variable includes .NET tools

- [ ] EF Core tools installed
- [ ] Terminal/Console restarted
- [ ] `dotnet ef --version` works

---

### Issue: "No migrations configuration type was found"

**Error Message**:
```
Unable to create an object of type 'ApplicationDbContext'
```

**Solution**: Ensure you're in the correct directory:
```bash
# Should be in the directory containing PracticeAPI.csproj
cd C:\Personal\PracticeAPI\PracticeAPI

# Verify
dir *.csproj
# Should show PracticeAPI.csproj
```

- [ ] Verified correct directory
- [ ] Can see `PracticeAPI.csproj` file

---

### Issue: "Connection refused" or "Cannot connect to PostgreSQL"

**Error Message**:
```
Npgsql.NpgsqlException: Connection refused
```

**Solution 1**: Check if PostgreSQL is running

**Windows**:
```bash
# Check status
sc query postgresql-x64-16

# Start if stopped
net start postgresql-x64-16
```

**Solution 2**: Verify connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=EmployeeManagementDB;Username=postgres;Password=postgres"
  }
}
```

**Solution 3**: Test connection manually:
```bash
psql -U postgres -h localhost -p 5432
# Enter password when prompted
```

- [ ] PostgreSQL service is running
- [ ] Connection string is correct
- [ ] Can connect manually with psql

---

### Issue: "Database 'EmployeeManagementDB' does not exist"

**Error Message**:
```
Npgsql.PostgresException: 3D000: database "EmployeeManagementDB" does not exist
```

**Solution**: Create the database first:

**Option A: Using psql**
```bash
psql -U postgres

CREATE DATABASE "EmployeeManagementDB";

\q
```

**Option B: Using pgAdmin**
1. Open pgAdmin
2. Right-click on "Databases"
3. Select "Create" → "Database..."
4. Enter name: `EmployeeManagementDB`
5. Click "Save"

**Then run migration again**:
```bash
dotnet ef database update
```

- [ ] Database created manually
- [ ] Migration re-applied successfully

---

### Issue: "Build failed" during migration

**Error Message**:
```
Build started...
Build FAILED.
```

**Solution**: Build the project first to see errors:
```bash
dotnet build
```

Fix any compilation errors, then retry:
```bash
dotnet ef migrations add InitialCreateWithAuth
```

- [ ] Project builds successfully
- [ ] No compilation errors

---

### Issue: Migration already exists

**Error Message**:
```
The migration '20240XXX_InitialCreateWithAuth' has already been applied to the database.
```

**Solution 1**: Remove and recreate migration:
```bash
# Remove the migration
dotnet ef migrations remove

# Create new migration
dotnet ef migrations add InitialCreateWithAuth

# Apply it
dotnet ef database update
```

**Solution 2**: Just apply existing migration:
```bash
dotnet ef database update
```

- [ ] Old migration removed (if needed)
- [ ] New migration created
- [ ] Migration applied

---

### Issue: Permission denied on PostgreSQL

**Error Message**:
```
Permission denied for database
```

**Solution**: Check PostgreSQL user permissions:
```sql
-- Connect as superuser
psql -U postgres

-- Grant permissions
GRANT ALL PRIVILEGES ON DATABASE "EmployeeManagementDB" TO postgres;

-- Quit
\q
```

- [ ] Permissions granted
- [ ] Can connect and create tables

---

## 🗂️ What Tables Will Be Created

### 1. **Departments**
```sql
CREATE TABLE "Departments" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL
);
CREATE UNIQUE INDEX "IX_Departments_Name" ON "Departments" ("Name");
```

### 2. **Employees**
```sql
CREATE TABLE "Employees" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Gender" VARCHAR(50),
    "Salary" DECIMAL(18,2) NOT NULL,
    "DepartmentId" INT,
    FOREIGN KEY ("DepartmentId") REFERENCES "Departments"("Id") ON DELETE SET NULL
);
CREATE INDEX "IX_Employees_DepartmentId" ON "Employees" ("DepartmentId");
```

### 3. **Roles**
```sql
CREATE TABLE "Roles" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(50) NOT NULL
);
CREATE UNIQUE INDEX "IX_Roles_Name" ON "Roles" ("Name");
```

### 4. **Users**
```sql
CREATE TABLE "Users" (
    "Id" SERIAL PRIMARY KEY,
    "Username" VARCHAR(100) NOT NULL,
    "PasswordHash" TEXT NOT NULL,
    "RoleId" INT NOT NULL,
    "IsActive" BOOLEAN DEFAULT TRUE,
    "CreatedAt" TIMESTAMP DEFAULT NOW(),
    FOREIGN KEY ("RoleId") REFERENCES "Roles"("Id") ON DELETE RESTRICT
);
CREATE UNIQUE INDEX "IX_Users_Username" ON "Users" ("Username");
```

### 5. **RefreshTokens**
```sql
CREATE TABLE "RefreshTokens" (
    "Id" SERIAL PRIMARY KEY,
    "Token" VARCHAR(500) NOT NULL,
    "UserId" INT NOT NULL,
    "ExpiresAt" TIMESTAMP NOT NULL,
    "IsRevoked" BOOLEAN DEFAULT FALSE,
    "CreatedAt" TIMESTAMP DEFAULT NOW(),
    "CreatedByIp" VARCHAR(50),
    FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE
);
CREATE UNIQUE INDEX "IX_RefreshTokens_Token" ON "RefreshTokens" ("Token");
CREATE INDEX "IX_RefreshTokens_UserId" ON "RefreshTokens" ("UserId");
```

---

## 📋 Quick Command Reference

**All commands in order**:

```bash
# 1. Install EF Core tools (first time only)
dotnet tool install --global dotnet-ef

# 2. Navigate to project directory
cd C:\Personal\PracticeAPI\PracticeAPI

# 3. Verify you're in correct location
dir *.csproj
# Should show: PracticeAPI.csproj

# 4. Create migration
dotnet ef migrations add InitialCreateWithAuth

# 5. Apply migration
dotnet ef database update

# 6. Run application
dotnet run

# 7. Verify database (optional)
psql -U postgres -d EmployeeManagementDB -c "\dt"
```

**If you need to start fresh**:
```bash
# Drop and recreate database
dotnet ef database drop --force
dotnet ef database update

# Or manually in psql
psql -U postgres
DROP DATABASE "EmployeeManagementDB";
CREATE DATABASE "EmployeeManagementDB";
\q

# Then apply migrations
dotnet ef database update
```

---

## ✅ Success Indicators

After running migrations successfully, you should see:

### 1. Migration Files Created
```
PracticeAPI/
└── Migrations/
    ├── 20240XXX_InitialCreateWithAuth.cs
    └── ApplicationDbContextModelSnapshot.cs
```

### 2. Database Tables
Execute this in psql:
```sql
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public' 
ORDER BY table_name;
```

**Expected tables**:
- Departments
- Employees
- Roles
- Users
- RefreshTokens
- __EFMigrationsHistory

### 3. Application Logs
When you run `dotnet run`, you should see:
```
[INF] Database seeding completed successfully
[INF] Now listening on: https://localhost:7xxx
```

### 4. Seeded Data
Query the database:
```sql
-- Check roles
SELECT * FROM "Roles";
-- Should show: SuperAdmin, Admin, Manager, User

-- Check users
SELECT "Username", "IsActive" FROM "Users";
-- Should show: superadmin, admin, manager, user (in dev)
```

---

## 🎯 After Migration - Next Steps

### 1. Verify Seeded Data

**Using psql**:
```sql
psql -U postgres -d EmployeeManagementDB

-- Check roles
SELECT * FROM "Roles";

-- Check users
SELECT "Id", "Username", "RoleId", "IsActive" FROM "Users";

-- Check if passwords are hashed
SELECT "Username", LENGTH("PasswordHash") as hash_length 
FROM "Users" 
WHERE "Username" = 'superadmin';
-- hash_length should be 60 (BCrypt)

\q
```

- [ ] Query executed successfully
- [ ] 4 roles exist
- [ ] All users visible
- [ ] Passwords are hashed (length = 60)

### 2. Test API Endpoints

**Test Login** (using curl or Postman):
```bash
curl -X POST https://localhost:7xxx/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"superadmin","password":"SuperAdmin@123"}'
```

**Expected Response**:
```json
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "...",
  "expiresIn": 900,
  "username": "superadmin",
  "role": "SuperAdmin"
}
```

- [ ] Login successful
- [ ] Tokens received
- [ ] Can authenticate

### 3. Access Documentation

Open Scalar UI:
```
https://localhost:7xxx/scalar/v1
```

- [ ] UI loads correctly
- [ ] All endpoints visible
- [ ] Can test endpoints interactively

---

## 🔄 Alternative: Complete Fresh Start

If you want to **completely reset** everything:

```bash
# Stop the application first

# 1. Drop database
psql -U postgres -c "DROP DATABASE \"EmployeeManagementDB\";"

# 2. Create fresh database
psql -U postgres -c "CREATE DATABASE \"EmployeeManagementDB\";"

# 3. Navigate to project
cd C:\Personal\PracticeAPI\PracticeAPI

# 4. Remove ALL migrations (optional)
dotnet ef migrations remove
# Repeat until: "No migrations were found"

# 5. Delete Migrations folder (optional)
Remove-Item -Recurse -Force .\Migrations

# 6. Create new migration
dotnet ef migrations add InitialCreate

# 7. Apply migration
dotnet ef database update

# 8. Run application (seeding happens automatically)
dotnet run
```

**Fresh Start Checklist**:
- [ ] Database dropped
- [ ] Database recreated
- [ ] Old migrations removed
- [ ] Migrations folder deleted
- [ ] New migration created
- [ ] Migration applied
- [ ] Application running
- [ ] Seeding completed successfully

---

## 📞 Still Need Help?

### Common Checks

1. **EF Core Tools Installed?**
   ```bash
   dotnet ef --version
   ```
   Should show version, not "command not found"

2. **Correct Directory?**
   ```bash
   dir *.csproj
   ```
   Should show `PracticeAPI.csproj`

3. **PostgreSQL Running?**
   ```bash
   psql -U postgres -c "SELECT version();"
   ```
   Should connect and show PostgreSQL version

4. **Project Builds?**
   ```bash
   dotnet build
   ```
   Should succeed with no errors

5. **Connection String Correct?**
   Check `appsettings.json`:
   ```json
   "DefaultConnection": "Host=localhost;Port=5432;Database=EmployeeManagementDB;Username=postgres;Password=postgres"
   ```

**Troubleshooting Checklist**:
- [ ] EF Core tools installed and verified
- [ ] PostgreSQL service running
- [ ] Connection string correct in appsettings.json
- [ ] In correct project directory
- [ ] Project builds successfully
- [ ] .NET 10 SDK installed

---

## 🎉 Setup Complete!

Once all checkboxes above are marked:
- ✅ Your database is set up
- ✅ Tables are created with proper relationships
- ✅ Data is seeded (roles and users)
- ✅ Authentication is working
- ✅ API is ready for development!

**Next Steps**:
1. ✅ Explore the API using Scalar UI at `/scalar/v1`
2. ✅ Test all authentication flows (login, refresh, logout)
3. ✅ Review the authentication documentation
4. ⚠️ **IMPORTANT**: Change all default passwords!
5. ⚠️ **IMPORTANT**: Change JWT SecretKey before production!

---

## 📚 Related Documentation

- `AUTHENTICATION_GUIDE.md` - Complete authentication documentation
- `QUICK_START_AUTH.md` - Quick testing guide
- `SWAGGER_MIGRATION.md` - Scalar UI documentation
- `DEPLOYMENT_CHECKLIST.md` - Pre-deployment checklist

---

**Current Status**: ⏳ **Ready to run migrations**

**Quick Start Commands**:
```bash
# In Package Manager Console or Terminal:
dotnet tool install --global dotnet-ef
cd C:\Personal\PracticeAPI\PracticeAPI
dotnet ef migrations add InitialCreateWithAuth
dotnet ef database update
dotnet run
```

**Then access**: `https://localhost:7xxx/scalar/v1` 🚀
