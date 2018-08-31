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
                    (id, name, passwordhash, passwordresetrequired, role, creationdate, lastupdatedate, lastupdatebyid)
                VALUES
                    (@id, @name, @passwordhash, @passwordresetrequired, @role, @creationdate, @lastupdatedate, @lastupdatebyid);
            ");

            builder.AddParameters(new
            {
                id = account.Id,
                name = account.Name,
                passwordhash = account.PasswordHash,
                passwordresetrequired = account.PasswordResetRequired,
                role = account.Role.ToString(),
                creationdate = account.CreationDate,
                lastupdatedate = account.LastUpdateDate,
                lastupdatebyid = account.LastUpdateById,
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
        /// <param name="filters">Optional query filters.</param>
        /// <returns>A list of Accounts.</returns>
        public IEnumerable<Account> GetAll(Filters filters = null)
        {
            filters = filters ?? new Filters();
            var builder = new SqlBuilder();

            var query = builder.AddTemplate($@"
                SELECT
                    a1.id,
                    a1.name,
                    a1.passwordhash,
                    a1.passwordresetrequired,
                    a1.role,
                    a1.creationdate,
                    a1.lastupdatedate,
                    COALESCE(a2.name, '(Deleted user)') AS lastupdateby,
                    a1.lastupdatebyid
                FROM accounts a1
                LEFT JOIN accounts a2 ON a1.lastupdatebyid = a2.id
                /**where**/
                ORDER BY a1.name {filters.OrderBy.ToString()}
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
                    builder.Where("a1.role = @role", new { role = accountFilters.Role.ToString() });
                }

                if (!string.IsNullOrWhiteSpace(accountFilters.Name))
                {
                    builder.Where("a1.name = @name", new { accountFilters.Name });
                }

                if (accountFilters.Id != null)
                {
                    builder.Where("a1.id = @id", new { accountFilters.Id });
                }

                if (accountFilters.PasswordResetRequired != null)
                {
                    builder.Where("a1.passwordresetrequired = @passwordresetrequired", new { accountFilters.PasswordResetRequired });
                }

                if (accountFilters.CreationDateStart != null && accountFilters.CreationDateStart != null)
                {
                    builder.Where("a1.creationdate BETWEEN @start AND @end", new { start = accountFilters.CreationDateStart, end = accountFilters.CreationDateEnd });
                }

                if (accountFilters.LastUpdateDateStart != null && accountFilters.LastUpdateDateEnd != null)
                {
                    builder.Where("a1.lastupdate BETWEEN @start AND @end", new { start = accountFilters.LastUpdateDateStart, end = accountFilters.LastUpdateDateEnd });
                }

                if (!string.IsNullOrWhiteSpace(accountFilters.LastUpdateBy))
                {
                    builder.Where("a1.lastupdateby = @lastupdateby", new { lastupdateby = accountFilters.LastUpdateBy });
                }

                if (accountFilters.LastUpdateById != null)
                {
                    builder.Where("a1.lastupdatebyid = @lastupdatebyid", new { lastupdatebyid = accountFilters.LastUpdateById });
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
                    passwordresetrequired = @passwordresetrequired,
                    role = @role,
                    lastupdatedate = @lastupdatedate,
                    lastupdatebyid = @lastupdatebyid
                WHERE id = @id;
            ");

            builder.AddParameters(new
            {
                name = account.Name,
                passwordhash = account.PasswordHash,
                passwordresetrequired = account.PasswordResetRequired,
                role = account.Role.ToString(),
                id = account.Id,
                lastupdatedate = account.LastUpdateDate,
                lastupdatebyid = account.LastUpdateById,
            });

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query.RawSql, query.Parameters);
            }

            return Get(account.Id);
        }
    }
}