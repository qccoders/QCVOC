namespace QCVOC.Api.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using NLog;

    public enum ExceptionMiddlwareVerbosity
    {
        Terse,
        Verbose,
    }

    public class ExceptionMiddleware
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly RequestDelegate next;

        public ExceptionMiddleware(RequestDelegate next, ExceptionMiddlewareOptions options = null)
        {
            this.next = next;
            Options = options ?? new ExceptionMiddlewareOptions();
        }

        private ExceptionMiddlewareOptions Options { get; set; }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (context.Response.HasStarted)
            {
                return Task.CompletedTask;
            }

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var response = Options.Verbosity == ExceptionMiddlwareVerbosity.Terse ? JsonConvert.SerializeObject(exception.Message) :
                JsonConvert.SerializeObject(exception);

            return context.Response.WriteAsync(response);
        }
    }

    public class ExceptionMiddlewareOptions
    {
        public ExceptionMiddlwareVerbosity Verbosity { get; set; }
    }
}