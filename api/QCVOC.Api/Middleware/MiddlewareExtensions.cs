namespace QCVOC.Api.Middleware
{
    using System;
    using Microsoft.AspNetCore.Builder;

    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder, Action<ExceptionMiddlewareOptions> configureOptions = null)
        {
            var options = new ExceptionMiddlewareOptions();

            configureOptions?.Invoke(options);

            return builder.UseMiddleware<ExceptionMiddleware>(options);
        }

        public static IApplicationBuilder UseLogger(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }
}