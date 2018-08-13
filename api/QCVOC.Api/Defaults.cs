// <copyright file="Defaults.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api
{
    public static class Defaults
    {
        public static readonly string AppRoot = string.Empty;
        public static readonly string DbConnectionString = "User ID=QCVOC;Password=QCVOC;Host=SQL;Port=5432;Database=QCVOC;Pooling = true;";
        public static readonly int JwtAccessTokenExpiry = 30;
        public static readonly string JwtAudience = "QCVOC";
        public static readonly string JwtIssuer = "QCVOC";
        public static readonly string JwtKey = "EE26B0DD4AF7E749AA1A8EE3C10AE9923F618980772E473F8819A5D4940E0DB27AC185F8A0E1D5F84F88BC887FD67B143732C304CC5FA9AD8E6F57F50028A8FF";
        public static readonly int JwtRefreshTokenExpiry = 3600;
    }
}