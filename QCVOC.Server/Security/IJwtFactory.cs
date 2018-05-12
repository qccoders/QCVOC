namespace QCVOC.Server.Security
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using QCVOC.Server.Data.Model.Security;

    public interface IJwtFactory
    {
        #region Public Methods

        Jwt GetJwt(Account account);

        Jwt GetJwt(Account account, Guid refreshTokenId);

        bool TryParseJwtSecurityToken(string token, out JwtSecurityToken jwtSecurityToken);

        #endregion Public Methods
    }
}
