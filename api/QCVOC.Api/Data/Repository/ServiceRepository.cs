namespace QCVOC.Api.Data.Repository
{
    using System;
    using System.Collections.Generic;
    using Dapper;
    using QCVOC.Api.Data.ConnectionFactory;
    using QCVOC.Api.Data.Model;

    public class ServiceRepository : IRepository<Service>
    {
        public ServiceRepository(IDbConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
        }

        private IDbConnectionFactory ConnectionFactory { get; }

        public Service Create(Service service)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(
                    @"
                        INSERT INTO services
                        (id, name, limit)
                        VALUES (@id, @name, @limit)
                    ",
                    new
                    {
                        id = service.Id,
                        name = service.Name,
                        limit = service.Limit,
                    }
                );

                var inserted = Get(service.Id);
                return inserted;
            }
        }

        public void Delete(Guid id)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute("DELETE FROM services WHERE id = @id", new { id = id });
            }
        }

        public void Delete(Service service)
        {
            if (service == null)
                throw new ArgumentException("Service cannot be null.", nameof(service));

            Delete(service.Id);
        }

        public Service Get(Guid id)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                return db.QueryFirstOrDefault<Service>(
                    @"
                        SELECT
                            id,
                            name,
                            limit
                        FROM services
                        WHERE id = @id;
                    ", new { id = id }
                );
            }
        }

        public IEnumerable<Service> GetAll()
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                return db.Query<Service>(
                    @"
                        SELECT
                            id,
                            name,
                            limit
                        FROM services
                        WHERE id = @id;
                    "
                );
            }
        }

        public Service Update(Service service)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(
                    @"
                        UPDATE services
                        SET
                            id = @id,
                            name = @name,
                            limit = @limit
                        WHERE id = @id
                    ",
                    new
                    {
                        id = service.Id,
                        name = service.Name,
                        limit = service.Limit
                    }
                );

                return Get(service.Id);
            }
        }
    }
}