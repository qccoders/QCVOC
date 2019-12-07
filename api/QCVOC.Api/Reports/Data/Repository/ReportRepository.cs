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

    public class ReportRepository : IReportRepository
    {
        public ReportRepository(IDbConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
        }

        private IDbConnectionFactory ConnectionFactory { get; }

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
    }
}