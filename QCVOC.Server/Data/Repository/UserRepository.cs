using Dapper;
using QCVOC.Server.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace QCVOC.Server.Data.Repository
{
    public class UserRepository : IRepository<User>, IUserRepository
    {
        #region Public Constructors

        public UserRepository(IDbConnection dbConnection)
        {
            DbConnection = dbConnection;
        }

        #endregion Public Constructors

        #region Private Properties

        private IDbConnection DbConnection { get; set; }
        private List<User> Users { get; set; }

        #endregion Private Properties

        #region Public Methods

        public void Create(User user)
        {
        }

        public void Delete(Guid id)
        {
        }

        public void Delete(User user)
        {
        }

        public User Get(Guid id)
        {
            FetchAccounts();

            return Users.Where(u => u.Id == id).FirstOrDefault();
        }

        public User Get(string name)
        {
            FetchAccounts();

            return Users.Where(u => u.Name == name).FirstOrDefault();
        }

        public IList<User> GetAll()
        {
            FetchAccounts();

            return Users;
        }

        public void Update(Guid id, User updatedUser)
        {
        }

        public void Update(User user, User updatedUser)
        {
        }

        #endregion Public Methods

        private void FetchAccounts()
        {
            var query = "SELECT id, name, passwordhash, role FROM account";

            Users = (List<User>)DbConnection.Query<User>(query);
        }
    }
}