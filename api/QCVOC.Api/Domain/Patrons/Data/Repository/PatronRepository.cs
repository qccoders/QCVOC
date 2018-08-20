// <copyright file="PatronRepository.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Domain.Patrons.Data.Repository
{
    using System;
    using System.Collections.Generic;
    using Dapper;
    using QCVOC.Api.Common;
    using QCVOC.Api.Common.Data.ConnectionFactory;
    using QCVOC.Api.Common.Data.Repository;
    using QCVOC.Api.Domain.Patrons.Data.Model;

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
        ///     Creates a new <see cref="Patron"/> from the specified <paramref name="patron"/>.
        /// </summary>
        /// <param name="patron">The Patron to create.</param>
        /// <returns>The created Patron.</returns>
        public Patron Create(Patron patron)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                var query = @"
                    INSERT INTO patrons (
                        id,
                        memberid,
                        firstname,
                        lastname,
                        address,
                        primaryphone,
                        secondaryphone,
                        email,
                        enrollmentdate
                    )
                    VALUES (
                        @id,
                        @memberId,
                        @firstName,
                        @lastName,
                        @address,
                        @primaryPhone,
                        @secondaryPhone,
                        @email,
                        @enrollmentDate
                    )
                ";

                var param = new
                {
                    id = patron.Id,
                    memberId = patron.MemberId,
                    firstName = patron.FirstName,
                    lastName = patron.LastName,
                    address = patron.Address,
                    primaryPhone = patron.PrimaryPhone,
                    secondaryPhone = patron.SecondaryPhone,
                    email = patron.Email,
                    enrollmentDate = patron.EnrollmentDate,
                };

                db.Execute(query, param);

                var inserted = Get(patron.Id);
                return inserted;
            }
        }

        /// <summary>
        ///     Deletes the <see cref="Patron"/> matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the <see cref="Patron"/> to delete.</param>
        public void Delete(Guid id)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                var query = @"
                    DELETE
                    FROM patrons
                    WHERE id = @id
                ";

                db.Execute(query, new { id });
            }
        }

        /// <summary>
        ///     Deletes the specified <paramref name="patron"/>.
        /// </summary>
        /// <param name="patron">The Patron to delete.</param>
        public void Delete(Patron patron)
        {
            if (patron == null)
            {
                throw new ArgumentException("patron cannot be null.", nameof(patron));
            }

            Delete(patron.Id);
        }

        /// <summary>
        ///     Retrieves the <see cref="Patron"/> matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the <see cref="Patron"/> to retrieve.</param>
        /// <returns>The Patron matching the specified id.</returns>
        public Patron Get(Guid id)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                var query = @"
                    SELECT
                        id,
                        memberid,
                        firstname,
                        lastname,
                        address,
                        primaryphone,
                        secondaryphone,
                        email,
                        enrollmentdate
                    FROM patrons
                    WHERE id = @id;
                ";

                return db.QueryFirstOrDefault<Patron>(query, new { id });
            }
        }

        /// <summary>
        ///     Retrieves a list of all <see cref="Patron"/> objects in the collection.
        /// </summary>
        /// <returns>A list of all <see cref="Patron"/> objects in the collection.</returns>
        public IEnumerable<Patron> GetAll(QueryParameters queryParameters = null)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                var query = @"
                    SELECT
                        id,
                        memberid,
                        firstname,
                        lastname,
                        address,
                        primaryphone,
                        secondaryphone,
                        email,
                        enrollmentdate
                    FROM patrons;
                ";

                return db.Query<Patron>(query);
            }
        }

        /// <summary>
        ///     Updates the specified <paramref name="patron"/>.
        /// </summary>
        /// <param name="patron">The Patron to update.</param>
        /// <returns>The updated Patron.</returns>
        public Patron Update(Patron patron)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                var query = @"
                    UPDATE patrons
                    SET
                        memberid = @memberId,
                        firstname = @firstName,
                        lastname = @lastName,
                        address = @address,
                        primaryphone = @primaryPhone,
                        secondaryphone = @secondaryPhone,
                        email = @email,
                        enrollmentdate = @enrollmentDate
                    WHERE id = @id
                ";

                var param = new
                {
                    memberId = patron.MemberId,
                    firstName = patron.FirstName,
                    lastName = patron.LastName,
                    address = patron.Address,
                    primaryPhone = patron.PrimaryPhone,
                    secondaryPhone = patron.SecondaryPhone,
                    email = patron.Email,
                    enrollmentDate = patron.EnrollmentDate,
                    id = patron.Id
                };

                db.Execute(query, param);

                return Get(patron.Id);
            }
        }
    }
}