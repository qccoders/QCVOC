// <copyright file="ReportRepository.cs" company="QC Coders">
//     Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Reports.Data
{
    using System;
    using System.Collections.Generic;
    using Dapper;
    using QCVOC.Api.Common.Data.ConnectionFactory;
    using QCVOC.Api.Veterans.Data.Model;

    /// <summary>
    ///     Provides data access for report datasets.
    /// </summary>
    public class ReportRepository : IReportRepository
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReportRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The database connection factory used for data access.</param>
        public ReportRepository(IDbConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
        }

        private IDbConnectionFactory ConnectionFactory { get; }

        /// <summary>
        ///     Gets the event master report dataset.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <returns>The event master dataset.</returns>
        public IEnumerable<EventMaster> GetEventMaster(DateTime startTime, DateTime endTime)
        {
            var builder = new SqlBuilder();

            var query = builder.AddTemplate(@"
                SELECT
	                events.*,
	                COUNT(scans.*) AS scans,
	                COUNT(scans.*) FILTER (WHERE scans.eventid = events.id AND scans.serviceid = '00000000-0000-0000-0000-000000000000') AS checkins,
	                COUNT(scans.*) FILTER (WHERE scans.eventid = events.id AND scans.serviceid = '00000000-0000-0000-0000-000000000000' AND scans.plusone) as plusones
                FROM events 
                INNER JOIN scans ON scans.eventid = events.id
                WHERE NOT events.deleted
                AND events.startdate BETWEEN @starttime AND @endtime
                GROUP BY events.id
            ");

            builder.AddParameters(new
            {
                starttime = startTime,
                endtime = endTime,
            });

            using (var db = ConnectionFactory.CreateConnection())
            {
                return db.Query<EventMaster>(query.RawSql, query.Parameters);
            }
        }

        /// <summary>
        ///     Retrieves all Veterans, less `<see cref="Veteran.PhotoBase64"/>.
        /// </summary>
        /// <returns>A list of Veterans</returns>
        public IEnumerable<Veteran> GetVeterans()
        {
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
                    v.primaryphone,
                    v.email,
                    v.enrollmentdate,
                    v.enrollmentbyid,
                    b.name AS enrollmentby,
                    v.verificationmethod
                FROM veterans v
                LEFT JOIN accounts a ON v.lastupdatebyid = a.id
                LEFT JOIN accounts b ON v.enrollmentbyid = b.id
            ");

            using (var db = ConnectionFactory.CreateConnection())
            {
                return db.Query<Veteran>(query.RawSql, query.Parameters);
            }
        }
    }
}