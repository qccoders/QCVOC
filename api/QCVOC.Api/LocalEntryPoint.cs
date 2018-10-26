// <copyright file="LocalEntryPoint.cs" company="QC Coders">
//     Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    /// <summary>
    ///     The local entry point for the application.
    /// </summary>
    public class LocalEntryPoint
    {
        /// <summary>
        ///     Builds the application <see cref="IWebHost"/>.
        /// </summary>
        /// <param name="args">Command line arguments passed at startup.</param>
        /// <returns>The constructed IWebHost.</returns>
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        /// <param name="args">Command line arguments passed at startup.</param>
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }
    }
}