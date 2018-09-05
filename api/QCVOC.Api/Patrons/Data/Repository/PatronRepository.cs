// <copyright file="PatronRepository.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Patrons.Data.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using QCVOC.Api.Common;
    using QCVOC.Api.Common.Data.ConnectionFactory;
    using QCVOC.Api.Common.Data.Repository;
    using QCVOC.Api.Patrons.Data.Model;

    /// <summary>
    ///     Provides data access for <see cref="Patron"/>.
    /// </summary>
    public class PatronRepository : IRepository<Patron>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PatronRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The database connection factory used for data access.</param>
        public PatronRepository(IDbConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
        }

        private IDbConnectionFactory ConnectionFactory { get; }

        /// <summary>
        ///     Creates a new Patron from the specified <paramref name="patron"/>.
        /// </summary>
        /// <param name="patron">The Patron to create.</param>
        /// <returns>The created Patron.</returns>
        public Patron Create(Patron patron)
        {
            var builder = new SqlBuilder();

            var query = builder.AddTemplate(@"
                INSERT INTO patrons (
                    id,
                    memberid,
                    firstname,
                    lastname,
                    lastupdatedate,
                    lastupdatebyid,
                    address,
                    primaryphone,
                    email,
                    enrollmentdate
                )
                VALUES (
                    @id,
                    @memberid,
                    @firstname,
                    @lastname,
                    @lastupdatedate,
                    @lastupdatebyid,
                    @address,
                    @primaryphone,
                    @email,
                    @enrollmentdate
                )
            ");

            builder.AddParameters(new
            {
                id = patron.Id,
                memberid = patron.MemberId,
                firstname = patron.FirstName,
                lastname = patron.LastName,
                lastupdatedate = patron.LastUpdateDate,
                lastupdatebyid = patron.LastUpdateById,
                address = patron.Address,
                primaryphone = patron.PrimaryPhone,
                email = patron.Email,
                enrollmentdate = patron.EnrollmentDate,
            });

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query.RawSql, query.Parameters);
            }

            return Get(patron.Id);
        }

        /// <summary>
        ///     Deletes the Patron matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the Patron to delete.</param>
        public void Delete(Guid id)
        {
            var builder = new SqlBuilder();

            var query = builder.AddTemplate(@"
                DELETE FROM patrons
                WHERE id = @id
            ");

            builder.AddParameters(new { id });

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query.RawSql, query.Parameters);
            }
        }

        /// <summary>
        ///     Deletes the specified <paramref name="patron"/>.
        /// </summary>
        /// <param name="patron">The Patron to delete.</param>
        public void Delete(Patron patron)
        {
            Delete(patron.Id);
        }

        /// <summary>
        ///     Retrieves the Patron matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the <see cref="Patron"/> to retrieve.</param>
        /// <returns>The Patron matching the specified id.</returns>
        public Patron Get(Guid id)
        {
            return GetAll(new PatronFilters() { Id = id }).SingleOrDefault();
        }

        /// <summary>
        ///     Retrieves all Patrons after applying optional <paramref name="filters"/>.
        /// </summary>
        /// <param name="filters">Optional query filters.</param>
        /// <returns>A list of Patrons</returns>
        public IEnumerable<Patron> GetAll(Filters filters = null)
        {
            filters = filters ?? new Filters();
            var builder = new SqlBuilder();

            var query = builder.AddTemplate($@"
                SELECT
                    p.id,
                    memberid,
                    firstname,
                    lastname,
                    p.lastupdatedate,
                    COALESCE(a.name, '(Deleted user)') AS lastupdateby,
                    p.lastupdatebyid,
                    address,
                    primaryphone,
                    email,
                    enrollmentdate
                FROM patrons p
                LEFT JOIN accounts a ON p.lastupdatebyid = a.id 
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

            if (filters is PatronFilters patronFilters)
            {
                builder
                    .ApplyFilter(FilterType.Equals, "address", patronFilters.Address)
                    .ApplyFilter(FilterType.Equals, "email", patronFilters.Email)
                    .ApplyFilter(FilterType.Between, "enrollmentdate", patronFilters.EnrollmentDateStart, patronFilters.EnrollmentDateEnd)
                    .ApplyFilter(FilterType.Equals, "firstname", patronFilters.FirstName)
                    .ApplyFilter(FilterType.Equals, "p.id", patronFilters.Id)
                    .ApplyFilter(FilterType.Equals, "lastname", patronFilters.LastName)
                    .ApplyFilter(FilterType.Between, "lastupdatedate", patronFilters.LastUpdateDateStart, patronFilters.LastUpdateDateEnd)
                    .ApplyFilter(FilterType.Equals, "a.name", patronFilters.LastUpdateBy)
                    .ApplyFilter(FilterType.Equals, "lastupdatebyid", patronFilters.LastUpdateById)
                    .ApplyFilter(FilterType.Equals, "memberid", patronFilters.MemberId)
                    .ApplyFilter(FilterType.Equals, "primaryphone", patronFilters.PrimaryPhone);
            }

            using (var db = ConnectionFactory.CreateConnection())
            {
                return db.Query<Patron>(query.RawSql, query.Parameters);
            }
        }

        /// <summary>
        ///     Updates the specified <paramref name="patron"/>.
        /// </summary>
        /// <param name="patron">The Patron to update.</param>
        /// <returns>The updated Patron.</returns>
        public Patron Update(Patron patron)
        {
            var builder = new SqlBuilder();

            var query = builder.AddTemplate(@"
                UPDATE patrons
                SET
                    memberid = @memberId,
                    firstname = @firstName,
                    lastname = @lastName,
                    lastupdatedate = @lastupdatedate,
                    lastupdatebyid = @lastupdatebyid,
                    address = @address,
                    secondaryphone = @secondaryPhone,
                    email = @email,
                    enrollmentdate = @enrollmentDate
                WHERE id = @id
            ");

            builder.AddParameters(new
            {
                memberId = patron.MemberId,
                firstName = patron.FirstName,
                lastName = patron.LastName,
                lastupdatedate = patron.LastUpdateDate,
                lastupdatebyid = patron.LastUpdateById,
                address = patron.Address,
                primaryPhone = patron.PrimaryPhone,
                email = patron.Email,
                enrollmentDate = patron.EnrollmentDate,
                id = patron.Id
            });

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query.RawSql, query.Parameters);
            }

            return Get(patron.Id);
        }
    }
}