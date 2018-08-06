// <copyright file="TokenValidator.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Security
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    ///     Validates <see cref="JwtSecurityToken"/> instances.
    /// </summary>
    public class TokenValidator : ITokenValidator
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TokenValidator"/> class.
        /// </summary>
        /// <param name="tokenValidationParameters">The parameters with which to validate tokens.</param>
        public TokenValidator(TokenValidationParameters tokenValidationParameters)
        {
            TokenValidationParameters = tokenValidationParameters;
        }

        private TokenValidationParameters TokenValidationParameters { get; set; }

        /// <summary>
        ///     Attempts to validate and return a <see cref="JwtSecurityToken"/> instance from the specified <paramref name="token"/>.
        /// </summary>
        /// <param name="token">The string token to validate.</param>
        /// <param name="jwtSecurityToken">The validated JwtSecurityToken.</param>
        /// <returns>A value indicating whether the specified <paramref name="token"/> is valid.</returns>
        public bool TryParseAndValidateToken(string token, out JwtSecurityToken jwtSecurityToken)
        {
            jwtSecurityToken = default(JwtSecurityToken);

            try
            {
                SecurityToken securityToken;
                new JwtSecurityTokenHandler().ValidateToken(token, TokenValidationParameters, out securityToken);

                jwtSecurityToken = new JwtSecurityToken(token);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Attempts to validate the specified <paramref name="token"/>.
        /// </summary>
        /// <param name="token">The string token to validate.</param>
        /// <returns>A value indicating whether the specified <paramref name="token"/> is valid.</returns>
        public bool TryValidateToken(string token)
        {
            return TryParseAndValidateToken(token, out JwtSecurityToken jwtSecurityToken);
        }
    }
}