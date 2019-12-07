// <copyright file="IReportRepository.cs" company="QC Coders">
//     Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Reports.Data
{
    using System;
    using System.Collections.Generic;

    public interface IReportRepository
    {
        IEnumerable<EventMaster> GetEventMaster(DateTime startTime, DateTime endTime);
    }
}