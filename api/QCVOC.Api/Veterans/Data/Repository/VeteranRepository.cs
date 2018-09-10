// <copyright file="VeteranRepository.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
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
    public class VeteranRepository : IRepository<Veteran>
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
                    primaryphone,
                    email,
                    enrollmentdate,
                    enrollmentbyid,
                    deleted
                )
                VALUES (
                    @id,
                    @cardnumber,
                    @firstname,
                    @lastname,
                    @lastupdatedate,
                    @lastupdatebyid,
                    @address,
                    @primaryphone,
                    @email,
                    @enrollmentdate,
                    @enrollmentbyid,
                    @deleted
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
                primaryphone = veteran.PrimaryPhone,
                email = veteran.Email,
                enrollmentdate = veteran.EnrollmentDate,
                enrollmentbyid = veteran.EnrollmentById,
                deleted = false,
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
                    deleted = true
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
                    p.id,
                    cardnumber,
                    firstname,
                    lastname,
                    p.lastupdatedate,
                    a.name AS lastupdateby,
                    p.lastupdatebyid,
                    address,
                    primaryphone,
                    email,
                    enrollmentdate,
                    enrollmentbyid,
                    b.name AS enrollmentby
                FROM veterans p
                LEFT JOIN accounts a ON p.lastupdatebyid = a.id 
                LEFT JOIN accounts b ON p.enrollmentbyid = b.id
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

            builder.ApplyFilter(FilterType.Equals, "deleted", false);

            if (filters is VeteranFilters veteranFilters)
            {
                builder
                    .ApplyFilter(FilterType.Equals, "address", veteranFilters.Address)
                    .ApplyFilter(FilterType.Equals, "email", veteranFilters.Email)
                    .ApplyFilter(FilterType.Between, "enrollmentdate", veteranFilters.EnrollmentDateStart, veteranFilters.EnrollmentDateEnd)
                    .ApplyFilter(FilterType.Equals, "enrollmentbyid", veteranFilters.EnrollmentById)
                    .ApplyFilter(FilterType.Equals, "enrollmentby", veteranFilters.EnrollmentBy)
                    .ApplyFilter(FilterType.Equals, "firstname", veteranFilters.FirstName)
                    .ApplyFilter(FilterType.Equals, "p.id", veteranFilters.Id)
                    .ApplyFilter(FilterType.Equals, "lastname", veteranFilters.LastName)
                    .ApplyFilter(FilterType.Between, "lastupdatedate", veteranFilters.LastUpdateDateStart, veteranFilters.LastUpdateDateEnd)
                    .ApplyFilter(FilterType.Equals, "a.name", veteranFilters.LastUpdateBy)
                    .ApplyFilter(FilterType.Equals, "lastupdatebyid", veteranFilters.LastUpdateById)
                    .ApplyFilter(FilterType.Equals, "lastupdateby", veteranFilters.LastUpdateBy)
                    .ApplyFilter(FilterType.Equals, "cardnumber", veteranFilters.CardNumber)
                    .ApplyFilter(FilterType.Equals, "primaryphone", veteranFilters.PrimaryPhone);
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
                    primaryphone = @primaryphone,
                    email = @email
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
                primaryPhone = veteran.PrimaryPhone,
                email = veteran.Email,
                id = veteran.Id,
            });

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query.RawSql, query.Parameters);
            }

            return Get(veteran.Id);
        }
    }
}