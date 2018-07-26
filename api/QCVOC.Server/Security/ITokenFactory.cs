namespace QCVOC.Server.Security
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using QCVOC.Server.Data.Model.Security;

    public interface ITokenFactory
    {
        #region Public Methods

        JwtSecurityToken GetAccessToken(Account account, Guid refreshTokenId);

        JwtSecurityToken GetAccessToken(Account account, JwtSecurityToken refreshToken);

        JwtSecurityToken GetRefreshToken();

        JwtSecurityToken GetRefreshToken(Guid refreshTokenId);

        JwtSecurityToken GetRefreshToken(Guid refreshTokenId, int ttlInMinutes);

        JwtSecurityToken GetRefreshToken(Guid refreshTokenId, DateTime expiresUtc);

        JwtSecurityToken GetRefreshToken(Guid refreshTokenId, DateTime expiresUtc, DateTime issuedUtc);

        #endregion Public Methods
    }
}