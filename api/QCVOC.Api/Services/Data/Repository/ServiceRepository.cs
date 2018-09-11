// <copyright file="ServiceRepository.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Services.Data.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using QCVOC.Api.Common;
    using QCVOC.Api.Common.Data.ConnectionFactory;
    using QCVOC.Api.Common.Data.Repository;
    using QCVOC.Api.Services.Data.Model;

    /// <summary>
    ///     Provides data access for <see cref="Service"/>.
    /// </summary>
    public class ServiceRepository : IRepository<Service>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The database connection factory used for data access.</param>
        public ServiceRepository(IDbConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
        }

        private IDbConnectionFactory ConnectionFactory { get; }

        /// <summary>
        ///     Creates a new Service from the specified <paramref name="service"/>.
        /// </summary>
        /// <param name="service">The Service to create.</param>
        /// <returns>The created Service.</returns>
        public Service Create(Service service)
        {
            var builder = new SqlBuilder();

            var query = builder.AddTemplate(@"
                INSERT INTO services (
                    id,
                    name,
                    description,
                    creationdate,
                    creationbyid,
                    lastupdatedate,
                    lastupdatebyid,
                    deleted
                )
                VALUES (
                    @id,
                    @name,
                    @description,
                    @creationdate,
                    @creationbyid,  
                    @lastupdatedate,
                    @lastupdatebyid,
                    @deleted
                )
            ");

            builder.AddParameters(new
            {
                id = service.Id,
                name = service.Name,
                description = service.Description,
                creationdate = service.CreationDate,
                creationbyid = service.CreationById,
                lastupdatedate = service.LastUpdateDate,
                lastupdatebyid = service.LastUpdateById,
                deleted = false,
            });

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query.RawSql, query.Parameters);
            }

                return Get(service.Id);
        }

        /// <summary>
        ///     Deletes the Service matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the Service to delete.</param>
        public void Delete(Guid id)
        {
            var builder = new SqlBuilder();

            var query = builder.AddTemplate(@"
                UPDATE services
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
        ///     Deletes the specified <paramref name="service"/>.
        /// </summary>
        /// <param name="service">The Service to delete.</param>
        public void Delete(Service service)
        {
            Delete(service.Id);
        }

        /// <summary>
        ///     Retrieves the Service matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the <see cref="Service"/> to retrieve.</param>
        /// <returns>The Service matching the specified id.</returns>
        public Service Get(Guid id)
        {
            return GetAll(new ServiceFilters() { Id = id }).SingleOrDefault();
        }

        /// <summary>
        ///     Retrieves all Services after applying optional <paramref name="filters"/>.
        /// </summary>
        /// <param name="filters">Optional query filters.</param>
        /// <returns>A list of Services.</returns>
        public IEnumerable<Service> GetAll(Filters filters = null)
        {
            filters = filters ?? new Filters();
            var builder = new SqlBuilder();

            var query = builder.AddTemplate($@"
                SELECT
                    s.id,
                    s.name,
                    s.description,
                    a.name AS creationby,
                    s.creationbyid,
                    s.creationdate,
                    b.name AS lastupdateby,
                    s.lastupdatebyid,
                    s.lastupdatedate
                FROM services s
                LEFT JOIN accounts a ON s.creationbyid = a.id
                LEFT JOIN accounts b ON s.lastupdatebyid = b.id
                /**where**/
                ORDER BY s.name {filters.OrderBy.ToString()}
                LIMIT @limit OFFSET @offset
            ");

            builder.AddParameters(new
            {
                limit = filters.Limit,
                offset = filters.Offset,
                orderby = filters.OrderBy.ToString(),
            });

            builder.ApplyFilter(FilterType.Equals, "s.deleted", false);

            if (filters is ServiceFilters serviceFilters)
            {
                builder
                    .ApplyFilter(FilterType.Equals, "s.id", serviceFilters.Id)
                    .ApplyFilter(FilterType.Equals, "s.name", serviceFilters.Name)
                    .ApplyFilter(FilterType.Equals, "s.description", serviceFilters.Description)
                    .ApplyFilter(FilterType.Equals, "creationby", serviceFilters.CreationBy)
                    .ApplyFilter(FilterType.Equals, "creationbyid", serviceFilters.CreationById)
                    .ApplyFilter(FilterType.Between, "creationdate", serviceFilters.CreationDateStart, serviceFilters.CreationDateEnd)
                    .ApplyFilter(FilterType.Equals, "lastupdateby", serviceFilters.LastUpdateBy)
                    .ApplyFilter(FilterType.Equals, "lastupdatebyid", serviceFilters.LastUpdateById)
                    .ApplyFilter(FilterType.Between, "lastupdatedate", serviceFilters.LastUpdateDateStart, serviceFilters.LastUpdateDateEnd);
            }

            using (var db = ConnectionFactory.CreateConnection())
            {
                return db.Query<Service>(query.RawSql, query.Parameters);
            }
        }

        /// <summary>
        ///     Updates the specified <paramref name="service"/>.
        /// </summary>
        /// <param name="service"> The Service to update.</param>
        /// <returns>The updated Service.</returns>
        public Service Update(Service service)
        {
            var builder = new SqlBuilder();

            var query = builder.AddTemplate(@"
                UPDATE services
                SET
                    name = @name,
                    description = @description,
                    lastupdatedate = @lastupdatedate,
                    lastupdatebyid = @lastupdatebyid
                WHERE id = @id
            ");

            builder.AddParameters(new
            {
                name = service.Name,
                description = service.Description,
                lastupdatedate = service.LastUpdateDate,
                lastupdatebyid = service.LastUpdateById,
                id = service.Id,
            });

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query.RawSql, query.Parameters);
            }

            return Get(service.Id);
        }
    }
}