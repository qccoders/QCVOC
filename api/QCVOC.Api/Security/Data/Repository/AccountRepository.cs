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
            return GetAll(new AccountFilters() { Id = id }).SingleOrDefault();
        }

        /// <summary>
        ///     Retrieves all accounts.
        /// </summary>
        /// <returns>A list of accounts.</returns>
        /// <param name="filters">The optional query filters.</param>
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