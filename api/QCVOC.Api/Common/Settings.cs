// <copyright file="Settings.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Common
{
    /// <summary>
    ///     Application settings.
    /// </summary>
    public static class Settings
    {
        /// <summary>
        ///     The name of the application root URI setting.
        /// </summary>
        public static readonly string AppRoot = "QCVOC_AppRoot";

        /// <summary>
        ///     The name of the database connection string setting.
        /// </summary>
        public static readonly string DbConnectionString = "QCVOC_DbConnectionString";

        /// <summary>
        ///     The name of the access token expiry setting.
        /// </summary>
        public static readonly string JwtAccessTokenExpiry = "QCVOC_JwtAccessTokenExpiry";

        /// <summary>
        ///     The name of the token audience setting.
        /// </summary>
        public static readonly string JwtAudience = "QCVOC_JwtAudience";

        /// <summary>
        ///     The name of the token issuer setting.
        /// </summary>
        public static readonly string JwtIssuer = "QCVOC_JwtIssuer";

        /// <summary>
        ///     The name of the token key setting.
        /// </summary>
        public static readonly string JwtKey = "QCVOC_JwtKey";

        /// <summary>
        ///     The name of the refresh token expiry setting.
        /// </summary>
        public static readonly string JwtRefreshTokenExpiry = "QCVOC_JwtRefreshTokenExpiry";

        /// <summary>
        ///     Default application settings.
        /// </summary>
        public static class Defaults
        {
            /// <summary>
            ///     The root URI for the application.
            /// </summary>
            public static readonly string AppRoot = string.Empty;

            /// <summary>
            ///     The default database connection string.
            /// </summary>
            public static readonly string DbConnectionString = "User ID=test;Password=test;Host=SQL;Port=5432;Database=QCVOC;Pooling = true;";

            /// <summary>
            ///     The default access token expiry.
            /// </summary>
            public static readonly int JwtAccessTokenExpiry = 30;

            /// <summary>
            ///     The default token audience.
            /// </summary>
            public static readonly string JwtAudience = "QCVOC";

            /// <summary>
            ///     The default token issuer.
            /// </summary>
            public static readonly string JwtIssuer = "QCVOC";

            /// <summary>
            ///     The default token key.
            /// </summary>
            public static readonly string JwtKey = "EE26B0DD4AF7E749AA1A8EE3C10AE9923F618980772E473F8819A5D4940E0DB27AC185F8A0E1D5F84F88BC887FD67B143732C304CC5FA9AD8E6F57F50028A8FF";

            /// <summary>
            ///     The default refresh token expiry.
            /// </summary>
            public static readonly int JwtRefreshTokenExpiry = 3600;
        }
    }
}