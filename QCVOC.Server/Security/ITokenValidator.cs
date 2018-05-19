﻿namespace QCVOC.Server.Security
{
    using System.IdentityModel.Tokens.Jwt;

    public interface ITokenValidator
    {
        #region Public Methods

        bool TryParseAndValidateToken(string token, out JwtSecurityToken jwtSecurityToken);

        #endregion Public Methods
    }
}