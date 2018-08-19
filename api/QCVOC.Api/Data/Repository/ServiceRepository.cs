// <copyright file="ServiceRepository.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Data.Repository
{
    using System;
    using System.Collections.Generic;
    using Dapper;
    using QCVOC.Api.Data.ConnectionFactory;
    using QCVOC.Api.Data.Model;
    using static QCVOC.Api.Controllers.AccountsController;

    public class ServiceRepository : IRepository<Service>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory"></param>
        public ServiceRepository(IDbConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
        }

        private IDbConnectionFactory ConnectionFactory { get; }

        public Service Create(Service service)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                var query = @"
                    INSERT
                    INTO services
                    (
                        id,
                        name,
                        limit
                    )
                    VALUES(
                        @id,
                        @name,
                        @limit
                    )
                ";

                var param = new
                {
                    id = service.Id,
                    name = service.Name,
                    limit = service.Limit
                };

                db.Execute(query, param);

                var inserted = Get(service.Id);
                return inserted;
            }
        }

        public void Delete(Guid id)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                var query = @"
                    DELETE
                    FROM services
                    WHERE id = @id;
                ";

                var param = new { id };
                db.Execute(query, param);
            }
        }

        public void Delete(Service service)
        {
            if (service == null)
            {
                throw new ArgumentException("Service cannot be null.", nameof(service));
            }

            Delete(service.Id);
        }

        public Service Get(Guid id)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                var query = @"
                    SELECT
                        id,
                        name,
                        limit
                    FROM services
                    WHERE id = @id;
                ";

                var param = new { id };

                return db.QueryFirstOrDefault<Service>(query, param);
            }
        }

        public IEnumerable<Service> GetAll(QueryParameters queryParameters = null)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                var query = @"
                    SELECT
                        id,
                        name,
                        limit
                    FROM services
                    WHERE id = @id;
                ";

                return db.Query<Service>(query);
            }
        }

        public Service Update(Service service)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                var query = @"
                    UPDATE services
                    SET
                        id = @id,
                        name = @name,
                        limit = @limit
                    WHERE id = @id
                ";

                var param = new
                {
                    id = service.Id,
                    name = service.Name,
                    limit = service.Limit
                };

                db.Execute(query, param);

                return Get(service.Id);
            }
        }
    }
}