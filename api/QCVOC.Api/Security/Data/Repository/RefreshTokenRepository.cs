// <copyright file="RefreshTokenRepository.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
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
    ///     Provides data access for <see cref="RefreshToken"/>.
    /// </summary>
    public class RefreshTokenRepository : IRepository<RefreshToken>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RefreshTokenRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The database connection factory used for data access.</param>
        public RefreshTokenRepository(IDbConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
        }

        private IDbConnectionFactory ConnectionFactory { get; }

        /// <summary>
        ///     Creates a new <see cref="RefreshToken"/> from the specified <paramref name="refreshToken"/>.
        /// </summary>
        /// <param name="refreshToken">The RefreshToken to create.</param>
        /// <returns>The created RefreshToken.</returns>
        public RefreshToken Create(RefreshToken refreshToken)
        {
            var builder = new SqlBuilder();

            var query = builder.AddTemplate(@"
                INSERT INTO refreshtokens (
                    id,
                    issued,
                    expires,
                    accountid
                )
                VALUES (
                    @id,
                    @issued,
                    @expires,
                    @accountid
                );
            ");

            builder.AddParameters(new
            {
                id = refreshToken.Id,
                issued = refreshToken.Issued,
                expires = refreshToken.Expires,
                accountid = refreshToken.AccountId
            });

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query.RawSql, query.Parameters);
            }

            return Get(refreshToken.AccountId);
        }

        /// <summary>
        ///     Deletes the <see cref="RefreshToken"/> matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the <see cref="RefreshToken"/> to delete.</param>
        public void Delete(Guid id)
        {
            var builder = new SqlBuilder();

            var query = builder.AddTemplate(@"
                DELETE FROM refreshtokens
                WHERE id = @id
            ");

            builder.AddParameters(new { id });

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query.RawSql, query.Parameters);
            }
        }

        /// <summary>
        ///     Deletes the specified <paramref name="refreshToken"/>
        /// </summary>
        /// <param name="refreshToken">The RefreshToken to delete.</param>
        public void Delete(RefreshToken refreshToken)
        {
            Delete(refreshToken.Id);
        }

        /// <summary>
        ///     Retrieves the <see cref="RefreshToken"/> matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the <see cref="RefreshToken"/> to retrieve.</param>
        /// <returns>The RefreshToken matching the specified id.</returns>
        public RefreshToken Get(Guid id)
        {
            return GetAll(new RefreshTokenFilters() { Id = id }).SingleOrDefault();
        }

        /// <summary>
        ///     Retrieves a lisst of all <see cref="RefreshToken"/> objects in the collection.
        /// </summary>
        /// <param name="filters">Optional query filters.</param>
        /// <returns>A list of all <see cref="RefreshToken"/> objects in the collection.</returns>
        public IEnumerable<RefreshToken> GetAll(Filters filters = null)
        {
            filters = filters ?? new Filters();
            var builder = new SqlBuilder();

            var query = builder.AddTemplate(@"
                SELECT
                    accountid AS AccountID,
                    expires AS Expires,
                    issued AS Issued,
                    id AS Id
                FROM refreshtokens
                /**where**/
            ");

            builder.AddParameters(new
            {
                limit = filters.Limit,
                offset = filters.Offset,
                orderby = filters.OrderBy.ToString(),
            });

            if (filters is RefreshTokenFilters refreshTokenFilters)
            {
                builder.ApplyFilter(FilterType.Equals, "id", refreshTokenFilters.Id);
                builder.ApplyFilter(FilterType.Equals, "accountid", refreshTokenFilters.AccountId);
            }

            using (var db = ConnectionFactory.CreateConnection())
            {
                return db.Query<RefreshToken>(query.RawSql, query.Parameters);
            }
        }

        /// <summary>
        ///     Updates the specified <paramref name="refreshToken"/>
        /// </summary>
        /// <param name="refreshToken">The RefreshToken to update.</param>
        /// <returns>The updated RefreshToken.</returns>
        public RefreshToken Update(RefreshToken refreshToken)
        {
            throw new NotImplementedException("Refresh tokens are immutable.");
        }
    }
}