namespace QCVOC.Api.Middleware
{
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using NLog;
    using System;
    using System.Threading.Tasks;

    public class ExceptionMiddleware
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly RequestDelegate next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
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

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (context.Response.HasStarted)
            {
                return Task.CompletedTask;
            }

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(JsonConvert.SerializeObject(exception));
        }
    }
}
