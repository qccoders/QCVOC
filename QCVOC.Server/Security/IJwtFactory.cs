namespace QCVOC.Server.Security
{
    using System;
    using QCVOC.Server.Data.Model.Security;

    public interface IJwtFactory
    {
        #region Public Methods

        Jwt GetJwt(Account account);

        Jwt GetJwt(Account account, Guid refreshTokenId);

        bool TryParseRefreshTokenId(string jwt, out Guid refreshTokenId);

        #endregion Public Methods
    }
}