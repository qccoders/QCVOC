// <copyright file="AccountRepository.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Security.Data.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using QCVOC.Api.Common;
    using QCVOC.Api.Common.Data.ConnectionFactory;
    using QCVOC.Api.Common.Data.Repository;
    using QCVOC.Api.Security.Data.Model;

    /// <summary>
    ///     Provides data access for <see cref="Account"/>.
    /// </summary>
    public class AccountRepository : IRepository<Account>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AccountRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The database connection factory used for data access.</param>
        public AccountRepository(IDbConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
        }

        private IDbConnectionFactory ConnectionFactory { get; }

        /// <summary>
        ///     Creates a new Account from the specified <paramref name="account"/>.
        /// </summary>
        /// <param name="account">The Account to create.</param>
        /// <returns>The created Account</returns>
        public Account Create(Account account)
        {
            var builder = new SqlBuilder();

            var query = builder.AddTemplate(@"
                INSERT INTO accounts
                    (id, name, passwordhash, role)
                VALUES
                    (@id, @name, @passwordhash, @role);
            ");

            builder.AddParameters(new
            {
                id = account.Id,
                name = account.Name,
                passwordhash = account.PasswordHash,
                role = account.Role,
            });

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query.RawSql, query.Parameters);
            }

            return Get(account.Id);
        }

        /// <summary>
        ///     Deletes the Account matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The Id of the account to delete.</param>
        public void Delete(Guid id)
        {
            var builder = new SqlBuilder();

            var query = builder.AddTemplate(@"
                DELETE FROM accounts
                WHERE id = @id;
            ");

            builder.AddParameters(new { id });

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query.RawSql, query.Parameters);
            }
        }

        /// <summary>
        ///     Deletes the specified <paramref name="account"/>.
        /// </summary>
        /// <param name="account">The Account to delete.</param>
        public void Delete(Account account)
        {
            Delete(account.Id);
        }

        /// <summary>
        ///     Retrieves the Account with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The Id of the account to retrieve.</param>
        /// <returns>The account with the specified Id.</returns>
        public Account Get(Guid id)
        {
            return GetAll(new AccountFilters() { Id = id }).SingleOrDefault();
        }

        /// <summary>
        ///     Retrieves all Accounts after applying optional <paramref name="filters"/>.
        /// </summary>
        /// <returns>A list of accounts.</returns>
        /// <param name="filters">Optional query filters.</param>
        public IEnumerable<Account> GetAll(Filters filters = null)
        {
            filters = filters ?? new Filters();
            var builder = new SqlBuilder();

            var query = builder.AddTemplate($@"
                SELECT
                    id,
                    name,
                    passwordhash,
                    role
                FROM accounts
                /**where**/
                ORDER BY name {filters.OrderBy.ToString()}
                LIMIT @limit OFFSET @offset
            ");

            builder.AddParameters(new
            {
                limit = filters.Limit,
                offset = filters.Offset,
                orderby = filters.OrderBy.ToString(),
            });

            if (filters is AccountFilters accountFilters)
            {
                if (accountFilters.Role != null)
                {
                    builder.Where("role = @role", new { role = accountFilters.Role.ToString() });
                }

                if (!string.IsNullOrWhiteSpace(accountFilters.Name))
                {
                    builder.Where("name = @name", new { accountFilters.Name });
                }

                if (accountFilters.Id != null)
                {
                    builder.Where("id = @id", new { accountFilters.Id });
                }
            }

            using (var db = ConnectionFactory.CreateConnection())
            {
                return db.Query<Account>(query.RawSql, query.Parameters);
            }
        }

        /// <summary>
        ///     Updates the specified <paramref name="account"/>.
        /// </summary>
        /// <param name="account">The account to update.</param>
        /// <returns>The updated account.</returns>
        public Account Update(Account account)
        {
            var builder = new SqlBuilder();

            var query = builder.AddTemplate(@"
                UPDATE accounts
                SET 
                    name = @name,
                    passwordhash = @passwordhash,
                    role = @role
                WHERE id = @id;
            ");

            builder.AddParameters(new
            {
                name = account.Name,
                passwordhash = account.PasswordHash,
                role = account.Role,
                id = account.Id,
            });

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query.RawSql, query.Parameters);
            }

            return Get(account.Id);
        }
    }
}