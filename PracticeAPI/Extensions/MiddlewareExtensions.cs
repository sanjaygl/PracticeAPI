using PracticeAPI.Middleware;

namespace PracticeAPI.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }

        public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
        {
            app.UseGlobalExceptionHandler();
            
            // Add more custom middlewares here in the future
            // app.UseMiddleware<RequestLoggingMiddleware>();
            // app.UseMiddleware<PerformanceMonitoringMiddleware>();
            
            return app;
        }
    }
}
