// <copyright file="ITokenValidator.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Security
{
    using System.IdentityModel.Tokens.Jwt;

    /// <summary>
    ///     Validates <see cref="JwtSecurityToken"/> instances.
    /// </summary>
    public interface ITokenValidator
    {
        /// <summary>
        ///     Attempts to validate and return a <see cref="JwtSecurityToken"/> instance from the specified <paramref name="token"/>.
        /// </summary>
        /// <param name="token">The string token to validate.</param>
        /// <param name="jwtSecurityToken">The validated JwtSecurityToken.</param>
        /// <returns>A value indicating whether the specified <paramref name="token"/> is valid.</returns>
        bool TryParseAndValidateToken(string token, out JwtSecurityToken jwtSecurityToken);

        /// <summary>
        ///     Attempts to validate the specified <paramref name="token"/>.
        /// </summary>
        /// <param name="token">The string token to validate.</param>
        /// <returns>A value indicating whether the specified <paramref name="token"/> is valid.</returns>
        bool TryValidateToken(string token);
    }
}