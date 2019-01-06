// <copyright file="ScanRepository.cs" company="QC Coders">
//     Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Scans.Data.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using QCVOC.Api.Common;
    using QCVOC.Api.Common.Data.ConnectionFactory;
    using QCVOC.Api.Common.Data.Repository;
    using QCVOC.Api.Scans.Data.Model;

    /// <summary>
    ///     Provides data access for <see cref="Scan"/>.
    /// </summary>
    public class ScanRepository : ITripleKeyRepository<Scan>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ScanRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The database connection factory used for data access.</param>
        public ScanRepository(IDbConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
        }

        private IDbConnectionFactory ConnectionFactory { get; }

        /// <summary>
        ///     Creates a new Scan from the specified <paramref name="scan"/>.
        /// </summary>
        /// <param name="scan">The Scan to create.</param>
        /// <returns>The created Scan.</returns>
        public Scan Create(Scan scan)
        {
            var builder = new SqlBuilder();

            var query = builder.AddTemplate(@"
                INSERT INTO scans
                    (eventid, veteranid, serviceid, plusone, scandate, scanbyid, deleted)
                VALUES
                    (@eventid, @veteranid, @serviceid, @plusone, @scandate, @scanbyid, @deleted)
            ");

            builder.AddParameters(new
            {
                eventid = scan.EventId,
                veteranid = scan.VeteranId,
                serviceid = scan.ServiceId,
                plusone = scan.PlusOne,
                scandate = scan.ScanDate,
                scanbyid = scan.ScanById,
                deleted = false,
            });

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query.RawSql, query.Parameters);
            }

            return Get(scan.EventId, scan.VeteranId, scan.ServiceId);
        }

        /// <summary>
        ///     Deletes the specified <paramref name="scan"/>.
        /// </summary>
        /// <param name="scan">The Scan to delete.</param>
        public void Delete(Scan scan)
        {
            Delete(scan.EventId, scan.VeteranId, scan.ServiceId);
        }

        /// <summary>
        ///     Deletes the Scan matching the specified key.
        /// </summary>
        /// <param name="eventId">The Event id of the scan.</param>
        /// <param name="veteranId">The Veteran id of the scan.</param>
        /// <param name="serviceId">The optional Service id of the scan.</param>
        public void Delete(Guid eventId, Guid veteranId, Guid? serviceId)
        {
            var builder = new SqlBuilder();

            var query = builder.AddTemplate($@"
                UPDATE scans
                SET
                    deleted = true
                WHERE eventid = @eventid
                AND veteranid = @veteranid
                {(serviceId != null ? "AND serviceid = @serviceid" : string.Empty)}
            ");

            builder.AddParameters(new
            {
                eventid = eventId,
                veteranid = veteranId,
                serviceid = serviceId,
            });

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query.RawSql, query.Parameters);
            }
        }

        /// <summary>
        ///     Retrieves the Scan with the specified key.
        /// </summary>
        /// <param name="eventId">The Event id of the scan.</param>
        /// <param name="veteranId">The Veteran id of the scan.</param>
        /// <param name="serviceId">The optional Service id of the scan.</param>
        /// <returns>The Scan with the specified key.</returns>
        public Scan Get(Guid eventId, Guid veteranId, Guid? serviceId)
        {
            return GetAll(new ScanFilters() { EventId = eventId, VeteranId = veteranId, ServiceId = serviceId })
                .SingleOrDefault();
        }

        /// <summary>
        ///     Retrieves all Scans after applying optional <paramref name="filters"/>.
        /// </summary>
        /// <param name="filters">Optional query filters.</param>
        /// <returns>A list of Scans.</returns>
        public IEnumerable<Scan> GetAll(Filters filters = null)
        {
            filters = filters ?? new Filters();
            var builder = new SqlBuilder();

            var query = builder.AddTemplate($@"
                SELECT
                    s.eventid,
                    s.veteranid,
                    s.serviceid,
                    s.plusone, 
                    s.scandate, 
                    s.scanbyid,
                    a.name AS scanby
                FROM scans s
                LEFT JOIN accounts a ON s.scanbyid = a.id
                INNER JOIN veterans v ON s.veteranid = v.id
                /**where**/
                ORDER BY s.scandate {filters.OrderBy.ToString()}
                LIMIT @limit OFFSET @offset
            ");

            builder.AddParameters(new
            {
                limit = filters.Limit,
                offset = filters.Offset,
                orderby = filters.OrderBy.ToString(),
            });

            builder.ApplyFilter(FilterType.Equals, "s.deleted", false);

            if (filters is ScanFilters scanFilters)
            {
                builder
                    .ApplyFilter(FilterType.Equals, "s.eventid", scanFilters.EventId)
                    .ApplyFilter(FilterType.Equals, "s.veteranid", scanFilters.VeteranId)
                    .ApplyFilter(FilterType.Equals, "s.serviceid", scanFilters.ServiceId)
                    .ApplyFilter(FilterType.Equals, "s.plusone", scanFilters.PlusOne);
            }

            using (var db = ConnectionFactory.CreateConnection())
            {
                return db.Query<Scan>(query.RawSql, query.Parameters);
            }
        }

        /// <summary>
        ///     Updates the specified <paramref name="scan"/>.
        /// </summary>
        /// <param name="scan">the Scan to update.</param>
        /// <returns>The updated Scan.</returns>
        public Scan Update(Scan scan)
        {
            var builder = new SqlBuilder();

            var query = builder.AddTemplate(@"
                UPDATE scans
                SET
                    plusone = @plusone,
                    scandate = @scandate,
                    scanbyid = @scanbyid
                WHERE eventid = @eventid
                AND veteranid = @veteranid
                AND serviceid = @serviceid
            ");

            builder.AddParameters(new
            {
                plusone = scan.PlusOne,
                scandate = scan.ScanDate,
                scanbyid = scan.ScanById,
                eventid = scan.EventId,
                veteranid = scan.VeteranId,
                serviceid = scan.ServiceId,
            });

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(query.RawSql, query.Parameters);
            }

            return Get(scan.EventId, scan.VeteranId, scan.ServiceId);
        }
    }
}
