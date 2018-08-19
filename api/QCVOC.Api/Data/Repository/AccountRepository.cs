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
            var command = @"
                DELETE FROM accounts
                WHERE id = @id;
            ";

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(command, new { id });
            }
        }

        /// <summary>
        ///     Deletes the specified account.
        /// </summary>
        /// <param name="account">The account to delete.</param>
        public void Delete(Account account)
        {
            Delete(account.Id);
        }

        /// <summary>
        ///     Retrieves the account with the specified Id.
        /// </summary>
        /// <param name="id">The Id of the account to retrieve.</param>
        /// <returns>The account with the specified Id.</returns>
        public Account Get(Guid id)
        {
            var query = @"
                SELECT
                    id,
                    name,
                    passwordhash,
                    role
                FROM accounts
                WHERE id = @id;
            ";

            using (var db = ConnectionFactory.CreateConnection())
            {
                return db.QueryFirstOrDefault<Account>(query, new { id });
            }
        }

        /// <summary>
        ///     Retrieves all accounts.
        /// </summary>
        /// <returns>A list of accounts.</returns>
        public IEnumerable<Account> GetAll()
        {
            var query = @"
                SELECT
                    id,
                    name,
                    passwordhash,
                    role
                FROM accounts
            ";

            using (var db = ConnectionFactory.CreateConnection())
            {
                return db.Query<Account>(query);
            }
        }

        /// <summary>
        ///     Updates the specified account.
        /// </summary>
        /// <param name="account">The account to update.</param>
        /// <returns>The updated account.</returns>
        public Account Update(Account account)
        {
            var command = @"
                UPDATE accounts
                SET 
                    name = @name,
                    passwordhash = @passwordhash,
                    role = @role
                WHERE id = @id;
            ";

            var param = new
            {
                name = account.Name,
                passwordhash = account.PasswordHash,
                role = account.Role,
                id = account.Id,
            };

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(command, param);

                return Get(account.Id);
            }
        }
    }
}