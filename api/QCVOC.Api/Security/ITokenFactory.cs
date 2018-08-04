// <copyright file="ITokenFactory.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Security
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using QCVOC.Api.Data.Model.Security;

    public interface ITokenFactory
    {
        JwtSecurityToken GetAccessToken(Account account, Guid refreshTokenId);

        JwtSecurityToken GetAccessToken(Account account, JwtSecurityToken refreshToken);

        JwtSecurityToken GetRefreshToken();

        JwtSecurityToken GetRefreshToken(Guid refreshTokenId);

        JwtSecurityToken GetRefreshToken(Guid refreshTokenId, int ttlInMinutes);

        JwtSecurityToken GetRefreshToken(Guid refreshTokenId, DateTime expiresUtc);

        JwtSecurityToken GetRefreshToken(Guid refreshTokenId, DateTime expiresUtc, DateTime issuedUtc);
    }
}