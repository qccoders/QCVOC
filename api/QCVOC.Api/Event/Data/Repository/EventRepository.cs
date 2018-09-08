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
    }
}
