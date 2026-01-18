using Microsoft.EntityFrameworkCore;
using PracticeAPI.Models;

namespace PracticeAPI.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                // Ensure database is created
                await context.Database.MigrateAsync();

                // Seed Roles
                await SeedRolesAsync(context, logger);

                // Seed Default SuperAdmin User
                await SeedSuperAdminAsync(context, logger);

                logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }

        private static async Task SeedRolesAsync(ApplicationDbContext context, ILogger logger)
        {
            var roles = new[]
            {
                "SuperAdmin",
                "Admin",
                "Manager",
                "User"
            };

            foreach (var roleName in roles)
            {
                if (!await context.Roles.AnyAsync(r => r.Name == roleName))
                {
                    await context.Roles.AddAsync(new Role { Name = roleName });
                    logger.LogInformation("Seeded role: {RoleName}", roleName);
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedSuperAdminAsync(ApplicationDbContext context, ILogger logger)
        {
            const string superAdminUsername = "superadmin";

            // Check if SuperAdmin user already exists
            if (await context.Users.AnyAsync(u => u.Username == superAdminUsername))
            {
                logger.LogInformation("SuperAdmin user already exists");
                return;
            }

            // Get SuperAdmin role
            var superAdminRole = await context.Roles
                .FirstOrDefaultAsync(r => r.Name == "SuperAdmin");

            if (superAdminRole == null)
            {
                logger.LogError("SuperAdmin role not found. Cannot seed SuperAdmin user.");
                return;
            }

            // Create SuperAdmin user with hashed password
            var superAdminUser = new User
            {
                Username = superAdminUsername,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("SuperAdmin@123"), // Default password
                RoleId = superAdminRole.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(superAdminUser);
            await context.SaveChangesAsync();

            logger.LogInformation("Seeded SuperAdmin user with username: {Username}", superAdminUsername);
            logger.LogWarning("Default SuperAdmin password is 'SuperAdmin@123'. Please change it immediately!");
        }

        public static async Task SeedTestUsersAsync(ApplicationDbContext context, ILogger logger)
        {
            var testUsers = new[]
            {
                new { Username = "admin", Password = "Admin@123", Role = "Admin" },
                new { Username = "manager", Password = "Manager@123", Role = "Manager" },
                new { Username = "user", Password = "User@123", Role = "User" }
            };

            foreach (var testUser in testUsers)
            {
                if (!await context.Users.AnyAsync(u => u.Username == testUser.Username))
                {
                    var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == testUser.Role);
                    if (role != null)
                    {
                        await context.Users.AddAsync(new User
                        {
                            Username = testUser.Username,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(testUser.Password),
                            RoleId = role.Id,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        });

                        logger.LogInformation("Seeded test user: {Username}", testUser.Username);
                    }
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
