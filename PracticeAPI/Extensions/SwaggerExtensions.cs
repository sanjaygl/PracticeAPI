using Scalar.AspNetCore;

namespace PracticeAPI.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            // Use native .NET 10 OpenAPI support
            services.AddOpenApi();

            return services;
        }

        public static WebApplication UseSwaggerConfiguration(this WebApplication app, IWebHostEnvironment env)
        {
            // Map OpenAPI endpoint
            app.MapOpenApi();

            if (env.IsDevelopment())
            {
                // Use Scalar UI (modern alternative to Swagger UI for .NET 10)
                app.MapScalarApiReference();
            }

            return app;
        }
    }
}
