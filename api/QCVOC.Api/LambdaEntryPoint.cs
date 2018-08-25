// <copyright file="LambdaEntryPoint.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api
{
    using Amazon.Lambda.AspNetCoreServer;
    using Microsoft.AspNetCore.Hosting;

    /// <summary>
    ///     The AWS Lambda entry point for the application.
    /// </summary>
    public class LambdaEntryPoint : APIGatewayProxyFunction
    {
        /// <summary>
        ///     Builds the application <see cref="IWebHost"/>.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        protected override void Init(IWebHostBuilder builder)
        {
            builder
                .UseStartup<Startup>()
                .UseApiGateway();
        }
    }
}