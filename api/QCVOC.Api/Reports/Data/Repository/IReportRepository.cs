// <copyright file="IReportRepository.cs" company="QC Coders">
//     Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Reports.Data
{
    using System;
    using System.Collections.Generic;
    using QCVOC.Api.Veterans.Data.Model;

    /// <summary>
    ///     Provides data access for report datasets.
    /// </summary>
    public interface IReportRepository
    {
        /// <summary>
        ///     Gets the event master report dataset.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <returns>The event master dataset.</returns>
        IEnumerable<EventMaster> GetEventMaster(DateTime startTime, DateTime endTime);

        /// <summary>
        ///     Gets the veteran report dataset.
        /// </summary>
        /// <returns>The veteran report dataset.</returns>
        IEnumerable<Veteran> GetVeterans();
    }
}