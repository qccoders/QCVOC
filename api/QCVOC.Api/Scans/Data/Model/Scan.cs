// <copyright file="Scan.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Scans.Data.Model
{
    using System;

    /// <summary>
    ///     A record of the Scan of a Veteran's membership card.
    /// </summary>
    public class Scan : IEquatable<Scan>
    {
        /// <summary>
        ///     Gets or sets the id of the Event.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        ///     Gets or sets the id of the Veteran.
        /// </summary>
        public Guid VeteranId { get; set; }

        /// <summary>
        ///     Gets or sets the id of the Service.
        /// </summary>
        /// <remarks>
        ///     If null, represents a check-in Scan.
        /// </remarks>
        public Guid? ServiceId { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the Veteran brought a guest.
        /// </summary>
        public bool PlusOne { get; set; }

        /// <summary>
        ///     Gets or sets the name of the user who performed the Scan.
        /// </summary>
        public string ScanBy { get; set; }

        /// <summary>
        ///     Gets or sets the id of the user who performed the Scan.
        /// </summary>
        public Guid ScanById { get; set; }

        /// <summary>
        ///     Gets or sets the data on which the Scan was performed.
        /// </summary>
        public DateTime ScanDate { get; set; }

        /// <summary>
        ///     Compares two Scan instances.
        /// </summary>
        /// <param name="scan">The Scan to which to compare.</param>
        /// <returns>A value indicating whether the compared instances are equa.</returns>
        public bool Equals(Scan scan)
        {
            if (scan == null)
            {
                return this == null;
            }

            return this.EventId == scan.EventId
                && this.VeteranId == scan.VeteranId
                && this.ServiceId == scan.ServiceId
                && this.PlusOne == scan.PlusOne
                && this.ScanBy == scan.ScanBy
                && this.ScanById == scan.ScanById
                && this.ScanDate - scan.ScanDate <= TimeSpan.FromSeconds(1);
        }
    }
}
