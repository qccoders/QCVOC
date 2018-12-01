// <copyright file="ScanResponse.cs" company="QC Coders">
//     Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Scans.Data.DTO
{
    using System;
    using QCVOC.Api.Scans.Data.Model;
    using QCVOC.Api.Veterans.Data.Model;

    /// <summary>
    ///     DTO containing the result of a Scan.
    /// </summary>
    public class ScanResponse
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ScanResponse"/> class with the specified <paramref name="scan"/> and <paramref name="veteran"/>.
        /// </summary>
        /// <param name="scan">The Scan to include in the response.</param>
        /// <param name="veteran">The Veteran associated with the scan, if applicable.</param>
        public ScanResponse(Scan scan, Veteran veteran = null)
        {
            EventId = scan.EventId;
            VeteranId = scan.VeteranId;
            Veteran = veteran;
            ServiceId = scan.ServiceId;
            PlusOne = scan.PlusOne;
            ScanBy = scan.ScanBy;
            ScanById = scan.ScanById;
            ScanDate = scan.ScanDate;
        }

        /// <summary>
        ///     Gets or sets the id of the Event.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        ///     Gets or sets the id of the Veteran.
        /// </summary>
        public Guid VeteranId { get; set; }

        /// <summary>
        ///     Gets or sets the Veteran associated with the Scan.
        /// </summary>
        public Veteran Veteran { get; set; }

        /// <summary>
        ///     Gets or sets the id of the Service.
        /// </summary>
        /// <remarks>
        ///     If null, represents a check-in Scan.
        /// </remarks>
        public Guid ServiceId { get; set; }

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
        ///     Gets or sets the date on which the Scan was performed.
        /// </summary>
        public DateTime ScanDate { get; set; }
    }
}
