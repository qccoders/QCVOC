// <copyright file="RefreshTokenRepository.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Security.Data.Repository
{
    using System;
    using System.Collections.Generic;
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
            var query = @"
                INSERT INTO refreshtokens (
                    tokenid,
                    issued,
                    expires,
                    accountid
                )
                VALUES (
                    @tokenid,
                    @issued,
                    @expires,
                    @accountid
                );
            ";

            var param = new
            {
                tokenid = refreshToken.TokenId,
                issued = refreshToken.Issued,
                expires = refreshToken.Expires,
                accountid = refreshToken.AccountId
            };

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query, param);
                return Get(refreshToken.AccountId);
            }
        }

        /// <summary>
        ///     Deletes the <see cref="RefreshToken"/> matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the <see cref="RefreshToken"/> to delete.</param>
        public void Delete(Guid id)
        {
            var query = @"
                DELETE FROM refreshtokens
                WHERE accountId = @id
            ";

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query, new { id });
            }
        }

        /// <summary>
        ///     Deletes the specified <paramref name="refreshToken"/>
        /// </summary>
        /// <param name="refreshToken">The RefreshToken to delete.</param>
        public void Delete(RefreshToken refreshToken)
        {
            if (refreshToken == null)
            {
                throw new ArgumentException("token cannot be null.", nameof(refreshToken));
            }

            Delete(refreshToken.AccountId);
        }

        /// <summary>
        ///     Retrieves the <see cref="RefreshToken"/> matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the <see cref="RefreshToken"/> to retrieve.</param>
        /// <returns>The RefreshToken matching the specified id.</returns>
        public RefreshToken Get(Guid id)
        {
            var query = @"
                SELECT
                    accountid AS AccountID,
                    expires AS Expires,
                    issued AS Issued,
                    tokenid AS TokenId
                FROM refreshtokens
                WHERE tokenid = @tokenid
            ";

            using (var db = ConnectionFactory.CreateConnection())
            {
                return db.QueryFirstOrDefault<RefreshToken>(query, new { tokenid = id });
            }
        }

        /// <summary>
        ///     Retrieves a lisst of all <see cref="RefreshToken"/> objects in the collection.
        /// </summary>
        /// <returns>A list of all <see cref="RefreshToken"/> objects in the collection.</returns>
        public IEnumerable<RefreshToken> GetAll(QueryParameters queryParameters = null)
        {
            var query = @"
                SELECT
                    accountid AS AccountID,
                    expires AS Expires,
                    issued AS Issued,
                    tokenid AS TokenId
                FROM refreshtokens
            ";

            using (var db = ConnectionFactory.CreateConnection())
            {
                return db.Query<RefreshToken>(query);
            }
        }

        /// <summary>
        ///     Updates the specified <paramref name="refreshToken"/>
        /// </summary>
        /// <param name="refreshToken">The RefreshToken to update.</param>
        /// <returns>The updated RefreshToken.</returns>
        public RefreshToken Update(RefreshToken refreshToken)
        {
            var query = @"
                UPDATE refreshtokens
                SET
                    expires = @expires,
                    issued = @issued,
                    tokenid = @tokenid
                WHERE accountid = @id;
            ";

            var param = new
            {
                id = refreshToken.AccountId,
                expires = refreshToken.Expires,
                issued = refreshToken.Issued,
                tokenid = refreshToken.TokenId
            };

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query, param);
                return Get(refreshToken.AccountId);
            }
        }
    }
}