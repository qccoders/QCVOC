// <copyright file="TokenValidator.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Security
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.IdentityModel.Tokens;

    public class TokenValidator : ITokenValidator
    {
        public TokenValidator(TokenValidationParameters tokenValidationParameters)
        {
            TokenValidationParameters = tokenValidationParameters;
        }

        private TokenValidationParameters TokenValidationParameters { get; set; }

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

        public bool TryValidateToken(string token)
        {
            return TryParseAndValidateToken(token, out JwtSecurityToken jwtSecurityToken);
        }
    }
}