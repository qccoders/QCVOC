// <copyright file="ITokenValidator.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Security
{
    using System.IdentityModel.Tokens.Jwt;

    public interface ITokenValidator
    {
        bool TryParseAndValidateToken(string token, out JwtSecurityToken jwtSecurityToken);

        bool TryValidateToken(string token);
    }
}