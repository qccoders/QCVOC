// <copyright file="LoggingMiddleware.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Middleware
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using NLog;

    /// <summary>
    ///     Extension methods for the <see cref="LoggingMiddleware"/> class.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1649:FileNameMustMatchTypeName", Justification = "Reviewed.")]
    public static class LoggingMiddlewareExtensions
    {
        /// <summary>
        ///     Attaches the <see cref="LoggingMiddleware"/> middleware to the specified <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The IApplicationBuilder instance to configure.</param>
        /// <returns>The modified IApplicationBuilder instance.</returns>
        public static IApplicationBuilder UseLogger(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }

    /// <summary>
    ///     Provides logging for HTTP requests.
    /// </summary>
    public class LoggingMiddleware
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly RequestDelegate next;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoggingMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the chain.</param>
        public LoggingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        ///     Invokes the middleware function and transfers execution to the next middleware component.
        /// </summary>
        /// <param name="context">The context within which the middleware is to execute</param>
        /// <returns>The result of the asynchronous middleware function.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            Stopwatch benchmark = new Stopwatch();
            benchmark.Start();

            await next.Invoke(context);

            benchmark.Stop();

            string logString = GetLogString(context, benchmark.Elapsed);
            logger.Info(logString);
        }

        private string GetFormattedSize(double value, int decimalPlaces = 1)
        {
            string[] sizeSuffixes = { "bytes", "KB", "MB", "GB" };

            if (value <= 0)
            {
                return "0 bytes";
            }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format(
                "{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                sizeSuffixes[mag]);
        }

        private string GetLogString(HttpContext context, TimeSpan duration)
        {
            StringBuilder message = new StringBuilder();

            message.Append("[" + context.Response.StatusCode.ToString() + "] ");
            message.Append(context.Request.Method + " ");
            message.Append(context.Request.Path.Value);
            message.Append(" (" + context.Connection.RemoteIpAddress + ") ");
            message.Append(GetFormattedSize((double)(context.Response.ContentLength ?? 0), 2) + ", " + duration.TotalMilliseconds + "ms");

            return message.ToString();
        }
    }
}