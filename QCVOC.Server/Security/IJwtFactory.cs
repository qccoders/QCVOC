using QCVOC.Server.Data.Model;

namespace QCVOC.Server.Security
{
    internal interface IJwtFactory
    {
        #region Public Methods

        string GetJwt(User user);

        #endregion Public Methods
    }
}