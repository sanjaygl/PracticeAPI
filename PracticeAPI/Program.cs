using PracticeAPI.Data;
using PracticeAPI.Extensions;

namespace PracticeAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register all application services (Database, Repositories, Services, Auth, Swagger)
            builder.Services.AddApplicationConfiguration(builder.Configuration);

            var app = builder.Build();

            // Seed the database
            await app.SeedDatabaseAsync();

            // Configure the HTTP request pipeline
            app.UseCustomMiddlewares();
            app.UseSwaggerConfiguration(app.Environment);
            app.UseHttpsRedirection();

            // Enable Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
