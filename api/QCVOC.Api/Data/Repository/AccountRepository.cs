// <copyright file="AccountRepository.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Data.Repository
{
    using System;
    using System.Collections.Generic;
    using Dapper;
    using QCVOC.Api.Data.ConnectionFactory;
    using QCVOC.Api.Data.Model.Security;

    /// <summary>
    /// </summary>
    /// <typeparam name="Account"></typeparam>
    public class AccountRepository : IRepository<Account>
    {
        public AccountRepository(IDbConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
        }

        private IDbConnectionFactory ConnectionFactory { get; }

        /// <summary>
        ///     Creates a new account.
        /// </summary>
        /// <param name="account">The account to create.</param>
        /// <returns>The created account</returns>
        public Account Create(Account account)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                var command = @"
                    INSERT INTO accounts
                        (id, name, passwordhash, role)
                    VALUES
                        (@id, @name, @passwordhash, @role);
                ";

                var param = new
                {
                    id = account.Id,
                    name = account.Name,
                    passwordhash = account.PasswordHash,
                    role = account.Role,
                };

                db.Execute(command, param);

                var inserted = Get(account.Id);
                return inserted;
            }
        }

        /// <summary>
        ///     Deletes an account with the specified Id.
        /// </summary>
        /// <param name="id">The Id of the account to delete.</param>
        public void Delete(Guid id)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute("DELETE FROM accounts WHERE id = @id", new { id = id });
            }
        }

        /// <summary>
        ///     Deletes the specified account.
        /// </summary>
        /// <param name="account">The account to delete.</param>
        /// <exception cref="ArgumentException"></exception>
        public void Delete(Account account)
        {
            if (account == null)
                throw new ArgumentException("account cannot be null.", nameof(account));

            Delete(account.Id);
        }

        /// <summary>
        ///     Retrieves the account with the specified Id.
        /// </summary>
        /// <param name="id">The Id of the account to retrieve.</param>
        /// <returns>The account with the specified Id.</returns>
        public Account Get(Guid id)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                return db.QueryFirstOrDefault<Account>("SELECT id, name, passwordhash, role FROM accounts where id = @id;", new { id = id });
            }
        }

        /// <summary>
        ///     Retrieves all accounts.
        /// </summary>
        /// <returns>A list of accounts.</returns>
        public IEnumerable<Account> GetAll()
        {
            //TODO: Add paging when necessary.
            using (var db = ConnectionFactory.CreateConnection())
            {
                return db.Query<Account>("SELECT id, name, passwordhash, role FROM accounts;");
            }
        }

        /// <summary>
        ///     Updates the specified account.
        /// </summary>
        /// <param name="account">The account to update.</param>
        /// <returns>The updated account.</returns>
        public Account Update(Account account)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute("UPDATE accounts set name = @name, passwordhash = @passwordhash, role = @role WHERE id = @id;", new
                {
                    name = account.Name,
                    passwordhash = account.PasswordHash,
                    role = account.Role,
                    id = account.Id
                });

                return Get(account.Id);
            }
        }
    }
}