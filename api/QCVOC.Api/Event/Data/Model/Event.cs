// <copyright file="Event.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Event.Data.Model
{
    using System;

    /// <summary>
    ///     An Event.
    /// </summary>
    public class Event
    {
        /// <summary>
        ///     Gets the name of the user which created the Event.
        /// </summary>
        public string CreationBy { get; }

        /// <summary>
        ///     Gets or sets the id of the user which created the Event.
        /// </summary>
        public Guid CreationById { get; set; }

        /// <summary>
        ///     Gets or sets the Event creation date.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        ///     Gets the name of the user which performed the last update.
        /// </summary>
        public string LastUpdateBy { get; }

        /// <summary>
        ///     Gets or sets the id of the user which performed the last update.
        /// </summary>
        public Guid LastUpdateById { get; set; }

        /// <summary>
        ///     Gets or sets the date on which the Event was last updated.
        /// </summary>
        public DateTime LastUpdateDate { get; set; }

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
        ///     Gets or sets the starting time and date of the Event.
        /// </summary>
        public DateTime StartDate { get; set; }
    }
}