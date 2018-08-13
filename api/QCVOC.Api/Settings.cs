// <copyright file="Settings.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api
{
    public static class Settings
    {
        public static readonly string AppRoot = "QCVOC_AppRoot";
        public static readonly string DbConnectionString = "QCVOC_DbConnectionString";
        public static readonly string JwtAccessTokenExpiry = "QCVOC_JwtAccessTokenExpiry";
        public static readonly string JwtAudience = "QCVOC_JwtAudience";
        public static readonly string JwtIssuer = "QCVOC_JwtIssuer";
        public static readonly string JwtKey = "QCVOC_JwtKey";
        public static readonly string JwtRefreshTokenExpiry = "QCVOC_JwtRefreshTokenExpiry";
    }
}