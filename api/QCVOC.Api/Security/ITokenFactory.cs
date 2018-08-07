// <copyright file="ITokenFactory.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Security
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using QCVOC.Api.Data.Model.Security;

    /// <summary>
    ///     Creates <see cref="JwtSecurityToken"/> instances.
    /// </summary>
    public interface ITokenFactory
    {
        /// <summary>
        ///     Creates an access JwtSecurityToken given an <paramref name="account"/> and <paramref name="refreshTokenId"/>.
        /// </summary>
        /// <param name="account">The Account from which to retrieve user claims.</param>
        /// <param name="refreshTokenId">The refresh token id to use for the jti claim.</param>
        /// <returns>The created JwtSecurityToken.</returns>
        JwtSecurityToken GetAccessToken(Account account, Guid refreshTokenId);

        /// <summary>
        ///     Creates an access JwtSecurityToken given an <paramref name="account"/> and <paramref name="refreshToken"/>.
        /// </summary>
        /// <param name="account">The Account from which to retrieve user claims.</param>
        /// <param name="refreshToken">The JwtSecurityToken from which to retrieve the refresh token id.</param>
        /// <returns>The created JwtSecurityToken.</returns>
        JwtSecurityToken GetAccessToken(Account account, JwtSecurityToken refreshToken);

        /// <summary>
        ///     Creates a refresh JwtSecurityToken with a default id and expiry.
        /// </summary>
        /// <returns>The created JwtSecurityToken.</returns>
        JwtSecurityToken GetRefreshToken();

        /// <summary>
        ///     Creates a refresh JwtSecurityToken with the specified <paramref name="refreshTokenId"/> and default expiry.
        /// </summary>
        /// <param name="refreshTokenId">The token id.</param>
        /// <returns>The created JwtSecurityToken.</returns>
        JwtSecurityToken GetRefreshToken(Guid refreshTokenId);

        /// <summary>
        ///     Creates a refresh JwtSecurityToken with the specified <paramref name="refreshTokenId"/> and expiry set to the
        ///     current time plus the specified <paramref name="ttlInMinutes"/>.
        /// </summary>
        /// <param name="refreshTokenId">The token id.</param>
        /// <param name="ttlInMinutes">The ttl for the token, in minutes.</param>
        /// <returns>The created JwtSecurityToken.</returns>
        JwtSecurityToken GetRefreshToken(Guid refreshTokenId, int ttlInMinutes);

        /// <summary>
        ///     Creates a refresh JwtSecurityToken with the specified <paramref name="refreshTokenId"/> and expiry <paramref name="expiresUtc"/>.
        /// </summary>
        /// <param name="refreshTokenId">The token id.</param>
        /// <param name="expiresUtc">The time at which the token expires.</param>
        /// <returns>The created JwtSecurityToken.</returns>
        JwtSecurityToken GetRefreshToken(Guid refreshTokenId, DateTime expiresUtc);

        /// <summary>
        ///     Creates a refresh JwtSecurityToken with the specified <paramref name="refreshTokenId"/>, expiry
        ///     <paramref name="expiresUtc"/>, and issued time <paramref name="issuedUtc"/>.
        /// </summary>
        /// <param name="refreshTokenId">The token id.</param>
        /// <param name="expiresUtc">The time at which the token expires.</param>
        /// <param name="issuedUtc">The time at which the token was issued.</param>
        /// <returns>The created JwtSecurityToken.</returns>
        JwtSecurityToken GetRefreshToken(Guid refreshTokenId, DateTime expiresUtc, DateTime issuedUtc);
    }
}