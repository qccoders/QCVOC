// <copyright file="EventRepository.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Event.Data.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using QCVOC.Api.Common;
    using QCVOC.Api.Common.Data.ConnectionFactory;
    using QCVOC.Api.Common.Data.Repository;
    using QCVOC.Api.Event.Data.Model;
    using QCVOC.Api.Event;

    /// <summary>
    ///     Provides data access for <see cref="Event"/>.
    /// </summary>
    public class EventRepository : IRepository<Event>
    {
        public EventRepository(IDbConnectionFactory connectionFactory)
        {
            connectionFactory = connectionFactory;
        }

        private IDbConnectionFactory ConnectionFactory { get; }

        public Event Create(Event @event)
        {
            var builder = new SqlBuilder();

            var query = builder.AddTemplate(@"
                INSERT INTO events
                    (id, name, startdate, enddate, creationdate, creationbyid, lastupdatedate, lastupdatebyid)
                VALUES
                    (@id, @name, @startdate, @enddate, @creationdate, @creationbyid, @lastupdatedate, @lastupdatebyid)
            ");

            builder.AddParameters(new
            {
                id = @event.Id,
                name = @event.Name,
                startdate = @event.StartDate,
                enddate = @event.EndDate,
                creationdate = @event.CreationDate,
                creationbyid = @event.CreationById,
                lastupdatedate = @event.LastUpdateDate,
                lastupdatebyid = @event.LastUpdateById,
            });

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query.RawSql, query.Parameters);
            }

            return Get(@event.Id);
        }

        public Event Get(Guid id)
        {
            return GetAll(new EventFilters() { Id = id }).SingleOrDefault();
        }

        public IEnumerable<Event> GetAll(Filters filters = null)
        {
            filters = filters ?? new Filters();
            var builder = new SqlBuilder();

            var query = builder.AddTemplate($@"
                SELECT
                    e.id,
                    e.name,
                    e.startdate,
                    e.enddate,
                    e.crationdate,
                    e.creationbyid,
                    COALESCE(a1.name, '(Deleted user)') AS creationby,
                    e.lastupdatedate,
                    e.lastupdatebyid,
                    COALESCE(a2.name, '(Deleted user)') AS lastupdateby
                FROM events e
                LEFT JOIN accounts a1 ON e.creationbyid = a1.id
                LEFT JOIN accounts a2 ON e.lastupdatebyid = a2.id
                /**where**/
                ORDER BY e.startdate {filters.OrderBy.ToString()}
                LIMIT @limit OFFSET @offset
            ");
        }
    }
}
