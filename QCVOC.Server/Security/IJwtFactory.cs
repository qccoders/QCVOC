namespace QCVOC.Server.Security
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.IdentityModel.Tokens;
    using QCVOC.Server.Data.Model.Security;

    public interface IJwtFactory
    {
        #region Public Methods

        JwtSecurityToken GetAccessToken(Account account, Guid refreshTokenId);

        Jwt GetJwt(Account account);

        Jwt GetJwt(Account account, Guid refreshTokenId);

        JwtSecurityToken GetRefreshToken(Guid refreshTokenId);

        TokenValidationParameters GetTokenValidationParameters();

        bool TryParseAndValidateToken(string token, out JwtSecurityToken jwtSecurityToken);

        #endregion Public Methods
    }
}