using QCVOC.Server.Data.Model;
using System.IdentityModel.Tokens.Jwt;

namespace QCVOC.Server.Security
{
    public interface IJwtFactory
    {
        #region Public Methods

        JwtSecurityToken GetJwt(Account account);

        #endregion Public Methods
    }
}