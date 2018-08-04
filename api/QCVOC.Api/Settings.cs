// <copyright file="Settings.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api
{
    public static class Settings
    {
        public static readonly string DbConnectionString = "DbConnectionString";

        public static readonly string JwtAccessTokenExpiry = "JwtAccessTokenExpiry";
        public static readonly string JwtAudience = "JwtAudience";
        public static readonly string JwtIssuer = "JwtIssuer";
        public static readonly string JwtKey = "JwtKey";
        public static readonly string JwtRefreshTokenExpiry = "JwtRefreshTokenExpiry";
    }
}