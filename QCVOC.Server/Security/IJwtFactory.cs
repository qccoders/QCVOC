using QCVOC.Server.Data.Model;

namespace QCVOC.Server.Security
{
    public interface IJwtFactory
    {
        #region Public Methods

        string GetJwt(Account account);

        #endregion Public Methods
    }
}