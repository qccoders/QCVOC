// <copyright file="TokenFactory.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Security
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.IdentityModel.Tokens;
    using QCVOC.Api.Security.Data.Model;
    using Utility = Utility;

    /// <summary>
    ///     Creates <see cref="JwtSecurityToken"/> instances.
    /// </summary>
    public class TokenFactory : ITokenFactory
    {
        /// <summary>
        ///     Creates an access JwtSecurityToken given an <paramref name="account"/> and <paramref name="refreshTokenId"/>.
        /// </summary>
        /// <param name="account">The Account from which to retrieve user claims.</param>
        /// <param name="refreshTokenId">The refresh token id to use for the jti claim.</param>
        /// <returns>The created JwtSecurityToken.</returns>
        public JwtSecurityToken GetAccessToken(Account account, Guid refreshTokenId)
        {
            var expiry = Utility.GetSetting<int>(Settings.JwtAccessTokenExpiry);
            var key = Utility.GetSetting<string>(Settings.JwtKey);

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, account.Name),
                new Claim(ClaimTypes.Role, account.Role.ToString()),
                new Claim("sub", account.Id.ToString()),
                new Claim("name", account.Name),
                new Claim("role", account.Role.ToString()),
                new Claim("jti", refreshTokenId.ToString())
            };

            return GetJwtSecurityToken(claims, expiry);
        }

        /// <summary>
        ///     Creates an access JwtSecurityToken given an <paramref name="account"/> and <paramref name="refreshToken"/>.
        /// </summary>
        /// <param name="account">The Account from which to retrieve user claims.</param>
        /// <param name="refreshToken">The JwtSecurityToken from which to retrieve the refresh token id.</param>
        /// <returns>The created JwtSecurityToken.</returns>
        public JwtSecurityToken GetAccessToken(Account account, JwtSecurityToken refreshToken)
        {
            var jti = refreshToken.Claims.Where(c => c.Type == "jti").FirstOrDefault().Value;
            return GetAccessToken(account, Guid.Parse(jti));
        }

        /// <summary>
        ///     Creates a refresh JwtSecurityToken with a generated id and expiry.
        /// </summary>
        /// <returns>The created JwtSecurityToken.</returns>
        public JwtSecurityToken GetRefreshToken()
        {
            var expiry = Utility.GetSetting<int>(Settings.JwtRefreshTokenExpiry);
            return GetRefreshToken(Guid.NewGuid(), DateTime.UtcNow.AddMinutes(expiry), DateTime.UtcNow);
        }

        /// <summary>
        ///     Creates a refresh JwtSecurityToken with the specified <paramref name="refreshTokenId"/> and default expiry.
        /// </summary>
        /// <param name="refreshTokenId">The token id.</param>
        /// <returns>The created JwtSecurityToken.</returns>
        public JwtSecurityToken GetRefreshToken(Guid refreshTokenId)
        {
            var expiry = Utility.GetSetting<int>(Settings.JwtRefreshTokenExpiry);
            return GetRefreshToken(refreshTokenId, DateTime.UtcNow.AddMinutes(expiry), DateTime.UtcNow);
        }

        /// <summary>
        ///     Creates a refresh JwtSecurityToken with the specified <paramref name="refreshTokenId"/> and expiry set to the
        ///     current time plus the specified <paramref name="ttlInMinutes"/>.
        /// </summary>
        /// <param name="refreshTokenId">The token id.</param>
        /// <param name="ttlInMinutes">The ttl for the token, in minutes.</param>
        /// <returns>The created JwtSecurityToken.</returns>
        public JwtSecurityToken GetRefreshToken(Guid refreshTokenId, int ttlInMinutes)
        {
            return GetRefreshToken(refreshTokenId, DateTime.UtcNow.AddMinutes(ttlInMinutes), DateTime.UtcNow);
        }

        /// <summary>
        ///     Creates a refresh JwtSecurityToken with the specified <paramref name="refreshTokenId"/> and expiry <paramref name="expiresUtc"/>.
        /// </summary>
        /// <param name="refreshTokenId">The token id.</param>
        /// <param name="expiresUtc">The time at which the token expires.</param>
        /// <returns>The created JwtSecurityToken.</returns>
        public JwtSecurityToken GetRefreshToken(Guid refreshTokenId, DateTime expiresUtc)
        {
            return GetRefreshToken(refreshTokenId, expiresUtc, DateTime.UtcNow);
        }

        /// <summary>
        ///     Creates a refresh JwtSecurityToken with the specified <paramref name="refreshTokenId"/>, expiry
        ///     <paramref name="expiresUtc"/>, and issued time <paramref name="issuedUtc"/>.
        /// </summary>
        /// <param name="refreshTokenId">The token id.</param>
        /// <param name="expiresUtc">The time at which the token expires.</param>
        /// <param name="issuedUtc">The time at which the token was issued.</param>
        /// <returns>The created JwtSecurityToken.</returns>
        public JwtSecurityToken GetRefreshToken(Guid refreshTokenId, DateTime expiresUtc, DateTime issuedUtc)
        {
            var claims = new Claim[]
            {
                new Claim("jti", refreshTokenId.ToString())
            };

            return GetJwtSecurityToken(claims, expiresUtc, issuedUtc);
        }

        private JwtSecurityToken GetJwtSecurityToken(Claim[] claims, int ttlInMinuntes)
        {
            return GetJwtSecurityToken(claims, DateTime.UtcNow.AddMinutes(ttlInMinuntes), DateTime.UtcNow);
        }

        private JwtSecurityToken GetJwtSecurityToken(Claim[] claims, DateTime expiresUtc)
        {
            return GetJwtSecurityToken(claims, expiresUtc, DateTime.UtcNow);
        }

        private JwtSecurityToken GetJwtSecurityToken(Claim[] claims, DateTime expiresUtc, DateTime issuedUtc)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Utility.GetSetting<string>(Settings.JwtKey)));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                issuer: Utility.GetSetting<string>(Settings.JwtIssuer),
                audience: Utility.GetSetting<string>(Settings.JwtAudience),
                claims: claims,
                notBefore: issuedUtc,
                expires: expiresUtc,
                signingCredentials: credentials);

            return token;
        }
    }
}