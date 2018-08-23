// <copyright file="ExceptionMiddleware.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Middleware
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using NLog;

    /// <summary>
    ///     Exception verbosity levels.
    /// </summary>
    public enum ExceptionMiddlwareVerbosity
    {
        /// <summary>
        ///     Exception message only.
        /// </summary>
        Terse,

        /// <summary>
        ///     Exception message and stack trace.
        /// </summary>
        Verbose,
    }

    /// <summary>
    ///     Extension methods for the <see cref="ExceptionMiddleware"/> class.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1649:FileNameMustMatchTypeName", Justification = "Reviewed.")]
    public static class ExceptionMiddlewareExtensions
    {
        /// <summary>
        ///     Attaches the <see cref="ExceptionMiddleware"/> middleware to the specified <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The IApplicationBuilder instance to configure.</param>
        /// <param name="configureOptions">The options for the middleware.</param>
        /// <returns>The modified IApplicationBuilder instance.</returns>
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder, Action<ExceptionMiddlewareOptions> configureOptions = null)
        {
            var options = new ExceptionMiddlewareOptions();

            configureOptions?.Invoke(options);

            return builder.UseMiddleware<ExceptionMiddleware>(options);
        }
    }

    /// <summary>
    ///     Provides general Exception handling.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly RequestDelegate next;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExceptionMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the chain.</param>
        /// <param name="options">The options for the middleware.</param>
        public ExceptionMiddleware(RequestDelegate next, ExceptionMiddlewareOptions options = null)
        {
            this.next = next;
            Options = options ?? new ExceptionMiddlewareOptions();
        }

        private ExceptionMiddlewareOptions Options { get; set; }

        /// <summary>
        ///     Invokes the middleware function and transfers execution to the next middleware component.
        /// </summary>
        /// <param name="context">The context within which the middleware is to execute</param>
        /// <returns>The result of the asynchronous middleware function.</returns>
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

            var response = Options.Verbosity == ExceptionMiddlwareVerbosity.Terse ? JsonConvert.SerializeObject(new { exception.Message }) :
                JsonConvert.SerializeObject(exception);

            return context.Response.WriteAsync(response);
        }
    }

    /// <summary>
    ///     Options for the <see cref="ExceptionMiddleware"/> class.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Reviewed.")]
    public class ExceptionMiddlewareOptions
    {
        /// <summary>
        ///     Gets or sets the level of verbosity of reported Exceptions.
        /// </summary>
        public ExceptionMiddlwareVerbosity Verbosity { get; set; }
    }
}