# 🚨 Database Setup - Quick Migration Guide

## 📋 Prerequisites

- [ ] PostgreSQL is installed and running
- [ ] .NET 10 SDK installed
- [ ] Connection string in `appsettings.json` is correct

---

## 🚀 Quick Setup (5 Steps)

### Step 1: Install EF Core Tools

```powershell
dotnet tool install --global dotnet-ef --add-source https://api.nuget.org/v3/index.json --ignore-failed-sources
```

**Verify**:
```powershell
dotnet ef --version
```

---

### Step 2: Navigate to Project

```powershell
cd C:\Personal\PracticeAPI\PracticeAPI
```

**Verify**:
```powershell
dir *.csproj
# Should show: PracticeAPI.csproj
```

---

### Step 3: Create Migration

```powershell
dotnet ef migrations add InitialCreateWithAuth
```

**Expected**: `Done. To undo this action, use 'dotnet ef migrations remove'`

---

### Step 4: Apply Migration

```powershell
dotnet ef database update
```

**Expected**: `Applying migration 'XXX_InitialCreateWithAuth'. Done.`

---

### Step 5: Run Application

```powershell
dotnet run
```

**Expected**: Application starts and shows seeding logs

---

## ✅ Verification

After running the app, you should see:
```
[INF] Seeded role: SuperAdmin
[INF] Seeded role: Admin
[INF] Seeded role: Manager
[INF] Seeded role: User
[INF] Seeded SuperAdmin user with username: superadmin
[INF] Database seeding completed successfully
[INF] Now listening on: https://localhost:7xxx
```

**Access API**: `https://localhost:7xxx/scalar/v1`

**Test Login**: `superadmin` / `SuperAdmin@123`

---

## 🔧 Common Issues

### ❌ "dotnet-ef does not exist"
```powershell
# Clear cache and reinstall
dotnet nuget locals all --clear
dotnet nuget disable source "Lytx-ThirdParty-NuGet-Repo"
dotnet tool install --global dotnet-ef
```

See [NUGET_FIX_GUIDE.md](NUGET_FIX_GUIDE.md) for NuGet issues.

---

### ❌ "Database does not exist"
```powershell
# Create database first
psql -U postgres
CREATE DATABASE "EmployeeManagementDB";
\q

# Then retry
dotnet ef database update
```

---

### ❌ "Build failed"
```powershell
# Check build errors first
dotnet build

# Fix errors, then retry migration
```

---

### ❌ Migration already exists
```powershell
# Option 1: Remove and recreate
dotnet ef migrations remove
dotnet ef migrations add InitialCreateWithAuth
dotnet ef database update

# Option 2: Just apply existing
dotnet ef database update
```

---

## 🔄 Update Migration (After Model Changes)

```powershell
# Create new migration
dotnet ef migrations add YourMigrationName

# Apply to database
dotnet ef database update
```

---

## 🗑️ Remove Last Migration

```powershell
# Undo from database
dotnet ef database update PreviousMigrationName

# Remove migration files
dotnet ef migrations remove
```

---

## 🔄 Reset Database (Fresh Start)

```powershell
# Drop database
dotnet ef database drop --force

# Create and apply all migrations
dotnet ef database update

# Or manually
psql -U postgres
DROP DATABASE "EmployeeManagementDB";
CREATE DATABASE "EmployeeManagementDB";
\q
dotnet ef database update
```

---

## 📦 Database Tables Created

- **Departments** - Department information
- **Employees** - Employee information
- **Roles** - Authentication roles (SuperAdmin, Admin, Manager, User)
- **Users** - User accounts with password hashes
- **RefreshTokens** - JWT refresh tokens
- **__EFMigrationsHistory** - Migration tracking

---

## 📚 Default Users (Auto-Seeded)

| Username | Password | Role |
|----------|----------|------|
| superadmin | SuperAdmin@123 | SuperAdmin |
| admin | Admin@123 | Admin |
| manager | Manager@123 | Manager |
| user | User@123 | User |

⚠️ **Change these passwords immediately!**

---

## 🎯 Quick Command Reference

**Complete Setup**:
```powershell
# Install tools
dotnet tool install --global dotnet-ef --add-source https://api.nuget.org/v3/index.json --ignore-failed-sources

# Navigate
cd C:\Personal\PracticeAPI\PracticeAPI

# Create and apply migration
dotnet ef migrations add InitialCreateWithAuth
dotnet ef database update

# Run
dotnet run
```

**After Model Changes**:
```powershell
dotnet ef migrations add DescriptiveName
dotnet ef database update
```

**Fresh Start**:
```powershell
dotnet ef database drop --force
dotnet ef database update
```

**Remove Migration**:
```powershell
dotnet ef migrations remove
```

---

## 📖 More Help

- **NuGet Issues**: See [NUGET_FIX_GUIDE.md](NUGET_FIX_GUIDE.md)
- **Complete Guide**: See [COMPLETE_GUIDE.md](COMPLETE_GUIDE.md)
- **Documentation Index**: See [README.md](README.md)

---

**Status**: ⏳ **Ready to run migrations**

**Quick Start**: Copy commands from "Quick Command Reference" above! 🚀
