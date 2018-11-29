// <copyright file="VeteranRepository.cs" company="QC Coders">
//     Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Veterans.Data.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using QCVOC.Api.Common;
    using QCVOC.Api.Common.Data.ConnectionFactory;
    using QCVOC.Api.Common.Data.Repository;
    using QCVOC.Api.Veterans.Data.Model;

    /// <summary>
    ///     Provides data access for <see cref="Veteran"/>.
    /// </summary>
    public class VeteranRepository : ISingleKeyRepository<Veteran>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="VeteranRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The database connection factory used for data access.</param>
        public VeteranRepository(IDbConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
        }

        private IDbConnectionFactory ConnectionFactory { get; }

        /// <summary>
        ///     Creates a new Veteran from the specified <paramref name="veteran"/>.
        /// </summary>
        /// <param name="veteran">The Veteran to create.</param>
        /// <returns>The created Veteran.</returns>
        public Veteran Create(Veteran veteran)
        {
            var builder = new SqlBuilder();

            var query = builder.AddTemplate(@"
                INSERT INTO veterans (
                    id,
                    cardnumber,
                    firstname,
                    lastname,
                    lastupdatedate,
                    lastupdatebyid,
                    address,
                    photobase64,
                    primaryphone,
                    email,
                    enrollmentdate,
                    enrollmentbyid,
                    deleted,
                    verificationmethod
                )
                VALUES (
                    @id,
                    @cardnumber,
                    @firstname,
                    @lastname,
                    @lastupdatedate,
                    @lastupdatebyid,
                    @address,
                    @photobase64,
                    @primaryphone,
                    @email,
                    @enrollmentdate,
                    @enrollmentbyid,
                    @deleted,
                    @verificationmethod
                )
            ");

            builder.AddParameters(new
            {
                id = veteran.Id,
                cardnumber = veteran.CardNumber,
                firstname = veteran.FirstName,
                lastname = veteran.LastName,
                lastupdatedate = veteran.LastUpdateDate,
                lastupdatebyid = veteran.LastUpdateById,
                address = veteran.Address,
                photobase64 = veteran.PhotoBase64,
                primaryphone = veteran.PrimaryPhone,
                email = veteran.Email,
                enrollmentdate = veteran.EnrollmentDate,
                enrollmentbyid = veteran.EnrollmentById,
                deleted = false,
                verificationmethod = veteran.VerificationMethod.ToString(),
            });

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query.RawSql, query.Parameters);
            }

            return Get(veteran.Id);
        }

        /// <summary>
        ///     Deletes the Veteran matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the Veteran to delete.</param>
        public void Delete(Guid id)
        {
            var builder = new SqlBuilder();

            var query = builder.AddTemplate(@"
                UPDATE veterans
                SET 
                    deleted = true,
                    cardnumber = NULL
                WHERE id = @id
            ");

            builder.AddParameters(new { id });

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query.RawSql, query.Parameters);
            }
        }

        /// <summary>
        ///     Deletes the specified <paramref name="veteran"/>.
        /// </summary>
        /// <param name="veteran">The Veteran to delete.</param>
        public void Delete(Veteran veteran)
        {
            Delete(veteran.Id);
        }

        /// <summary>
        ///     Retrieves the Veteran matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the <see cref="Veteran"/> to retrieve.</param>
        /// <returns>The Veteran matching the specified id.</returns>
        public Veteran Get(Guid id)
        {
            return GetAll(new VeteranFilters() { Id = id }).SingleOrDefault();
        }

        /// <summary>
        ///     Retrieves all Veterans after applying optional <paramref name="filters"/>.
        /// </summary>
        /// <param name="filters">Optional query filters.</param>
        /// <returns>A list of Veterans</returns>
        public IEnumerable<Veteran> GetAll(Filters filters = null)
        {
            filters = filters ?? new Filters();
            var builder = new SqlBuilder();

            var query = builder.AddTemplate($@"
                SELECT
                    v.id,
                    v.cardnumber,
                    v.firstname,
                    v.lastname,
                    v.lastupdatedate,
                    a.name AS lastupdateby,
                    v.lastupdatebyid,
                    v.address,
                    v.photobase64,
                    v.primaryphone,
                    v.email,
                    v.enrollmentdate,
                    v.enrollmentbyid,
                    b.name AS enrollmentby,
                    v.verificationmethod
                FROM veterans v
                LEFT JOIN accounts a ON v.lastupdatebyid = a.id 
                LEFT JOIN accounts b ON v.enrollmentbyid = b.id
                /**where**/
                ORDER BY (firstname || lastname) {filters.OrderBy.ToString()}
                LIMIT @limit OFFSET @offset
            ");

            builder.AddParameters(new
            {
                limit = filters.Limit,
                offset = filters.Offset,
                orderby = filters.OrderBy.ToString(),
            });

            builder.ApplyFilter(FilterType.Equals, "v.deleted", false);

            if (filters is VeteranFilters veteranFilters)
            {
                builder
                    .ApplyFilter(FilterType.Equals, "v.address", veteranFilters.Address)
                    .ApplyFilter(FilterType.Equals, "v.email", veteranFilters.Email)
                    .ApplyFilter(FilterType.Equals, "v.firstname", veteranFilters.FirstName)
                    .ApplyFilter(FilterType.Equals, "v.id", veteranFilters.Id)
                    .ApplyFilter(FilterType.Equals, "v.lastname", veteranFilters.LastName)
                    .ApplyFilter(FilterType.Equals, "v.cardnumber", veteranFilters.CardNumber)
                    .ApplyFilter(FilterType.Equals, "v.primaryphone", veteranFilters.PrimaryPhone)
                    .ApplyFilter(FilterType.In, "v.verificationmethod", veteranFilters.VerificationMethod?.ToString());
            }

            using (var db = ConnectionFactory.CreateConnection())
            {
                return db.Query<Veteran>(query.RawSql, query.Parameters);
            }
        }

        /// <summary>
        ///     Updates the specified <paramref name="veteran"/>.
        /// </summary>
        /// <param name="veteran">The Veteran to update.</param>
        /// <returns>The updated Veteran.</returns>
        public Veteran Update(Veteran veteran)
        {
            var builder = new SqlBuilder();

            var query = builder.AddTemplate(@"
                UPDATE veterans
                SET
                    cardnumber = @cardnumber,
                    firstname = @firstName,
                    lastname = @lastName,
                    lastupdatedate = @lastupdatedate,
                    lastupdatebyid = @lastupdatebyid,
                    address = @address,
                    photobase64 = @photobase64,
                    primaryphone = @primaryphone,
                    email = @email,
                    verificationmethod = @verificationmethod
                WHERE id = @id
            ");

            builder.AddParameters(new
            {
                cardnumber = veteran.CardNumber,
                firstName = veteran.FirstName,
                lastName = veteran.LastName,
                lastupdatedate = veteran.LastUpdateDate,
                lastupdatebyid = veteran.LastUpdateById,
                address = veteran.Address,
                photobase64 = veteran.PhotoBase64,
                primaryPhone = veteran.PrimaryPhone,
                email = veteran.Email,
                id = veteran.Id,
                verificationmethod = veteran.VerificationMethod.ToString(),
            });

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query.RawSql, query.Parameters);
            }

            return Get(veteran.Id);
        }
    }
}