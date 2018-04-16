using QCVOC.Server.Data.Model;

namespace QCVOC.Server.Data.Repository
{
    internal interface IUserRepository : IRepository<User>
    {
        #region Public Methods

        User Get(string name);

        #endregion Public Methods
    }
}