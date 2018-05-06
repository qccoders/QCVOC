namespace QCVOC.Server.Data.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using QCVOC.Server.Data.Model;

    public class AccountRepository : IRepository<Account>, IAccountRepository
    {
        #region Public Constructors

        public AccountRepository(IDbConnection dbConnection)
        {
            Db = dbConnection;
        }

        #endregion Public Constructors

        #region Private Properties

        private IDbConnection Db { get; set; }
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
            string sql = "SELECT id, name, passwordhash, role FROM account";
            sql += " WHERE id = @Id";

            return Db.Query<Account>(sql, new { Id = id }).FirstOrDefault();
        }

        public Account Get(string name)
        {
            string sql = "SELECT id, name, passwordhash, role FROM account";
            sql += " WHERE name = @Name";

            return Db.Query<Account>(sql, new { Name = name }).FirstOrDefault();
        }

        public IList<Account> GetAll()
        {
            string sql = "SELECT id, name, passwordhash, role FROM account";

            return Db.Query<Account>(sql).ToList();
        }

        public void Update(Guid id, Account updatedAccount)
        {
        }

        public void Update(Account Account, Account updatedAccount)
        {
        }

        #endregion Public Methods
    }
}