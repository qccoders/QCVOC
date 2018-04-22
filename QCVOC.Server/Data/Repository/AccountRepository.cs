using Dapper;
using QCVOC.Server.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace QCVOC.Server.Data.Repository
{
    public class AccountRepository : IRepository<Account>, IAccountRepository
    {
        #region Public Constructors

        public AccountRepository(IDbConnection dbConnection)
        {
            DbConnection = dbConnection;
        }

        #endregion Public Constructors

        #region Private Properties

        private IDbConnection DbConnection { get; set; }
        private List<Account> Accounts { get; set; }

        #endregion Private Properties

        #region Public Methods

        public void Create(Account Account)
        {
        }

        public void Delete(Guid id)
        {
        }

        public void Delete(Account Account)
        {
        }

        public Account Get(Guid id)
        {
            FetchAccounts();

            return Accounts.Where(u => u.Id == id).FirstOrDefault();
        }

        public Account Get(string name)
        {
            FetchAccounts();

            return Accounts.Where(u => u.Name == name).FirstOrDefault();
        }

        public IList<Account> GetAll()
        {
            FetchAccounts();

            return Accounts;
        }

        public void Update(Guid id, Account updatedAccount)
        {
        }

        public void Update(Account Account, Account updatedAccount)
        {
        }

        #endregion Public Methods

        private void FetchAccounts()
        {
            var query = "SELECT id, name, passwordhash, role FROM account";

            Accounts = (List<Account>)DbConnection.Query<Account>(query);
        }
    }
}