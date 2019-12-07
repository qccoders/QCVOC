// <copyright file="EventMaster.cs" company="QC Coders">
//     Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Reports.Data
{
    using System;

    /// <summary>
    ///     The event master report.
    /// </summary>
    public class EventMaster
    {
        /// <summary>
        ///     Gets or sets the number of check in scans.
        /// </summary>
        public long Checkins { get; set; }

        /// <summary>
        ///     Gets or sets the ending time and date of the Event.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        ///     Gets or sets the Event id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Gets or sets the Event name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the number of check in scans which included a plus one.
        /// </summary>
        public long PlusOnes { get; set; }

        /// <summary>
        ///     Gets or sets the total number of scans.
        /// </summary>
        public long Scans { get; set; }

        /// <summary>
        ///     Gets or sets the starting time and date of the Event.
        /// </summary>
        public DateTime StartDate { get; set; }
    }
}