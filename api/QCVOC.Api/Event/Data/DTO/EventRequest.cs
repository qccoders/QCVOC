// <copyright file="EventRequest.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Event.Data.Model
{
    using System;

    /// <summary>
    ///     An Event.
    /// </summary>
    public class EventRequest
    {
        /// <summary>
        ///     Gets or sets the ending time and date of the Event.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        ///     Gets or sets the Event name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the starting time and date of the Event.
        /// </summary>
        public DateTime StartDate { get; set; }
    }
}