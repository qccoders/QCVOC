using QCVOC.Server.Data.Model;

namespace QCVOC.Server.Data.Repository
{
    public interface IAccountRepository : IRepository<Account>
    {
        #region Public Methods

        Account Get(string name);

        #endregion Public Methods
    }
}