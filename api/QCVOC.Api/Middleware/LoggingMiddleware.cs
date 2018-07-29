﻿namespace QCVOC.Api.Middleware
{
    using System.Linq;
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using NLog;

    public class LoggingMiddleware
    {

        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly RequestDelegate next;

        public LoggingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        ///     Invokes the middleware function and transfers execution to the next middleware component.
        /// </summary>
        /// <param name="context">The context within which the middleware is to execute</param>
        /// <returns>The result of the asynchronous middleware function.</returns>
        public async Task Invoke(HttpContext context)
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