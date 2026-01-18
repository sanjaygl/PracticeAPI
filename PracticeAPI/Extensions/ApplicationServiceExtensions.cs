namespace PracticeAPI.Extensions
{
    public static class ApplicationServiceExtensions
    {
        /// <summary>
        /// Registers all application services including database, repositories, business services, authentication, and Swagger
        /// </summary>
        public static IServiceCollection AddApplicationConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add Controllers
            services.AddControllers();

            // Configure Database
            services.AddDatabaseConfiguration(configuration);

            // Register Repositories
            services.AddRepositories();

            // Register Application Services (including Auth services)
            services.AddApplicationServices();

            // Configure JWT Authentication & Authorization
            services.AddJwtAuthentication(configuration);

            // Configure Swagger/OpenAPI
            services.AddSwaggerConfiguration();

            return services;
        }
    }
}
