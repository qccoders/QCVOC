namespace QCVOC.Api.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    public static class JsonRpcExtensions
    {
        public static IApplicationBuilder UseJsonRpc(this IApplicationBuilder builder, Action<JsonRpcOptions> configureOptions = null)
        {
            var options = new JsonRpcOptions();

            configureOptions?.Invoke(options);

            return builder.UseMiddleware<JsonRpcMiddleware>(options);
        }
    }

    public class JsonRpcMiddleware
    {
        private readonly RequestDelegate next;

        public JsonRpcMiddleware(RequestDelegate next, JsonRpcOptions options = null)
        {
            this.next = next;
            Options = options ?? new JsonRpcOptions();
        }

        private JsonRpcOptions Options { get; set; }

        public async Task InvokeAsync(HttpContext context)
        {
            // Enable seeking
            context.Request.EnableBuffering();
            // Read the stream as text
            var bodyAsText = await new System.IO.StreamReader(context.Request.Body).ReadToEndAsync();
            // Set the position of the stream to 0 to enable rereading
            context.Request.Body.Position = 0;

            try
            {
                var jsonRpcRequest = JsonConvert.DeserializeObject<JsonRpcRequest>(bodyAsText);
                await context.Response.WriteAsync(JsonConvert.SerializeObject(jsonRpcRequest));
            }
            catch
            {
                await next(context);
            }
        }
    }

    public class JsonRpcOptions
    {
    }

    public class JsonRpcRequest
    {
        public double JsonRpc { get; set; }
        public string Method { get; set; }
        public object Params { get; set; }
    }
}