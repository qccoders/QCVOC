// <copyright file="ScanRepository.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Scans.Data.Repository
{
    using System;
    using System.Collections.Generic;
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

        public void Delete(Scan resource)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid eventId, Guid veteranId, Guid? serviceId)
        {
            throw new NotImplementedException();
        }

        public Scan Get(Guid eventId, Guid veteranId, Guid? serviceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Scan> GetAll(Filters filters = null)
        {
            throw new NotImplementedException();
        }

        public Scan Update(Scan resource)
        {
            throw new NotImplementedException();
        }
    }
}
