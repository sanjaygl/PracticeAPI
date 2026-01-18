# ?? PracticeAPI Documentation

Welcome to the PracticeAPI documentation!

---

## ?? Main Documentation

### [?? Complete Guide](COMPLETE_GUIDE.md)
**The comprehensive guide covering everything:**
- Quick Start (5-step setup)
- Authentication System (JWT + Refresh Tokens)
- API Endpoints & Authorization Rules
- Database Setup & Configuration
- Security Best Practices
- Testing Guide (Scalar UI, cURL, Postman)
- Troubleshooting Common Issues
- Architecture & Design Patterns

**?? Start here for full documentation!**

---

## ?? Setup Guides

### [??? Database Setup Guide](DATABASE_SETUP_GUIDE.md)
**Step-by-step database configuration:**
- Installing EF Core tools
- Creating migrations
- Applying migrations
- Verifying tables
- Troubleshooting database issues

**?? Use this when setting up the database**

---

## ?? Quick Reference

### Quick Start Commands
```bash
# 1. Install EF Core tools
dotnet tool install --global dotnet-ef

# 2. Navigate to project
cd C:\Personal\PracticeAPI\PracticeAPI

# 3. Create and apply migration
dotnet ef migrations add InitialCreateWithAuth
dotnet ef database update

# 4. Run application
dotnet run

# 5. Access API
# Docs: https://localhost:7xxx/scalar/v1
```

### Default Login
- **Username**: `superadmin`
- **Password**: `SuperAdmin@123`
- ?? **Change immediately after first login!**

---

## ?? API Endpoints

### Authentication
- `POST /api/auth/login` - Login
- `POST /api/auth/refresh` - Refresh token
- `POST /api/auth/logout` - Logout
- `GET /api/auth/me` - Current user

### Business
- `GET|POST|PUT|DELETE /api/departments` - Departments CRUD
- `GET|POST|PUT|DELETE /api/employees` - Employees CRUD

---

## ?? Roles & Permissions

| Role | Access Level |
|------|--------------|
| **SuperAdmin** | Full system access (all CRUD) |
| **Admin** | Manage employees & departments |
| **Manager** | Manage employees only |
| **User** | Read-only access |

---

## ?? Common Issues

### "dotnet-ef does not exist"
```bash
dotnet tool install --global dotnet-ef
```

### "relation 'Roles' does not exist"
```bash
dotnet ef migrations add InitialCreateWithAuth
dotnet ef database update
```

### "401 Unauthorized"
- Check token format: `Authorization: Bearer {token}`
- Token might be expired (15 min lifetime)
- Use `/api/auth/login` to get new token

### "403 Forbidden"
- User doesn't have required role
- Check `/api/auth/me` to verify user's role
- Login with appropriate user

---

## ?? Need More Help?

1. Check **[Complete Guide](COMPLETE_GUIDE.md)** for detailed information
2. Check **[Database Setup Guide](DATABASE_SETUP_GUIDE.md)** for setup issues
3. Review application logs in console
4. Verify PostgreSQL is running

---

## ?? Technology Stack

- **.NET 10** - Framework
- **PostgreSQL** - Database
- **JWT + Refresh Tokens** - Authentication
- **BCrypt** - Password Hashing
- **Scalar UI** - API Documentation
- **EF Core** - ORM

---

## ? Setup Checklist

- [ ] PostgreSQL installed and running
- [ ] .NET 10 SDK installed
- [ ] EF Core tools installed
- [ ] Database migrated
- [ ] Can login with superadmin
- [ ] Can access Scalar UI

---

**?? Ready to start? Head to the [Complete Guide](COMPLETE_GUIDE.md)!**
