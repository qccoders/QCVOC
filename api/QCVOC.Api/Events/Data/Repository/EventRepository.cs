// <copyright file="EventRepository.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Events.Data.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using QCVOC.Api.Common;
    using QCVOC.Api.Common.Data.ConnectionFactory;
    using QCVOC.Api.Common.Data.Repository;
    using QCVOC.Api.Events.Data.Model;

    /// <summary>
    ///     Provides data access for <see cref="Event"/>.
    /// </summary>
    public class EventRepository : IRepository<Event>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EventRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The database connection factory used for data access.</param>
        public EventRepository(IDbConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
        }

        private IDbConnectionFactory ConnectionFactory { get; }

        /// <summary>
        ///     Creates a new Event from the specified <paramref name="event"/>.
        /// </summary>
        /// <param name="event">The Event to create.</param>
        /// <returns>The created Event.</returns>
        public Event Create(Event @event)
        {
            var builder = new SqlBuilder();

            var query = builder.AddTemplate(@"
                INSERT INTO events
                    (id, name, startdate, enddate, creationdate, creationbyid, lastupdatedate, lastupdatebyid, deleted)
                VALUES
                    (@id, @name, @startdate, @enddate, @creationdate, @creationbyid, @lastupdatedate, @lastupdatebyid, @deleted)
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
                deleted = false,
            });

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query.RawSql, query.Parameters);
            }

            return Get(@event.Id);
        }

        /// <summary>
        ///     Deletes the Event matching the specified <paramref name="id"/>
        /// </summary>
        /// <param name="id">The id of the Event to delete.</param>
        public void Delete(Guid id)
        {
            var builder = new SqlBuilder();

            var query = builder.AddTemplate(@"
                UPDATE events
                SET
                    deleted = true
                WHERE id = @id;
            ");

            builder.AddParameters(new { id });

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query.RawSql, query.Parameters);
            }
        }

        /// <summary>
        ///     Deletes the specified <paramref name="event"/>.
        /// </summary>
        /// <param name="event">The Event to delete.</param>
        public void Delete(Event @event)
        {
            Delete(@event.Id);
        }

        /// <summary>
        ///     Retrieves the Event with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the Event to retrieve.</param>
        /// <returns>The Event with the specified id.</returns>
        public Event Get(Guid id)
        {
            return GetAll(new EventFilters() { Id = id }).SingleOrDefault();
        }

        /// <summary>
        ///     Retrieves all Events after applying optional <paramref name="filters"/>.
        /// </summary>
        /// <param name="filters">Optional query filters.</param>
        /// <returns>A list of Events.</returns>
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
                    e.creationdate,
                    e.creationbyid,
                    a1.name AS creationby,
                    e.lastupdatedate,
                    e.lastupdatebyid,
                    a2.name AS lastupdateby
                FROM events e
                LEFT JOIN accounts a1 ON e.creationbyid = a1.id
                LEFT JOIN accounts a2 ON e.lastupdatebyid = a2.id
                /**where**/
                ORDER BY e.startdate {filters.OrderBy.ToString()}
                LIMIT @limit OFFSET @offset
            ");

            builder.AddParameters(new
            {
                limit = filters.Limit,
                offset = filters.Offset,
                orderby = filters.OrderBy.ToString(),
            });

            builder.ApplyFilter(FilterType.Equals, "e.deleted", false);

            if (filters is EventFilters eventFilters)
            {
                builder
                    .ApplyFilter(FilterType.Equals, "e.id", eventFilters.Id)
                    .ApplyFilter(FilterType.Like, "e.name", eventFilters.Name)
                    .ApplyFilter(FilterType.Between, "e.");

                if (eventFilters.DateStart != null && eventFilters.DateEnd != null)
                {
                    var dates = new { start = eventFilters.DateStart, end = eventFilters.DateEnd };

                    builder
                        .Where("e.startdate BETWEEN @start AND @end", dates)
                        .OrWhere("e.enddate BETWEEN @start AND @end", dates)
                        .OrWhere("(e.startdate < @start AND e.enddate > @end)", dates);
                }
            }

            using (var db = ConnectionFactory.CreateConnection())
            {
                return db.Query<Event>(query.RawSql, query.Parameters);
            }
        }

        /// <summary>
        ///     Updates the specified <paramref name="event"/>.
        /// </summary>
        /// <param name="event">The Event to update.</param>
        /// <returns>The updated Event.</returns>
        public Event Update(Event @event)
        {
            var builder = new SqlBuilder();

            var query = builder.AddTemplate(@"
                UPDATE events
                SET
                    name = @name,
                    startdate = @startdate,
                    enddate = @enddate,
                    lastupdatedate = @lastupdatedate,
                    lastupdatebyid = @lastupdatebyid
                WHERE id = @id
            ");

            builder.AddParameters(new
            {
                name = @event.Name,
                startdate = @event.StartDate,
                enddate = @event.EndDate,
                lastupdatedate = @event.LastUpdateDate,
                lastupdatebyid = @event.LastUpdateById,
                id = @event.Id,
            });

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query.RawSql, query.Parameters);
            }

            return Get(@event.Id);
        }
    }
}
