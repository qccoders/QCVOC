// <copyright file="AccountRepository.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Server.Data.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using Dapper.Contrib.Extensions;
    using QCVOC.Server.Data.Model;

    /// <summary>
    ///     A repository for application user Accounts.
    /// </summary>
    public class AccountRepository : IRepository<Account>, IAccountRepository
    {
        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AccountRepository"/> class with the specified <paramref name="dbConnection"/>.
        /// </summary>
        /// <param name="dbConnection">The database connection with which to persist Account information.</param>
        public AccountRepository(IDbConnection dbConnection)
        {
            Db = dbConnection;
        }

        #endregion Public Constructors

        #region Private Properties

        private List<Account> Accounts { get; set; }
        private IDbConnection Db { get; set; }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        ///     Creates a new user Account with the specified <paramref name="account"/> data.
        /// </summary>
        /// <param name="account">The information with which to create the new user Account.</param>
        public void Create(Account account)
        {
            Db.Insert<Account>(account);
        }

        /// <summary>
        ///     Deletes the user Account matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the user Account to delete.</param>
        public void Delete(Guid id)
        {
            Delete(Get(id));
        }

        /// <summary>
        ///     Deletes the specified <paramref name="account"/>.
        /// </summary>
        /// <param name="account">The user Account to delete.</param>
        public void Delete(Account account)
        {
            Db.Delete<Account>(account);
        }

        /// <summary>
        ///     Retrieves the user Account matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the user Account to retrieve.</param>
        /// <returns>The user Account matching the specified id.</returns>
        public Account Get(Guid id)
        {
            return Db.Get<Account>(id);
        }

        /// <summary>
        ///     Retrieves the user Account matching the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name associated with the user Account to retrieve.</param>
        /// <returns>The user Account matching the specified name.</returns>
        public Account Get(string name)
        {
            string sql = "SELECT id, name, passwordhash, role FROM account";
            sql += " WHERE name = @Name";

            return Db.Query<Account>(sql, new { Name = name }).FirstOrDefault();
        }

        /// <summary>
        ///     Retrieves a list of all user Accounts.
        /// </summary>
        /// <returns>A list of all user Accounts.</returns>
        public IEnumerable<Account> GetAll()
        {
            return Db.GetAll<Account>();
        }

        /// <summary>
        ///     Updates the user Account matching the specified <paramref name="id"/> with the specified <paramref name="updatedAccount"/>.
        /// </summary>
        /// <param name="id">The id of the user Account to update.</param>
        /// <param name="updatedAccount">The information with which to update the user Account.</param>
        public void Update(Guid id, Account updatedAccount)
        {
            Update(Get(id), updatedAccount);
        }

        /// <summary>
        ///     Updates the specified <paramref name="account"/> with the specified <paramref name="updatedAccount"/>.
        /// </summary>
        /// <param name="account">The user Account to update.</param>
        /// <param name="updatedAccount">The information with which to update the user Account.</param>
        public void Update(Account account, Account updatedAccount)
        {
            account.Name = updatedAccount.Name;
            account.PasswordHash = updatedAccount.PasswordHash;
            account.Role = updatedAccount.Role;

            Db.Update<Account>(account);
        }

        #endregion Public Methods
    }
}